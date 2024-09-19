using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.Sockets;
using System.Net;
using Tarea2_BD1.Models;

namespace Tarea2_BD1.Controllers
{
    public class EmpleadoController : Controller
    {
        public readonly Dbtarea2Context _dbContext;

        public EmpleadoController(Dbtarea2Context _context)
        {
            _dbContext = _context;
        }

        //public IActionResult ListarFiltrar()
        //{
        //    return View();
        //}

        [HttpPost]
        public IActionResult ListarFiltrar()
        {
            try
            {
                //Se crea a conexión se abre
                SqlConnection connection = (SqlConnection)_dbContext.Database.GetDbConnection();
                connection.Open();

                //Se crea el SP
                SqlCommand comando = connection.CreateCommand();
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.CommandText = "SP_ListarEmpleados";

                //Código para crear parámetros al Store Procedure
                //SqlParameter paramUsername = new SqlParameter
                //{
                //    ParameterName = "@inUsername",
                //    SqlDbType = SqlDbType.VarChar,
                //    Size = 64,
                //    Value = username,
                //    Direction = ParameterDirection.Input
                //};
                SqlParameter paramResultado = new SqlParameter
                {
                    ParameterName = "@outResult",
                    SqlDbType = SqlDbType.Int,
                    Value = -345678,
                    Direction = ParameterDirection.InputOutput
                };

                //Se agrega cada parámetro al SP
                //comando.Parameters.Add(paramUsername);
                comando.Parameters.Add(paramResultado);

                //Se leen los datos devueltos por el SP(dataset)
                SqlDataReader reader = comando.ExecuteReader();
                List<Models.Empleado> listaEmpleados = new List<Models.Empleado>();
                while (reader.Read())
                {
                    Empleado empleado = new Empleado();
                    empleado.Nombre = Convert.ToString(reader["Nombre"])!;
                    empleado.ValorDocumentoIdentidad = Convert.ToInt32(reader["ValorDocumentoIdentidad"]);
                    listaEmpleados.Add(empleado);
                }
                reader.Close();

                comando.ExecuteNonQuery();

                //Se leen los parámetros de salida
                string SPresult = comando.Parameters["@outResult"].Value.ToString()!;
                Console.WriteLine("\n------------------- SE HA EJECUTADO EL SP_ListarEmpleados -------------------");
                Console.WriteLine(" El codigo de salida del sp es: " + SPresult);
                Console.WriteLine("-----------------------------------------------------------------------------\n");

                connection.Close();

                //return SPresult;
                return View(listaEmpleados);
            }
            catch (Exception ex)
            {
                //return String.Format("El error es: {0}", ex.ToString());
                //return View(null);
                return BadRequest(ex.Message);
            }
        }

    }
}
