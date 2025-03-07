using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebFPRTest.Areas.Interno.Models.CartaPase;
using WebFPRTest.Interface;

namespace WebFPRTest.Areas.Interno.Controllers
{
    [Area("Interno")]
    [Authorize]
    public class CartaPaseController:Controller
    {
        private readonly ITiposService _tiposService;

        public CartaPaseController(ITiposService tiposService)
        {
            _tiposService = tiposService;
        }
        [HttpGet]
        public IActionResult GuardarJugadorSeleccionado(int Id_Jugador)
        {
            var tipoUsuario = User.FindFirstValue("Id_011_TipoUsuario");
            if (tipoUsuario == null || (tipoUsuario != "406" && tipoUsuario != "407" && tipoUsuario != "408"))
            {
                return RedirectToAction("Login", "Login");
            }
            TempData["Id_Jugador"] = Id_Jugador;
            return RedirectToAction("CartaPase", "CartaPase", new { area = "Interno" });
        }
        [HttpGet]
        public IActionResult CartaPase()
        {
            var tipoUsuario = User.FindFirstValue("Id_011_TipoUsuario");
            if (tipoUsuario == null || (tipoUsuario != "406" && tipoUsuario != "407" && tipoUsuario != "408"))
            {
                return RedirectToAction("Login", "Login");
            }
            int Id_Jugador = TempData.Peek("Id_Jugador") as int? ?? 0;
            CartaPaseViewModel cartaPaseViewModel = new CartaPaseViewModel();
            return View(cartaPaseViewModel);
        }
    }
}
