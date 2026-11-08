using System;
using System.Collections.Generic;

namespace TasinmazYonetimi.Models;

public partial class Tasinmazlar
{
    public int Id { get; set; }

    public int KullaniciId { get; set; }

    public int MahalleId { get; set; }

    public string Ada { get; set; } = null!;

    public string Parsel { get; set; } = null!;

    public string Nitelik { get; set; } = null!;

    public decimal Enlem { get; set; }

    public decimal Boylam { get; set; }

    public DateTime KayitTarihi { get; set; }

    public virtual Kullanicilar Kullanici { get; set; } = null!;

    public virtual Mahalleler Mahalle { get; set; } = null!;
}
