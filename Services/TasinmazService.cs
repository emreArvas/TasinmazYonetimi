
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TasinmazYonetimi.Constants;
using TasinmazYonetimi.Data;
using TasinmazYonetimi.DTOs;
using TasinmazYonetimi.Interfaces;
using TasinmazYonetimi.Models;

namespace TasinmazYonetimi.Services
{
    public class TasinmazService : ITasinmazService
    {
        private readonly TasinmazYonetimiContext _context;
        private readonly IMapper _mapper;
        private readonly ILogService _logService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IExcelService _excelService;

        public TasinmazService(
            TasinmazYonetimiContext context,
            IMapper mapper,
            ILogService logService,
            IHttpContextAccessor httpContextAccessor,
            IExcelService excelService)
        {
            _context = context;
            _mapper = mapper;
            _logService = logService;
            _httpContextAccessor = httpContextAccessor;
            _excelService = excelService;
        }

        // İstek yapan kullanıcının IP adresini getir
        private string? GetKullaniciIp()
        {
            return _httpContextAccessor.HttpContext?
                .Connection.RemoteIpAddress?.ToString();
        }

        // 1. Kullanıcının bütün taşınmazlarını getir
        public async Task<List<TasinmazDto>> GetAllByKullaniciIdAsync(
            int kullaniciId)
        {
            var tasinmazlar = await _context.Set<Tasinmazlar>()
                .AsNoTracking()
                .Include(t => t.Mahalle)
                    .ThenInclude(m => m.Ilce)
                    .ThenInclude(i => i.Il)
                .Where(t => t.KullaniciId == kullaniciId)
                .OrderBy(t => t.Id)
                .ToListAsync();

            return _mapper.Map<List<TasinmazDto>>(tasinmazlar);
        }

        // 2. ID'ye göre kullanıcıya ait taşınmazı getir
        public async Task<TasinmazDto?> GetByIdAsync(
            int id, int kullaniciId)
        {
            var tasinmaz = await _context.Set<Tasinmazlar>()
                .AsNoTracking()
                .Include(t => t.Mahalle)
                    .ThenInclude(m => m.Ilce)
                    .ThenInclude(i => i.Il)
                .FirstOrDefaultAsync(t =>
                    t.Id == id &&
                    t.KullaniciId == kullaniciId);

            if (tasinmaz == null)
                return null;

            return _mapper.Map<TasinmazDto>(tasinmaz);
        }

        // 3. Yeni taşınmaz oluştur
        public async Task<TasinmazDto> CreateAsync(
            TasinmazCreateDto dto, int kullaniciId)
        {
            // Mahalle mevcut mu?
            var mahalleVarMi = await _context.Set<Mahalleler>()
                .AnyAsync(m => m.Id == dto.MahalleId);

            // Mahalle bulunamazsa başarısız log oluştur
            if (!mahalleVarMi)
            {
                await _logService.AddLogAsync(
                    kullaniciId,
                    IslemTipleriSabit.YeniKayit,
                    DurumlarSabit.Basarisiz,
                    $"Taşınmaz eklenemedi. Mahalle ID: {dto.MahalleId} bulunamadı.",
                    GetKullaniciIp());

                throw new ArgumentException(
                    "Seçilen mahalle bulunamadı.");
            }

            // Transaction başlat
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            // DTO -> Entity dönüşümü
            var tasinmaz = _mapper.Map<Tasinmazlar>(dto);
            tasinmaz.KullaniciId = kullaniciId;

            // Taşınmazı ekle
            _context.Set<Tasinmazlar>().Add(tasinmaz);

            await _context.SaveChangesAsync();

            // Başarılı ekleme logu
            await _logService.AddLogAsync(
                kullaniciId,
                IslemTipleriSabit.YeniKayit,
                DurumlarSabit.Basarili,
                $"ID: {tasinmaz.Id} olan taşınmaz eklendi.",
                GetKullaniciIp());

            // Taşınmaz ve log kayıtlarını kalıcı hâle getir
            await transaction.CommitAsync();

            return (await GetByIdAsync(
                tasinmaz.Id, kullaniciId))!;
        }

