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

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginDto dto)
        {
            var sonuc = await _authService.LoginAsync(dto);

            if (sonuc == null)
            {
                return Unauthorized(new
                {
                    message = "E-posta veya parola hatalı."
                });
            }

            return Ok(sonuc);
        }
    }
}
