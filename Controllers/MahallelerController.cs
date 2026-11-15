
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasinmazYonetimi.Interfaces;

namespace TasinmazYonetimi.Controllers
{
    [ApiController]
    [Route("api/mahalleler")]
    [Authorize]
    public class MahallelerController : ControllerBase
    {
        private readonly IMahalleService _mahalleService;

        public MahallelerController(
            IMahalleService mahalleService)
        {
            _mahalleService = mahalleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var mahalleler =
                await _mahalleService.GetAllAsync();

            var sonuc = mahalleler.Select(m => new
            {
                m.Id,
                m.MahalleAdi,
                m.IlceId
            });

            return Ok(sonuc);
        }

        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var mahalle =
                await _mahalleService.GetByIdAsync(id);

            if (mahalle == null)
            {
                return NotFound(new
                {
                    message = "Mahalle bulunamadı."
                });
            }

            return Ok(new
            {
                mahalle.Id,
                mahalle.MahalleAdi,
                mahalle.IlceId
            });
        }

        // Seçilen ilçeye bağlı mahalleleri getir
        [HttpGet("ilceye-gore/{ilceId:int}")]
        public async Task<IActionResult> GetByIlceId(int ilceId)
        {
            var mahalleler =
                await _mahalleService.GetByIlceIdAsync(ilceId);

            var sonuc = mahalleler.Select(m => new
            {
                m.Id,
                m.MahalleAdi,
                m.IlceId
            });

            return Ok(sonuc);
        }
    }
}
