using TasinmazYonetimi.DTOs;

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
    }
}
