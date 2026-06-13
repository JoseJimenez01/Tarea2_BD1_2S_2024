using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tarea2_BD1.Models;

public partial class Usuario
{
    public int Id { get; set; }

    [Required(ErrorMessage = "The space cannot be left blank")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "The space cannot be left blank")]
    [MaxLength(64, ErrorMessage = "The password must have a maximum of 64 characters")]
    public string Password { get; set; } = null!;

    public virtual ICollection<BitacoraEvento> BitacoraEventos { get; set; } = new List<BitacoraEvento>();

    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