        // 4. Taşınmaz güncelle
        public async Task<bool> UpdateAsync(
            int id,
            TasinmazUpdateDto dto,
            int kullaniciId)
        {
            // Kullanıcıya ait taşınmazı bul
            var tasinmaz = await _context.Set<Tasinmazlar>()
                .FirstOrDefaultAsync(t =>
                    t.Id == id &&
                    t.KullaniciId == kullaniciId);

            // Taşınmaz bulunamazsa başarısız log oluştur
            if (tasinmaz == null)
            {
                await _logService.AddLogAsync(
                    kullaniciId,
                    IslemTipleriSabit.Guncelleme,
                    DurumlarSabit.Basarisiz,
                    $"Taşınmaz güncellenemedi. ID: {id} bulunamadı veya kullanıcıya ait değil.",
                    GetKullaniciIp());

                return false;
            }

            // Mahalle mevcut mu?
            var mahalleVarMi = await _context.Set<Mahalleler>()
                .AnyAsync(m => m.Id == dto.MahalleId);

            // Mahalle bulunamazsa başarısız log oluştur
            if (!mahalleVarMi)
            {
                await _logService.AddLogAsync(
                    kullaniciId,
                    IslemTipleriSabit.Guncelleme,
                    DurumlarSabit.Basarisiz,
                    $"Taşınmaz güncellenemedi. Mahalle ID: {dto.MahalleId} bulunamadı.",
                    GetKullaniciIp());

                throw new ArgumentException(
                    "Seçilen mahalle bulunamadı.");
            }

            // Transaction başlat
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            // DTO'daki izin verilen alanları güncelle
            _mapper.Map(dto, tasinmaz);

            await _context.SaveChangesAsync();

            // Başarılı güncelleme logu
            await _logService.AddLogAsync(
                kullaniciId,
                IslemTipleriSabit.Guncelleme,
                DurumlarSabit.Basarili,
                $"ID: {id} olan taşınmaz güncellendi.",
                GetKullaniciIp());

            // Güncelleme ve log kayıtlarını kalıcı hâle getir
            await transaction.CommitAsync();

            return true;
        }

        // 5. Taşınmaz sil
        public async Task<bool> DeleteAsync(
            int id, int kullaniciId)
        {
            // Kullanıcıya ait taşınmazı bul
            var tasinmaz = await _context.Set<Tasinmazlar>()
                .FirstOrDefaultAsync(t =>
                    t.Id == id &&
                    t.KullaniciId == kullaniciId);

            // Taşınmaz bulunamazsa başarısız log oluştur
            if (tasinmaz == null)
            {
                await _logService.AddLogAsync(
                    kullaniciId,
                    IslemTipleriSabit.Silme,
                    DurumlarSabit.Basarisiz,
                    $"Taşınmaz silinemedi. ID: {id} bulunamadı veya kullanıcıya ait değil.",
                    GetKullaniciIp());

                return false;
            }

            // Transaction başlat
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            // Taşınmazı sil
            _context.Set<Tasinmazlar>().Remove(tasinmaz);

            await _context.SaveChangesAsync();

            // Başarılı silme logu
            await _logService.AddLogAsync(
                kullaniciId,
                IslemTipleriSabit.Silme,
                DurumlarSabit.Basarili,
                $"ID: {id} olan taşınmaz silindi.",
                GetKullaniciIp());

            // Silme ve log kayıtlarını kalıcı hâle getir
            await transaction.CommitAsync();

            return true;
        }

