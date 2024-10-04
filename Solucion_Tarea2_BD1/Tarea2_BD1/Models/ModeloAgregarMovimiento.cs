using System.ComponentModel.DataAnnotations;

namespace Tarea2_BD1.Models
{
    public class ModeloAgregarMovimiento
    {
        public Empleado empleado { get; set; } = new Empleado();

        public Movimiento movimiento { get; set; } = new Movimiento();

    }
}
