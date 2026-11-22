
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
        private readonly IKullaniciService _kullaniciService;

        public AuthController(
            IAuthService authService,
            IKullaniciService kullaniciService)
        {
            _authService = authService;
            _kullaniciService = kullaniciService;
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

        // Yeni kullanıcı kayıt işlemi
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] KullaniciCreateDto dto)
        {
            try
            {
                // Yeni hesaplar normal kullanıcı rolüyle açılır.
                dto.RolId = 2;

                var kullanici =
                    await _kullaniciService.CreateAsync(dto);

                return StatusCode(201, new
                {
                    message = "Kullanıcı başarıyla oluşturuldu.",
                    kullaniciId = kullanici.Id
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
