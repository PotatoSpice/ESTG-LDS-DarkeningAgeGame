using AutoMapper;
using GameWebAPI.Entities;
using GameWebAPI.Exceptions;
using GameWebAPI.Models.PlayerMatchdata;
using GameWebAPI.Models.Response;
using GameWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GameWebAPI.Controllers
{

    [ApiController]
    [Route("match-history")]
    [Produces("application/json")]
    public class PlayerMatchdataController : ControllerBase
    {
        private readonly ILogger<PlayerMatchdataController> _logger;
        private readonly IMatchdataService _service;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public PlayerMatchdataController(IMatchdataService service, IConfiguration config,
            ILogger<PlayerMatchdataController> logger, IMapper mapper)
        {
            this._logger = logger;
            this._service = service;
            this._mapper = mapper;
            this._config = config;
        }

        /// <summary>
        /// Endpoint para adição de novas partidas ao histórico de um jogador.
        /// 
        /// Este endpoint só deve ser acedido pelo servidor do jogo, responsável por toda a lógica relativamente aos jogos.
        /// </summary>
        /// <returns>Pedido de amizade criado com sucesso.</returns>
        /// <response code="200">Dados criados com sucesso</response>
        /// <response code="401">Adição de novas entradas de histórico autorizado pelo servidor de jogo.</response>
        /// <response code="500">Retorna se ocorrer algum problema interno no servidor.</response>
        [HttpPost ("save-match")]
        public async Task<IActionResult> SaveMatchData([FromBody] PlayerMatchdataRequest data)
        {
            string key = Request.Headers["Authorization"], check = _config["AppSettings:GameServerKey"];
            if (string.IsNullOrEmpty(key) || !key.Equals(check)) 
            {
                return Unauthorized(new Error("InvalidCredentials", "Only the GameServer is able to add new matchdata entries!")); 
            }
            PlayerMatchdata playerMatchdata = new PlayerMatchdata();
            playerMatchdata.gameID = data.gameID;
            playerMatchdata.playerId = data.playerId;
            playerMatchdata.placement = data.placement;
            playerMatchdata.armiesCreated = data.armiesCreated;
            playerMatchdata.regionsConquered = data.regionsConquered;
            playerMatchdata.date = DateTime.Now;
            try
            {
               await _service.SavePlayerMatchdata(playerMatchdata);
               return Ok(new Success("Request saved successfuly")); 
            }
            catch (Exception exc)
            {
                this._logger.LogError(exc, exc.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Obtêm o histórico de partidas de um player com sessão iniciada
        /// </summary>
        /// <returns>Lista de partidas efetuadas pelo jogador.</returns>
        /// <response code="200">Lista de partidas efetuadas pelo jogador</response>
        /// <response code="401">Quem fez o pedido não tem sessão iniciada.</response>
        /// <response code="500">Retorna se ocorrer algum problema interno no servidor.</response>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetPlayerHistory([FromQuery(Name = "nview")] int nview)
        {
            if (nview <= 0)
                nview = int.MaxValue;
            try
            {
                Player player = (Player)HttpContext.Items["SessionPlayer"];

                return Ok(await _service.GetPlayerMatchdata(player.username, nview));
            }
            catch (Exception exc)
            {
                this._logger.LogError(exc, exc.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Obtêm o histórico de partidas de um determinado player 
        /// </summary>
        /// <returns>Lista de partidas efetuadas pelo jogador.</returns>
        /// <response code="200">Lista de partidas efetuadas pelo jogador</response>
        /// <response code="401">Quem fez o pedido não tem sessão iniciada.</response>
        /// <response code="404">O jogador de quem se fez o pedido não existe</response>
        /// <response code="500">Retorna se ocorrer algum problema interno no servidor.</response>
        [Authorize]
        [HttpGet("player/{playerId}")]
        public async Task<IActionResult> getPlayerHistory(string playerId, [FromQuery(Name = "nview")] int nview)
        {
            if (nview <= 0)
                nview = 1;
            try
            {
                return Ok(await _service.GetPlayerMatchdata(playerId, nview));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(new Error("PlayerNotFound", "User Not Found"));
            }
            catch (Exception exc)
            {
                this._logger.LogError(exc, exc.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Obtêm os dados relativamente a uma partida decorrida numa sala em específico.
        /// </summary>
        /// <returns>Dados para uma partida em específico.</returns>
        /// <response code="200">Dados para uma partida em específico.</response>
        /// <response code="401">Quem fez o pedido não tem sessão iniciada.</response>
        /// <response code="500">Retorna se ocorrer algum problema interno no servidor.</response>
        [Authorize]
        [HttpGet("id/{roomID}")]
        public async Task<IActionResult> GetRoomDetails(string roomID)
        {
            try
            {
                return Ok(await _service.GetGameMatchdata(roomID));
            }
            catch (Exception exc)
            {
                this._logger.LogError(exc, exc.Message);
                return StatusCode(500);
            }
        }

    }
}