        // 7. Birden fazla taşınmazı sil (hepsi kullanıcıya ait olmalı)
        public async Task<bool> DeleteManyAsync(
            List<int> ids, int kullaniciId)
        {
            var benzersizIdler = ids.Distinct().ToList();

            var tasinmazlar = await _context.Set<Tasinmazlar>()
                .Where(t =>
                    benzersizIdler.Contains(t.Id) &&
                    t.KullaniciId == kullaniciId)
                .ToListAsync();

            var idMetni = string.Join(", ", benzersizIdler);

            // Bir tanesi bile bulunamazsa hiçbiri silinmez
            if (tasinmazlar.Count != benzersizIdler.Count)
            {
                await _logService.AddLogAsync(
                    kullaniciId,
                    IslemTipleriSabit.Silme,
                    DurumlarSabit.Basarisiz,
                    $"Toplu silme yapılamadı. ID: {idMetni} içinde bulunamayan veya kullanıcıya ait olmayan kayıt var.",
                    GetKullaniciIp());

                return false;
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            _context.Set<Tasinmazlar>().RemoveRange(tasinmazlar);
            await _context.SaveChangesAsync();

            await _logService.AddLogAsync(
                kullaniciId,
                IslemTipleriSabit.Silme,
                DurumlarSabit.Basarili,
                $"ID: {idMetni} olan taşınmazlar silindi.",
                GetKullaniciIp());

            await transaction.CommitAsync();

            return true;
        }

        // Ortak filtre sorgusu (liste ve Excel aynı filtreyi kullanır)
        private IQueryable<Tasinmazlar> FiltreliSorgu(
            TasinmazFilterDto filter,
            int kullaniciId)
        {
            // Sadece giriş yapan kullanıcının taşınmazları
            var query = _context.Set<Tasinmazlar>()
                .AsNoTracking()
                .Where(t => t.KullaniciId == kullaniciId)
                .AsQueryable();

            // İl filtresi
            if (filter.IlId.HasValue)
            {
                query = query.Where(t =>
                    t.Mahalle.Ilce.IlId == filter.IlId.Value);
            }

            // İlçe filtresi
            if (filter.IlceId.HasValue)
            {
                query = query.Where(t =>
                    t.Mahalle.IlceId == filter.IlceId.Value);
            }

            // Mahalle filtresi
            if (filter.MahalleId.HasValue)
            {
                query = query.Where(t =>
                    t.MahalleId == filter.MahalleId.Value);
            }

            // Ada filtresi
            if (!string.IsNullOrWhiteSpace(filter.Ada))
            {
                var ada = filter.Ada.Trim();

                query = query.Where(t =>
                    t.Ada.Contains(ada));
            }

            // Parsel filtresi
            if (!string.IsNullOrWhiteSpace(filter.Parsel))
            {
                var parsel = filter.Parsel.Trim();

                query = query.Where(t =>
                    t.Parsel.Contains(parsel));
            }

            // Nitelik filtresi
            if (!string.IsNullOrWhiteSpace(filter.Nitelik))
            {
                var nitelik = filter.Nitelik.Trim();

                query = query.Where(t =>
                    t.Nitelik == nitelik);
            }

            // Koordinat filtreleri
            if (filter.MinEnlem.HasValue)
                query = query.Where(t => t.Enlem >= filter.MinEnlem.Value);

            if (filter.MaxEnlem.HasValue)
                query = query.Where(t => t.Enlem <= filter.MaxEnlem.Value);

            if (filter.MinBoylam.HasValue)
                query = query.Where(t => t.Boylam >= filter.MinBoylam.Value);

            if (filter.MaxBoylam.HasValue)
                query = query.Where(t => t.Boylam <= filter.MaxBoylam.Value);

            // Kayıt tarihi filtreleri
            if (filter.BaslangicTarihi.HasValue)
            {
                var baslangic = DateTime.SpecifyKind(
                    filter.BaslangicTarihi.Value.Date, DateTimeKind.Utc);

                query = query.Where(t => t.KayitTarihi >= baslangic);
            }

            if (filter.BitisTarihi.HasValue)
            {
                // Bitiş gününü de dahil etmek için ertesi günün başına kadar
                var bitis = DateTime.SpecifyKind(
                    filter.BitisTarihi.Value.Date.AddDays(1), DateTimeKind.Utc);

                query = query.Where(t => t.KayitTarihi < bitis);
            }

            return query;
        }

        // 6. Taşınmazları filtrele ve sayfala
        public async Task<PagedResultDto<TasinmazDto>> GetFilteredAsync(
            TasinmazFilterDto filter,
            int kullaniciId)
        {
            // Ortak filtreler uygulanmış sorgu
            var query = FiltreliSorgu(filter, kullaniciId);

            // Sayfalama öncesinde toplam kayıt sayısı
            var totalCount = await query.CountAsync();

            // Geçerli sayfa değerleri
            var pageNumber = Math.Max(1, filter.PageNumber);
            var pageSize = Math.Clamp(filter.PageSize, 1, 100);

            // İlgili sayfadaki taşınmazları getir
            var tasinmazlar = await query
                .OrderBy(t => t.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(t => t.Mahalle)
                    .ThenInclude(m => m.Ilce)
                    .ThenInclude(i => i.Il)
                .ToListAsync();

            return new PagedResultDto<TasinmazDto>
            {
                Items = _mapper.Map<List<TasinmazDto>>(tasinmazlar),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        // 8. Filtreye uyan bütün taşınmazları Excel'e aktar (sayfalama yok)
        public async Task<byte[]> ExcelOlusturAsync(
            TasinmazFilterDto filter, int kullaniciId)
        {
            var tasinmazlar = await FiltreliSorgu(filter, kullaniciId)
                .Include(t => t.Mahalle)
                    .ThenInclude(m => m.Ilce)
                    .ThenInclude(i => i.Il)
                .OrderBy(t => t.Id)
                .ToListAsync();

            var kayitlar = _mapper.Map<List<TasinmazDto>>(tasinmazlar);

            return _excelService.Olustur<TasinmazDto>(
                "Taşınmazlar",
                kayitlar,
                ("ID", t => t.Id),
                ("İl", t => t.IlAdi),
                ("İlçe", t => t.IlceAdi),
                ("Mahalle", t => t.MahalleAdi),
                ("Ada", t => t.Ada),
                ("Parsel", t => t.Parsel),
                ("Nitelik", t => t.Nitelik),
                ("Enlem", t => t.Enlem),
                ("Boylam", t => t.Boylam),
                ("Kayıt Tarihi", t => t.KayitTarihi.ToLocalTime()
                    .ToString("dd.MM.yyyy HH:mm")));
        }
    }
}
