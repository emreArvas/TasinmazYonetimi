using TasinmazYonetimi.Models;

namespace TasinmazYonetimi.Interfaces
{
    public interface IIlService
    {
        Task<List<Iller>> GetAllAsync();
        Task<Iller?> GetByIdAsync(int id);
    }
}
