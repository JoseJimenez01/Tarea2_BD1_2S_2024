namespace Tarea2_BD1.Models
{
    public class VistaListarMovimientos
    {
        public List<Empleado> empleados {  get; set; } = new List<Empleado>();

        public List<Movimiento> movimientos { get; set; } = new List<Movimiento>();

    }
}
