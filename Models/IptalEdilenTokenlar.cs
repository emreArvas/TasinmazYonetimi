namespace TasinmazYonetimi.Models;

// Çıkış yapılarak geçersiz kılınan JWT token'ları
public class IptalEdilenTokenlar
{
    public int Id { get; set; }

    public string TokenId { get; set; } = null!;

    public DateTime BitisZamani { get; set; }
}
