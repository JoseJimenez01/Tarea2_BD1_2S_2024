using System;
using System.Collections.Generic;

namespace Tarea2_BD1.Models;

public partial class TipoEvento
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<BitacoraEvento> BitacoraEventos { get; set; } = new List<BitacoraEvento>();
}
