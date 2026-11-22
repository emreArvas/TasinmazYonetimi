
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasinmazYonetimi.DTOs;
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

        // Logları filtrele ve sayfala
        [HttpGet("filtrele")]
        public async Task<ActionResult<PagedResultDto<LogDto>>> GetFiltered(
            [FromQuery] LogFilterDto filter)
        {
            var sonuc = await _logService.GetFilteredAsync(filter);

            return Ok(sonuc);
        }

        // Filtreye uyan logları Excel olarak indir
        [HttpGet("excel")]
        public async Task<IActionResult> ExportExcel(
            [FromQuery] LogFilterDto filter)
        {
            var dosya = await _logService.ExcelOlusturAsync(filter);

            return File(
                dosya,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Loglar_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        }

        // Durum listesi (Başarılı / Başarısız)
        [HttpGet("durumlar")]
        public async Task<IActionResult> GetDurumlar()
        {
            var durumlar = await _logService.GetDurumlarAsync();

            return Ok(durumlar.Select(d => new
            {
                d.Id,
                d.DurumAdi
            }));
        }

        // İşlem tipi listesi (Yeni Kayıt, Güncelleme, Silme, ...)
        [HttpGet("islem-tipleri")]
        public async Task<IActionResult> GetIslemTipleri()
        {
            var islemTipleri = await _logService.GetIslemTipleriAsync();

            return Ok(islemTipleri.Select(i => new
            {
                i.Id,
                i.IslemTipiAdi
            }));
        }
    }
}
