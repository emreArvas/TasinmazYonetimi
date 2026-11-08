using System.ComponentModel.DataAnnotations;

namespace TasinmazYonetimi.DTOs
{
    public class TasinmazCreateDto
    {
        [Required]
        public int? MahalleId { get; set; }

        [Required]
        [MaxLength(30)]
        public string Ada { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string Parsel { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Arsa|Tarla|Mesken)$")]
        public string Nitelik { get; set; } = string.Empty;

        [Required]
        [Range(-90, 90)]
        public decimal? Enlem { get; set; }

        [Required]
        [Range(-180, 180)]
        public decimal? Boylam { get; set; }
    }
}
