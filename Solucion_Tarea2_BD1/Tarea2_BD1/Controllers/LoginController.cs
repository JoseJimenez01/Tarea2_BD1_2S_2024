using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;
using Tarea2_BD1.Models;

namespace Tarea2_BD1.Controllers
{
    public class LoginController : Controller
    {
        public readonly Dbtarea2Context _dbContext;

        /// <summary>
        /// Get the context of the DB
        /// </summary>
        /// <param name="_context"></param>
        public LoginController(Dbtarea2Context _context)
        {
            _dbContext = _context;
        }

        /// <summary>
        /// Return the view to login in the platform
        /// </summary>
        [HttpGet("/Login")]
        public IActionResult SignIn()
        {
            //If there is an active login
            if (User.Identity != null &&
                User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Listar", "Empleado");
            }
            return View();
        }

        /// <summary>
        /// Execute the store procedure SP_ConsultaInicioDeSesionFallidos
        /// </summary>
        /// <returns>Times that the user has tried to login</returns>
        [HttpPost]
        public async Task<int> ConsultaInicioSesionFallidos(int tiempo, string usuario)
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
                comando.CommandText = "SP_ConsultaInicioDeSesionFallidos";

                //Código para crear parámetros al Store Procedure
                SqlParameter paramUsername = new SqlParameter
                {
                    ParameterName = "@inUsername",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 64,
                    Value = usuario,
                    Direction = ParameterDirection.Input
                };
                SqlParameter paramTiempo = new SqlParameter
                {
                    ParameterName = "@in20minOr30min",
                    SqlDbType = SqlDbType.Int,
                    Value = tiempo,
                    Direction = ParameterDirection.Input
                };
                SqlParameter paramPostInIP = new SqlParameter
                {
                    ParameterName = "@inPostInIP",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 32,
                    Value = ippaddress?.ToString() ?? "No ip encontrado",
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
                comando.Parameters.Add(paramUsername);
                comando.Parameters.Add(paramTiempo);
                comando.Parameters.Add(paramPostInIP);
                comando.Parameters.Add(paramResultado);

                //Se leen los datos devueltos por el SP(dataset)
                SqlDataReader reader = await comando.ExecuteReaderAsync();
                int cantidad = -88888;
                if (await reader.ReadAsync())
                {
                    //la cantidad de intentos de inicio de sesion fallidos dependiendo de la cantidad de tiempo especificado
                    cantidad = reader.GetInt32(0);
                }
                await reader.CloseAsync();

                await comando.ExecuteNonQueryAsync();

                //Se leen los parámetros de salida
                string SPresult = comando.Parameters["@outResult"].Value.ToString()!;
                Console.WriteLine("\n------------------- SE HA EJECUTADO EL SP_ConsultaInicioDeSesionFallidos -------------------");
                Console.WriteLine(" El codigo de salida del sp es: " + SPresult);
                Console.WriteLine("-----------------------------------------------------------------------------\n");

                await connection.CloseAsync();
                
                return cantidad;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al ejecutar ConsultaInicioSesionFallidos: " + ex.Message);
                //Algun error pero no sabemos cual
                return -1000;
            }
        }

        /// <summary>
        /// Execute the store procedure SP_ConsultaError
        /// </summary>
        /// <param name="codigo">Code store in a catalog table</param>
        /// <returns>The description of a code error</returns>
        [HttpPost]
        public async Task<string> ConsultaCodError(string codigo)
        {
            try
            {
                //Se crea a conexión se abre
                SqlConnection connection = (SqlConnection)_dbContext.Database.GetDbConnection();
                await connection.OpenAsync();

                //Se crea el SP
                SqlCommand comando = connection.CreateCommand();
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                comando.CommandText = "SP_ConsultaError";

                SqlParameter paramCodigo = new SqlParameter
                {
                    ParameterName = "@inCodigo",
                    SqlDbType = SqlDbType.Int,
                    Value = int.Parse(codigo), //lo cambiamos a int, porque asi esta guardado en la base de datos
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
                //comando.Parameters.Add(paramUsername);
                comando.Parameters.Add(paramCodigo);
                comando.Parameters.Add(paramResultado);

                //Se leen los datos devueltos por el SP(dataset)
                SqlDataReader reader = await comando.ExecuteReaderAsync();
                await reader.ReadAsync();
                string descripcionError = reader.GetString(0);
                await reader.CloseAsync();

                await comando.ExecuteNonQueryAsync();

                //Se leen los parámetros de salida
                string SPresult = comando.Parameters["@outResult"].Value.ToString()!;
                Console.WriteLine("\n------------------- SE HA EJECUTADO EL SP_ConsultaError -------------------");
                Console.WriteLine(" El codigo de salida del sp es: " + SPresult);
                Console.WriteLine("-----------------------------------------------------------------------------\n");

                await connection.CloseAsync();

                return descripcionError;
            }
            catch (Exception ex)
            {
                return String.Format("El error es: {0}", ex.ToString());
            }
        }

        /// <summary>
        /// Execute the store procedure SP_SignIn
        /// It validates if the user exists in the DB
        /// </summary>
        /// <param name="usernameForm">Username entered in the web form</param>
        /// <param name="passwordForm">Password entered in the web form</param>
        /// <param name="cantSesionesFallidas">Times that the user has tried to login before</param>
        /// <returns>Result code of SP, 0 = succed</returns>
        [HttpPost]
        public async Task<string> InicioDeSesion(string usernameForm, string passwordForm, int cantSesionesFallidas)
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
                SqlParameter paramSesionesFallidas = new SqlParameter
                {
                    ParameterName = "@inCantIntentosSesionFallidos",
                    SqlDbType = SqlDbType.Int,
                    Value = cantSesionesFallidas,
                    Direction = ParameterDirection.Input
                };
                SqlParameter paramPostInIP = new SqlParameter
                {
                    ParameterName = "@inPostInIP",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 32,
                    Value = ippaddress?.ToString() ?? "No ip encontrado",
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
                comando.Parameters.Add(paramUsername);
                comando.Parameters.Add(paramPassword);
                comando.Parameters.Add(paramSesionesFallidas);
                comando.Parameters.Add(paramPostInIP);
                comando.Parameters.Add(paramResultado);

                await comando.ExecuteNonQueryAsync();

                //Se leen los parámetros de salida
                string SPresult = comando.Parameters["@outResult"].Value.ToString()!;
                Console.WriteLine("\n------------------- SE HA EJECUTADO EL SP_SignIn -------------------");
                Console.WriteLine(" El codigo de salida del sp es: " + SPresult);
                Console.WriteLine("-----------------------------------------------------------------------------\n");

                await connection.CloseAsync();

                return SPresult;
            }
            catch (Exception ex)
            {
                return String.Format("El error es: {0}", ex.ToString());
            }
        }

