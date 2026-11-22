
using System.ComponentModel.DataAnnotations;

namespace TasinmazYonetimi.DTOs
{
    public class TasinmazFilterDto
    {
        [Range(1, int.MaxValue)]
        public int? IlId { get; set; }

        [Range(1, int.MaxValue)]
        public int? IlceId { get; set; }

        [Range(1, int.MaxValue)]
        public int? MahalleId { get; set; }

        public string? Ada { get; set; }

        public string? Parsel { get; set; }

        public string? Nitelik { get; set; }

        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;
    }
}
