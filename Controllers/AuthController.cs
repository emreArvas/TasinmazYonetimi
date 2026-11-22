
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasinmazYonetimi.DTOs;
using TasinmazYonetimi.Interfaces;

namespace TasinmazYonetimi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // Kullanıcı giriş işlemi
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginDto dto)
        {
            var sonuc = await _authService.LoginAsync(
                dto, HttpContext.Connection.RemoteIpAddress?.ToString());

            if (sonuc == null)
            {
                return Unauthorized(new
                {
                    message = "E-posta veya parola hatalı."
                });
            }

            return Ok(sonuc);
        }

        // Kullanıcı çıkış işlemi
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var idDegeri = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tokenId = User.FindFirstValue(JwtRegisteredClaimNames.Jti);
            var expDegeri = User.FindFirstValue(JwtRegisteredClaimNames.Exp);

            if (!int.TryParse(idDegeri, out var kullaniciId) ||
                tokenId == null ||
                !long.TryParse(expDegeri, out var exp))
                return Unauthorized();

            await _authService.LogoutAsync(
                kullaniciId,
                tokenId,
                DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime,
                HttpContext.Connection.RemoteIpAddress?.ToString());

            return Ok(new { message = "Çıkış işlemi başarılı." });
        }
    }
}
