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
            JugadoresFiltroViewModel jugadoresFiltro = new JugadoresFiltroViewModel();
            jugadoresFiltro.ListaDivisiones = await _tiposService.ParametroTipo_Listar(7);
            jugadoresFiltro.ListaJugadores = await _listJugadoresService.Jugador_Bandeja(jugadoresFiltro);
            jugadoresFiltro.EquiposList = await _tiposService.Equipo_Listar();
            return View(jugadoresFiltro);
        }
    }
}
