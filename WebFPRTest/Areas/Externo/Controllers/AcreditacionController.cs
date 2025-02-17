using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebFPRTest.Areas.Externo.Interface.Acreditacion;
using WebFPRTest.Areas.Externo.Models.Acreditacion;

namespace WebFPRTest.Areas.Externo.Controllers
{
    [Authorize] // Requiere autenticación
    [Area("Externo")]
    public class AcreditacionController:Controller
    {
        private readonly IAcreditacionService _acreditacionService;

        public AcreditacionController(IAcreditacionService acreditacionService)
        {
            _acreditacionService = acreditacionService;
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
            acreditacionFiltroViewModel.ListaJugadores = await _acreditacionService.JugadoresAcreditados_Bandeja(acreditacionFiltroViewModel);
            return View(acreditacionFiltroViewModel);
        }
    }
}
