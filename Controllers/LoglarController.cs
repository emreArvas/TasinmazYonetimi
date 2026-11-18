
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasinmazYonetimi.Interfaces;

namespace TasinmazYonetimi.Controllers
{
    [ApiController]
    [Route("api/loglar")]
    [Authorize(Roles = "Admin")]
    public class LoglarController : ControllerBase
    {
        private readonly ILogService _logService;

        public LoglarController(ILogService logService)
        {
            _logService = logService;
        }

        // Sistemdeki bütün log kayıtlarını getir
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var loglar = await _logService.GetAllAsync();

            return Ok(loglar);
        }

        // Belirli bir kullanıcının log kayıtlarını getir
        [HttpGet("kullanici/{kullaniciId:int}")]
        public async Task<IActionResult> GetByKullaniciId(
            int kullaniciId)
        {
            if (kullaniciId <= 0)
            {
                return BadRequest(new
                {
                    message = "Geçerli bir kullanıcı ID'si giriniz."
                });
            }

            var loglar =
                await _logService.GetByKullaniciIdAsync(
                    kullaniciId);

            return Ok(loglar);
        }
    }
}
