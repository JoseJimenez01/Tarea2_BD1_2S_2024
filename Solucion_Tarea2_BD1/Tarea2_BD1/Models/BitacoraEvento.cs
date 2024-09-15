using System;
using System.Collections.Generic;

namespace Tarea2_BD1.Models;

public partial class BitacoraEvento
{
    public int Id { get; set; }

    public int IdTipoEvento { get; set; }

    public int IdPostByUser { get; set; }

    public string Descripcion { get; set; } = null!;

    public string PostInIp { get; set; } = null!;

    public DateTime PostTime { get; set; }

    public virtual Usuario IdPostByUserNavigation { get; set; } = null!;

    public virtual TipoEvento IdTipoEventoNavigation { get; set; } = null!;
}
