
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TasinmazYonetimi.Constants;
using TasinmazYonetimi.Data;
using TasinmazYonetimi.DTOs;
using TasinmazYonetimi.Interfaces;
using TasinmazYonetimi.Models;

namespace TasinmazYonetimi.Services
{
    public class KullaniciService : IKullaniciService
    {
        private readonly TasinmazYonetimiContext _context;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<Kullanicilar> _passwordHasher;
        private readonly ILogService _logService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public KullaniciService(
            TasinmazYonetimiContext context,
            IMapper mapper,
            IPasswordHasher<Kullanicilar> passwordHasher,
            ILogService logService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _logService = logService;
            _httpContextAccessor = httpContextAccessor;
        }

        // Log yazar. İşlemi yapan: giriş yapan kullanıcı,
        // giriş yoksa (register) parametredeki kullanıcı
        private Task LogYazAsync(
            string islemTipi,
            string durum,
            string aciklama,
            int? kullaniciId = null)
        {
            var http = _httpContextAccessor.HttpContext;

            var islemYapanId = int.TryParse(
                http?.User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var id) ? id : kullaniciId;

            return _logService.AddLogAsync(
                islemYapanId,
                islemTipi,
                durum,
                aciklama,
                http?.Connection.RemoteIpAddress?.ToString());
        }

        public async Task<List<KullaniciDto>> GetAllAsync()
        {
            var kullanicilar = await _context.Set<Kullanicilar>()
                .AsNoTracking()
                .OrderBy(k => k.Id)
                .ToListAsync();

            return _mapper.Map<List<KullaniciDto>>(kullanicilar);
        }

        public async Task<KullaniciDto?> GetByIdAsync(int id)
        {
            var kullanici = await _context.Set<Kullanicilar>()
                .AsNoTracking()
                .FirstOrDefaultAsync(k => k.Id == id);

            return kullanici == null
                ? null
                : _mapper.Map<KullaniciDto>(kullanici);
        }

        public async Task<KullaniciDto> CreateAsync(
            KullaniciCreateDto dto)
        {
            if (dto.RolId != 2)
            {
                await LogYazAsync(
                    IslemTipleriSabit.YeniKayit,
                    DurumlarSabit.Basarisiz,
                    "Kullanıcı eklenemedi. Geçersiz rol seçimi.");

                throw new ArgumentException(
                    "Yeni kullanıcı yalnızca Kullanici rolüyle oluşturulabilir.");
            }

            var emailVarMi = await _context.Set<Kullanicilar>()
                .AnyAsync(k => k.Email == dto.Email);

            if (emailVarMi)
            {
                await LogYazAsync(
                    IslemTipleriSabit.YeniKayit,
                    DurumlarSabit.Basarisiz,
                    $"Kullanıcı eklenemedi. E-posta zaten kayıtlı: {dto.Email}");

                throw new ArgumentException(
                    "Bu e-posta adresi zaten kayıtlı.");
            }

            var kullanici = _mapper.Map<Kullanicilar>(dto);

            kullanici.RolId = 2;

            kullanici.ParolaHash = _passwordHasher.HashPassword(
                kullanici, dto.Parola);

            _context.Set<Kullanicilar>().Add(kullanici);
            await _context.SaveChangesAsync();

            await LogYazAsync(
                IslemTipleriSabit.YeniKayit,
                DurumlarSabit.Basarili,
                $"ID: {kullanici.Id} olan kullanıcı eklendi.",
                kullanici.Id);

            return _mapper.Map<KullaniciDto>(kullanici);
        }

        public async Task<bool> UpdateAsync(
            int id, KullaniciUpdateDto dto)
        {
            var kullanici = await _context.Set<Kullanicilar>()
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kullanici == null)
            {
                await LogYazAsync(
                    IslemTipleriSabit.Guncelleme,
                    DurumlarSabit.Basarisiz,
                    $"Kullanıcı güncellenemedi. ID: {id} bulunamadı.");

                return false;
            }

            if (dto.RolId != kullanici.RolId)
            {
                await LogYazAsync(
                    IslemTipleriSabit.Guncelleme,
                    DurumlarSabit.Basarisiz,
                    $"ID: {id} olan kullanıcının rolü değiştirilemedi.");

                throw new ArgumentException(
                    "Kullanıcı rolü bu işlemle değiştirilemez.");
            }

            var emailVarMi = await _context.Set<Kullanicilar>()
                .AnyAsync(k =>
                    k.Email == dto.Email &&
                    k.Id != id);

            if (emailVarMi)
            {
                await LogYazAsync(
                    IslemTipleriSabit.Guncelleme,
                    DurumlarSabit.Basarisiz,
                    $"ID: {id} olan kullanıcı güncellenemedi. E-posta başka kullanıcıya ait: {dto.Email}");

                throw new ArgumentException(
                    "Bu e-posta adresi başka bir kullanıcıya ait.");
            }

            _mapper.Map(dto, kullanici);

            await _context.SaveChangesAsync();

            await LogYazAsync(
                IslemTipleriSabit.Guncelleme,
                DurumlarSabit.Basarili,
                $"ID: {id} olan kullanıcı güncellendi.");

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var kullanici = await _context.Set<Kullanicilar>()
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kullanici == null)
            {
                await LogYazAsync(
                    IslemTipleriSabit.Silme,
                    DurumlarSabit.Basarisiz,
                    $"Kullanıcı silinemedi. ID: {id} bulunamadı.");

                return false;
            }

            _context.Set<Kullanicilar>().Remove(kullanici);
            await _context.SaveChangesAsync();

            await LogYazAsync(
                IslemTipleriSabit.Silme,
                DurumlarSabit.Basarili,
                $"ID: {id} olan kullanıcı silindi.");

            return true;
        }

        public async Task<bool> UpdateProfilAsync(
            int kullaniciId, ProfilUpdateDto dto)
        {
            var kullanici = await _context.Set<Kullanicilar>()
                .FirstOrDefaultAsync(k => k.Id == kullaniciId);

            if (kullanici == null)
            {
                await LogYazAsync(
                    IslemTipleriSabit.Guncelleme,
                    DurumlarSabit.Basarisiz,
                    "Profil güncellenemedi. Kullanıcı bulunamadı.",
                    kullaniciId);

                return false;
            }

            var emailVarMi = await _context.Set<Kullanicilar>()
                .AnyAsync(k =>
                    k.Email == dto.Email &&
                    k.Id != kullaniciId);

            if (emailVarMi)
            {
                await LogYazAsync(
                    IslemTipleriSabit.Guncelleme,
                    DurumlarSabit.Basarisiz,
                    $"Profil güncellenemedi. E-posta başka kullanıcıya ait: {dto.Email}",
                    kullaniciId);

                throw new ArgumentException(
                    "Bu e-posta adresi başka bir kullanıcıya ait.");
            }

            kullanici.Ad = dto.Ad;
            kullanici.Soyad = dto.Soyad;
            kullanici.Email = dto.Email;
            kullanici.Adres = dto.Adres;

            await _context.SaveChangesAsync();

            await LogYazAsync(
                IslemTipleriSabit.Guncelleme,
                DurumlarSabit.Basarili,
                "Kullanıcı profilini güncelledi.",
                kullaniciId);

            return true;
        }

    }
}
