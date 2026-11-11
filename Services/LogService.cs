
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TasinmazYonetimi.Data;
using TasinmazYonetimi.DTOs;
using TasinmazYonetimi.Interfaces;
using TasinmazYonetimi.Models;

namespace TasinmazYonetimi.Services
{
    public class LogService : ILogService
    {
        private readonly TasinmazYonetimiContext _context;
        private readonly IMapper _mapper;

        public LogService(
            TasinmazYonetimiContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Bütün log kayıtlarını getir
        public async Task<List<LogDto>> GetAllAsync()
        {
            var loglar = await _context.Set<Loglar>()
                .AsNoTracking()
                .Include(l => l.Durum)
                .Include(l => l.IslemTipi)
                .OrderByDescending(l => l.TarihSaat)
                .ToListAsync();

            return _mapper.Map<List<LogDto>>(loglar);
        }

        // Kullanıcıya ait log kayıtlarını getir
        public async Task<List<LogDto>> GetByKullaniciIdAsync(
            int kullaniciId)
        {
            var loglar = await _context.Set<Loglar>()
                .AsNoTracking()
                .Include(l => l.Durum)
                .Include(l => l.IslemTipi)
                .Where(l => l.KullaniciId == kullaniciId)
                .OrderByDescending(l => l.TarihSaat)
                .ToListAsync();

            return _mapper.Map<List<LogDto>>(loglar);
        }

        // Yeni log kaydı oluştur
        public async Task AddLogAsync(
            int? kullaniciId,
            string islemTipi,
            string durum,
            string? aciklama,
            string? kullaniciIp)
        {
            var islem = await _context.Set<IslemTipleri>()
                .FirstOrDefaultAsync(i =>
                    i.IslemTipiAdi == islemTipi);

            if (islem == null)
                throw new ArgumentException(
                    "İşlem tipi bulunamadı.");

            var durumKaydi = await _context.Set<Durumlar>()
                .FirstOrDefaultAsync(d =>
                    d.DurumAdi == durum);

            if (durumKaydi == null)
                throw new ArgumentException(
                    "İşlem durumu bulunamadı.");

            var log = new Loglar
            {
                KullaniciId = kullaniciId,
                IslemTipiId = islem.Id,
                DurumId = durumKaydi.Id,
                Aciklama = aciklama,
                KullaniciIp = kullaniciIp,
                TarihSaat = DateTime.UtcNow
            };

            _context.Set<Loglar>().Add(log);

            await _context.SaveChangesAsync();
        }
    }
}
