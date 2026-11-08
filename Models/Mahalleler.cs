using System;
using System.Collections.Generic;

namespace TasinmazYonetimi.Models;

public partial class Mahalleler
{
    public int Id { get; set; }

    public string MahalleAdi { get; set; } = null!;

    public int IlceId { get; set; }

    public virtual Ilceler Ilce { get; set; } = null!;

    public virtual ICollection<Tasinmazlar> Tasinmazlars { get; set; } = new List<Tasinmazlar>();
}
