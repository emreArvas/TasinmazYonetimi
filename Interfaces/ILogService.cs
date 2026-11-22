using TasinmazYonetimi.DTOs;
using TasinmazYonetimi.Models;

namespace TasinmazYonetimi.Interfaces
{
    public interface ILogService
    {
        Task<List<LogDto>> GetAllAsync();

        Task<List<LogDto>> GetByKullaniciIdAsync(
            int kullaniciId);

        Task<PagedResultDto<LogDto>> GetFilteredAsync(
            LogFilterDto filter);

        Task<byte[]> ExcelOlusturAsync(LogFilterDto filter);

        Task<List<Durumlar>> GetDurumlarAsync();

        Task<List<IslemTipleri>> GetIslemTipleriAsync();

        Task AddLogAsync(
            int? kullaniciId,
            string islemTipi,
            string durum,
            string? aciklama,
            string? kullaniciIp);
    }
}
