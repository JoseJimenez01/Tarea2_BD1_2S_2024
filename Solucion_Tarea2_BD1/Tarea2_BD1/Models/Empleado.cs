using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tarea2_BD1.Models;

public partial class Empleado
{
    public int Id { get; set; }

    public int IdPuesto { get; set; }

    [Required(ErrorMessage = "You must enter the identity document number")]
    public int ValorDocumentoIdentidad { get; set; }

    [Required(ErrorMessage = "You must enter the name")]
    [MaxLength(128, ErrorMessage = "The name can have a maximum of 128 characters")]
    public string Nombre { get; set; } = null!;

    public DateOnly FechaContratacion { get; set; }

    [Required]
    public decimal SaldoVacaciones { get; set; }

    public bool EsActivo { get; set; }

    public virtual Puesto IdPuestoNavigation { get; set; } = new Puesto();
    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();

}
