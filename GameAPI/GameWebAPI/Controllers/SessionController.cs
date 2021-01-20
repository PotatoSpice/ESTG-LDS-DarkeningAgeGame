using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GameWebAPI.Entities;
using GameWebAPI.Exceptions;
using GameWebAPI.Models.Auth;
using GameWebAPI.Models.Player;
using GameWebAPI.Models.Response;
using GameWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace GameWebAPI.Controllers
{
    [ApiController]
    [Route("session")]
    [Produces("application/json")]
    public class SessionController : ControllerBase
    {
        private readonly ILogger<SessionController> _logger;
        private readonly ISessionService _service;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        
        public SessionController(ILogger<SessionController> logger, IConfiguration config,
            ISessionService service, IEmailService emailService,IMapper mapper)
        {
            this._logger = logger;
            this._service = service;
            this._emailService = emailService;
            this._mapper = mapper;
            this._config = config;
        }

        /// <summary>
        /// Retorna os dados identificativos do utilizador em sessão, se houver alguma sessão iniciada.
        /// </summary>
        /// <returns>Dados sobre o utilizador em sessão.</returns>
        /// <response code="200">Formato da resposta:
        /// <code>
        /// {
        ///     "username" : "trickyBatata",
        ///     "email": "batatas@email.com",
        ///     "firstName" : "Spicy",
        ///     "lastName" : "Potatoes"
        /// }</code></response>
        /// <response code="401">Não existe uma sessão iniciada.</response>
        [Authorize]
        [HttpGet("session-user")]
        public ActionResult<PlayerModel> GetSessionPlayerData()
        {
            Player player = (Player) HttpContext.Items["SessionPlayer"];
            var response = _mapper.Map<PlayerModel>(player);
            return Ok(response);
        }

        /// <summary>
        /// Remove a cookie de sessão criada após um Sign In bem sucedido.
        /// </summary>
        /// <returns>Cookie removida com sucesso.</returns>
        /// <response code="200">Cookie removida com sucesso.</response>
        /// <response code="404">Não existe nenhum utilizador com a sessão iniciada.</response>
        [HttpPost("sign-out")]
        public ActionResult SignOut()
        {
            Player player = (Player) HttpContext.Items["SessionPlayer"];
            if (player != null)
            {
                Response.Cookies.Delete("auth-token"); // send delete httpOnly cookie response
                return Ok(new Success("Session cookie cleared successfully!"));
            }
            else
            {
                return NotFound(new Error("NoSession", "No player is in session at the moment."));
            }
        }

        /// <summary>
        /// Autentica um jogador. É gerado um token JWT para sessão, retornado pela resposta ao mesmo 
        /// tempo que é criada uma cookie.
        /// </summary>
        /// <returns>Sessão criada com sucesso.</returns>
        /// <response code="200">Sessão criada com sucesso.</response>
        /// <response code="401">Username ou palavra passe inválidos.</response>
        /// <response code="500">Retorna se ocorrer algum problema interno no servidor.</response>
        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] AuthRequest loginData)
        {
            try
            {
                var player = await _service.SignIn(loginData.username, loginData.password);

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["AppSettings:JwtSettings:Secret"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Email, player.email),
                        new Claim(ClaimTypes.Name, player.username)
                    }),
                    Expires = DateTime.UtcNow.AddHours(12),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                Response.Cookies.Append(
                    "auth-token", tokenString, 
                    new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddHours(12),
                        HttpOnly = true,
                        SameSite = SameSiteMode.Lax
                    });
                
                var playerDTO = _mapper.Map<PlayerDTO>(player);
                return Ok(new AuthResponse(tokenString, playerDTO));
            }
            catch (EntityNotFoundException)
            { // always return the same error response to prevent user enumeration
                return Unauthorized(new Error("LoginFailure", "Invalid credentials."));
            }
            catch (InvalidPasswordException)
            { // always return the same error response to prevent user enumeration
                return Unauthorized(new Error("LoginFailure", "Invalid credentials."));
            }
            catch (Exception exc)
            {
                this._logger.LogError(exc, exc.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Regista um novo jogador.
        /// </summary>
        /// <returns>Conta de utilizador registada com sucesso.</returns>
        /// <response code="200">Conta de utilizador registada com sucesso.</response>
        /// <response code="404">Username já existe na base de dados.</response>
        /// <response code="404">Email já existe na base de dados.</response>
        /// <response code="500">Retorna se ocorrer algum problema interno no servidor.</response>
        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest registerData)
        {
            try
            {
                var data = _mapper.Map<Player>(registerData);

                var player = await _service.SignUp(data, registerData.password);

                var playerDTO = _mapper.Map<PlayerDTO>(player);
                return Ok(playerDTO);
            }
            catch (EntityAlreadyExistsException exc)
            {
                if (exc is AlreadyExistsEmailException) {
                    exc = (AlreadyExistsEmailException) exc;
                    return NotFound(new Error("EmailExists", exc.Message));
                } else if (exc is AlreadyExistsUsernameException) {
                    exc = (AlreadyExistsUsernameException) exc;
                    return NotFound(new Error("UsernameExists", exc.Message));
                } else {
                    return NotFound(new Error("AlreadyExists", exc.Message));
                }
            }
            catch (Exception exc)
            {
                this._logger.LogError(exc, exc.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Realiza um pedido para atualização de password. É enviado um email para o endereço disponibilizado.
        /// </summary>
        /// <returns>Email para atualização de password enviado com sucesso.</returns>
        /// <response code="200">Email para atualização de password enviado com sucesso.</response>
        /// <response code="500">Retorna se ocorrer algum problema interno no servidor.</response>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest requester)
        {
            try
            {
                var player = await _service.CreatePasswordResetToken(requester.email);

                if (player != null)
                    _emailService.SendPasswordResetEmail(
                        player.email,"Darkening Age API - Reset Password",
                        player.pwdResetToken, Request.Headers["Origin"]
                    );
                // always return ok response to prevent user enumeration
                return Ok(new Success("Check your email for password reset instructions"));
            }
            catch (Exception exc)
            {
                this._logger.LogError(exc, exc.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Atualiza a password para um utilizador a partir de um token gerado por um pedido de atualização prévio.
        /// </summary>
        /// <returns>Password atualizada com sucesso.</returns>
        /// <response code="200">Password atualizada com sucesso.</response>
        /// <response code="404">Token para atualização de password inválido.</response>
        /// <response code="500">Retorna se ocorrer algum problema interno no servidor.</response>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest resetData)
        {
            try
            {
                var player = await _service.UpdatePasswordReset(resetData.token, resetData.password);

                return Ok(new Success("PasswordResetSuccessful", player.email));
            }
            catch (InvalidPasswordException exc)
            {
                return NotFound(new Error("InvalidResetToken", exc.Message));
            }
            catch (Exception exc)
            {
                this._logger.LogError(exc, exc.Message);
                return StatusCode(500);
            }
        }
    }
}