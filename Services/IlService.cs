
using Microsoft.EntityFrameworkCore;
using TasinmazYonetimi.Data;
using TasinmazYonetimi.Interfaces;
using TasinmazYonetimi.Models;

namespace TasinmazYonetimi.Services
{
    public class IlService : IIlService
    {
        private readonly TasinmazYonetimiContext _context;

        public IlService(TasinmazYonetimiContext context)
        {
            _context = context;
        }

        public async Task<List<Iller>> GetAllAsync()
        {
            return await _context.Set<Iller>()
                .AsNoTracking()
                .OrderBy(i => i.Id)
                .ToListAsync();
        }

        public async Task<Iller?> GetByIdAsync(int id)
        {
            return await _context.Set<Iller>()
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);
        }
    }
}
