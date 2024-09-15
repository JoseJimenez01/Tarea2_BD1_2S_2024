using System;
using System.Collections.Generic;

namespace Tarea2_BD1.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<BitacoraEvento> BitacoraEventos { get; set; } = new List<BitacoraEvento>();

    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
