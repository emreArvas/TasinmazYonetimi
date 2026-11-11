using TasinmazYonetimi.DTOs;

namespace TasinmazYonetimi.Interfaces
{
    public interface ILogService
    {
        Task<List<LogDto>> GetAllAsync();

        Task<List<LogDto>> GetByKullaniciIdAsync(
            int kullaniciId);

        Task AddLogAsync(
            int? kullaniciId,
            string islemTipi,
            string durum,
            string? aciklama,
            string? kullaniciIp);
    }
}
