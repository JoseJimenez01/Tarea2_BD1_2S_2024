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
        public string ConsultaCodError(string codigo)
        {
            
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

                //Se leen los parámetros de salida
                comando.ExecuteNonQuery();
                string SPresult = comando.Parameters["@outResult"].Value.ToString()!;
                Console.WriteLine("\n------------------- SE HA EJECUTADO EL SP_SignIn -------------------");
                Console.WriteLine(" El codigo de salida del sp es: " + SPresult);
                Console.WriteLine("-----------------------------------------------------------------------------\n");

                connection.Close();

                if (SPresult == "0")
                {
                    return String.Format("exito");
                }
            }
            catch (Exception ex)
            {
                return String.Format("El error es: {0}", ex.ToString());
            }
            return "Hubo algun error inesperado";
        }

        public ActionResult HacerAviso(string nombreVista, string aviso, Usuario modelo)
        {
            TempData["Message"] = aviso;
            if (nombreVista == "ListarFiltrar")
            {
                return RedirectToAction(nombreVista);
            }
            return RedirectToAction(nombreVista, modelo);
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
                    return HacerAviso("ListarFiltrar", "Inicio de sesión exitoso", usuario);
                }
                else if (resultado == "50001")
                {
                    //return HacerAviso("SignIn", resultado, usuario);
                }
                else if (resultado == "50008")
                { 
                }
                else
                {
                    return HacerAviso("SignIn", resultado, usuario);
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
