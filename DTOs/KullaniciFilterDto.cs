using System.ComponentModel.DataAnnotations;

namespace TasinmazYonetimi.DTOs
{
    public class KullaniciFilterDto
    {
        // İçinde geçen metin (büyük/küçük harf duyarsız)
        public string? Ad { get; set; }

        public string? Soyad { get; set; }

        public string? Email { get; set; }

        [Range(1, int.MaxValue)]
        public int? RolId { get; set; }

        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;
    }
}
