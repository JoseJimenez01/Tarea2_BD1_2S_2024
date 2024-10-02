using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.Sockets;
using System.Net;
using Tarea2_BD1.Models;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Text.RegularExpressions;
using System.Diagnostics;

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

        [HttpPost]
        [Route("agregarEmpleado")]
        public string AgregarEmpleado(string inValorDocIdent, string inNombre, string inPuesto)
        {
            Console.WriteLine("Entro en AgregarEmpleados");
            try
            {
                //Sacamos la ip desde donde se ejecuta
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                var ippaddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

                //Revisamos si el valor del documento de identidad es entero o si tiene letras intercaladas en el string.
                int esValorDocIdentEntero;

                //Para hacer la conversion
                int valorDocIdent;

                //Si lo convierte, no es alfabetico
                if (int.TryParse(inValorDocIdent, out valorDocIdent))
                {
                    esValorDocIdentEntero = 1;
                }
                //Si no lo convierte, entonces es alfabetico
                else
                {
                    esValorDocIdentEntero = 2;
                }

                //Definimos el patron de la expresion regular para ver si hay numeros intercalados en el nombre
                string patron = @"^[A-Za-z\ \-\xC1\xC9\xCD\xD3\xDA\xDC\xE1\xE9\xED\xF3\xFA\xFC\xD1\xF1]+$";

                //Para revisar la expresion regular
                int esNombreAlfabetico;

                if(Regex.IsMatch(inNombre, patron))
                {
                    esNombreAlfabetico = 1;
                }
                else
                {
                    esNombreAlfabetico = 2;
                }

                //Se crea a conexión se abre
                SqlConnection connection = (SqlConnection)_dbContext.Database.GetDbConnection();
                connection.Open();

                //Se crea el SP
                SqlCommand comando = connection.CreateCommand();
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.CommandText = "SP_AgregarEmpleado";

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
                SqlParameter paramPuestoEmpleado = new SqlParameter
                {
                    ParameterName = "@inPuesto",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 128,
                    Value = inPuesto,
                    Direction = ParameterDirection.Input
                };
                SqlParameter paramConfirmarNombreAlfabetico = new SqlParameter
                {
                    ParameterName = "@inNombreEsAlfabetico",
                    SqlDbType = SqlDbType.Int,
                    Value = esNombreAlfabetico,
                    Direction = ParameterDirection.Input
                };
                SqlParameter paramConfirmarValorDocIdentEntero = new SqlParameter
                {
                    ParameterName = "@inValorDocNoEsAlfabetico",
                    SqlDbType = SqlDbType.Int,
                    Value = esValorDocIdentEntero,
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
                comando.Parameters.Add(paramPuestoEmpleado);
                comando.Parameters.Add(paramConfirmarNombreAlfabetico);
                comando.Parameters.Add(paramConfirmarValorDocIdentEntero);
                comando.Parameters.Add(paramPostInIP);
                comando.Parameters.Add(paramResultado);

                comando.ExecuteNonQuery();

                //Se leen los parámetros de salida
                string SPresult = comando.Parameters["@outResult"].Value.ToString()!;
                Console.WriteLine("\n------------------- SE HA EJECUTADO EL SP_AgregarEmpleado -------------------");
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

        public ActionResult HacerAviso(string nombreVista, string codigo, Empleado empleado) //---------------------------------------------------------------------------------FALTA TERMINAR
        {
            if (nombreVista == "Listar")
            {
                TempData["Message"] = "Inserción exitosa";
                return RedirectToAction(nombreVista, "Empleado");
            }
            //Error generado en el try and catch del metodo que agrega el empleado a la BD
            else if (codigo != "0" && codigo != "50009" && codigo != "50010" && codigo != "50004" && codigo != "50005")
            {
                TempData["Message"] = codigo; //el mismo codigo seria el error generado en el metodo AgregarEmpleado
                return RedirectToAction(nombreVista, empleado);
            }
            else if (nombreVista == "Agregar")
            {
                //Consulta el error y lo guarda comno aviso cuando redireccione a la pagina de inicio de sesion
                TempData["Message"] = ValidacionesEstaticas.ConsultaCodError(codigo, this._dbContext);
                return RedirectToAction(nombreVista, empleado);
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult ControlDeErroresAvisos(Empleado empleado, string stringPuesto)
        {
            Console.WriteLine("Entro en ControlDeErroresAvisos");
            Console.WriteLine("El modelo es valido? " + ModelState.IsValid.ToString());
            if (ModelState.IsValid)
            {
                if (stringPuesto == "nada")
                {
                    TempData["Message"] = "Debe seleccionar algún puesto";
                    return RedirectToAction("Agregar", "Empleado", empleado);
                }

                //Intentamos agregar el usuario
                Console.WriteLine("Entro en ModelState IsValid");
                string resultadoSP = AgregarEmpleado(empleado.ValorDocumentoIdentidad.ToString(), empleado.Nombre, stringPuesto);

                if (resultadoSP == "0")
                {
                    return HacerAviso("Listar", resultadoSP, empleado);
                }
                else
                {
                    return HacerAviso("Agregar", resultadoSP, empleado);
                }
            }
            return Ok();
        }//end method

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }//end class
}//end namespace
