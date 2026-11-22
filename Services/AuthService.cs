
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TasinmazYonetimi.Constants;
using TasinmazYonetimi.Data;
using TasinmazYonetimi.DTOs;
using TasinmazYonetimi.Interfaces;
using TasinmazYonetimi.Models;

namespace TasinmazYonetimi.Services
{
    public class AuthService : IAuthService
    {
        private readonly TasinmazYonetimiContext _context;
        private readonly IPasswordHasher<Kullanicilar> _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly ILogService _logService;

        public AuthService(
            TasinmazYonetimiContext context,
            IPasswordHasher<Kullanicilar> passwordHasher,
            IConfiguration configuration,
            ILogService logService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _logService = logService;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto dto, string? ip)
        {
            var kullanici = await _context.Set<Kullanicilar>()
                .AsNoTracking()
                .Include(k => k.Rol)
                .FirstOrDefaultAsync(k => k.Email == dto.Email);

            // Kullanıcı yok veya parola yanlış
            if (kullanici == null ||
                _passwordHasher.VerifyHashedPassword(
                    kullanici, kullanici.ParolaHash, dto.Parola)
                    == PasswordVerificationResult.Failed)
            {
                await _logService.AddLogAsync(
                    kullanici?.Id,
                    IslemTipleriSabit.Giris,
                    DurumlarSabit.Basarisiz,
                    $"Başarısız giriş denemesi: {dto.Email}",
                    ip);

                return null;
            }

            var jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "JWT anahtarı bulunamadı.");

            var keyBytes = Convert.FromBase64String(jwtKey);

            if (keyBytes.Length < 32)
                throw new InvalidOperationException(
                    "JWT anahtarı en az 32 bayt olmalıdır.");

            var issuer = _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException(
                    "JWT Issuer bulunamadı.");

            var audience = _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException(
                    "JWT Audience bulunamadı.");

            var expireMinutes = _configuration
                .GetValue<int>("Jwt:ExpireMinutes");

            if (expireMinutes <= 0)
                throw new InvalidOperationException(
                    "JWT geçerlilik süresi hatalı.");

            var expiresAt = DateTime.UtcNow.AddMinutes(expireMinutes);

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    kullanici.Id.ToString()),

                new Claim(
                    ClaimTypes.Email,
                    kullanici.Email),

                new Claim(
                    ClaimTypes.Role,
                    kullanici.Rol.RolAdi),

                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString())
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            // Başarılı giriş logu
            await _logService.AddLogAsync(
                kullanici.Id,
                IslemTipleriSabit.Giris,
                DurumlarSabit.Basarili,
                "Kullanıcı sisteme giriş yaptı.",
                ip);

            return new LoginResponseDto
            {
                Token = new JwtSecurityTokenHandler()
                    .WriteToken(token),

                ExpiresAt = expiresAt,
                KullaniciId = kullanici.Id,
                Email = kullanici.Email,
                Rol = kullanici.Rol.RolAdi
            };
        }

        // Çıkış: token iptal listesine eklenir ve loglanır
        public async Task LogoutAsync(
            int kullaniciId,
            string tokenId,
            DateTime bitisZamani,
            string? ip)
        {
            _context.Set<IptalEdilenTokenlar>().Add(new IptalEdilenTokenlar
            {
                TokenId = tokenId,
                BitisZamani = bitisZamani
            });

            await _context.SaveChangesAsync();

            await _logService.AddLogAsync(
                kullaniciId,
                IslemTipleriSabit.Cikis,
                DurumlarSabit.Basarili,
                "Kullanıcı sistemden çıkış yaptı.",
                ip);
        }
    }
}
