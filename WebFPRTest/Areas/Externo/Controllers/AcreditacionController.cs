using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebFPRTest.Areas.Externo.Interface.Acreditacion;
using WebFPRTest.Areas.Externo.Models.Acreditacion;
using WebFPRTest.Interface;

namespace WebFPRTest.Areas.Externo.Controllers
{
    [Authorize] // Requiere autenticación
    [Area("Externo")]
    public class AcreditacionController:Controller
    {
        private readonly IAcreditacionService _acreditacionService;
        private readonly ITiposService _tiposService;

        public AcreditacionController(IAcreditacionService acreditacionService, ITiposService tiposService)
        {
            _acreditacionService = acreditacionService;
            _tiposService = tiposService;
        }

        [HttpGet]
        public async Task<IActionResult> Acreditacion()
        {
            var tipoUsuario = User.FindFirstValue("Id_011_TipoUsuario");
            if (tipoUsuario == null || tipoUsuario != "409")
            {
                return RedirectToAction("Login", "Login");
            }
            var idEquipoStr = User.FindFirst("Id_Equipo")?.Value ?? "0";
            var Id_Equipo = int.Parse(idEquipoStr);

            AcreditacionFiltroViewModel acreditacionFiltroViewModel = new AcreditacionFiltroViewModel();
            acreditacionFiltroViewModel.Id_Equipo = Id_Equipo;
            acreditacionFiltroViewModel.TipoPagos = await _tiposService.ParametroTipo_Listar(15);
            acreditacionFiltroViewModel.ListaJugadores = await _acreditacionService.JugadoresAcreditados_Bandeja(acreditacionFiltroViewModel);
            acreditacionFiltroViewModel.SaldoAFavor = await _acreditacionService.EquipoSaldo(Id_Equipo);
            return View(acreditacionFiltroViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Acreditacion(AcreditacionFiltroViewModel acreditacionFiltroViewModel)
        {
            // Obtener Id_Usuario del Claim
            var idUsuarioStr = User.FindFirst("Id_Usuario")?.Value ?? "0";
            var Id_Usuario = int.Parse(idUsuarioStr);
            if (Id_Usuario == 0)
            {
                return RedirectToAction("Login", "Account");
            }
            var idEquipoStr = User.FindFirst("Id_Equipo")?.Value ?? "0";
            var Id_Equipo = int.Parse(idEquipoStr);
            if (acreditacionFiltroViewModel.jugadoresSeleccionados == null || !acreditacionFiltroViewModel.jugadoresSeleccionados.Any())
            {
                TempData["Mensaje"] = "Debe seleccionar al menos un jugador.";
                return RedirectToAction("Acreditacion");
            }

            try
            {
                foreach (var idJugador in acreditacionFiltroViewModel.jugadoresSeleccionados)
                {
                    // Llamamos al servicio que ejecuta el SP en la base de datos
                    await _acreditacionService.Jugador_SolicitudAcreditacion(idJugador);
                }

                TempData["Mensaje"] = "Los jugadores fueron acreditados correctamente.";
                var saldo = acreditacionFiltroViewModel.ImporteTotal - acreditacionFiltroViewModel.TotalPagoAcreditacion;
                await _acreditacionService.Equipo_ActualizarSaldo(Id_Equipo,saldo);
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Ocurrió un error al procesar la acreditación.";
            }

            return RedirectToAction("Acreditacion");
        }
    }
}
