
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

        public TasinmazService(
            TasinmazYonetimiContext context,
            IMapper mapper,
            ILogService logService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _logService = logService;
            _httpContextAccessor = httpContextAccessor;
        }

        // İstek yapan kullanıcının IP adresi
        private string? GetKullaniciIp()
        {
            return _httpContextAccessor.HttpContext?
                .Connection.RemoteIpAddress?.ToString();
        }

        // Kullanıcının taşınmazlarını getir
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

        // ID'ye göre taşınmaz getir
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

        // Yeni taşınmaz oluştur
        public async Task<TasinmazDto> CreateAsync(
            TasinmazCreateDto dto, int kullaniciId)
        {
            var mahalleVarMi = await _context.Set<Mahalleler>()
                .AnyAsync(m => m.Id == dto.MahalleId);

            if (!mahalleVarMi)
                throw new ArgumentException(
                    "Seçilen mahalle bulunamadı.");

            var tasinmaz = _mapper.Map<Tasinmazlar>(dto);
            tasinmaz.KullaniciId = kullaniciId;

            _context.Set<Tasinmazlar>().Add(tasinmaz);
            await _context.SaveChangesAsync();

            // Başarılı ekleme işlemini logla
            await _logService.AddLogAsync(
                kullaniciId,
                "Yeni Kayıt",
                "Basarili",
                $"ID: {tasinmaz.Id} olan taşınmaz eklendi.",
                GetKullaniciIp());

            return (await GetByIdAsync(
                tasinmaz.Id, kullaniciId))!;
        }

        // Taşınmaz güncelle
        public async Task<bool> UpdateAsync(
            int id,
            TasinmazUpdateDto dto,
            int kullaniciId)
        {
            var tasinmaz = await _context.Set<Tasinmazlar>()
                .FirstOrDefaultAsync(t =>
                    t.Id == id &&
                    t.KullaniciId == kullaniciId);

            if (tasinmaz == null)
                return false;

            var mahalleVarMi = await _context.Set<Mahalleler>()
                .AnyAsync(m => m.Id == dto.MahalleId);

            if (!mahalleVarMi)
                throw new ArgumentException(
                    "Seçilen mahalle bulunamadı.");

            _mapper.Map(dto, tasinmaz);

            await _context.SaveChangesAsync();

            // Başarılı güncelleme işlemini logla
            await _logService.AddLogAsync(
                kullaniciId,
                "Güncelleme",
                "Basarili",
                $"ID: {id} olan taşınmaz güncellendi.",
                GetKullaniciIp());

            return true;
        }

        // Taşınmaz sil
        public async Task<bool> DeleteAsync(
            int id, int kullaniciId)
        {
            var tasinmaz = await _context.Set<Tasinmazlar>()
                .FirstOrDefaultAsync(t =>
                    t.Id == id &&
                    t.KullaniciId == kullaniciId);

            if (tasinmaz == null)
                return false;

            _context.Set<Tasinmazlar>().Remove(tasinmaz);
            await _context.SaveChangesAsync();

            // Başarılı silme işlemini logla
            await _logService.AddLogAsync(
                kullaniciId,
                "Silme",
                "Basarili",
                $"ID: {id} olan taşınmaz silindi.",
                GetKullaniciIp());

            return true;
        }
    }
}
