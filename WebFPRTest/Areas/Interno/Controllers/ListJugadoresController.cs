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
            var estadoJugadores = await _tiposService.ParametroTipo_Listar(9);
            jugadoresFiltroViewModel.ListaEstadoJugador = estadoJugadores.OrderBy(d => d.Id_Parametros).ToList();
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
            var estadoJugadores = await _tiposService.ParametroTipo_Listar(9);
            jugadoresFiltroViewModel.ListaEstadoJugador = estadoJugadores.OrderBy(d => d.Id_Parametros).ToList();
            return View(jugadoresFiltroViewModel);
        }
        public IActionResult GuardarJugadorSeleccionado(int Id_Jugador, int Id_Equipo)
        {
            var tipoUsuario = User.FindFirstValue("Id_011_TipoUsuario");
            if (tipoUsuario == null || (tipoUsuario != "406" && tipoUsuario != "407" && tipoUsuario != "408"))
            {
                return RedirectToAction("Login", "Login");
            }
            TempData["Id_Jugador"] = Id_Jugador;
            TempData["Id_Equipo"] = Id_Equipo;
            return RedirectToAction("JugadorDatos", "ListJugadores", new { area = "Interno" });
        }
        [HttpGet]
        public async Task<IActionResult> JugadorDatos()
        {
            var tipoUsuario = User.FindFirstValue("Id_011_TipoUsuario");
            if (tipoUsuario == null || (tipoUsuario != "406" && tipoUsuario != "407" && tipoUsuario != "408"))
            {
                return RedirectToAction("Login", "Login");
            }
            int Id_Jugador = TempData.Peek("Id_Jugador") as int? ?? 0;
            TempData.Keep("Id_Jugador");
            int Id_Equipo = TempData.Peek("Id_Equipo") as int? ?? 0;
            TempData.Keep("Id_Equipo");
            JugadorDatosViewModel jugadorDatosViewModel = new JugadorDatosViewModel();
            jugadorDatosViewModel = await _listJugadoresService.Jugador_Select(Id_Jugador);
            jugadorDatosViewModel.DatosApoderado = await _listJugadoresService.Apoderado_Select(jugadorDatosViewModel.Id_Persona);
            jugadorDatosViewModel.TipoDocumentos = await _tiposService.ParametroTipo_Listar(1);
            jugadorDatosViewModel.Paises = await _tiposService.ParametroTipo_Listar(3);
            jugadorDatosViewModel.Nacionalidades = await _tiposService.ParametroTipo_Listar(4);
            jugadorDatosViewModel.Sexos = await _tiposService.ParametroTipo_Listar(2);
            jugadorDatosViewModel.TipoSeguros = await _tiposService.ParametroTipo_Listar(5);
            jugadorDatosViewModel.TipoVehiculos = await _tiposService.ParametroTipo_Listar(6);
            jugadorDatosViewModel.DivisionList = await _tiposService.ParametroTipo_Listar(7);
            jugadorDatosViewModel.SituacionList = await _tiposService.ParametroTipo_Listar(8);
            jugadorDatosViewModel.TipoSangre = await _tiposService.ParametroTipo_Listar(14);
            jugadorDatosViewModel.RutaFoto = await _listJugadoresService.Archivo_Ruta(jugadorDatosViewModel.Id_Equipo, jugadorDatosViewModel.Id_Jugador, 431);
            jugadorDatosViewModel.RutaDeslinde = await _listJugadoresService.Archivo_Ruta(jugadorDatosViewModel.Id_Equipo, jugadorDatosViewModel.Id_Jugador, 432);
            ViewData["ActiveTab"] = "DatosGenerales";
            jugadorDatosViewModel.Flag_Aprobacion1 = jugadorDatosViewModel.Id_009_EstadoJugador >= 442;
            return View(jugadorDatosViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> JugadorDatos(JugadorDatosViewModel jugadorDatosViewModel)
        {
            var idUsuarioStr = User.FindFirst("Id_Usuario")?.Value ?? "0";
            var Id_Usuario = int.Parse(idUsuarioStr);
            if (Id_Usuario == 0)
            {
                return RedirectToAction("Login", "Account");
            }
            if (jugadorDatosViewModel.Id_009_EstadoJugador == 441 && jugadorDatosViewModel.Flag_Aprobacion1 == true)
            {
                jugadorDatosViewModel.Id_009_EstadoJugador = 442;
                await _listJugadoresService.Jugador_Actualizar(jugadorDatosViewModel, Id_Usuario);
                TempData["Mensaje"] = "Jugador habilitado con éxito";
            }
            if (jugadorDatosViewModel.Id_009_EstadoJugador == 442 && jugadorDatosViewModel.Flag_Aprobacion1 == false)
            {
                jugadorDatosViewModel.Id_009_EstadoJugador = 441;
                await _listJugadoresService.Jugador_Actualizar(jugadorDatosViewModel, Id_Usuario);
                TempData["Mensaje"] = "Jugador deshabilitado";
            }
            if (jugadorDatosViewModel.Id_009_EstadoJugador == 441 && jugadorDatosViewModel.Flag_Aprobacion1 == false)
            {
                await _listJugadoresService.Jugador_Actualizar(jugadorDatosViewModel, Id_Usuario);
                TempData["Mensaje"] = "Observacion registrada con éxito";
            }
            return RedirectToAction("GuardarJugadorSeleccionado", "ListJugadores", new {Id_Jugador = jugadorDatosViewModel.Id_Jugador });
        }
        [HttpGet]
        public async Task<IActionResult> JugadorDocumentos() 
        {
            int Id_Jugador = TempData.Peek("Id_Jugador") as int? ?? 0;
            TempData.Keep("Id_Jugador");
            int Id_Equipo = TempData.Peek("Id_Equipo") as int? ?? 0;
            TempData.Keep("Id_Equipo");
            JugadorDocumentosViewModel jugadorDocumentosViewModel = new JugadorDocumentosViewModel();
            var archivos = await _listJugadoresService.ArchivosInscripcion(Id_Equipo, Id_Jugador);
            if (archivos != null)
            {
                jugadorDocumentosViewModel.RutaActaMedica = archivos.RutaActaMedica;
                jugadorDocumentosViewModel.FechaRegistroActaMedica = archivos.FechaRegistroActaMedica;
                jugadorDocumentosViewModel.FechaVencimientoActaMedica = archivos.FechaVencimientoActaMedica;
                jugadorDocumentosViewModel.RutaRugbyReady = archivos.RutaRugbyReady;
                jugadorDocumentosViewModel.FechaRegistroRugbyReady = archivos.FechaRegistroRugbyReady;
                jugadorDocumentosViewModel.FechaVencimientoRugbyReady = archivos.FechaVencimientoRugbyReady;
                jugadorDocumentosViewModel.RutaRugbyLaws = archivos.RutaRugbyLaws;
                jugadorDocumentosViewModel.FechaRegistroRugbyLaws = archivos.FechaRegistroRugbyLaws;
                jugadorDocumentosViewModel.FechaVencimientoRugbyLaws = archivos.FechaVencimientoRugbyLaws;
                jugadorDocumentosViewModel.RutaKeepRugbyClean = archivos.RutaKeepRugbyClean;
                jugadorDocumentosViewModel.FechaRegistroKeepRugbyClean = archivos.FechaRegistroKeepRugbyClean;
                jugadorDocumentosViewModel.FechaVencimientoKeepRugbyClean = archivos.FechaVencimientoKeepRugbyClean;
                jugadorDocumentosViewModel.RutaPrimerosAuxilios = archivos.RutaPrimerosAuxilios;
                jugadorDocumentosViewModel.FechaRegistroPrimerosAuxilios = archivos.FechaRegistroPrimerosAuxilios;
                jugadorDocumentosViewModel.FechaVencimientoPrimerosAuxilios = archivos.FechaVencimientoPrimerosAuxilios;
                jugadorDocumentosViewModel.RutaConmocionCerebral = archivos.RutaConmocionCerebral;
                jugadorDocumentosViewModel.FechaRegistroConmocionCerebral = archivos.FechaRegistroConmocionCerebral;
                jugadorDocumentosViewModel.FechaVencimientoConmocionCerebral = archivos.FechaVencimientoConmocionCerebral;
            }
            ViewData["ActiveTab"] = "DocumentosInscripcion";
            return View(jugadorDocumentosViewModel);
        }
    }
}
