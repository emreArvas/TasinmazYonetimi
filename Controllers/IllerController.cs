using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasinmazYonetimi.Interfaces;

namespace TasinmazYonetimi.Controllers
{
    [ApiController]
    [Route("api/iller")]
    [Authorize]
    public class IllerController:ControllerBase
    {
        private readonly IIlService _ilService;

        public IllerController(IIlService ilService)
        {
            _ilService = ilService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var iller = await _ilService.GetAllAsync();

            var sonuc = iller.Select(i => new
            {
                i.Id,
                i.IlAdi
            });

            return Ok(sonuc);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var il = await _ilService.GetByIdAsync(id);

            if (il == null)
                return NotFound(new
                {
                    message = "İl bulunamadı."
                });

            return Ok(new
            {
                il.Id,
                il.IlAdi
            });
        }
    }
}
