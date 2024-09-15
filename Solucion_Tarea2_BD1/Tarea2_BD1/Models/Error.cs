using System;
using System.Collections.Generic;

namespace Tarea2_BD1.Models;

public partial class Error
{
    public int Id { get; set; }

    public int Codigo { get; set; }

    public string Descripcion { get; set; } = null!;
}
