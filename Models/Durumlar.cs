using System;
using System.Collections.Generic;

namespace TasinmazYonetimi.Models;

public partial class Durumlar
{
    public int Id { get; set; }

    public string DurumAdi { get; set; } = null!;

    public virtual ICollection<Loglar> Loglars { get; set; } = new List<Loglar>();
}
