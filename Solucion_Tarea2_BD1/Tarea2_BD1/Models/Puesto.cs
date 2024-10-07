using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tarea2_BD1.Models;

public partial class Puesto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal SalarioXhora { get; set; }

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
