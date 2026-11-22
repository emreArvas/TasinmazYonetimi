using ClosedXML.Excel;
using TasinmazYonetimi.Interfaces;

namespace TasinmazYonetimi.Services
{
    public class ExcelService : IExcelService
    {
        public byte[] Olustur<T>(
            string sayfaAdi,
            IEnumerable<T> kayitlar,
            params (string Baslik, Func<T, object?> Deger)[] kolonlar)
        {
            using var workbook = new XLWorkbook();
            var sayfa = workbook.Worksheets.Add(sayfaAdi);

            // Başlık satırı
            for (var i = 0; i < kolonlar.Length; i++)
            {
                var hucre = sayfa.Cell(1, i + 1);
                hucre.Value = kolonlar[i].Baslik;
                hucre.Style.Font.Bold = true;
            }

            // Veri satırları
            var satir = 2;

            foreach (var kayit in kayitlar)
            {
                for (var i = 0; i < kolonlar.Length; i++)
                {
                    sayfa.Cell(satir, i + 1).Value =
                        XLCellValue.FromObject(kolonlar[i].Deger(kayit));
                }

                satir++;
            }

            sayfa.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }
    }
}
