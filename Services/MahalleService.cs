
using Microsoft.EntityFrameworkCore;
using TasinmazYonetimi.Data;
using TasinmazYonetimi.Interfaces;
using TasinmazYonetimi.Models;

namespace TasinmazYonetimi.Services
{
    public class MahalleService : IMahalleService
    {
        private readonly TasinmazYonetimiContext _context;

        public MahalleService(TasinmazYonetimiContext context)
        {
            _context = context;
        }

        public async Task<List<Mahalleler>> GetAllAsync()
        {
            return await _context.Set<Mahalleler>()
                .AsNoTracking()
                .OrderBy(m => m.MahalleAdi)
                .ToListAsync();
        }

        public async Task<Mahalleler?> GetByIdAsync(int id)
        {
            return await _context.Set<Mahalleler>()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<Mahalleler>> GetByIlceIdAsync(int ilceId)
        {
            return await _context.Set<Mahalleler>()
                .AsNoTracking()
                .Where(m => m.IlceId == ilceId)
                .OrderBy(m => m.MahalleAdi)
                .ToListAsync();
        }
    }
}
