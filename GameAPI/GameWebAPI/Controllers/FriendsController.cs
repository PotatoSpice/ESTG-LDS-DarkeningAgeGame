using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GameWebAPI.Entities;
using GameWebAPI.Exceptions;
using GameWebAPI.Models;
using GameWebAPI.Models.Response;
using GameWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameWebAPI.Controllers
{
    [ApiController]
    [Route("friends")]
    [Produces("application/json")]
    public class FriendsController : ControllerBase
    {
        private readonly ILogger<SessionController> _logger;
        private readonly IFriendsService _service;
        private readonly IMapper _mapper;

        public FriendsController(ILogger<SessionController> logger, IFriendsService service, IMapper mapper)
        {
            this._logger = logger;
            this._service = service;
            this._mapper = mapper;
        }

        /// <summary>
        /// Obtêm a lista de amigos do utilizador com a sessão iniciada.
        /// </summary>
        /// <returns>Lista de amigos, nomeadamente o nome e se está online.</returns>
        /// <response code="200">Exemplo de cada elemento da lista:
        /// <code>
        /// [{
        ///     "username" : "trickyBatata",
        ///     "online" : false
        /// }]</code></response>
        /// <response code="401">Quem fez o pedido não tem sessão iniciada.</response>
        /// <response code="500">Retorna se ocorrer algum problema interno no servidor.</response>
        [Authorize]
        [HttpGet()]
        public async Task<IActionResult> GetUserInSessionFriends()
        {
            try
            {
                Player player = (Player) HttpContext.Items["SessionPlayer"];

                ICollection<Player> friendsList = await _service.GetFriendsList(player.username);
                ICollection<PlayerFriendInfo> response = new List<PlayerFriendInfo>();
                foreach (Player p in friendsList)
                {
                    response.Add(_mapper.Map<PlayerFriendInfo>(p));
                }

                return Ok(response);
            }
            catch (Exception exc)
            {
                this._logger.LogError(exc, exc.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Obtêm a lista de pedidos de amizade do utilizador com a sessão iniciada.
        /// </summary>
        /// <returns>Lista de pedidos de amizade.</returns>
        /// <response code="200">
        /// ["trickyBatata", "okokok"]</response>
        /// <response code="401">Quem fez o pedido não tem sessão iniciada.</response>
        /// <response code="500">Retorna se ocorrer algum problema interno no servidor.</response>
        [Authorize]
        [HttpGet("invites")]
        public async Task<IActionResult> GetFriendInvites()
        {
            try
            {
                Player player = (Player)HttpContext.Items["SessionPlayer"];
                var requestList = await _service.GetFriendInvites(player.username);
                ICollection<string> sendUserRequestsList = new List<string>();
                foreach(FriendInvite f in requestList) {
                    sendUserRequestsList.Add(f.playerId);
                }
                return Ok(sendUserRequestsList);
            }
            catch (Exception exc)
            {
                this._logger.LogError(exc, exc.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// O utilizador com sessão iniciada envia um pedido de amizade para um determinado utilizador
        /// </summary>
        /// <remarks>
        /// Exemplo de um pedido:
        ///
        ///     POST /sendRequest
        ///     {
        ///         "targetUsername" : "trickyBatata",
        ///     }
        ///
        /// </remarks>
        /// <param name="friendInvite"></param>
        /// <returns>Pedido de amizade criado com sucesso.</returns>
        /// <response code="200">Pedido de amizade criado com sucesso.</response>
        /// <response code="400">Não pode enviar para ele mesmo.</response>
        /// <response code="400">Já existe um pedido de amizade entre os dois utilizadores.</response>
        /// <response code="400">Os dois utilizadores já são amigos.</response>
        /// <response code="401">Quem fez o pedido não tem sessão iniciada.</response>
        /// <response code="404">O utilzador para quem pretende enviar o pedido nao existe.</response>
        /// <response code="500">Retorna se ocorrer algum problema interno no servidor.</response>
        [Authorize]
        [HttpPost("send-invite")]
        public async Task<IActionResult> SendFriendInvite([FromBody] FriendInviteRequest friendInvite)
        {
            try
            {
                Player player = (Player)HttpContext.Items["SessionPlayer"];

                await _service.SendFriendInvite(player.username, friendInvite.targetUsername);

                return Ok(new Success("Request send successfully."));
            }
            catch(SendToYourselfException e)
            {
                return BadRequest(new Error("SendYourself", e.Message));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(new Error("UserNotFound", "User Not Found"));
            }
            catch (AlreadyExistsFriendRequest e)
            {
                return BadRequest(new Error("AlreadyRequested", e.Message));
            }
            catch(AlreadyFriendsException e)
            {
                return BadRequest(new Error("AlreadyFriends", e.Message));
            }
            catch (Exception exc)
            {
                this._logger.LogError(exc, exc.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// O utilizador com sessão iniciada aceita/recusa pedido de amizade de outro utilizador.
        /// </summary>
        /// <remarks>
        /// Exemplo de um pedido:
        ///
        ///     POST /acceptRequest
        ///     {
        ///        "username": "OKOKOK",
        ///        "response": true
        ///     }
        ///
        /// </remarks>
        /// <param name="request"></param>
        /// <returns>O pedido de amizade foi tratado corretamente.</returns>
        /// <response code="200">O pedido de amizade foi tratado corretamente.</response>
        /// <response code="400">Pedido de amizade não foi encontrado.</response>
        /// <response code="401">Quem fez o pedido não tem sessão iniciada.</response>
        /// <response code="404">O utilzador que pretende aceitar/recusar pedido de amizade nao existe.</response>
        /// <response code="500">Retorna se ocorrer algum problema interno no servidor.</response>
        [Authorize]
        [HttpPost("accept-invite")]
        public async Task<IActionResult> AcceptFriendInvite([FromBody] FriendResponse request)
        {
            try
            {
                Player player = (Player)HttpContext.Items["SessionPlayer"];

                FriendInvite friendRequest = new FriendInvite();
                friendRequest.playerId = request.username;
                friendRequest.targetPlayerId = player.username;
                await _service.AcceptFriendInvite(friendRequest, request.response);
                return Ok(new Success("Invite handled successfully."));
            }
            catch (EntityNotFoundException e)
            {
                if (e is FriendRequestNotFound)
                {
                    return BadRequest(new Error("FriendRequestNotFound", e.Message));
                }
                return NotFound(new Error("UserNotFound", "User Not Found."));
            }
            catch (Exception exc)
            {
                this._logger.LogError(exc, exc.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// O utilizador com sessão iniciada remove um determinado utilizador da sua lista de amigos.
        /// </summary>
        /// <param name="targetUsername"></param>
        /// <returns>Utilizador removido da lista de amigos com sucesso.</returns>
        /// <response code="200">Utilizador removido da lista de amigos com sucesso.</response>
        /// <response code="400">O jogador indicado não se encontra na lista de amigos</response>
        /// <response code="401">Quem fez o pedido não tem sessão iniciada.</response>
        /// <response code="404">O utilzador para quem pretende enviar o pedido nao existe.</response>
        /// <response code="500">Retorna se ocorrer algum problema interno no servidor.</response>
        [Authorize]
        [HttpDelete("unfriend/{targetUsername}")]
        public async Task<IActionResult> UnfriendPlayer(string targetUsername)
        {
            try
            {
                Player player = (Player) HttpContext.Items["SessionPlayer"];
                
                await _service.UnfriendPlayer(player.username, targetUsername);

                return Ok(new Success("Friend removed successfully."));
            }
            catch (EntityNotFoundException e)
            {
                if (e is FriendNotFoundException)
                {
                    return BadRequest(new Error("FriendNotFound", e.Message));
                }
                return NotFound(new Error("UserNotFound", "User Not Found."));
            }
            catch (Exception exc)
            {
                this._logger.LogError(exc, exc.Message);
                return StatusCode(500);
            }
        }
    }
}
