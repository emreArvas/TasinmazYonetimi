
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public KullaniciService(
            TasinmazYonetimiContext context,
            IMapper mapper,
            IPasswordHasher<Kullanicilar> passwordHasher)
        {
            _context = context;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
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
                throw new ArgumentException(
                    "Yeni kullanıcı yalnızca Kullanici rolüyle oluşturulabilir.");

            var emailVarMi = await _context.Set<Kullanicilar>()
                .AnyAsync(k => k.Email == dto.Email);

            if (emailVarMi)
                throw new ArgumentException(
                    "Bu e-posta adresi zaten kayıtlı.");

            var kullanici = _mapper.Map<Kullanicilar>(dto);

            kullanici.RolId = 2;

            kullanici.ParolaHash = _passwordHasher.HashPassword(
                kullanici, dto.Parola);

            _context.Set<Kullanicilar>().Add(kullanici);
            await _context.SaveChangesAsync();

            return _mapper.Map<KullaniciDto>(kullanici);
        }

        public async Task<bool> UpdateAsync(
            int id, KullaniciUpdateDto dto)
        {
            var kullanici = await _context.Set<Kullanicilar>()
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kullanici == null)
                return false;

            if (dto.RolId != kullanici.RolId)
                throw new ArgumentException(
                    "Kullanıcı rolü bu işlemle değiştirilemez.");

            var emailVarMi = await _context.Set<Kullanicilar>()
                .AnyAsync(k =>
                    k.Email == dto.Email &&
                    k.Id != id);

            if (emailVarMi)
                throw new ArgumentException(
                    "Bu e-posta adresi başka bir kullanıcıya ait.");

            _mapper.Map(dto, kullanici);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var kullanici = await _context.Set<Kullanicilar>()
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kullanici == null)
                return false;

            _context.Set<Kullanicilar>().Remove(kullanici);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
