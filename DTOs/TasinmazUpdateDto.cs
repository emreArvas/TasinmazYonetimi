using System.ComponentModel.DataAnnotations;

namespace TasinmazYonetimi.DTOs
{
    public class TasinmazUpdateDto
    {
        [Required]
        [Range(1, int.MaxValue)]
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
    }
}
