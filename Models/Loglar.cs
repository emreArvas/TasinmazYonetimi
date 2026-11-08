using System;
using System.Collections.Generic;

namespace TasinmazYonetimi.Models;

public partial class Loglar
{
    public long Id { get; set; }

    public int? KullaniciId { get; set; }

    public int DurumId { get; set; }

    public int IslemTipiId { get; set; }

    public string? Aciklama { get; set; }

    public DateTime TarihSaat { get; set; }

    public string? KullaniciIp { get; set; }

    public virtual Durumlar Durum { get; set; } = null!;

    public virtual IslemTipleri IslemTipi { get; set; } = null!;

    public virtual Kullanicilar? Kullanici { get; set; }
}
