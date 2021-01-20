using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GameWebAPI.Entities;
using GameWebAPI.Exceptions;
using GameWebAPI.Models.GameInvite;
using GameWebAPI.Models.Response;
using GameWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameWebAPI.Controllers
{
    [ApiController]
    [Route("game-invites")]
    [Produces("application/json")]
    public class GameInviteController : ControllerBase
    {
        private readonly ILogger<SessionController> _logger;
        private readonly IGameInviteService _service;
        private readonly IMapper _mapper;

        public GameInviteController(ILogger<SessionController> logger, IGameInviteService service, IMapper mapper)
        {
            this._logger = logger;
            this._service = service;
            this._mapper = mapper;
        }

        /// <summary>
        /// Obtêm a lista de convites para jogos.
        /// </summary>
        /// <returns>Lista de convites para jogos.</returns>
        /// <response code="200">Exemplo de cada elemento da lista:
        /// <code>
        /// [{
        ///   "roomId" : "fgretg43v34tvvt",
        ///   "hostId" : "trickyBatata",
        ///   "createDate : "2020/12/20",
        ///   "gameType" : "Costum"
        /// }]</code></response>
        /// <response code="401">Quem fez o pedido não tem sessão iniciada.</response>
        /// <response code="500">Retorna se ocorrer algum problema interno no servidor.</response>
        [Authorize]
        [HttpGet()]
        public async Task<IActionResult> GetUserInSessionGameInvites()
        {
            try
            {
                Player player = (Player)HttpContext.Items["SessionPlayer"];

                ICollection<GameInvite> invitesList = await _service.GetPlayerGameInvites(player.username);
                ICollection<GameInviteResponse> response = new List<GameInviteResponse>();
                foreach (GameInvite i in invitesList)
                {
                    response.Add(_mapper.Map<GameInviteResponse>(i));
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
        /// Criar um invite para um determinado jogador.
        /// </summary>
        /// <remarks>
        /// Exemplo de um pedido:
        ///
        ///     POST /create
        ///     {
        ///        "roomId": "sdf4wf23obfb234234",
        ///        "invitedId": "trickyBatata",
        ///         "gameType": "Custom"
        ///     }
        ///
        /// </remarks>
        /// <param name="request"></param>
        /// <returns>Confirmação da criação do invite.</returns>
        /// <response code="200">Confirmação da criação do invite.</response>
        /// <response code="401">Quem fez o pedido não tem sessão iniciada.</response>
        /// <response code="404">Utilizador que pretende convidar nao foi encontrado.</response>
        /// <response code="400">O valor do gameType tem de ser 'Custom' ou 'Matchmaking'</response>
        /// <response code="400">Não pode enviar para ele mesmo.</response>
        /// <response code="400">Os jogadores nao sao amigos.</response>
        /// <response code="500">Retorna se ocorrer algum problema interno no servidor.</response>
        [Authorize]
        [HttpPost("send")]
        public async Task<IActionResult> SendGameInvite([FromBody] GameInviteRequest request)
        {
            try
            {
                Player player = (Player)HttpContext.Items["SessionPlayer"];

                GameInvite invite = new GameInvite(request.roomId, request.invitedId, player.username, request.gameType);
                await _service.SendGameInvite(invite);
                return Ok(new Success("Invite send successfully."));
            }
            catch(SendToYourselfException e)
            {
                return BadRequest(new Error("SendYourself", e.Message));
            }
            catch(EntityNotFoundException e)
            {
                if (e is FriendNotFoundException)
                {
                    return BadRequest(new Error("FriendNotFound", e.Message));
                }
                return NotFound(new Error("UserNotFound", "User Not Found"));
            }
            catch(AlreadyExistsGameInvite e)
            {
                return BadRequest(new Error("AlreadyRequested", e.Message));
            }
            catch (Exception exc)
            {
                this._logger.LogError(exc, exc.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Apagar um convite de jogo.
        /// </summary>
        /// <remarks>
        /// Exemplo de um pedido:
        ///
        ///     DELETE /delete/324vvn34v5b0mv8ndf7sv
        ///
        /// </remarks>
        /// <param name="roomId"></param>
        /// <returns>Confirmação da eliminação do invite.</returns>
        /// <response code="200">Confirmação da eliminação do invite.</response>
        /// <response code="401">Quem fez o pedido não tem sessão iniciada.</response>
        /// <response code="404">Convite nao foi encontrado.</response>
        /// <response code="500">Retorna se ocorrer algum problema interno no servidor.</response>
        [Authorize]
        [HttpDelete("delete/{roomId}")]
        public async Task<IActionResult> DeleteGameInvite(string roomId)
        {
            try
            {
                Player player = (Player)HttpContext.Items["SessionPlayer"];

                await _service.DeleteGameInvite(player.username, roomId);
                return Ok(new Success("Invite deleted successfully."));
            }
            catch(GameInviteNotFoundException)
            {
                return NotFound(new Error("InviteNotFound", "Invite Not Found"));
            }
            catch (Exception exc)
            {
                this._logger.LogError(exc, exc.Message);
                return StatusCode(500);
            }
        }
    }
}