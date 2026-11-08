namespace TasinmazYonetimi.DTOs
{
    public class KullaniciDto
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Adres { get; set; }
        public int RolId { get; set; }
    }
}
