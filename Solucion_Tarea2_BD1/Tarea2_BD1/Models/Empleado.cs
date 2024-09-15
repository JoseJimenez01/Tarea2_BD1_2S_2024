using System;
using System.Collections.Generic;

namespace Tarea2_BD1.Models;

public partial class Empleado
{
    public int Id { get; set; }

    public int IdPuesto { get; set; }

    public int ValorDocumentoIdentidad { get; set; }

    public string Nombre { get; set; } = null!;

    public DateOnly FehaContratacion { get; set; }

    public decimal SaldoVacaciones { get; set; }

    public bool EsActivo { get; set; }

    public virtual Puesto IdPuestoNavigation { get; set; } = null!;

    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
