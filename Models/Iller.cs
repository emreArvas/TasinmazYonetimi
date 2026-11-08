using System;
using System.Collections.Generic;

namespace TasinmazYonetimi.Models;

public partial class Iller
{
    public int Id { get; set; }

    public string IlAdi { get; set; } = null!;

    public virtual ICollection<Ilceler> Ilcelers { get; set; } = new List<Ilceler>();
}
