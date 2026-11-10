namespace TasinmazYonetimi.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public int KullaniciId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Rol { get; set; } = string.Empty;
    }
}
