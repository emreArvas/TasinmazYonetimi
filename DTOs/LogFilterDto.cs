using System.ComponentModel.DataAnnotations;

namespace TasinmazYonetimi.DTOs
{
    public class LogFilterDto
    {
        [Range(1, int.MaxValue)]
        public int? KullaniciId { get; set; }

        [Range(1, int.MaxValue)]
        public int? DurumId { get; set; }

        [Range(1, int.MaxValue)]
        public int? IslemTipiId { get; set; }

        // İçinde geçen metin (büyük/küçük harf duyarsız)
        public string? Aciklama { get; set; }

        public string? KullaniciIp { get; set; }

        // Tarih aralığı (gün bazında, örn. 2026-11-01)
        public DateTime? BaslangicTarihi { get; set; }

        public DateTime? BitisTarihi { get; set; }

        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;
    }
}
