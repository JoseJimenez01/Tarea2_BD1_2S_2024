using System;
using System.Collections.Generic;

namespace Tarea2_BD1.Models;

public partial class TipoMovimiento
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string TipoAccion { get; set; } = null!;

    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
