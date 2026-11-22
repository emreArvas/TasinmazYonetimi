
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

        // Ortak log filtre sorgusu (liste ve Excel aynı filtreyi kullanır)
        private IQueryable<Loglar> FiltreliSorgu(LogFilterDto filter)
        {
            var query = _context.Set<Loglar>()
                .AsNoTracking()
                .AsQueryable();

            if (filter.KullaniciId.HasValue)
                query = query.Where(l => l.KullaniciId == filter.KullaniciId.Value);

            if (filter.DurumId.HasValue)
                query = query.Where(l => l.DurumId == filter.DurumId.Value);

            if (filter.IslemTipiId.HasValue)
                query = query.Where(l => l.IslemTipiId == filter.IslemTipiId.Value);

            // Açıklama: büyük/küçük harf duyarsız arama (PostgreSQL ILIKE)
            if (!string.IsNullOrWhiteSpace(filter.Aciklama))
            {
                var aciklama = $"%{filter.Aciklama.Trim()}%";

                query = query.Where(l =>
                    l.Aciklama != null &&
                    EF.Functions.ILike(l.Aciklama, aciklama));
            }

            if (!string.IsNullOrWhiteSpace(filter.KullaniciIp))
            {
                var ip = filter.KullaniciIp.Trim();

                query = query.Where(l =>
                    l.KullaniciIp != null &&
                    l.KullaniciIp.Contains(ip));
            }

            if (filter.BaslangicTarihi.HasValue)
            {
                var baslangic = DateTime.SpecifyKind(
                    filter.BaslangicTarihi.Value.Date, DateTimeKind.Utc);

                query = query.Where(l => l.TarihSaat >= baslangic);
            }

            if (filter.BitisTarihi.HasValue)
            {
                // Bitiş gününü de dahil etmek için ertesi günün başına kadar
                var bitis = DateTime.SpecifyKind(
                    filter.BitisTarihi.Value.Date.AddDays(1), DateTimeKind.Utc);

                query = query.Where(l => l.TarihSaat < bitis);
            }

            return query;
        }

        // Logları filtrele ve sayfala (en yeni kayıt en üstte)
        public async Task<PagedResultDto<LogDto>> GetFilteredAsync(
            LogFilterDto filter)
        {
            var query = FiltreliSorgu(filter);

            var totalCount = await query.CountAsync();

            var pageNumber = Math.Max(1, filter.PageNumber);
            var pageSize = Math.Clamp(filter.PageSize, 1, 100);

            var loglar = await query
                .OrderByDescending(l => l.TarihSaat)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(l => l.Durum)
                .Include(l => l.IslemTipi)
                .Include(l => l.Kullanici)
                .ToListAsync();

            return new PagedResultDto<LogDto>
            {
                Items = _mapper.Map<List<LogDto>>(loglar),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        // Filtre combobox'ları için durum listesi
        public async Task<List<Durumlar>> GetDurumlarAsync()
        {
            return await _context.Set<Durumlar>()
                .AsNoTracking()
                .OrderBy(d => d.Id)
                .ToListAsync();
        }

        // Filtre combobox'ları için işlem tipi listesi
        public async Task<List<IslemTipleri>> GetIslemTipleriAsync()
        {
            return await _context.Set<IslemTipleri>()
                .AsNoTracking()
                .OrderBy(i => i.Id)
                .ToListAsync();
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
