using TasinmazYonetimi.DTOs;

namespace TasinmazYonetimi.Interfaces
{
    public interface ITasinmazService
    {
        Task<List<TasinmazDto>> GetAllByKullaniciIdAsync(
            int kullaniciId);

        Task<TasinmazDto?> GetByIdAsync(
            int id, int kullaniciId);

        Task<TasinmazDto> CreateAsync(
            TasinmazCreateDto dto, int kullaniciId);

        Task<bool> UpdateAsync(
            int id, TasinmazUpdateDto dto, int kullaniciId);

        Task<bool> DeleteAsync(
            int id, int kullaniciId);
    }
}
