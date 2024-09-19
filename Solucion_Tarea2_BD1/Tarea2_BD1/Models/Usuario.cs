using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tarea2_BD1.Models;

public partial class Usuario
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El espacio no puede quedar en blanco")]
    [RegularExpression(@"^[A-Za-z\ \-\xC1\xC9\xCD\xD3\xDA\xDC\xE1\xE9\xED\xF3\xFA\xFC\xD1\xF1]+$", ErrorMessage = "El nombre pude contener letras, espacios, tildes o un guión")]
    [MaxLength(64, ErrorMessage = "El usuario puede tener un máximo de 64 caracteres")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "El espacio no puede quedar en blanco")]
    [MaxLength(64, ErrorMessage = "La contraseña debe tener un máximo de 64 caracteres")]
    public string Password { get; set; } = null!;

    public virtual ICollection<BitacoraEvento> BitacoraEventos { get; set; } = new List<BitacoraEvento>();

    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
