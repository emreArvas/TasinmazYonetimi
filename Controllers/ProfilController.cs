
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasinmazYonetimi.DTOs;
using TasinmazYonetimi.Interfaces;

namespace TasinmazYonetimi.Controllers
{
    [ApiController]
    [Route("api/profil")]
    [Authorize]
    public class ProfilController : ControllerBase
    {
        private readonly IKullaniciService _kullaniciService;

        public ProfilController(
            IKullaniciService kullaniciService)
        {
            _kullaniciService = kullaniciService;
        }

        private int? GetKullaniciId()
        {
            var idDegeri = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return int.TryParse(idDegeri, out var id)
                ? id
                : null;
        }

        // Giriş yapan kullanıcının profilini getir
        [HttpGet]
        public async Task<IActionResult> GetProfil()
        {
            var kullaniciId = GetKullaniciId();

            if (kullaniciId == null)
                return Unauthorized();

            var kullanici = await _kullaniciService.GetByIdAsync(
                kullaniciId.Value);

            if (kullanici == null)
                return NotFound(new
                {
                    message = "Kullanıcı bulunamadı."
                });

            return Ok(kullanici);
        }

        // Giriş yapan kullanıcının profilini güncelle
        [HttpPut]
        public async Task<IActionResult> UpdateProfil(
            [FromBody] ProfilUpdateDto dto)
        {
            var kullaniciId = GetKullaniciId();

            if (kullaniciId == null)
                return Unauthorized();

            try
            {
                var basarili =
                    await _kullaniciService.UpdateProfilAsync(
                        kullaniciId.Value, dto);

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
    }
}
