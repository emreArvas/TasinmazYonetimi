
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasinmazYonetimi.DTOs;
using TasinmazYonetimi.Interfaces;

namespace TasinmazYonetimi.Controllers
{
    [ApiController]
    [Route("api/kullanicilar")]
    [Authorize(Roles = "Admin")]
    public class KullanicilarController : ControllerBase
    {
        private readonly IKullaniciService _kullaniciService;

        public KullanicilarController(
            IKullaniciService kullaniciService)
        {
            _kullaniciService = kullaniciService;
        }

        // Bütün kullanıcıları getir
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var kullanicilar =
                await _kullaniciService.GetAllAsync();

            return Ok(kullanicilar);
        }

        // ID'ye göre kullanıcı getir
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var kullanici =
                await _kullaniciService.GetByIdAsync(id);

            if (kullanici == null)
                return NotFound(new
                {
                    message = "Kullanıcı bulunamadı."
                });

            return Ok(kullanici);
        }

        // Yeni kullanıcı oluştur
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] KullaniciCreateDto dto)
        {
            try
            {
                var kullanici =
                    await _kullaniciService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = kullanici.Id },
                    kullanici);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // Kullanıcı güncelle
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] KullaniciUpdateDto dto)
        {
            try
            {
                var basarili =
                    await _kullaniciService.UpdateAsync(
                        id, dto);

                if (!basarili)
                    return NotFound(new
                    {
                        message = "Kullanıcı bulunamadı."
                    });

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // Kullanıcı sil
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var basarili =
                    await _kullaniciService.DeleteAsync(id);

                if (!basarili)
                    return NotFound(new
                    {
                        message = "Kullanıcı bulunamadı."
                    });

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // Kullanıcıları filtrele ve sayfala
        [HttpGet("filtrele")]
        public async Task<ActionResult<PagedResultDto<KullaniciDto>>> GetFiltered(
            [FromQuery] KullaniciFilterDto filter)
        {
            var sonuc = await _kullaniciService.GetFilteredAsync(filter);

            return Ok(sonuc);
        }

        // Birden fazla kullanıcı sil
        [HttpDelete]
        public async Task<IActionResult> DeleteMany(
            [FromQuery] List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return BadRequest(new { message = "Silinecek kullanıcı seçilmedi." });

            try
            {
                var basarili = await _kullaniciService.DeleteManyAsync(ids);

                if (!basarili)
                    return NotFound(new { message = "Seçilen kullanıcılardan bazıları bulunamadı." });

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Rol listesi (Admin / Kullanici)
        [HttpGet("roller")]
        public async Task<IActionResult> GetRoller()
        {
            var roller = await _kullaniciService.GetRollerAsync();

            return Ok(roller.Select(r => new
            {
                r.Id,
                r.RolAdi
            }));
        }
    }
}
