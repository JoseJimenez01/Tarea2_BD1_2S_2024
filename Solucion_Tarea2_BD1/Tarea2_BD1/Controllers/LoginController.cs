using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Tarea2_BD1.Models;

namespace Tarea2_BD1.Controllers
{
    public class LoginController : Controller
    {
        public readonly Dbtarea2Context _dbContext;

        public LoginController(Dbtarea2Context _context)
        {
            _dbContext = _context;
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public string ConsultaCodError(string codigo, string usuario)
        {
            try
            {
                //Se crea a conexión se abre
                SqlConnection connection = (SqlConnection)_dbContext.Database.GetDbConnection();
                connection.Open();

                //Se crea el SP
                SqlCommand comando = connection.CreateCommand();
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.CommandText = "SP_ConsultaError";

                //Código para crear parámetros al Store Procedure
                SqlParameter paramUsername = new SqlParameter
                {
                    ParameterName = "@inUsername",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 64,
                    Value = usuario,
                    Direction = ParameterDirection.Input
                };

                SqlParameter paramCodigo = new SqlParameter
                {
                    ParameterName = "@inCodigo",
                    SqlDbType = SqlDbType.Int,
                    Value = int.Parse(codigo), //lo cambiamos a int, porque asi esta guardado en la base de datos
                    Direction = ParameterDirection.Input
                };

                SqlParameter paramCod = new SqlParameter
                {
                    ParameterName = "@outResult",
                    SqlDbType = SqlDbType.Int,
                    Value = -345678,
                    Direction = ParameterDirection.InputOutput
                };

                //Se agrega cada parámetro al SP
                comando.Parameters.Add(paramUsername);
                comando.Parameters.Add(paramCodigo);
                comando.Parameters.Add(paramCod);

                //Se leen los datos devueltos por el SP(dataset)
                SqlDataReader reader = comando.ExecuteReader();
                string descripcionError = Convert.ToString(reader["E.Descripcion"])!;
                reader.Close();

                comando.ExecuteNonQuery();

                //Se leen los parámetros de salida
                string SPresult = comando.Parameters["@outResult"].Value.ToString()!;
                Console.WriteLine("\n------------------- SE HA EJECUTADO EL SP_ConsultaError -------------------");
                Console.WriteLine(" El codigo de salida del sp es: " + SPresult);
                Console.WriteLine("-----------------------------------------------------------------------------\n");

                connection.Close();

                return descripcionError;
            }
            catch (Exception ex)
            {
                return String.Format("El error es: {0}", ex.ToString());
            }
        }

        [HttpPost]
        public string InicioDeSesion(string usernameForm, string passwordForm)
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
                comando.CommandText = "SP_SignIn";

                //Código para crear parámetros al Store Procedure
                SqlParameter paramUsername = new SqlParameter
                {
                    ParameterName = "@inUsername",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 64,
                    Value = usernameForm,
                    Direction = ParameterDirection.Input
                };

                SqlParameter paramPassword = new SqlParameter
                {
                    ParameterName = "@inPassword",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 64,
                    Value = passwordForm,
                    Direction = ParameterDirection.Input
                };

                SqlParameter paramPostInIP = new SqlParameter
                {
                    ParameterName = "@inPostInIP",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 32,
                    Value = ippaddress,
                    Direction = ParameterDirection.Input
                };

                SqlParameter paramCod = new SqlParameter
                {
                    ParameterName = "@outResult",
                    SqlDbType = SqlDbType.Int,
                    Value = -345678,
                    Direction = ParameterDirection.InputOutput
                };

                //Se agrega cada parámetro al SP
                comando.Parameters.Add(paramUsername);
                comando.Parameters.Add(paramPassword);
                comando.Parameters.Add(paramPostInIP);
                comando.Parameters.Add(paramCod);

                comando.ExecuteNonQuery();

                //Se leen los parámetros de salida
                string SPresult = comando.Parameters["@outResult"].Value.ToString()!;
                Console.WriteLine("\n------------------- SE HA EJECUTADO EL SP_SignIn -------------------");
                Console.WriteLine(" El codigo de salida del sp es: " + SPresult);
                Console.WriteLine("-----------------------------------------------------------------------------\n");

                connection.Close();

                return SPresult;
            }
            catch (Exception ex)
            {
                return String.Format("El error es: {0}", ex.ToString());
            }
        }

        public ActionResult HacerAviso(string nombreVista, Usuario modeloUsuario, string codigo)
        {
            if (nombreVista == "ListarFiltrar")
            {
                TempData["Message"] = "Inicio de sesión exitoso";
                return RedirectToAction(nombreVista, modeloUsuario);
            }
            else if (nombreVista == "SignIn")
            {
                //Consulta el error y lo guarda comno aviso cuando redireccione a la pagina de inicio de sesion
                TempData["Message"] = ConsultaCodError(codigo, modeloUsuario.Username);
                return RedirectToAction(nombreVista, modeloUsuario);
            }
            //Error generado en el try and catch del metodo que hace el inicio de sesion en la BD
            else if (codigo != "0"  && codigo != "50001" && codigo != "50008")
            {
                return RedirectToAction(nombreVista, modeloUsuario);
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult ValidarDataAnnotations(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                //Resultado del inicio de sesion
                string resultado = InicioDeSesion(usuario.Username, usuario.Password);

                if (resultado == "0")
                {
                    return HacerAviso("ListarFiltrar", usuario, resultado);
                }
                else if (resultado == "50001" || resultado == "50008")
                {
                    return HacerAviso("SignIn", usuario, resultado);
                }
                else
                {
                    return HacerAviso("SignIn", usuario, resultado);
                }
            }
            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
