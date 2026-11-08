using System;
using System.Collections.Generic;

namespace TasinmazYonetimi.Models;

public partial class IslemTipleri
{
    public int Id { get; set; }

    public string IslemTipiAdi { get; set; } = null!;

    public virtual ICollection<Loglar> Loglars { get; set; } = new List<Loglar>();
}
