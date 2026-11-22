using System.ComponentModel.DataAnnotations;

namespace TasinmazYonetimi.DTOs
{
    public class KullaniciCreateDto
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

        [Required]
        [RegularExpression(
            @"^(?=.*[A-Za-zÇĞİÖŞÜçğıöşü])(?=.*\d)(?=.*[^A-Za-z0-9ÇĞİÖŞÜçğıöşü\s]).{8,}$",
            ErrorMessage = "Parola en az 8 karakter olmalı; en az 1 harf, 1 rakam ve 1 özel karakter içermelidir.")]
        public string Parola { get; set; } = string.Empty;

        public string? Adres { get; set; }

        [Required]
        public int? RolId { get; set; }
    }
}