        /// <summary>
        /// Depending on the SP's result codes or the view, this method redirect and have a diferent message to a user.
        /// </summary>
        /// <param name="nombreVista">Where to redirect</param>
        /// <param name="modeloUsuario">Model of the web form</param>
        /// <param name="codigo">Result of the SP's</param>
        /// <returns>Redirect to a specific view.</returns>
        public async Task<ActionResult> HacerAviso(string nombreVista, Usuario modeloUsuario, string codigo)
        {
            if (nombreVista == "Listar")
            {
                TempData["Message"] = "Successful login";
                TempData["Type"] = "success";
                return RedirectToAction(nombreVista, "Empleado");
            }
            //Error generado en el try and catch del metodo que hace el inicio de sesion en la BD
            //O tambien, que tuvo demasiados intentos fallidos en 30 mins
            else if (codigo != "0" && codigo != "50001" && codigo != "50002" && codigo != "50008")
            {
                TempData["Message"] = codigo;
                TempData["Type"] = "error";
                return RedirectToAction(nombreVista, modeloUsuario);
            }
            else if (nombreVista == "SignIn")
            {
                //Consulta el error y lo guarda comno aviso cuando redireccione a la pagina de inicio de sesion
                TempData["Message"] = await ConsultaCodError(codigo); //In SignIn.resx, i added the key in spanish, because is the value that the DB returns
                TempData["Type"] = "error";
                return RedirectToAction(nombreVista, modeloUsuario);
            }
            return Ok();
        }

        /// <summary>
        /// Validate if the user can login or not in the platform using the SP's result codes,
        /// if succeed create the cookie and add credentials
        /// </summary>
        /// <param name="usuario">Username entered in the web form model</param>
        /// <returns>Function HacerAviso() that have retroalimentation</returns>
        [HttpPost]
        public async Task<IActionResult> ValidarDataAnnotations(Usuario usuario)
        {
            //Valida la cantidad de inicios de sesion fallidos en 30 mins
            //si es mayor que 5 deshabilita el boton
            int cantidadFallos = await ConsultaInicioSesionFallidos(30, usuario.Username);
            if (cantidadFallos > 5)
            {
                return await HacerAviso("SignIn", usuario, "Too many login attempts, please try again in 10 minutes");
            }
            
            if (ModelState.IsValid)
            {
                //Para la descripcion se ocupa lo mismo pero en 20 minutos
                cantidadFallos = await ConsultaInicioSesionFallidos(20, usuario.Username);
                //Resultado del inicio de sesion
                string resultado = await InicioDeSesion(usuario.Username, usuario.Password, cantidadFallos);

                if (resultado == "0")
                {
                    //This section creates the cookie and add credentials
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuario.Username)
                    };

                    var identity = new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal);

                    return await HacerAviso("Listar", usuario, resultado);
                }
                else
                {
                    return await HacerAviso("SignIn", usuario, resultado);
                }
            }
            return Ok();
        }

        /// <summary>
        /// Log out of the cookie
        /// </summary>
        /// <returns>Redirection to Login page</returns>
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("SignIn", "Login");
        }

        /// <summary>
        /// In case of denied access
        /// </summary>
        /// <returns>Redirection to Login page</returns>
        public async Task<ActionResult> Denied()
        {
            TempData["Message"] = "Inicio de sesión fallido";
            TempData["Type"] = "error";
            return RedirectToAction("SignIn", "Login");
        }

        /// <summary>
        /// Dont cached errors when login.
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }//end class
}//end namespace
