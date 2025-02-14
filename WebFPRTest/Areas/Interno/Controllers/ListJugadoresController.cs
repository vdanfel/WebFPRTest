using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebFPRTest.Areas.Externo.Models.Jugador;
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
        [HttpGet]
        public async Task<IActionResult> JugadorIndividual()
        {
            var tipoUsuario = User.FindFirstValue("Id_011_TipoUsuario");
            if (tipoUsuario == null || (tipoUsuario != "406" && tipoUsuario != "407" && tipoUsuario != "408"))
            {
                return RedirectToAction("Login", "Login");
            }
            JugadorIndividualViewModel jugadorIndividualViewModel = new JugadorIndividualViewModel();
            jugadorIndividualViewModel.Paises = await _tiposService.ParametroTipo_Listar(3);
            jugadorIndividualViewModel.Nacionalidades = await _tiposService.ParametroTipo_Listar(4);
            jugadorIndividualViewModel.Sexos = await _tiposService.ParametroTipo_Listar(2);
            jugadorIndividualViewModel.TipoSeguros = await _tiposService.ParametroTipo_Listar(5);
            jugadorIndividualViewModel.TipoVehiculos = await _tiposService.ParametroTipo_Listar(6);
            jugadorIndividualViewModel.DivisionList = await _tiposService.ParametroTipo_Listar(7);
            jugadorIndividualViewModel.SituacionList = await _tiposService.ParametroTipo_Listar(8);
            jugadorIndividualViewModel.TipoSangre = await _tiposService.ParametroTipo_Listar(14);
            jugadorIndividualViewModel.TipoDocumentos = await _tiposService.ParametroTipo_Listar(1);
            return View(jugadorIndividualViewModel);
        }
        public IActionResult GuardarJugadorSeleccionado(int Id_Jugador)
        {
            var tipoUsuario = User.FindFirstValue("Id_011_TipoUsuario");
            if (tipoUsuario == null || (tipoUsuario != "406" && tipoUsuario != "407" && tipoUsuario != "408"))
            {
                return RedirectToAction("Login", "Login");
            }
            TempData["Id_Jugador"] = Id_Jugador;
            return RedirectToAction("JugadorIndividual", "ListJugadores", new { area = "Interno" });
        }
    }
}
