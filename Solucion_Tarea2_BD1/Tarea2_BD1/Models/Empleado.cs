using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tarea2_BD1.Models;

public partial class Empleado
{
    public int Id { get; set; }

    public int IdPuesto { get; set; }

    [Required(ErrorMessage = "Debe ingresar el valor del documento de identidad")]
    public int ValorDocumentoIdentidad { get; set; }

    [Required(ErrorMessage = "Debe ingresar el nombre")]
    [MaxLength(128, ErrorMessage = "El nombre puede tener un máximo de 128 caracteres")]
    public string Nombre { get; set; } = null!;

    public DateOnly FechaContratacion { get; set; }

    public decimal SaldoVacaciones { get; set; }

    public bool EsActivo { get; set; }

    public virtual Puesto IdPuestoNavigation { get; set; } = null!;

    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();

}
