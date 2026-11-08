using TasinmazYonetimi.Models;

namespace TasinmazYonetimi.Interfaces
{
    public interface IIlceService
    {
        Task<List<Ilceler>> GetAllAsync();

        Task<Ilceler?> GetByIdAsync(int id);

        Task<List<Ilceler>> GetByIlIdAsync(int ilId);
    }
}
