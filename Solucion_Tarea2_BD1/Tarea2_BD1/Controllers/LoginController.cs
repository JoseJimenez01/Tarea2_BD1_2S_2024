using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
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

        public ActionResult HacerAviso(string nombreVista, string aviso, Empleado modelo)
        {
            TempData["Message"] = aviso;
            if (nombreVista == "Listar")
            {
                return RedirectToAction(nombreVista);
            }
            return RedirectToAction(nombreVista, modelo);
        }

        [HttpPost]
        public IActionResult ValidarDataAnnotations(Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                //Código para capturar la ip del usuario
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                var ippaddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
                //TempData["Message"] = String.Format("La ip es: {0}", ippaddress);
                RedirectToAction("SignIn");
                //string json = IngresarEmpleadoEnBD(empleado.Nombre, empleado.Salario);
                //dynamic mensaje = JsonConvert.DeserializeObject(json);
                //if (mensaje["mensaje"].ToString() == "El empleado ya existe en la base de datos")
                //{
                //    return HacerAviso("Agregar", "Nombre de empleado ya existe", empleado);
                //}
                //else if (mensaje["mensaje"].ToString() == "Empleado agregado exitosamente")
                //{
                //    return HacerAviso("Listar", "Inserción exitosa", empleado);
                //}
            }
            //return HacerAviso("Listar", String.Format("La ip es: {0}", ippaddress), empleado);
             return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
