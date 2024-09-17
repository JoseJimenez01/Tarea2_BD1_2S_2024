using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tarea2_BD1.Models;

public partial class Usuario
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El espacio no puede quedar en blanco")]
    [MaxLength(64, ErrorMessage = "El usuario puede tener un máximo de 64 caracteres")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "El espacio no puede quedar en blanco")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener un mínimo de 8 caracteres")]
    [MaxLength(64, ErrorMessage = "La contraseña debe tener un máximo de 64 caracteres")]
    public string Password { get; set; } = null!;

    public virtual ICollection<BitacoraEvento> BitacoraEventos { get; set; } = new List<BitacoraEvento>();

    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
