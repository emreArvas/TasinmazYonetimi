using TasinmazYonetimi.Models;

namespace TasinmazYonetimi.Interfaces
{
    public interface IMahalleService
    {
        Task<List<Mahalleler>> GetAllAsync();

        Task<Mahalleler?> GetByIdAsync(int id);

        Task<List<Mahalleler>> GetByIlceIdAsync(int ilceId);
        
    }
}
