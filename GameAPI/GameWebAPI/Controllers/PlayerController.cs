using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GameWebAPI.Entities;
using GameWebAPI.Exceptions;
using GameWebAPI.Models.Player;
using GameWebAPI.Models.Response;
using GameWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameWebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("players")]
    public class PlayerController : ControllerBase
    {
        private readonly ILogger<PlayerController> _logger;
        private readonly IPlayerService _service;
        private readonly IMapper _mapper;

        public PlayerController(ILogger<PlayerController> logger, IPlayerService service, IMapper mapper)
        {
            this._logger = logger;
            this._service = service;
            this._mapper = mapper;
        }

        /// <summary>
        /// Obtêm uma lista com a identificação de todos os jogadores inscritos na aplicação.
        /// </summary>
        /// <returns>Lista de jogadores inscritos na aplicação, nome identificativo e alguns dados adicionais não sensíveis</returns>
        /// <response code="200">Exemplo de cada elemento da lista:
        /// <code>
        /// [{
        ///     "username" : "trickyBatata",
        ///     "firstName" : "Spicy",
        ///     "lastName" : "Mania"
        /// }]</code></response>
        /// <response code="401">Quem fez o pedido não tem sessão iniciada.</response>
        /// <response code="500">Retorna se ocorrer algum problema interno no servidor.</response>
        [HttpGet]
        public async Task<ActionResult<ICollection<ProfileDTO>>> GetPlayers()
        {
            try
            {
                var players = await _service.GetPlayers();
                ICollection<ProfileDTO> result = new List<ProfileDTO>();
                foreach (Player player in players)
                {
                    result.Add(_mapper.Map<ProfileDTO>(player));
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                this._logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }
        
        /// <summary>
        /// Consultar dados de perfil para um determinado jogador.
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Dados de perfil sobre o jogador requerido.</returns>
        /// <response code="200">Formato da resposta:
        /// <code>
        /// {
        ///     "username" : "trickyBatata",
        ///     "firstName" : "Spicy",
        ///     "lastName" : "Mania"
        /// }</code></response>
        /// <response code="404">O utilzador nao foi encontrado</response>
        /// <response code="500">Retorna se ocorrer algum problema interno no servidor.</response>
        [HttpGet("profile/{username}")]
        public async Task<ActionResult<ProfileDTO>> GetProfile(string username){
            try
            {
                Player player = await this._service.GetByUsername(username);
                return Ok(_mapper.Map<ProfileDTO>(player));
            }
            catch(EntityNotFoundException)
            {
                return NotFound(new Error("UserNotFound", "User Not Found"));
            }
            catch (Exception e)
            {
                this._logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Atualizar o perfil do jogador que tem sessao iniciada
        /// </summary>
        /// <remarks>
        /// Exemplo de um pedido:
        ///
        ///     PUT session/update
        ///     {
        ///         "currentPassword" : "dsfukjihbdsfa",
        ///         "newPassword" : "xcbvxcb",
        ///         "email" : "asdasda@gmail.com",
        ///         "firstName" : "",
        ///         "lastName" : ""
        ///     }    
        ///
        /// </remarks>
        /// <param name="updateInfo"></param>
        /// <returns>Informaçoes atualizadas</returns>
        /// <response code="200">Formato da resposta:
        /// <code>
        /// {
        ///     "username" : "trickyBatata",
        ///     "email" : "asdasda@gmail.com"
        /// }</code></response>
        /// <response code="404">O utilzador nao foi encontrado.</response>
        /// <response code="400">A password fornecida e a real nao coincidem.</response>
        /// <response code="500">Retorna se ocorrer algum problema interno no servidor.</response>
        [Authorize]
        [HttpPut("update")]
        public async Task<ActionResult<PlayerDTO>> UpdateProfile([FromBody] UpdatePlayerRequest updateInfo){
            try
            {
                Player player = (Player)HttpContext.Items["SessionPlayer"];
                
                return Ok(_mapper.Map<PlayerDTO>(await this._service.Update(player.username, updateInfo)));
            }
            catch(EntityNotFoundException)
            {
                return NotFound(new Error("UserNotFound", "User Not Found."));
            }
            catch(PasswordsDontMatch e)
            {
                return BadRequest(new Error("PasswordsDontMatch", e.Message));;
            }
            catch(Exception e)
            {
                this._logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Atualizar estado do jogador com sessao iniciada
        /// </summary>
        /// <remarks>
        /// Exemplo de um pedido:
        ///
        ///     PUT session/update-status
        ///     {
        ///         "status" : true,
        ///     }    
        ///
        /// </remarks>
        /// <param name="playerStatus"></param>
        /// <returns></returns>
        /// <response code="200"></response>
        /// <response code="500">Retorna se ocorrer algum problema interno no servidor.</response>
        [Authorize]
        [HttpPut("update-status")]
        public async Task<ActionResult> UpdateStatus([FromBody] PlayerStatusRequest playerStatus){
            try{
                Player player = (Player)HttpContext.Items["SessionPlayer"];
                await _service.ChangeUserStatus(player, playerStatus);
                return Ok();
            }catch(Exception e){
                this._logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }
    }
}