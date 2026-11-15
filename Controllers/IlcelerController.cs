
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasinmazYonetimi.Interfaces;

namespace TasinmazYonetimi.Controllers
{
    [ApiController]
    [Route("api/ilceler")]
    [Authorize]
    public class IlcelerController : ControllerBase
    {
        private readonly IIlceService _ilceService;

        public IlcelerController(IIlceService ilceService)
        {
            _ilceService = ilceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var ilceler = await _ilceService.GetAllAsync();

            var sonuc = ilceler.Select(i => new
            {
                i.Id,
                i.IlceAdi,
                i.IlId
            });

            return Ok(sonuc);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ilce = await _ilceService.GetByIdAsync(id);

            if (ilce == null)
                return NotFound(new
                {
                    message = "İlçe bulunamadı."
                });

            return Ok(new
            {
                ilce.Id,
                ilce.IlceAdi,
                ilce.IlId
            });
        }

        [HttpGet("ile-gore/{ilId:int}")]
        public async Task<IActionResult> GetByIlId(int ilId)
        {
            var ilceler =
                await _ilceService.GetByIlIdAsync(ilId);

            var sonuc = ilceler.Select(i => new
            {
                i.Id,
                i.IlceAdi,
                i.IlId
            });

            return Ok(sonuc);
        }
    }
}
