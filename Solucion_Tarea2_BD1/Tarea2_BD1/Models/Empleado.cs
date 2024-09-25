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
    [RegularExpression(@"^[A-Za-z\ \-\xC1\xC9\xCD\xD3\xDA\xDC\xE1\xE9\xED\xF3\xFA\xFC\xD1\xF1]+$", ErrorMessage = "El nombre pude contener letras, espacios, tildes o un guión")]
    [MaxLength(128, ErrorMessage = "El nombre puede tener un máximo de 128 caracteres")]
    public string Nombre { get; set; } = null!;

    public DateOnly FechaContratacion { get; set; }

    public decimal SaldoVacaciones { get; set; }

    public bool EsActivo { get; set; }

    //public virtual Puesto IdPuestoNavigation { get; set; } = null!;
    //[Required(ErrorMessage = "Debe seleccionar algún puesto")]
    public virtual Puesto IdPuestoNavigation { get; set; } = new Puesto();

    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();

}
