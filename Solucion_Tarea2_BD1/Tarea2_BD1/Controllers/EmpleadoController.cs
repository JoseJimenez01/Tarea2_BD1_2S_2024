using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.Sockets;
using System.Net;
using Tarea2_BD1.Models;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tarea2_BD1.Controllers
{
    public class EmpleadoController : Controller
    {
        public readonly Dbtarea2Context _dbContext;

        public EmpleadoController(Dbtarea2Context _context)
        {
            _dbContext = _context;
        }

        public IActionResult Agregar()
        {

            return View();
        }

        [HttpGet]
        //[Route("listar_empleados")] /*----------------------------------------------------------------------- descomentar cuando se ponga el patron de inicio origianl -----------------------------------------------*/
        public IActionResult Listar()
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

                SqlParameter paramResultado = new SqlParameter
                {
                    ParameterName = "@outResult",
                    SqlDbType = SqlDbType.Int,
                    Value = -345678,
                    Direction = ParameterDirection.InputOutput
                };

                //Se agrega cada parámetro al SP
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

                return View(listaEmpleados);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }// end meethod

        [HttpPost]
        public IActionResult AgregarEmpleado() //------------------------------------------------------------------------------------------FALTA TERMINAR
        {
            try
            {
                //Sacamos la ip desde donde se ejecuta
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                var ippaddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

                //Para hacerle saber a la base de datos cual es para poder agregarla a la bitacora de eventos
                //11 si es por nombre, 12 si es por valor
                int porNombreOporValor;

                //Para hacer la conversion
                int valorDocIdent;

                //Se revisa si la entrada es nulla o vacia, si son espacios en blanco se toman como vacio
                //para asignarle un valor en blanco.
                //Y se tomaria como consulta por nombre
                if (string.IsNullOrEmpty(entradaStringFiltro))
                {
                    entradaStringFiltro = " ";
                    porNombreOporValor = 11;
                }
                //en caso de true, seria filtro por valor del documento de identidad
                else if (int.TryParse(entradaStringFiltro, out valorDocIdent))
                {
                    porNombreOporValor = 12;
                }
                //Si ninguna de las anteriores, entonces es por nombre tambien
                else
                {
                    porNombreOporValor = 11;
                }

                //Se crea a conexión se abre
                SqlConnection connection = (SqlConnection)_dbContext.Database.GetDbConnection();
                connection.Open();

                //Se crea el SP
                SqlCommand comando = connection.CreateCommand();
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.CommandText = "SP_Filtro";

                SqlParameter paramStringABuscar = new SqlParameter
                {
                    ParameterName = "@inStringFiltro",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 128,
                    Value = entradaStringFiltro,
                    Direction = ParameterDirection.Input
                };
                SqlParameter paramConsultaPorNombreOValorDoc = new SqlParameter
                {
                    ParameterName = "@inNameOrValueDoc",
                    SqlDbType = SqlDbType.Int,
                    Value = porNombreOporValor,
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
                comando.Parameters.Add(paramStringABuscar);
                comando.Parameters.Add(paramConsultaPorNombreOValorDoc);
                comando.Parameters.Add(paramPostInIP);
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
                Console.WriteLine("\n------------------- SE HA EJECUTADO EL SP_Filtro -------------------");
                Console.WriteLine(" El codigo de salida del sp es: " + SPresult);
                Console.WriteLine("-----------------------------------------------------------------------------\n");

                connection.Close();
                
                return PartialView("_VistaParcialFiltro", listaEmpleados);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }//end method

        [HttpPost]
        public IActionResult Filtrar(string entradaStringFiltro)
        {
            try
            {
                //Sacamos la ip desde donde se ejecuta
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                var ippaddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

                //Para hacerle saber a la base de datos cual es para poder agregarla a la bitacora de eventos
                //11 si es por nombre, 12 si es por valor
                int porNombreOporValor;

                //Para hacer la conversion
                int valorDocIdent;

                //Se revisa si la entrada es nulla o vacia, si son espacios en blanco se toman como vacio
                //para asignarle un valor en blanco.
                //Y se tomaria como consulta por nombre
                if (string.IsNullOrEmpty(entradaStringFiltro))
                {
                    entradaStringFiltro = " ";
                    porNombreOporValor = 11;
                }
                //en caso de true, seria filtro por valor del documento de identidad
                else if (int.TryParse(entradaStringFiltro, out valorDocIdent))
                {
                    porNombreOporValor = 12;
                }
                //Si ninguna de las anteriores, entonces es por nombre tambien
                else
                {
                    porNombreOporValor = 11;
                }

                //Se crea a conexión se abre
                SqlConnection connection = (SqlConnection)_dbContext.Database.GetDbConnection();
                connection.Open();

                //Se crea el SP
                SqlCommand comando = connection.CreateCommand();
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.CommandText = "SP_Filtro";

                SqlParameter paramStringABuscar = new SqlParameter
                {
                    ParameterName = "@inStringFiltro",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 128,
                    Value = entradaStringFiltro,
                    Direction = ParameterDirection.Input
                };
                SqlParameter paramConsultaPorNombreOValorDoc = new SqlParameter
                {
                    ParameterName = "@inNameOrValueDoc",
                    SqlDbType = SqlDbType.Int,
                    Value = porNombreOporValor,
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
                comando.Parameters.Add(paramStringABuscar);
                comando.Parameters.Add(paramConsultaPorNombreOValorDoc);
                comando.Parameters.Add(paramPostInIP);
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
                Console.WriteLine("\n------------------- SE HA EJECUTADO EL SP_Filtro -------------------");
                Console.WriteLine(" El codigo de salida del sp es: " + SPresult);
                Console.WriteLine("-----------------------------------------------------------------------------\n");

                connection.Close();

                return PartialView("_VistaParcialFiltro", listaEmpleados);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }//end method

        public ActionResult HacerAviso(string nombreVista, Usuario modeloUsuario, string codigo) //---------------------------------------------------------------------------------FALTA TERMINAR
        {
            //if (nombreVista == "Listar")
            //{
            //    TempData["Message"] = "Inicio de sesión exitoso";
            //    return RedirectToAction(nombreVista, "Empleado");
            //}
            ////Error generado en el try and catch del metodo que hace el inicio de sesion en la BD
            ////O tambien, que tuvo demasiados intentos fallidos en 30 mins
            //else if (codigo != "0" && codigo != "50001" && codigo != "50002" && codigo != "50008")
            //{
            //    TempData["Message"] = codigo;
            //    return RedirectToAction(nombreVista, modeloUsuario);
            //}
            //else if (nombreVista == "SignIn")
            //{
            //    //Consulta el error y lo guarda comno aviso cuando redireccione a la pagina de inicio de sesion
            //    TempData["Message"] = ConsultaCodError(codigo, modeloUsuario.Username);
            //    return RedirectToAction(nombreVista, modeloUsuario);
            //}
            return Ok();
        }

        [HttpPost]
        public IActionResult ControlDeErroresAvisos(string stringValorDocIdent, string stringNombre, string stringPuesto) //-----------------------------------------------------------------------FALTA TERMINAR
        {
            ////Valida la cantidad de inicios de sesion fallidos en 30 mins
            ////si es mayor que 5 deshabilita el boton
            //int cantidadFallos = ConsultaInicioSesionFallidos(30, usuario.Username);
            //if (cantidadFallos > 5)
            //{
            //    return HacerAviso("SignIn", usuario, "Demasiados intentos de login, intente de nuevo dentro de 10 minutos");
            //}

            //if (ModelState.IsValid)
            //{
            //    //Para la descripcion se ocupa lo mismo pero en 20 minutos
            //    cantidadFallos = ConsultaInicioSesionFallidos(20, usuario.Username);
            //    //Resultado del inicio de sesion
            //    string resultado = InicioDeSesion(usuario.Username, usuario.Password, cantidadFallos);

            //    if (resultado == "0")
            //    {
            //        return HacerAviso("Listar", usuario, resultado);
            //    }
            //    else
            //    {
            //        return HacerAviso("SignIn", usuario, resultado);
            //    }
            //}
            return Ok();
        }

    }//end class
}//end namespace
