namespace Tarea2_BD1.Models
{
    public class ActualizarEmpleado
    {
        public Empleado empleadoOriginal { get; set; } = new Empleado();

        public Empleado empleadoNuevo { get; set; } = new Empleado();
    }
}
