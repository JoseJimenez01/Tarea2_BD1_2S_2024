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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace Tarea2_BD1.Controllers
{
    [Authorize]
    public class EmpleadoController : Controller
    {
        public readonly Dbtarea2Context _dbContext;

        /// <summary>
        /// Constructor for the EmpleadoController class. Initializes the database context.
        /// </summary>
        /// <param name="_context">The Entity Framework Core database context.</param>
        public EmpleadoController(Dbtarea2Context _context)
        {
            _dbContext = _context;
        }

        /// <summary>
        /// Displays the view to add a new employee.
        /// </summary>
        /// <returns>The "Agregar" view containing the employee registration form.</returns>
        public IActionResult Agregar()
        {
            return View();
        }

        /// <summary>
        /// Obtains the complete list of employees from the database using the SP_ListarEmpleados stored procedure.
        /// </summary>
        /// <returns>The "Listar" view containing the active employees list, or a BadRequest if an error occurs.</returns>
        [HttpGet("/Empleados")]
        public async Task<IActionResult> Listar()
        {
            try
            {
                //Se crea a conexión se abre
                SqlConnection connection = (SqlConnection)_dbContext.Database.GetDbConnection();
                await connection.OpenAsync();

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
                SqlDataReader reader = await comando.ExecuteReaderAsync();
                List<Models.Empleado> listaEmpleados = new List<Models.Empleado>();
                while (await reader.ReadAsync())
                {
                    Empleado empleado = new Empleado();
                    empleado.Nombre = Convert.ToString(reader["Nombre"])!;
                    empleado.ValorDocumentoIdentidad = Convert.ToInt32(reader["ValorDocumentoIdentidad"]);
                    listaEmpleados.Add(empleado);
                }
                await reader.CloseAsync();

                await comando.ExecuteNonQueryAsync();

                //Se leen los parámetros de salida
                string SPresult = comando.Parameters["@outResult"].Value.ToString()!;
                Console.WriteLine("\n------------------- SE HA EJECUTADO EL SP_ListarEmpleados -------------------");
                Console.WriteLine(" El codigo de salida del sp es: " + SPresult);
                Console.WriteLine("-----------------------------------------------------------------------------\n");
                
                await connection.CloseAsync();

                return View(listaEmpleados);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }// end meethod

        /// <summary>
        /// Filters the employee list by name or by identity document value using the SP_Filtro stored procedure.
        /// Logs the client's IP address in the event log.
        /// </summary>
        /// <param name="entradaStringFiltro">The term to filter by (name or identity document value).</param>
        /// <returns>A partial view "_VistaParcialFiltro" containing the filtered results, or a BadRequest if an error occurs.</returns>
        [HttpPost]
        public async Task<IActionResult> Filtrar(string entradaStringFiltro)
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
                await connection.OpenAsync();

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
                SqlDataReader reader = await comando.ExecuteReaderAsync();
                List<Models.Empleado> listaEmpleados = new List<Models.Empleado>();
                while (await reader.ReadAsync())
                {
                    Empleado empleado = new Empleado();
                    empleado.Nombre = Convert.ToString(reader["Nombre"])!;
                    empleado.ValorDocumentoIdentidad = Convert.ToInt32(reader["ValorDocumentoIdentidad"]);
                    listaEmpleados.Add(empleado);
                }
                await reader.CloseAsync();

                await comando.ExecuteNonQueryAsync();

                //Se leen los parámetros de salida
                string SPresult = comando.Parameters["@outResult"].Value.ToString()!;
                Console.WriteLine("\n------------------- SE HA EJECUTADO EL SP_Filtro -------------------");
                Console.WriteLine(" El codigo de salida del sp es: " + SPresult);
                Console.WriteLine("-----------------------------------------------------------------------------\n");

                await connection.CloseAsync();

                return PartialView("_VistaParcialFiltro", listaEmpleados);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }//end method

        /// <summary>
        /// Registers a new employee in the database by calling the SP_AgregarEmpleado stored procedure.
        /// Validates via regular expression if the entered name contains only alphabetic characters.
        /// </summary>
        /// <param name="inValorDocIdent">The identity document value as a string.</param>
        /// <param name="inNombre">The full name of the employee.</param>
        /// <param name="inPuesto">The name of the position to assign.</param>
        /// <returns>The exit code from the stored procedure or the error message in case of an exception.</returns>
        [HttpPost]
        public async Task<string> AgregarEmpleado(string inValorDocIdent, string inNombre, string inPuesto)
        {
            try
            {
                //Sacamos la ip desde donde se ejecuta
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                var ippaddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

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
                await connection.OpenAsync();

                //Se crea el SP
                SqlCommand comando = connection.CreateCommand();
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.CommandText = "SP_AgregarEmpleado";

                SqlParameter paramValorDocIdentEmpleado = new SqlParameter
                {
                    ParameterName = "@inValorDocIdent",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 128,
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
                comando.Parameters.Add(paramPostInIP);
                comando.Parameters.Add(paramResultado);

                await comando.ExecuteNonQueryAsync();

                //Se leen los parámetros de salida
                string SPresult = comando.Parameters["@outResult"].Value.ToString()!;
                Console.WriteLine("\n------------------- SE HA EJECUTADO EL SP_AgregarEmpleado -------------------");
                Console.WriteLine(" El codigo de salida del sp es: " + SPresult);
                Console.WriteLine("-----------------------------------------------------------------------------\n");

                await connection.CloseAsync();

                return SPresult;
            }
            catch (Exception ex)
            {
                return System.String.Format("El error es: {0}", ex.ToString());
            }
        }//end method

        /// <summary>
        /// Manages notifications and TempData messages based on the response code received when adding an employee.
        /// </summary>
        /// <param name="nombreVista">The name of the view to redirect to.</param>
        /// <param name="codigo">The result code returned by the database or exception.</param>
        /// <param name="empleado">The Empleado object containing the entered data.</param>
        /// <returns>An ActionResult redirecting to the corresponding action with the configured message.</returns>
        public async Task<ActionResult> HacerAviso(string nombreVista, string codigo, Empleado empleado)
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
                TempData["Message"] = await ValidacionesEstaticas.ConsultaCodError(codigo, this._dbContext);
                return RedirectToAction(nombreVista, empleado);
            }
            return Ok();
        }

        /// <summary>
        /// Controller action that processes the employee creation form submission. Validates model state,
        /// calls AgregarEmpleado, and manages notifications according to the stored procedure's result.
        /// </summary>
        /// <param name="empleado">The Empleado model with basic data.</param>
        /// <param name="stringPuesto">The selected position in text format.</param>
        /// <param name="form">The form collection containing the complete identity document value.</param>
        /// <returns>A redirect to the corresponding view containing the operation result.</returns>
        [HttpPost]
        public async Task<IActionResult> ControlDeErroresAvisos(Empleado empleado, string stringPuesto, IFormCollection form)
        {
            //Se quita la validacion que no valida nada del formulario realmente
            ModelState.Remove("IdPuestoNavigation");
            ModelState.Remove("ValorDocumentoIdentidad");
            ModelState.Remove("IdPuestoNavigation.Nombre");
            if (ModelState.IsValid)
            {
                //Intentamos agregar el usuario
                string resultadoSP = await AgregarEmpleado(form["ValorDocumentoIdentidad"].ToString(), empleado.Nombre, stringPuesto);

                if (resultadoSP == "0")
                {
                    return await HacerAviso("Listar", resultadoSP, empleado);
                }
                else
                {
                    return await HacerAviso("Agregar", resultadoSP, empleado);
                }
            }
            TempData["Message"] = "Seleccione un puesto valido";
            return RedirectToAction("Agregar", "Empleado", empleado);
        }//end method

        /// <summary>
        /// Calls the SP_ActualizarEmpleado stored procedure to modify the details of an existing employee.
        /// Validates the alphabetic structure of the new name and logs the client's IP address.
        /// </summary>
        /// <param name="inValorDocIdentOriginal">The current identity document value of the employee.</param>
        /// <param name="inNombreOriginal">The current name of the employee.</param>
        /// <param name="inPuestoOriginal">The current position of the employee.</param>
        /// <param name="inValorDocIdent">The proposed new identity document value.</param>
        /// <param name="inNombre">The proposed new name.</param>
        /// <param name="inPuesto">The proposed new position.</param>
        /// <returns>The exit code from the stored procedure or the error message in case of an exception.</returns>
        [HttpPost]
        public async Task<string> ActualizarEmpleado(string inValorDocIdentOriginal, string inNombreOriginal, string inPuestoOriginal, string inValorDocIdent, string inNombre, string inPuesto)
        {
            try
            {
                //Sacamos la ip desde donde se ejecuta
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                var ippaddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

                //Definimos el patron de la expresion regular para ver si hay numeros intercalados en el nombre
                string patron = @"^[A-Za-z\ \-\xC1\xC9\xCD\xD3\xDA\xDC\xE1\xE9\xED\xF3\xFA\xFC\xD1\xF1]+$";

                //Para revisar la expresion regular
                int esNombreAlfabetico;

                if (Regex.IsMatch(inNombre, patron))
                {
                    esNombreAlfabetico = 1;
                }
                else
                {
                    esNombreAlfabetico = 2;
                }

                //Se crea a conexión se abre
                SqlConnection connection = (SqlConnection)_dbContext.Database.GetDbConnection();
                await connection.OpenAsync();

                //Se crea el SP
                SqlCommand comando = connection.CreateCommand();
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.CommandText = "SP_ActualizarEmpleado";

                SqlParameter paramValorDocIdentEmpleadoOriginal = new SqlParameter
                {
                    ParameterName = "@inValorDocIdentOriginal",
                    SqlDbType = SqlDbType.Int,
                    Value = inValorDocIdentOriginal,
                    Direction = ParameterDirection.Input
                };
                SqlParameter paramNombreEmpleadoOriginal = new SqlParameter
                {
                    ParameterName = "@inNombreOriginal",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 128,
                    Value = inNombreOriginal,
                    Direction = ParameterDirection.Input
                };
                SqlParameter paramPuestoEmpleadoOriginal = new SqlParameter
                {
                    ParameterName = "@inPuestoOriginal",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 128,
                    Value = inPuestoOriginal,
                    Direction = ParameterDirection.Input
                };
                SqlParameter paramValorDocIdentEmpleado = new SqlParameter
                {
                    ParameterName = "@inValorDocIdent",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 128,
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
                comando.Parameters.Add(paramValorDocIdentEmpleadoOriginal);
                comando.Parameters.Add(paramNombreEmpleadoOriginal);
                comando.Parameters.Add(paramPuestoEmpleadoOriginal);
                comando.Parameters.Add(paramValorDocIdentEmpleado);
                comando.Parameters.Add(paramNombreEmpleado);
                comando.Parameters.Add(paramPuestoEmpleado);
                comando.Parameters.Add(paramConfirmarNombreAlfabetico);
                comando.Parameters.Add(paramPostInIP);
                comando.Parameters.Add(paramResultado);

                await comando.ExecuteNonQueryAsync();

                //Se leen los parámetros de salida
                string SPresult = comando.Parameters["@outResult"].Value.ToString()!;
                Console.WriteLine("\n------------------- SE HA EJECUTADO EL SP_AgregarEmpleado -------------------");
                Console.WriteLine(" El codigo de salida del sp es: " + SPresult);
                Console.WriteLine("-----------------------------------------------------------------------------\n");

                await connection.CloseAsync();

                return SPresult;
            }
            catch (Exception ex)
            {
                return System.String.Format("El error es: {0}", ex.ToString());
            }
        }//end method

        /// <summary>
        /// Queries and retrieves the original data of an employee by name using SP_SacarEmpleado to fill the update model.
        /// </summary>
        /// <param name="inModelo">The ActualizarEmpleado model containing the name of the employee to search for.</param>
        /// <returns>The model containing the loaded employee information, or a model with an error message if the query fails.</returns>
        public async Task<ActualizarEmpleado> sacarEmpleadoUpdate(ActualizarEmpleado inModelo)
        {
            try
            {
                //Se crea a conexión se abre
                SqlConnection connection = (SqlConnection)_dbContext.Database.GetDbConnection();
                await connection.OpenAsync();

                //Se crea el SP
                SqlCommand comando = connection.CreateCommand();
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.CommandText = "SP_SacarEmpleado";

                SqlParameter paramNombreEmpleado = new SqlParameter
                {
                    ParameterName = "@inNombreEmpleado",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 128,
                    Value = inModelo.empleadoOriginal.Nombre,
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
                SqlDataReader reader = await comando.ExecuteReaderAsync();

                await reader.ReadAsync();
                ActualizarEmpleado modelo = new ActualizarEmpleado();
                modelo.empleadoOriginal.Nombre = Convert.ToString(reader["Nombre"])!;
                modelo.empleadoOriginal.ValorDocumentoIdentidad = Convert.ToInt32(reader["ValorDocumentoIdentidad"]);
                modelo.empleadoOriginal.SaldoVacaciones = Convert.ToDecimal(reader["SaldoVacaciones"]);
                modelo.empleadoOriginal.IdPuestoNavigation.Nombre = Convert.ToString(reader["PuestoNombre"])!;

                await reader.CloseAsync();

                await comando.ExecuteNonQueryAsync();

                //Se leen los parámetros de salida
                string SPresult = comando.Parameters["@outResult"].Value.ToString()!;
                Console.WriteLine("\n------------------- SE HA EJECUTADO EL SP_SacarEmpleado -------------------");
                Console.WriteLine(" El codigo de salida del sp es: " + SPresult);
                Console.WriteLine("-----------------------------------------------------------------------------\n");

                await connection.CloseAsync();

                return modelo;
            }
            catch (Exception ex)
            {
                ActualizarEmpleado modeloError = new ActualizarEmpleado();
                modeloError.empleadoOriginal.Nombre = ex.Message;
                return modeloError;
            }
        }// end meethod

        /// <summary>
        /// Displays the view to update an employee. Retrieves original data based on the Nombre parameter
        /// or TempData if returning from a previous failed attempt.
        /// </summary>
        /// <param name="Nombre">The name of the employee to update.</param>
        /// <returns>The "Update" view loaded with the ActualizarEmpleado model.</returns>
        public async Task<IActionResult> Update(string? Nombre)
        {
            //Se descerializa el modelo para seguir validando
            var modeloJson = TempData["Modelo"] as string;
            var modelo = new ActualizarEmpleado();

            if (modeloJson != null)
            {
                modelo = JsonConvert.DeserializeObject<ActualizarEmpleado>(modeloJson);
            }

            ActualizarEmpleado modeloEnviado = new ActualizarEmpleado();
            ActualizarEmpleado modeloRecibido = new ActualizarEmpleado();

            if (Nombre != null)
            {
                modeloEnviado.empleadoOriginal.Nombre = Nombre;
            }
            else
            {
                modeloEnviado.empleadoOriginal.Nombre = modelo.empleadoOriginal.Nombre;
            }
            modeloRecibido = await sacarEmpleadoUpdate(modeloEnviado);

            return View(modeloRecibido);
        }//end method

        /// <summary>
        /// Manages notifications and TempData messages based on the response code received when updating an employee.
        /// </summary>
        /// <param name="nombreVista">The name of the view to redirect to.</param>
        /// <param name="codigo">The result code returned by the database or exception.</param>
        /// <param name="modelo">The ActualizarEmpleado model involved in the transaction.</param>
        /// <returns>An ActionResult redirecting to the corresponding action with the configured message.</returns>
        public async Task<ActionResult> HacerAvisoUpdate(string nombreVista, string codigo, ActualizarEmpleado modelo)
        {
            if (nombreVista == "Listar")
            {
                TempData["Message"] = "Actualización exitosa";
                return RedirectToAction(nombreVista, "Empleado");
            }
            //Error generado en el try and catch del metodo que agrega el empleado a la BD
            else if (codigo != "0" && codigo != "50009" && codigo != "50010" && codigo != "50007" && codigo != "50006")
            {
                TempData["Message"] = codigo; //el mismo codigo seria el error generado en el metodo ActualizarEmpleado
                return RedirectToAction(nombreVista, modelo);
            }
            else if (nombreVista == "Update")
            {
                //Consulta el error y lo guarda comno aviso cuando redireccione a la pagina de inicio de sesion
                TempData["Message"] = await ValidacionesEstaticas.ConsultaCodError(codigo, this._dbContext);
                return RedirectToAction(nombreVista, "Empleado", new { modelo.empleadoOriginal.Nombre });
            }
            return Ok();
        }

        /// <summary>
        /// Processes the employee update form submission. Validates model state,
        /// calls ActualizarEmpleado, and manages notifications according to the stored procedure's result.
        /// </summary>
        /// <param name="modelo">The model containing the employee's original and new information.</param>
        /// <param name="form">The form collection with the new identity document value.</param>
        /// <returns>A redirect to the corresponding view containing the operation result.</returns>
        [HttpPost]
        public async Task<IActionResult> ControlErroresActualizar(ActualizarEmpleado modelo, IFormCollection form)
        {
            ModelState.Remove("empleadoNuevo.ValorDocumentoIdentidad");
            if (ModelState.IsValid)
            {
                //Intentamos actualizar el empleado
                string resultadoSP = await ActualizarEmpleado(modelo.empleadoOriginal.ValorDocumentoIdentidad.ToString(), modelo.empleadoOriginal.Nombre, modelo.empleadoOriginal.IdPuestoNavigation.Nombre, form["empleadoNuevo.ValorDocumentoIdentidad"].ToString(), modelo.empleadoNuevo.Nombre, modelo.empleadoNuevo.IdPuestoNavigation.Nombre);
                if (resultadoSP == "0")
                {
                    return await HacerAvisoUpdate("Listar", resultadoSP, modelo);
                }
                else
                {
                    return await HacerAvisoUpdate("Update", resultadoSP, modelo);
                }
            }

            TempData["Message"] = "Seleccione un puesto valido";

            //Se serializa el modelo en un Json para enviarlo por TempData ya que por temas de HttpPost y HttpGet que se hacen
            //entre un metodo y el otro, no deja enviar el modelo por parametro
            TempData["Modelo"] = JsonConvert.SerializeObject(modelo);
            return RedirectToAction("Update", "Empleado", new { modelo.empleadoOriginal.Nombre });

        }//end method

        //-------------------------------------------------------------------------------- Metodos para borrar empleado-------------------------------------------------

        /// <summary>
        /// Calls the SP_BorrarEmpleado stored procedure to logically or physically delete an employee
        /// depending on the provided confirmation, logging the user's IP address.
        /// </summary>
        /// <param name="inNombre">The name of the employee.</param>
        /// <param name="inValorDocIdent">The identity document value of the employee.</param>
        /// <param name="inPuesto">The position of the employee.</param>
        /// <param name="inSaldoVacaciones">The accumulated vacation balance of the employee.</param>
        /// <param name="inConfirmacion">The confirmation code (1 to proceed with deletion, 2 to cancel).</param>
        /// <returns>The exit code from the stored procedure or the error message in case of an exception.</returns>
        [HttpPost]
        public async Task<string> BorrarEmpleado(string inNombre, int inValorDocIdent, string inPuesto, Decimal inSaldoVacaciones, int inConfirmacion )
        {
            try
            {
                //Sacamos la ip desde donde se ejecuta
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                var ippaddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

                //Se crea a conexión se abre
                SqlConnection connection = (SqlConnection)_dbContext.Database.GetDbConnection();
                await connection.OpenAsync();

                //Se crea el SP
                SqlCommand comando = connection.CreateCommand();
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.CommandText = "SP_BorrarEmpleado";

                SqlParameter paramValorDocIdentEmpleado = new SqlParameter
                {
                    ParameterName = "@inValorDocIdent",
                    SqlDbType = SqlDbType.Int,
                    Value = inValorDocIdent,
                    Direction = ParameterDirection.Input
                };
                SqlParameter paramNombreEmpleado = new SqlParameter
                {
                    ParameterName = "@inNombreEmpleado",
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

                SqlParameter paramPuestoEmpleado = new SqlParameter
                {
                    ParameterName = "@inPuesto",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 128,
                    Value = inPuesto,
                    Direction = ParameterDirection.Input
                };
                SqlParameter paramConfirmacion = new SqlParameter
                {
                    ParameterName = "@inConfirmaOno",
                    SqlDbType = SqlDbType.Int,
                    Value = inConfirmacion,
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
                comando.Parameters.Add(paramPuestoEmpleado);
                comando.Parameters.Add(paramConfirmacion);
                comando.Parameters.Add(paramPostInIP);
                comando.Parameters.Add(paramResultado);

                await comando.ExecuteNonQueryAsync();

                //Se leen los parámetros de salida
                string SPresult = comando.Parameters["@outResult"].Value.ToString()!;
                Console.WriteLine("\n------------------- SE HA EJECUTADO EL SP_BorrarEmpleado -------------------");
                Console.WriteLine(" El codigo de salida del sp es: " + SPresult);
                Console.WriteLine("-----------------------------------------------------------------------------\n");

                await connection.CloseAsync();

                return SPresult;
            }
            catch (Exception ex)
            {
                return System.String.Format("El error es: {0}", ex.ToString());
            }
        }//end method

        /// <summary>
        /// Queries and retrieves the detailed data of an employee by calling the SP_SacarEmpleado stored procedure.
        /// </summary>
        /// <param name="inModelo">The Empleado model containing the name of the employee to search for.</param>
        /// <returns>An Empleado model filled with the corresponding data, or a model with an error message if it fails.</returns>
        public async Task<Empleado> sacarEmpleadoBorrar(Empleado inModelo)
        {
            try
            {
                //Se crea a conexión se abre
                SqlConnection connection = (SqlConnection)_dbContext.Database.GetDbConnection();
                await connection.OpenAsync();

                //Se crea el SP
                SqlCommand comando = connection.CreateCommand();
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.CommandText = "SP_SacarEmpleado";

                SqlParameter paramNombreEmpleado = new SqlParameter
                {
                    ParameterName = "@inNombreEmpleado",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 128,
                    Value = inModelo.Nombre,
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
                SqlDataReader reader = await comando.ExecuteReaderAsync();

                await reader.ReadAsync();
                Empleado modelo = new Empleado();
                modelo.Nombre = Convert.ToString(reader["Nombre"])!;
                modelo.ValorDocumentoIdentidad = Convert.ToInt32(reader["ValorDocumentoIdentidad"]);
                modelo.SaldoVacaciones = Convert.ToDecimal(reader["SaldoVacaciones"]);
                modelo.IdPuestoNavigation.Nombre = Convert.ToString(reader["PuestoNombre"])!;

                await reader.CloseAsync();

                await comando.ExecuteNonQueryAsync();

                //Se leen los parámetros de salida
                string SPresult = comando.Parameters["@outResult"].Value.ToString()!;
                Console.WriteLine("\n------------------- SE HA EJECUTADO EL SP_SacarEmpleado -------------------");
                Console.WriteLine(" El codigo de salida del sp es: " + SPresult);
                Console.WriteLine("-----------------------------------------------------------------------------\n");

                await connection.CloseAsync();

                return modelo;
            }
            catch (Exception ex)
            {
                Empleado modeloError = new Empleado();
                modeloError.Nombre = ex.Message;
                return modeloError;
            }
        }// end meethod

        /// <summary>
        /// Displays the confirmation view to delete an employee, loading their current data.
        /// </summary>
        /// <param name="Nombre">The name of the employee to delete.</param>
        /// <returns>The "Borrar" view loaded with the employee's data.</returns>
        public async Task<IActionResult> Borrar(string? Nombre)
        {
            //Se descerializa el modelo para seguir validando
            var modeloJson = TempData["Modelo"] as string;
            var modelo = new Empleado();

            if (modeloJson != null)
            {
                modelo = JsonConvert.DeserializeObject<Empleado>(modeloJson);
            }

            Empleado modeloEnviado = new Empleado();
            Empleado modeloRecibido = new Empleado();

            if (Nombre != null)
            {
                modeloEnviado.Nombre = Nombre;
            }
            else
            {
                modeloEnviado.Nombre = modelo.Nombre;
            }
            modeloRecibido = await sacarEmpleadoBorrar(modeloEnviado);

            return View(modeloRecibido);
        }//end method

        /// <summary>
        /// Manages notifications and TempData messages based on the response code received when deleting an employee.
        /// </summary>
        /// <param name="nombreVista">The name of the view to redirect to.</param>
        /// <param name="codigo">The result code returned by the database or exception.</param>
        /// <param name="modelo">The Empleado model involved in the transaction.</param>
        /// <returns>An ActionResult redirecting to the corresponding action with the configured message.</returns>
        public async Task<ActionResult> HacerAvisoBorrar(string nombreVista, string codigo, Empleado modelo)
        {
            if (nombreVista == "Listar")
            {
                TempData["Message"] = "Borrado exitoso";
                return RedirectToAction(nombreVista, "Empleado");
            }
            //Error generado en el try and catch del metodo que agrega el empleado a la BD
            else if (codigo != "0")
            {
                TempData["Message"] = codigo; //el mismo codigo seria el error generado en el metodo ActualizarEmpleado
                return RedirectToAction(nombreVista, modelo);
            }
            else if (nombreVista == "Borrar")
            {
                //Consulta el error y lo guarda comno aviso cuando redireccione a la pagina de inicio de sesion
                TempData["Message"] = await ValidacionesEstaticas.ConsultaCodError(codigo, this._dbContext);
                return RedirectToAction(nombreVista, "Empleado", new { modelo.Nombre });
            }
            return Ok();
        }

        /// <summary>
        /// Processes the employee deletion confirmation. Calls BorrarEmpleado and redirects according to
        /// the user's confirmation response ("Si" or "No").
        /// </summary>
        /// <param name="modelo">The model of the employee to be deleted.</param>
        /// <param name="confirmacion">The user's confirmation decision ("Si" or "No").</param>
        /// <returns>A redirect to the appropriate view based on the deletion flow.</returns>
        [HttpPost]
        public async Task<IActionResult> ControlErroresBorrar(Empleado modelo, string confirmacion)
        {
            if(confirmacion == "Si")
            {
                //Intentamos actualizar el empleado
                string resultadoSP = await BorrarEmpleado(modelo.Nombre, modelo.ValorDocumentoIdentidad, modelo.IdPuestoNavigation.Nombre, modelo.SaldoVacaciones, 1);
                if (resultadoSP == "0")
                {
                    return await HacerAvisoBorrar("Listar", resultadoSP, modelo);
                }
                else
                {
                    return await HacerAvisoBorrar("Borrar", resultadoSP, modelo);
                }
            }
            else if (confirmacion == "No")
            {
                TempData["Message"] = "No se ha borrado ningún empleado";
                await BorrarEmpleado(modelo.Nombre, modelo.ValorDocumentoIdentidad, modelo.IdPuestoNavigation.Nombre, modelo.SaldoVacaciones, 2);
                return RedirectToAction("Listar", "Empleado");
            }
            //Se serializa el modelo en un Json para enviarlo por TempData ya que por temas de HttpPost y HttpGet que se hacen
            //entre un metodo y el otro, no deja enviar el modelo por parametro
            TempData["Modelo"] = JsonConvert.SerializeObject(modelo);
            return RedirectToAction("Borrar", "Empleado", new { modelo.Nombre });

        }//end method

        //--------------------------------------------------------------------------------------- METODOS PARA CONSULTAR EMPLEADOS
        /// <summary>
        /// Displays the detailed query view with the information of a specific employee.
        /// </summary>
        /// <param name="Nombre">The name of the employee to query.</param>
        /// <returns>The "Consulta" view loaded with the employee's data.</returns>
        public async Task<IActionResult> Consulta(string Nombre)
        {
            Empleado modeloEnviado = new Empleado();
            Empleado modeloRecibido = new Empleado();

            modeloEnviado.Nombre = Nombre;

            modeloRecibido = await sacarEmpleadoBorrar(modeloEnviado);

            return View(modeloRecibido);
        }//end method

    }//end class
}//end namespace
