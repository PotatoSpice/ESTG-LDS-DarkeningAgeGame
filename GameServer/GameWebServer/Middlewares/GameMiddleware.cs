using System;
using System.IO;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using GameWebServer.Exceptions;
using GameWebServer.Models.Responses;
using GameWebServer.Services;

namespace GameWebServer.Middlewares
{
    public class GameMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        private readonly JsonSerializerSettings _responseJsonSettings;

        public GameMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;

            _responseJsonSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        // # COMMUNICATION LOGIC for UNITY GAME Socket Connections ...
        public async Task Invoke(HttpContext context, IMessageHandlerService handlerService, IHttpClientFactory httpFactory)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync( 
                    BuildResponse("" + context.Response.StatusCode, "Not a WebSocket HTTP Request"));
                return;
            }

            string conn_playerID = null, 
                conn_roomID = context.Request.Query["room-id"].ToString(), 
                auth_token = context.Request.Query["player-auth"].ToString();
            WebSocket socket = null;

            try // # HANDLE ERROR EXCEPTIONS, socket is closed
            {
                conn_playerID = await GetPlayerInformation(httpFactory, auth_token);
                if (string.IsNullOrWhiteSpace(conn_playerID))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
                // Everything is set, accept the socket connection and execute de handshake
                socket = await context.WebSockets.AcceptWebSocketAsync();

                if (handlerService.CheckIfPlayerInStartedGame(conn_roomID, conn_playerID))
                {
                    await handlerService.SendMessageAsync(socket, BuildResponse("PlayerGameStarted", 
                        "Game already started. Player should exit first before updating connection."));
                    return; // socket is closed on the 'finally' block
                }
                
                await handlerService.UpdateConnection(conn_roomID, conn_playerID, socket);

                await HandleReceivedMessage(socket, async(result, data) => {
                    try // # HANDLE WARNING EXCEPTIONS, socket keeps running
                    {
                        switch (result.MessageType) 
                        {
                            case WebSocketMessageType.Text:
                                // DO SOMETHING when received message is text
                                await handlerService.HandleGameAsync(conn_roomID, conn_playerID, socket, data);
                                break;
                            case WebSocketMessageType.Binary:
                                // DO SOMETHING when received message is binary
                                break;
                            default: // WebSocketMessageType.Close
                                // DO SOMETHING when connection closes
                                await handlerService.OnPlayerLeft(conn_roomID, conn_playerID);
                                break;
                        }
                    }
                    catch (Exception exc)
                    {
                        await HandleWarningExceptions(socket, handlerService, exc);
                    }
                });
            }
            catch (Exception exc) // catch socket and thread exceptions so the service doesn't break
            {
                await HandleErrorExceptions(socket, handlerService, exc);
            }
            finally // Make sure socket is closed when connection terminates
            {
                await CloseSocketConn(socket);
            }
        }

        // This methods verifies an authToken with the WebServiceAPI to confirm user identity
        private async Task<string> GetPlayerInformation(IHttpClientFactory httpFactory, string authToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _config["AppSettings:AuthApiEndpoint"]);
            request.Headers.Add("Authorization", $"Bearer {authToken}");
            request.Headers.Add("User-Agent", "DarkeningAge-GameServer");

            using (HttpClient client = httpFactory.CreateClient())
            using (HttpResponseMessage response = await client.SendAsync(request))
            {
                if (response.IsSuccessStatusCode)
                {
                    using (HttpContent content = response.Content)
                    {
                        AuthDataResponse data = JsonConvert.DeserializeObject<AuthDataResponse>(
                            await content.ReadAsStringAsync());
                        return data.username;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task HandleReceivedMessage(WebSocket socket, Action<WebSocketReceiveResult, string> handleMessage)
        {
            var buffer = new byte[1024 * 4];
            ArraySegment<byte> bufferArr = null;
            MemoryStream chunks = null;
            WebSocketReceiveResult result = null;
            while(socket.State == WebSocketState.Open)
            {
                chunks = new MemoryStream();
                bufferArr = new ArraySegment<byte>(buffer);
                
                do {
                    result = await socket.ReceiveAsync(buffer: bufferArr,
                                                        cancellationToken: CancellationToken.None);
                    chunks.Write(bufferArr.Array, bufferArr.Offset, result.Count);
                } while(!result.EndOfMessage);

                chunks.Seek(0, SeekOrigin.Begin);

                handleMessage(result, new StreamReader(chunks, Encoding.UTF8).ReadToEnd());
            }
        }

        private string BuildResponse(string title, string message)
        {
            return JsonConvert.SerializeObject(new ErrorResponse( title, message), _responseJsonSettings);
        }

        // The socket should keep running when this exceptions are thrown
        private async Task HandleWarningExceptions(WebSocket socket, IMessageHandlerService handlerService, Exception exc) 
        {
            if (exc is GameWarningException)
            {
                Console.WriteLine($"{exc.GetType()}: {exc.Message}");
                await handlerService.SendMessageAsync(socket, BuildResponse(exc.GetType().Name, exc.Message));
            }
            else if (exc is Newtonsoft.Json.JsonReaderException || 
                        exc is Newtonsoft.Json.JsonSerializationException)
            {
                Console.WriteLine($"{exc.GetType()}: {exc.Message}");
                await handlerService.SendMessageAsync(socket, BuildResponse( exc.GetType().Name, 
                        "Event Message is not a valid JSON with the correct structure: " + 
                            "{ eventType: 'event-type', data: 'event-data'"));
            }
            else {
                throw exc;
            }
        }

        // The socket is closed when this exceptions are thrown
        private async Task HandleErrorExceptions(WebSocket socket, IMessageHandlerService handlerService, Exception exc)
        {
            if (exc is PlayerNotFoundException || exc is RoomNotFoundException)
            {
                Console.WriteLine($"{exc.GetType()}: {exc.Message}");
                await handlerService.SendMessageAsync(socket, BuildResponse( exc.GetType().Name, exc.Message));
            }
            else {
                Console.WriteLine(exc);
            }
        }

        private async Task CloseSocketConn(WebSocket socket)
        {
            if (socket != null && socket.State == WebSocketState.Open) {
                await socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                                        statusDescription: "Connection closed by manager ...",
                                        cancellationToken: CancellationToken.None);
            }
        }
    }   
}