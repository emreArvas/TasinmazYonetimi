using System;
using System.Collections.Generic;

namespace TasinmazYonetimi.Models;

public partial class Ilceler
{
    public int Id { get; set; }

    public string IlceAdi { get; set; } = null!;

    public int IlId { get; set; }

    public virtual Iller Il { get; set; } = null!;

    public virtual ICollection<Mahalleler> Mahallelers { get; set; } = new List<Mahalleler>();
}
