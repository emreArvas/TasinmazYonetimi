using TasinmazYonetimi.DTOs;
using TasinmazYonetimi.Models;

namespace TasinmazYonetimi.Interfaces
{
    public interface IKullaniciService
    {
        Task<List<KullaniciDto>> GetAllAsync();

        Task<KullaniciDto?> GetByIdAsync(int id);

        Task<KullaniciDto> CreateAsync(
            KullaniciCreateDto dto);

        Task<bool> UpdateAsync(
            int id, KullaniciUpdateDto dto);

        Task<bool> DeleteAsync(int id);

        Task<List<Roller>> GetRollerAsync();
        Task<bool> UpdateProfilAsync(
    int kullaniciId, ProfilUpdateDto dto);
    }
}
