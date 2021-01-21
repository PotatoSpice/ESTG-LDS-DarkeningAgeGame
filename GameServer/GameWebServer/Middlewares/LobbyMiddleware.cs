using System;
using System.IO;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using GameWebServer.Entities.Player;
using GameWebServer.Models.Responses;
using GameWebServer.Exceptions;
using GameWebServer.Services;

namespace GameWebServer.Middlewares
{
    public class LobbyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        private readonly JsonSerializerSettings _responseJsonSettings;

        public LobbyMiddleware(RequestDelegate next, IConfiguration config)
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

        // # COMMUNICATION LOGIC for GAME CLIENT Socket Connections ...
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
            // The request must have an auth token and the player should be hosting or joining a room
            var routeValues = context.GetRouteData().Values;
            string auth_cookie = context.Request.Cookies["auth-token"];
            if (!(routeValues["host_join"].Equals("host") || routeValues["host_join"].Equals("join")
                || routeValues["host_join"].Equals("findmatch")) || string.IsNullOrWhiteSpace(auth_cookie))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            WebSocket socket = null;
            string conn_playerID = null, conn_roomID = null;

            try // # HANDLE ERROR EXCEPTIONS, socket is closed
            {
                conn_playerID = await HandlePlayerAuthentication(httpFactory, auth_cookie);
                if (string.IsNullOrWhiteSpace(conn_playerID))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
                // Everything is set, accept the socket connection and execute de handshake
                socket = await context.WebSockets.AcceptWebSocketAsync();
                
                string check_ingame = handlerService.CheckIfPlayerInGame(conn_playerID);
                if (check_ingame != null)
                {
                    await handlerService.SendMessageAsync(socket, BuildResponse("PlayerInGame", check_ingame));
                    return; // socket is closed on the 'finally' block
                }

                if (context.Request.Path.ToString().Contains("host"))
                {
                    conn_roomID = await handlerService.OnConnectedHostLobby( 
                        new RoomPlayer(conn_playerID, socket), false);
                }
                else if (context.Request.Path.ToString().Contains("findmatch"))
                {
                    conn_roomID = await handlerService.OnConnectedHostLobby( 
                        new RoomPlayer(conn_playerID, socket), true);
                }
                else if (context.Request.Path.ToString().Contains("join"))
                {
                    conn_roomID = context.Request.Query["room-id"].ToString();
                    
                    await handlerService.OnConnectedJoinLobby( conn_roomID, 
                    new RoomPlayer(conn_playerID, socket) );
                }

                await HandleReceivedMessage(socket, async(result, data) => {
                    try // # HANDLE WARNING EXCEPTIONS, socket keeps running
                    {
                        switch (result.MessageType) 
                        {
                            case WebSocketMessageType.Text:
                                await handlerService.HandleLobbyAsync(conn_roomID, conn_playerID, socket, data);
                                break;
                            default: // WebSocketMessageType.Close
                                await handlerService.CancelMatchmaking(conn_roomID);
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
        private async Task<string> HandlePlayerAuthentication(IHttpClientFactory httpFactory, string authToken)
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

        // This method handles the communication between the service and the client.
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
        private async Task HandleWarningExceptions(WebSocket socket, IMessageHandlerService handler, Exception exc) 
        {
            if (exc is LobbyWarningException)
            {
                Console.WriteLine($"{exc.GetType()}: {exc.Message}");
                await handler.SendMessageAsync(socket, BuildResponse( exc.GetType().Name, exc.Message));
            }
            else if (exc is Newtonsoft.Json.JsonReaderException || 
                        exc is Newtonsoft.Json.JsonSerializationException)
            {
                Console.WriteLine($"{exc.GetType()}: {exc.Message}");
                await handler.SendMessageAsync(socket, BuildResponse( exc.GetType().Name, 
                        "Event Message is not a valid JSON with the correct structure: " + 
                            "{ eventType: 'event-type', data: 'event-data'"));
            }
            else {
                Console.WriteLine($"{exc.GetType()}: {exc.Message}");
            }
        }

        // The socket is closed when this exceptions are thrown
        private async Task HandleErrorExceptions(WebSocket socket, IMessageHandlerService handler, Exception exc)
        {
            if (exc is AlreadyInLobbyException || exc is FullRoomException ||
                exc is PlayerNotFoundException || exc is RoomNotFoundException)
            {
                Console.WriteLine($"{exc.GetType()}: {exc.Message}");
                await handler.SendMessageAsync(socket, BuildResponse( exc.GetType().Name, exc.Message));
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