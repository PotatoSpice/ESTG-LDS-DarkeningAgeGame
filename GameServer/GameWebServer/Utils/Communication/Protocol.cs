using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using GameWebServer.Entities;
using GameWebServer.Entities.Room;
using GameWebServer.Models.Requests;
using GameWebServer.Models.Responses;

namespace GameWebServer.Utils
{
    /// <summary>
    /// <para>This class handles all the game communication protocol logic.</para>
    /// <para>This is the prefered protocol architeture, in JSON format:</para>
    /// <code>
    /// [ "Action":"ActionType", "Content":[ "Instruction1", "Instruction2" ] ]
    /// </code>
    /// <para> An example use:</para>
    /// <example>
    /// [ "EventType":"ChatMessage", "Content":[ "_username_", "_message-content_" ] ]
    /// </example>
    /// </summary>
    public static class Protocol
    {
        public static GameRequest requestParser(String request)
        {
            Console.WriteLine(request);
            return JsonConvert.DeserializeObject<GameRequest>(request);
        }

        public static string sendRequests(GameRequest request, GameInstance game, List<IdFactionResponse> idFactions)
        {
            Console.WriteLine(request.EventType);
            switch (request.EventType)
            {
                case "InitialInformation":
                    return sendInitialInformation(game, idFactions);
                case "UpdateInformation":
                    return sendUpdatedInformation(game);
                case "PlayerFactions":
                    return sendPlayerFactions(idFactions); 
                case "ChatMessage":
                    return "{\"EventType\":\"ChatMessage\",\"Username\":\"" + request.data[0] + "\", \"Message\": \"" + request.data[1] + "\"}";
                default:
                    return "BAD REQUEST"; 
            }
        }

        public static void executeAction(GameRequest request, GameRoom room, string playerID)
        {
            Console.WriteLine(request.EventType);

            GameInstance game = room.GameInstance;
            bool success = false;
            try {
                switch (request.EventType)
                {
                    case "CreateArmy":
                        success = game.createArmy(request.data[0], request.data[1], request.data[2], request.data[3], request.data[4]);
                        if (success == true)
                            room.getPlayerByID(playerID).armiesCreated++;
                        break;
                    case "CreateUnit":
                        game.createUnit(request.data[0], request.data[1], request.data[2], request.data[3]);
                        break;
                    case "ReinforceUnit":
                        game.reinforceUnits(request.data[0], request.data[1]);
                        break;
                    case "MoveArmy":
                        game.moveArmy(request.data[0], request.data[1], request.data[2]);
                        break;
                    case "SwapArmies":
                        game.swapArmiesInRegions(request.data[0], request.data[1], request.data[2]);
                        break;
                    case "AttackRegion":
                        success = game.attackRegion(request.data[0], request.data[1], request.data[2], request.data[3]);
                        if (success == true)
                            room.getPlayerByID(playerID).regionsConquered++;
                        break;
                    case "AnnexNeutralRegion":
                        success = game.annexNeutralRegion(request.data[0], request.data[1], request.data[2]);
                        if (success == true)
                            room.getPlayerByID(playerID).regionsConquered++;
                        break;
                    case "UpdateResources":
                        game.updateFactionResources();
                        break;
                    case "DeclareWar":
                        game.declareWar(request.data[0], request.data[1]);
                        break;
                    case "CheckGame":
                        game.checkGameStatus(room);
                        break;
                    case "Armistice":
                        game.declarePeace(request.data[0], request.data[1]);
                        break;
                    case "Capitulation":
                        game.capitulation(request.data[0]);
                        break;
                    default:
                        break;
                }
                }
                catch(ArgumentOutOfRangeException exc)
                {
                   Console.WriteLine("There are problems with data sent on events; \n"+ exc.ToString());
                }
        }

        private static string sendInitialInformation(GameInstance game, List<IdFactionResponse> idFactions)
        {
            return JsonConverter<InitInformationResponse>.convertObjectToJsonString(game.initialInformation(idFactions));
        }

        private static String sendUpdatedInformation(GameInstance game)
        {
            return JsonConverter<UpdatedInformationResponse>.convertObjectToJsonString(game.updatedInformationTurn());
        }

        public static string sendPlayerFactions(List<IdFactionResponse> idFactions)
        {
            IdFactionResponseEvent idFactionsEvent = new IdFactionResponseEvent("PlayerFactions", idFactions); 
            return JsonConverter<IdFactionResponseEvent>.convertObjectToJsonString(idFactionsEvent);
        }

    }

}
