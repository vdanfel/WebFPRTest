using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebFPRTest.Areas.Interno.Controllers
{
    [Area("Interno")]
    [Authorize]
    public class JugadoresController : Controller
    {
        [HttpGet]
        public IActionResult Jugadores()
        {
            var tipoUsuario = User.FindFirstValue("Id_011_TipoUsuario");
            if (tipoUsuario == null || tipoUsuario != "407")
            {
                return RedirectToAction("Login", "Login");
            }
            return View();
        }
    }
}
