
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasinmazYonetimi.DTOs;
using TasinmazYonetimi.Interfaces;

namespace TasinmazYonetimi.Controllers
{
    [ApiController]
    [Route("api/tasinmazlar")]
    [Authorize]
    public class TasinmazlarController : ControllerBase
    {
        private readonly ITasinmazService _tasinmazService;

        public TasinmazlarController(
            ITasinmazService tasinmazService)
        {
            _tasinmazService = tasinmazService;
        }

        private int? GetKullaniciId()
        {
            var idDegeri = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return int.TryParse(idDegeri, out var id)
                ? id
                : null;
        }

        // Kullanıcının bütün taşınmazlarını getir
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var kullaniciId = GetKullaniciId();

            if (kullaniciId == null)
                return Unauthorized();

            var tasinmazlar =
                await _tasinmazService.GetAllByKullaniciIdAsync(
                    kullaniciId.Value);

            return Ok(tasinmazlar);
        }

        // ID'ye göre taşınmaz getir
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var kullaniciId = GetKullaniciId();

            if (kullaniciId == null)
                return Unauthorized();

            var tasinmaz = await _tasinmazService.GetByIdAsync(
                id, kullaniciId.Value);

            if (tasinmaz == null)
                return NotFound(new
                {
                    message = "Taşınmaz bulunamadı."
                });

            return Ok(tasinmaz);
        }

        // Yeni taşınmaz oluştur
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] TasinmazCreateDto dto)
        {
            var kullaniciId = GetKullaniciId();

            if (kullaniciId == null)
                return Unauthorized();

            try
            {
                var tasinmaz = await _tasinmazService.CreateAsync(
                    dto, kullaniciId.Value);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = tasinmaz.Id },
                    tasinmaz);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Taşınmaz güncelle
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id, [FromBody] TasinmazUpdateDto dto)
        {
            var kullaniciId = GetKullaniciId();

            if (kullaniciId == null)
                return Unauthorized();

            try
            {
                var basarili = await _tasinmazService.UpdateAsync(
                    id, dto, kullaniciId.Value);

                if (!basarili)
                    return NotFound(new
                    {
                        message = "Taşınmaz bulunamadı."
                    });

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Taşınmaz sil
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var kullaniciId = GetKullaniciId();

            if (kullaniciId == null)
                return Unauthorized();

            var basarili = await _tasinmazService.DeleteAsync(
                id, kullaniciId.Value);

            if (!basarili)
                return NotFound(new
                {
                    message = "Taşınmaz bulunamadı."
                });

            return NoContent();
        }


        // Birden fazla taşınmaz sil
        [HttpDelete]
        public async Task<IActionResult> DeleteMany(
            [FromQuery] List<int> ids)
        {
            var kullaniciId = GetKullaniciId();

            if (kullaniciId == null)
                return Unauthorized();

            if (ids == null || ids.Count == 0)
                return BadRequest(new
                {
                    message = "Silinecek taşınmaz seçilmedi."
                });

            var basarili = await _tasinmazService.DeleteManyAsync(
                ids, kullaniciId.Value);

            if (!basarili)
                return NotFound(new
                {
                    message = "Seçilen taşınmazlardan bazıları bulunamadı."
                });

            return NoContent();
        }

        // Taşınmazları filtrele ve sayfala
        [HttpGet("filtrele")]
        public async Task<ActionResult<PagedResultDto<TasinmazDto>>> GetFiltered(
            [FromQuery] TasinmazFilterDto filter)
        {
            var kullaniciId = GetKullaniciId();

            // JWT içerisinden kullanıcı ID'si alınamadıysa
            if (!kullaniciId.HasValue)
            {
                return Unauthorized(
                    "Kullanıcı kimliği doğrulanamadı.");
            }

            var sonuc = await _tasinmazService.GetFilteredAsync(
                filter,
                kullaniciId.Value);

            return Ok(sonuc);
        }


    }
}
