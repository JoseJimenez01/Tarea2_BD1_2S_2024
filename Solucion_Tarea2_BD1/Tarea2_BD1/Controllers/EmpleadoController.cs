﻿using Microsoft.AspNetCore.Mvc;
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
                connection.Open();

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

        public ActionResult HacerAviso(string nombreVista, string codigo, Empleado empleado)
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
        public IActionResult ControlDeErroresAvisos(Empleado empleado, string stringPuesto, IFormCollection form)
        {
            //Se quita la validacion que no valida nada del formulario realmente
            ModelState.Remove("IdPuestoNavigation");
            ModelState.Remove("ValorDocumentoIdentidad");
            ModelState.Remove("IdPuestoNavigation.Nombre");
            if (ModelState.IsValid)
            {
                //Intentamos agregar el usuario
                string resultadoSP = AgregarEmpleado(form["ValorDocumentoIdentidad"].ToString(), empleado.Nombre, stringPuesto);

                if (resultadoSP == "0")
                {
                    return HacerAviso("Listar", resultadoSP, empleado);
                }
                else
                {
                    return HacerAviso("Agregar", resultadoSP, empleado);
                }
            }
            TempData["Message"] = "Seleccione un puesto valido";
            return RedirectToAction("Agregar", "Empleado", empleado);
        }//end method

        [HttpPost]
        [Route("actualizarEmpleado")]
        public string ActualizarEmpleado(string inValorDocIdentOriginal, string inNombreOriginal, string inPuestoOriginal, string inValorDocIdent, string inNombre, string inPuesto)
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
                connection.Open();

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

        public ActualizarEmpleado sacarEmpleadoUpdate(ActualizarEmpleado inModelo)
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
                SqlDataReader reader = comando.ExecuteReader();

                reader.Read();
                ActualizarEmpleado modelo = new ActualizarEmpleado();
                modelo.empleadoOriginal.Nombre = Convert.ToString(reader["Nombre"])!;
                modelo.empleadoOriginal.ValorDocumentoIdentidad = Convert.ToInt32(reader["ValorDocumentoIdentidad"]);
                modelo.empleadoOriginal.SaldoVacaciones = Convert.ToDecimal(reader["SaldoVacaciones"]);
                modelo.empleadoOriginal.IdPuestoNavigation.Nombre = Convert.ToString(reader["PuestoNombre"])!;

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
                ActualizarEmpleado modeloError = new ActualizarEmpleado();
                modeloError.empleadoOriginal.Nombre = ex.Message;
                return modeloError;
            }
        }// end meethod

        public IActionResult Update(string? Nombre)
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
            modeloRecibido = sacarEmpleadoUpdate(modeloEnviado);

            return View(modeloRecibido);
        }//end method

        public ActionResult HacerAvisoUpdate(string nombreVista, string codigo, ActualizarEmpleado modelo)
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
                TempData["Message"] = ValidacionesEstaticas.ConsultaCodError(codigo, this._dbContext);
                return RedirectToAction(nombreVista, "Empleado", new { modelo.empleadoOriginal.Nombre });
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult ControlErroresActualizar(ActualizarEmpleado modelo, IFormCollection form)
        {
            ModelState.Remove("empleadoNuevo.ValorDocumentoIdentidad");
            if (ModelState.IsValid)
            {
                //Intentamos actualizar el empleado
                string resultadoSP = ActualizarEmpleado(modelo.empleadoOriginal.ValorDocumentoIdentidad.ToString(), modelo.empleadoOriginal.Nombre, modelo.empleadoOriginal.IdPuestoNavigation.Nombre, form["empleadoNuevo.ValorDocumentoIdentidad"].ToString(), modelo.empleadoNuevo.Nombre, modelo.empleadoNuevo.IdPuestoNavigation.Nombre);
                if (resultadoSP == "0")
                {
                    return HacerAvisoUpdate("Listar", resultadoSP, modelo);
                }
                else
                {
                    return HacerAvisoUpdate("Update", resultadoSP, modelo);
                }
            }

            TempData["Message"] = "Seleccione un puesto valido";

            //Se serializa el modelo en un Json para enviarlo por TempData ya que por temas de HttpPost y HttpGet que se hacen
            //entre un metodo y el otro, no deja enviar el modelo por parametro
            TempData["Modelo"] = JsonConvert.SerializeObject(modelo);
            return RedirectToAction("Update", "Empleado", new { modelo.empleadoOriginal.Nombre });

        }//end method

        //-------------------------------------------------------------------------------- Metodos para borrar empleado-------------------------------------------------

        [HttpPost]
        [Route("borrar-empleado")]
        public string BorrarEmpleado(string inNombre, int inValorDocIdent, string inPuesto, Decimal inSaldoVacaciones, int inConfirmacion )
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

                comando.ExecuteNonQuery();

                //Se leen los parámetros de salida
                string SPresult = comando.Parameters["@outResult"].Value.ToString()!;
                Console.WriteLine("\n------------------- SE HA EJECUTADO EL SP_BorrarEmpleado -------------------");
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

        public Empleado sacarEmpleadoBorrar(Empleado inModelo)
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
                SqlDataReader reader = comando.ExecuteReader();

                reader.Read();
                Empleado modelo = new Empleado();
                modelo.Nombre = Convert.ToString(reader["Nombre"])!;
                modelo.ValorDocumentoIdentidad = Convert.ToInt32(reader["ValorDocumentoIdentidad"]);
                modelo.SaldoVacaciones = Convert.ToDecimal(reader["SaldoVacaciones"]);
                modelo.IdPuestoNavigation.Nombre = Convert.ToString(reader["PuestoNombre"])!;

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
                Empleado modeloError = new Empleado();
                modeloError.Nombre = ex.Message;
                return modeloError;
            }
        }// end meethod

        public IActionResult Borrar(string? Nombre)
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
            modeloRecibido = sacarEmpleadoBorrar(modeloEnviado);

            return View(modeloRecibido);
        }//end method

        public ActionResult HacerAvisoBorrar(string nombreVista, string codigo, Empleado modelo)
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
                TempData["Message"] = ValidacionesEstaticas.ConsultaCodError(codigo, this._dbContext);
                return RedirectToAction(nombreVista, "Empleado", new { modelo.Nombre });
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult ControlErroresBorrar(Empleado modelo, string confirmacion)
        {
            if(confirmacion == "Si")
            {
                //Intentamos actualizar el empleado
                string resultadoSP = BorrarEmpleado(modelo.Nombre, modelo.ValorDocumentoIdentidad, modelo.IdPuestoNavigation.Nombre, modelo.SaldoVacaciones, 1);
                if (resultadoSP == "0")
                {
                    return HacerAvisoBorrar("Listar", resultadoSP, modelo);
                }
                else
                {
                    return HacerAvisoBorrar("Borrar", resultadoSP, modelo);
                }
            }
            else if (confirmacion == "No")
            {
                TempData["Message"] = "No se ha borrado ningún empleado";
                BorrarEmpleado(modelo.Nombre, modelo.ValorDocumentoIdentidad, modelo.IdPuestoNavigation.Nombre, modelo.SaldoVacaciones, 2);
                return RedirectToAction("Listar", "Empleado");
            }
            //Se serializa el modelo en un Json para enviarlo por TempData ya que por temas de HttpPost y HttpGet que se hacen
            //entre un metodo y el otro, no deja enviar el modelo por parametro
            TempData["Modelo"] = JsonConvert.SerializeObject(modelo);
            return RedirectToAction("Borrar", "Empleado", new { modelo.Nombre });

        }//end method

        //--------------------------------------------------------------------------------------- METODOS PARA CONSULTAR EMPLEADOS
        public IActionResult Consulta(string Nombre)
        {
            Empleado modeloEnviado = new Empleado();
            Empleado modeloRecibido = new Empleado();

            modeloEnviado.Nombre = Nombre;

            modeloRecibido = sacarEmpleadoBorrar(modeloEnviado);

            return View(modeloRecibido);
        }//end method


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }//end class
}//end namespace
