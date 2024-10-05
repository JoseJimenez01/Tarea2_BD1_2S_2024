using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using Tarea2_BD1.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Drawing;

namespace Tarea2_BD1.Controllers
{
    public class MovimientoController : Controller
    {
        public readonly Dbtarea2Context _dbContext;

        public MovimientoController(Dbtarea2Context _context)
        {
            _dbContext = _context;
        }

        [HttpGet]
        [Route("listar_movimientos")]
        public IActionResult Listar(string Nombre)
        {
            try
            {
                //Se crea a conexión se abre
                SqlConnection connection = (SqlConnection)_dbContext.Database.GetDbConnection();
                connection.Open();

                //Se crea el SP
                SqlCommand comando = connection.CreateCommand();
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.CommandText = "SP_ListarMovimientos";

                SqlParameter paramNombreEmpleado = new SqlParameter
                {
                    ParameterName = "@inNombreEmpleado",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 128,
                    Value = Nombre,
                    Direction = ParameterDirection.Input
                };
                SqlParameter paramResultado = new SqlParameter
                {
                    ParameterName = "@outResult",
                    SqlDbType = SqlDbType.Int,
                    Value = -345678,
                    Direction = ParameterDirection.InputOutput
                };

                //Se agrega cada parámetro al SP
                comando.Parameters.Add(paramNombreEmpleado);
                comando.Parameters.Add(paramResultado);

                //Se leen los datos devueltos por el SP(dataset)
                SqlDataReader reader = comando.ExecuteReader();

                VistaListarMovimientos listasEmpleadoMovimientos = new VistaListarMovimientos();

                //Reader para leer el empleado
                while (reader.Read())
                {
                    Empleado empleado = new Empleado();
                    empleado.Nombre = Convert.ToString(reader["Nombre"])!;
                    empleado.ValorDocumentoIdentidad = Convert.ToInt32(reader["ValorDocumentoIdentidad"]);
                    empleado.SaldoVacaciones = Convert.ToDecimal(reader["SaldoVacaciones"]);
                    listasEmpleadoMovimientos.empleados.Add(empleado);
                }

                //Cambiamos de dataset
                reader.NextResult();

                //Reader para leer los movimientos del empleado
                while (reader.Read())
                {
                    Movimiento movimiento = new Movimiento();

                    movimiento.Fecha = DateOnly.FromDateTime(Convert.ToDateTime(reader["Fecha"]));
                    movimiento.IdTipoMovimientoNavigation.Nombre = Convert.ToString(reader["Nombre"])!;
                    movimiento.Monto = Convert.ToDecimal(reader["Monto"]);
                    movimiento.NuevoSaldo = Convert.ToDecimal(reader["NuevoSaldo"]);
                    movimiento.IdPostByUserNavigation.Username = Convert.ToString(reader["Username"])!;
                    movimiento.PostInIp = Convert.ToString(reader["PostInIP"])!;
                    movimiento.PostTime = Convert.ToDateTime(reader["PostTime"]);
                    listasEmpleadoMovimientos.movimientos.Add(movimiento);
                }
                reader.Close();

                comando.ExecuteNonQuery();

                //Se leen los parámetros de salida
                string SPresult = comando.Parameters["@outResult"].Value.ToString()!;
                Console.WriteLine("\n------------------- SE HA EJECUTADO EL SP_ListarMovimientos -------------------");
                Console.WriteLine(" El codigo de salida del sp es: " + SPresult);
                Console.WriteLine("-----------------------------------------------------------------------------\n");

                connection.Close();

                return View(listasEmpleadoMovimientos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }// end meethod

        [HttpGet]
        [Route("Empleado")]
        public ModeloAgregarMovimiento sacarEmpleado(ModeloAgregarMovimiento inModelo)
        {
            try
            {
                //Se crea a conexión se abre
                SqlConnection connection = (SqlConnection)_dbContext.Database.GetDbConnection();
                connection.Open();

                //Se crea el SP
                SqlCommand comando = connection.CreateCommand();
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.CommandText = "SP_SacarEmpleado";

                SqlParameter paramNombreEmpleado = new SqlParameter
                {
                    ParameterName = "@inNombreEmpleado",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 128,
                    Value = inModelo.empleado.Nombre,
                    Direction = ParameterDirection.Input
                };
                SqlParameter paramResultado = new SqlParameter
                {
                    ParameterName = "@outResult",
                    SqlDbType = SqlDbType.Int,
                    Value = -345678,
                    Direction = ParameterDirection.InputOutput
                };

                //Se agrega cada parámetro al SP
                comando.Parameters.Add(paramNombreEmpleado);
                comando.Parameters.Add(paramResultado);

                //Se leen los datos devueltos por el SP(dataset)
                SqlDataReader reader = comando.ExecuteReader();

                reader.Read();
                ModeloAgregarMovimiento modelo = new ModeloAgregarMovimiento();
                modelo.empleado.Nombre = Convert.ToString(reader["Nombre"])!;
                modelo.empleado.ValorDocumentoIdentidad = Convert.ToInt32(reader["ValorDocumentoIdentidad"]);
                modelo.empleado.SaldoVacaciones = Convert.ToDecimal(reader["SaldoVacaciones"]);

                reader.Close();

                comando.ExecuteNonQuery();

                //Se leen los parámetros de salida
                string SPresult = comando.Parameters["@outResult"].Value.ToString()!;
                Console.WriteLine("\n------------------- SE HA EJECUTADO EL SP_SacarEmpleado -------------------");
                Console.WriteLine(" El codigo de salida del sp es: " + SPresult);
                Console.WriteLine("-----------------------------------------------------------------------------\n");

                connection.Close();

                return modelo;
            }
            catch (Exception ex)
            {
                ModeloAgregarMovimiento modeloError = new ModeloAgregarMovimiento();
                modeloError.empleado.Nombre = ex.Message;
                return modeloError;
            }
        }// end meethod

        public IActionResult Agregar(string? Nombre)
        {
            //Se descerializa el modelo para seguir validando
            var modeloJson = TempData["Modelo"] as string;
            var modelo = new ModeloAgregarMovimiento();

            if (modeloJson != null)
            {
                modelo = JsonConvert.DeserializeObject<ModeloAgregarMovimiento>(modeloJson);
            }

            ModeloAgregarMovimiento modeloEnviado = new ModeloAgregarMovimiento();
            ModeloAgregarMovimiento modeloRecibido = new ModeloAgregarMovimiento();

            if (Nombre != null)
            {
                modeloEnviado.empleado.Nombre = Nombre;
            }
            else
            {
                modeloEnviado.empleado.Nombre = modelo.empleado.Nombre;
            }
            modeloRecibido = sacarEmpleado(modeloEnviado);

            return View(modeloRecibido);
        }

        [HttpPost]
        [Route("agregarMovimiento")]
        public string AgregarMovimiento(int inValorDocIdent, string inNombre, Decimal inSaldoVacaciones, Decimal inMonto, string inTipoMovimiento)
        {
            try
            {
                //Sacamos la ip desde donde se ejecuta
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                var ippaddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

                //Se crea a conexión se abre
                SqlConnection connection = (SqlConnection)_dbContext.Database.GetDbConnection();
                connection.Open();

                //Se crea el SP
                SqlCommand comando = connection.CreateCommand();
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.CommandText = "SP_AgregarMovimiento";

                SqlParameter paramValorDocIdentEmpleado = new SqlParameter
                {
                    ParameterName = "@inValorDocIdent",
                    SqlDbType = SqlDbType.Int,
                    Value = inValorDocIdent,
                    Direction = ParameterDirection.Input
                };
                SqlParameter paramNombreEmpleado = new SqlParameter
                {
                    ParameterName = "@inNombre",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 128,
                    Value = inNombre,
                    Direction = ParameterDirection.Input
                };
                SqlParameter paramSaldoVacasEmpleado = new SqlParameter
                {
                    ParameterName = "@inSaldoVacaciones",
                    SqlDbType = SqlDbType.Decimal,
                    Value = inSaldoVacaciones,
                    Direction = ParameterDirection.Input
                };
                SqlParameter paramMontoMovimiento = new SqlParameter
                {
                    ParameterName = "@inMonto",
                    SqlDbType = SqlDbType.Decimal,
                    Value = inMonto,
                    Direction = ParameterDirection.Input
                };
                SqlParameter paramTipoMovimiento = new SqlParameter
                {
                    ParameterName = "@inTipoMovimiento",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 32,
                    Value = inTipoMovimiento,
                    Direction = ParameterDirection.Input
                };
                SqlParameter paramPostInIP = new SqlParameter
                {
                    ParameterName = "@inPostInIP",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 32,
                    Value = ippaddress.ToString(),
                    Direction = ParameterDirection.Input
                };
                SqlParameter paramResultado = new SqlParameter
                {
                    ParameterName = "@outResult",
                    SqlDbType = SqlDbType.Int,
                    Value = -345678,
                    Direction = ParameterDirection.InputOutput
                };

                //Se agrega cada parámetro al SP
                comando.Parameters.Add(paramValorDocIdentEmpleado);
                comando.Parameters.Add(paramNombreEmpleado);
                comando.Parameters.Add(paramSaldoVacasEmpleado);
                comando.Parameters.Add(paramMontoMovimiento);
                comando.Parameters.Add(paramTipoMovimiento);
                comando.Parameters.Add(paramPostInIP);
                comando.Parameters.Add(paramResultado);

                comando.ExecuteNonQuery();

                //Se leen los parámetros de salida
                string SPresult = comando.Parameters["@outResult"].Value.ToString()!;
                Console.WriteLine("\n------------------- SE HA EJECUTADO EL SP_AgregarMovimiento -------------------");
                Console.WriteLine(" El codigo de salida del sp es: " + SPresult);
                Console.WriteLine("-----------------------------------------------------------------------------\n");

                connection.Close();

                return SPresult;
            }
            catch (Exception ex)
            {
                return System.String.Format("El error es: {0}", ex.ToString());
            }
        }//end method

        public ActionResult HacerAviso(string nombreVista, string codigo, ModeloAgregarMovimiento modelo)
        {
            if (nombreVista == "Listar")
            {
                TempData["Message"] = "Inserción de movimiento exitosa";
                return RedirectToAction(nombreVista, "Empleado");
            }
            //Error generado en el try and catch del metodo que agrega el empleado a la BD
            else if (codigo != "0" && codigo != "50011")
            {
                TempData["Message"] = codigo; //el mismo codigo seria el error generado en el metodo AgregarMovimiento
                return RedirectToAction(nombreVista, "Movimiento", modelo);
            }
            else if (nombreVista == "Agregar")
            {
                //Consulta el error y lo guarda comno aviso cuando redireccione a la pagina de inicio de sesion
                TempData["Message"] = ValidacionesEstaticas.ConsultaCodError(codigo, this._dbContext);
                return RedirectToAction("Agregar", "Movimiento", new { modelo.empleado.Nombre });
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult ControlErrores(ModeloAgregarMovimiento modelo)
        {
            //Se eliminan algunas validaciones que hace el ModelState que realmente no se necesitan para efectos del formulario
            ModelState.Remove("movimiento.PostInIp");
            ModelState.Remove("movimiento.IdEmpleadoNavigation");
            ModelState.Remove("movimiento.IdPostByUserNavigation.Password");
            ModelState.Remove("movimiento.IdPostByUserNavigation.Username");
            ModelState.Remove("movimiento.IdTipoMovimientoNavigation.TipoAccion");

            if (ModelState.IsValid)
            {
                //Intentamos agregar el movimiento
                string resultadoSP = AgregarMovimiento(modelo.empleado.ValorDocumentoIdentidad, modelo.empleado.Nombre, modelo.empleado.SaldoVacaciones, modelo.movimiento.Monto, modelo.movimiento.IdTipoMovimientoNavigation.Nombre);

                if (resultadoSP == "0")
                {
                    return HacerAviso("Listar", resultadoSP, modelo);
                }
                else
                {
                    return HacerAviso("Agregar", resultadoSP, modelo);
                }
            }

            TempData["Message"] = "Seleccione un tipo de movimiento valido";

            //Se serializa el modelo en un Json para enviarlo por TempData ya que por temas de HttpPost y HttpGet que se hacen
            //entre un metodo y el otro, no deja enviar el modelo por parametro
            TempData["Modelo"] = JsonConvert.SerializeObject(modelo);
            return RedirectToAction("Agregar", "Movimiento", new { modelo.empleado.Nombre });

        }//end method

    }//end class

}//end namespace
