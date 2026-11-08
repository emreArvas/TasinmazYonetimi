using System.ComponentModel.DataAnnotations;

namespace TasinmazYonetimi.DTOs
{
    public class KullaniciUpdateDto
    {
        [Required]
        [MaxLength(50)]
        public string Ad { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Soyad { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        public string? Adres { get; set; }

        [Required]
        public int? RolId { get; set; }
    }
}
