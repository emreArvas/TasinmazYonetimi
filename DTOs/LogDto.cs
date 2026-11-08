namespace TasinmazYonetimi.DTOs
{
    public class LogDto
    {
        public long Id { get; set; }
        public int? KullaniciId { get; set; }
        public string DurumAdi { get; set; } = string.Empty;
        public string IslemTipiAdi { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public DateTime TarihSaat { get; set; }
        public string? KullaniciIp { get; set; }
    }
}
