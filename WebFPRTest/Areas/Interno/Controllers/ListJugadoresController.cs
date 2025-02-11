using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebFPRTest.Areas.Interno.Interface.ListJugadores;
using WebFPRTest.Areas.Interno.Models.ListJugadores;
using WebFPRTest.Interface;

namespace WebFPRTest.Areas.Interno.Controllers
{
    [Area("Interno")]
    [Authorize]
    public class ListJugadoresController:Controller
    {
        private readonly ITiposService _tiposService;
        private readonly IListJugadoresService _listJugadoresService;
        public ListJugadoresController(ITiposService tiposService, IListJugadoresService listJugadoresService) 
        {
            _tiposService = tiposService;
            _listJugadoresService = listJugadoresService;
        }
        [HttpGet]
        public async Task<IActionResult> ListJugadores() 
        {
            var tipoUsuario = User.FindFirstValue("Id_011_TipoUsuario");
            if (tipoUsuario == null || (tipoUsuario != "406" && tipoUsuario != "407" && tipoUsuario != "408"))
            {
                return RedirectToAction("Login", "Login");
            }
            JugadoresFiltroViewModel jugadoresFiltroViewModel = new JugadoresFiltroViewModel();
            jugadoresFiltroViewModel.ListaDivisiones = await _tiposService.ParametroTipo_Listar(7);
            jugadoresFiltroViewModel.ListaJugadores = await _listJugadoresService.Jugador_Bandeja(jugadoresFiltroViewModel);
            jugadoresFiltroViewModel.EquiposList = await _tiposService.Equipo_Listar();
            return View(jugadoresFiltroViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> ListJugadores(JugadoresFiltroViewModel jugadoresFiltroViewModel)
        {
            var idUsuarioStr = User.FindFirst("Id_Usuario")?.Value ?? "0";
            var Id_Usuario = int.Parse(idUsuarioStr);
            if (Id_Usuario == 0)
            {
                return RedirectToAction("Login", "Account");
            }
            jugadoresFiltroViewModel.ListaDivisiones = await _tiposService.ParametroTipo_Listar(7);
            jugadoresFiltroViewModel.ListaJugadores = await _listJugadoresService.Jugador_Bandeja(jugadoresFiltroViewModel);
            jugadoresFiltroViewModel.EquiposList = await _tiposService.Equipo_Listar();
            return View(jugadoresFiltroViewModel);
        }
    }
}
