using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            var tipoUsuario = User.FindFirstValue("Id_011_TipoUsuario");
            if (tipoUsuario == null || (tipoUsuario != "406" && tipoUsuario != "407" && tipoUsuario != "408"))
            {
                return RedirectToAction("Login", "Login");
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
            return RedirectToAction("AcreditacionJugadores");
        }
        [HttpGet]
        public IActionResult AcreditacionJugadores()
        {
            return View();
        }
    }
}
