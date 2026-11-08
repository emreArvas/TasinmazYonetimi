using System;
using System.Collections.Generic;

namespace TasinmazYonetimi.Models;

public partial class Kullanicilar
{
    public int Id { get; set; }

    public string Ad { get; set; } = null!;

    public string Soyad { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string ParolaHash { get; set; } = null!;

    public string? Adres { get; set; }

    public int RolId { get; set; }

    public virtual ICollection<Loglar> Loglars { get; set; } = new List<Loglar>();

    public virtual Roller Rol { get; set; } = null!;

    public virtual ICollection<Tasinmazlar> Tasinmazlars { get; set; } = new List<Tasinmazlar>();
}
