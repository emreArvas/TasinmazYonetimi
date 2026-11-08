
using Microsoft.EntityFrameworkCore;
using TasinmazYonetimi.Data;
using TasinmazYonetimi.Interfaces;
using TasinmazYonetimi.Models;

namespace TasinmazYonetimi.Services
{
    public class IlceService : IIlceService
    {
        private readonly TasinmazYonetimiContext _context;

        public IlceService(TasinmazYonetimiContext context)
        {
            _context = context;
        }

        public async Task<List<Ilceler>> GetAllAsync()
        {
            return await _context.Set<Ilceler>()
                .AsNoTracking()
                .OrderBy(i => i.IlceAdi)
                .ToListAsync();
        }

        public async Task<Ilceler?> GetByIdAsync(int id)
        {
            return await _context.Set<Ilceler>()
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<Ilceler>> GetByIlIdAsync(int ilId)
        {
            return await _context.Set<Ilceler>()
                .AsNoTracking()
                .Where(i => i.IlId == ilId)
                .OrderBy(i => i.IlceAdi)
                .ToListAsync();
        }
    }
}
