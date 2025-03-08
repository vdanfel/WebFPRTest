using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebFPRTest.Areas.Interno.Interface.CartaPase;
using WebFPRTest.Areas.Interno.Models.CartaPase;
using WebFPRTest.Interface;

namespace WebFPRTest.Areas.Interno.Controllers
{
    [Area("Interno")]
    [Authorize]
    public class CartaPaseController:Controller
    {
        private readonly ITiposService _tiposService;
        private readonly ICartaPaseService _cartaPaseService;
        public CartaPaseController(ITiposService tiposService, ICartaPaseService cartaPaseService)
        {
            _tiposService = tiposService;
            _cartaPaseService = cartaPaseService;
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
        public async Task<IActionResult> CartaPase()
        {
            var tipoUsuario = User.FindFirstValue("Id_011_TipoUsuario");
            if (tipoUsuario == null || (tipoUsuario != "406" && tipoUsuario != "407" && tipoUsuario != "408"))
            {
                return RedirectToAction("Login", "Login");
            }
            int Id_Jugador = TempData.Peek("Id_Jugador") as int? ?? 0;
            CartaPaseViewModel cartaPaseViewModel = new CartaPaseViewModel();
            cartaPaseViewModel.TipoDocumentos = await _tiposService.ParametroTipo_Listar(1);
            cartaPaseViewModel.EquipoList = await _tiposService.Equipo_Listar();
            var jugador = await _cartaPaseService.Jugador_Datos(Id_Jugador);
            cartaPaseViewModel.Paterno = jugador.Paterno;
            cartaPaseViewModel.Materno = jugador.Materno;
            cartaPaseViewModel.Nombres = jugador.Nombres;
            cartaPaseViewModel.Id_001_TipoDocumento = jugador.Id_001_TipoDocumento;
            cartaPaseViewModel.NombreEquipo = jugador.NombreEquipo;
            cartaPaseViewModel.Documento = jugador.Documento;
            cartaPaseViewModel.Correo = jugador.Correo;
                
            return View(cartaPaseViewModel);
        }

    }
}
