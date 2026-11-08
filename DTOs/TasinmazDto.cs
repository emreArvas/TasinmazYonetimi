namespace TasinmazYonetimi.DTOs
{
    public class TasinmazDto
    {
        public int Id { get; set; }
        public int KullaniciId { get; set; }
        public string IlAdi { get; set; } = string.Empty;
        public string IlceAdi { get; set; } = string.Empty;
        public string MahalleAdi { get; set; } = string.Empty;
        public string Ada { get; set; } = string.Empty;
        public string Parsel { get; set; } = string.Empty;
        public string Nitelik { get; set; } = string.Empty;
        public decimal Enlem { get; set; }
        public decimal Boylam { get; set; }
        public DateTime KayitTarihi { get; set; }
    }
}
