using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebFPRTest.Areas.Externo.Controllers
{
    [Authorize] // Requiere autenticación
    [Area("Externo")]
    public class AcreditacionController:Controller
    {
        [HttpGet]
        public IActionResult Acreditacion()
        {
            return View();
        }
    }
}
