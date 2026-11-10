using TasinmazYonetimi.DTOs;

namespace TasinmazYonetimi.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginDto dto);
    }
}
