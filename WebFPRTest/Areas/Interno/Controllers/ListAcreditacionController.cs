using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using WebFPRTest.Areas.Interno.Interface.ListAcreditacion;
using WebFPRTest.Areas.Interno.Models.ListAcreditacion;
using WebFPRTest.Interface;

namespace WebFPRTest.Areas.Interno.Controllers
{
    [Area("Interno")]
    [Authorize]
    public class ListAcreditacionController:Controller
    {
        private readonly IListAcreditacionService _listAcreditacionService;
        private readonly ITiposService _tiposService;

        public ListAcreditacionController(IListAcreditacionService listAcreditacionService, ITiposService tiposService)
        {
            _listAcreditacionService = listAcreditacionService;
            _tiposService = tiposService;
        }
        [HttpGet]
        public async Task<IActionResult> ListAcreditacion()
        {
            var tipoUsuario = User.FindFirstValue("Id_011_TipoUsuario");
            if (tipoUsuario == null || (tipoUsuario != "406" && tipoUsuario != "407" && tipoUsuario != "408"))
            {
                return RedirectToAction("Login", "Login");
            }
            ComprobanteFiltroViewModel comprobanteFiltroViewModel = new ComprobanteFiltroViewModel();
            comprobanteFiltroViewModel.ListaComprobantes = await _listAcreditacionService.Comprobante_Bandeja(comprobanteFiltroViewModel);
            comprobanteFiltroViewModel.ListarEquipos = await _tiposService.Equipo_Listar();
            comprobanteFiltroViewModel.TipoDocumentos = await _tiposService.ParametroTipo_Listar(1);
            comprobanteFiltroViewModel.TipoPagos = await _tiposService.ParametroTipo_Listar(15);
            return View(comprobanteFiltroViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> ListAcreditacion(ComprobanteFiltroViewModel comprobanteFiltroViewModel)
        {
            var idUsuarioStr = User.FindFirst("Id_Usuario")?.Value ?? "0";
            var Id_Usuario = int.Parse(idUsuarioStr);
            if (Id_Usuario == 0)
            {
                return RedirectToAction("Login", "Account");
            }
            comprobanteFiltroViewModel.ListaComprobantes = await _listAcreditacionService.Comprobante_Bandeja(comprobanteFiltroViewModel);
            comprobanteFiltroViewModel.ListarEquipos = await _tiposService.Equipo_Listar();
            comprobanteFiltroViewModel.TipoDocumentos = await _tiposService.ParametroTipo_Listar(1);
            comprobanteFiltroViewModel.TipoPagos = await _tiposService.ParametroTipo_Listar(15);
            return View(comprobanteFiltroViewModel);
        }
        [HttpGet]
        public IActionResult GuardarComprobante(int Id_Comprobante)
        {
            var tipoUsuario = User.FindFirstValue("Id_011_TipoUsuario");
            if (tipoUsuario == null || (tipoUsuario != "406" && tipoUsuario != "407" && tipoUsuario != "408"))
            {
                return RedirectToAction("Login", "Login");
            }
            TempData["Id_Comprobante"] = Id_Comprobante;
            return RedirectToAction("AcreditacionJugadores", "ListAcreditacion", new { area="Interno"});
        }
        [HttpGet]
        public async Task<IActionResult> AcreditacionJugadores()
        {
            int Id_Comprobante = (int)TempData.Peek("Id_Comprobante");
            var acreditacionJugadoresViewModel = await _listAcreditacionService.Comprobante_Select(Id_Comprobante);
            acreditacionJugadoresViewModel.TipoPagos = await _tiposService.ParametroTipo_Listar(15);
            acreditacionJugadoresViewModel.ListaJugadores = await _listAcreditacionService.JugadorComprobante_Jugadores(Id_Comprobante);
            acreditacionJugadoresViewModel.RutaComprobante = await _listAcreditacionService.Archivo_RutaLogo(acreditacionJugadoresViewModel.Id_Equipo, Id_Comprobante, 449);
            return View(acreditacionJugadoresViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> AcreditacionJugadores(AcreditacionJugadoresViewModel acreditacionJugadoresViewModel, string jugadoresSeleccionados)
        {
            var idUsuarioStr = User.FindFirst("Id_Usuario")?.Value ?? "0";
            var Id_Usuario = int.Parse(idUsuarioStr);

            if (Id_Usuario == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            // Convertir la cadena separada por comas en una lista de enteros
            if (!string.IsNullOrEmpty(jugadoresSeleccionados))
            {
                acreditacionJugadoresViewModel.JugadoresSeleccionados = jugadoresSeleccionados
                    .Split(',')
                    .Select(int.Parse)
                    .ToList();

                foreach (var Id_Jugador in acreditacionJugadoresViewModel.JugadoresSeleccionados)
                {
                    await _listAcreditacionService.Jugador_ActualizarEstado444(Id_Jugador, Id_Usuario);
                }
            }

            return RedirectToAction("ListAcreditacion", "ListAcreditacion", new { area = "Interno" });
        }


    }
}
