using Microsoft.AspNetCore.Mvc;

namespace Tarea2_BD1.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult SignIn()
        {
            return View();
        }
    }
}
