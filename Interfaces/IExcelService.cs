namespace TasinmazYonetimi.Interfaces
{
    // Liste verisini Excel (.xlsx) dosyasına dönüştürür
    public interface IExcelService
    {
        byte[] Olustur<T>(
            string sayfaAdi,
            IEnumerable<T> kayitlar,
            params (string Baslik, Func<T, object?> Deger)[] kolonlar);
    }
}
