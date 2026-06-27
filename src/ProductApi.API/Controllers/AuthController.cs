using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;

namespace ProductApi.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        /// <summary>
        /// Demo login endpoint. Accepts any non-empty username/password and issues a JWT.
        /// In a real system this would validate against a Users table with hashed passwords.
        /// </summary>
        [HttpPost("login")]
        public ActionResult<LoginResponseDto> Login([FromBody] LoginRequestDto request)
        {
            // NOTE: For assessment purposes, credentials are not checked against a DB.
            // Replace with real user validation before production use.
            var (token, expiresAt) = _tokenService.GenerateToken(request.Username);

            return Ok(new LoginResponseDto { Token = token, ExpiresAt = expiresAt });
        }
    }
}
