using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tarea2_BD1.Models;

public partial class Movimiento
{
    public int Id { get; set; }

    public int IdEmpleado { get; set; }

    public int IdTipoMovimiento { get; set; }

    public int IdPostByUser { get; set; }

    public DateOnly Fecha { get; set; }

    [Required(ErrorMessage = "Debe agregar el monto")]
    [Range(1.00,10000000.00, ErrorMessage = "El monto debe estar entre {1} y {2}")]
    public decimal Monto { get; set; }

    public decimal NuevoSaldo { get; set; }

    public string PostInIp { get; set; } = null!;

    public DateTime PostTime { get; set; }

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

    public virtual Usuario IdPostByUserNavigation { get; set; } = new Usuario();

    public virtual TipoMovimiento IdTipoMovimientoNavigation { get; set; } = new TipoMovimiento();
}
