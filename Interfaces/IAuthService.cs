using TasinmazYonetimi.DTOs;

namespace TasinmazYonetimi.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginDto dto, string? ip);

        Task LogoutAsync(
            int kullaniciId,
            string tokenId,
            DateTime bitisZamani,
            string? ip);
    }
}
