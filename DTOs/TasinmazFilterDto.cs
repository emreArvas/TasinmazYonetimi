
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

        // Koordinat aralığı
        [Range(-90, 90)]
        public decimal? MinEnlem { get; set; }

        [Range(-90, 90)]
        public decimal? MaxEnlem { get; set; }

        [Range(-180, 180)]
        public decimal? MinBoylam { get; set; }

        [Range(-180, 180)]
        public decimal? MaxBoylam { get; set; }

        // Kayıt tarihi aralığı (gün bazında, örn. 2026-11-01)
        public DateTime? BaslangicTarihi { get; set; }

        public DateTime? BitisTarihi { get; set; }

        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;
    }
}
