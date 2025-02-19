using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebFPRTest.Areas.Externo.Interface.Acreditacion;
using WebFPRTest.Areas.Externo.Models.Acreditacion;
using WebFPRTest.Areas.Interno.Models.ListJugadores;
using WebFPRTest.Interface;

namespace WebFPRTest.Areas.Externo.Controllers
{
    [Authorize] // Requiere autenticación
    [Area("Externo")]
    public class AcreditacionController:Controller
    {
        private readonly IAcreditacionService _acreditacionService;
        private readonly ITiposService _tiposService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AcreditacionController(IAcreditacionService acreditacionService, ITiposService tiposService, IWebHostEnvironment webHostEnvironment)
        {
            _acreditacionService = acreditacionService;
            _tiposService = tiposService;
            _webHostEnvironment = webHostEnvironment;
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
            acreditacionFiltroViewModel.Id_Equipo = Id_Equipo;
            var comprobanteResult = await _acreditacionService.Comprobante_Insert(acreditacionFiltroViewModel, Id_Usuario);
            if (acreditacionFiltroViewModel.Comprobante != null)
            {
                string nuevaRutaComprobante = await GuardarArchivo(
                    acreditacionFiltroViewModel.Comprobante,
                    Id_Equipo,
                    comprobanteResult.Correlativo,
                    449,
                    ".jpg, .jpeg, .png"
                );

                if (!string.IsNullOrEmpty(nuevaRutaComprobante))
                {
                    await _acreditacionService.Archivo_Insertar(Id_Equipo, 0, 449, nuevaRutaComprobante, Id_Usuario);
                }
            }

            if (!string.IsNullOrEmpty(acreditacionFiltroViewModel.jugadoresSeleccionados))
            {
                var jugadores = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AcreditacionTabla>>(acreditacionFiltroViewModel.jugadoresSeleccionados);

                foreach (var jugador in jugadores)
                {
                    // Registrar la solicitud de acreditación
                    await _acreditacionService.Jugador_SolicitudAcreditacion(jugador.Id_Jugador, Id_Usuario);

                    // Asociar el jugador con el comprobante
                    await _acreditacionService.JugadorComprobante_Insert(comprobanteResult.Id_Comprobante, jugador.Id_Jugador, jugador.CostoAcreditacion);
                }
                TempData["Mensaje"] = "Los jugadores fueron acreditados correctamente.";
                var saldo = acreditacionFiltroViewModel.ImporteTotal - acreditacionFiltroViewModel.TotalPagoAcreditacion;
                await _acreditacionService.Equipo_ActualizarSaldo(Id_Equipo, saldo, Id_Usuario);
            }
            

            //if (comprobanteResult.Id_Comprobante > 0)
            //{
            //    if (acreditacionFiltroViewModel.jugadoresSeleccionados == null || !acreditacionFiltroViewModel.jugadoresSeleccionados.Any())
            //    {
            //        TempData["Mensaje"] = "Debe seleccionar al menos un jugador.";
            //        return RedirectToAction("Acreditacion");
            //    }
            //    try
            //    {
            //        foreach (var jugador in acreditacionFiltroViewModel.jugadoresSeleccionados)
            //        {
            //            //await _acreditacionService.Jugador_SolicitudAcreditacion(jugador.Id_Jugador, Id_Usuario);
            //            //await _acreditacionService.JugadorComprobante_Insert(comprobanteResult.Id_Comprobante, jugador.Id_Jugador, jugador.ValorAcreditacion);
            //        }

            //        TempData["Mensaje"] = "Los jugadores fueron acreditados correctamente.";
            //        var saldo = acreditacionFiltroViewModel.ImporteTotal - acreditacionFiltroViewModel.TotalPagoAcreditacion;
            //        await _acreditacionService.Equipo_ActualizarSaldo(Id_Equipo, saldo);
            //    }
            //    catch (Exception ex)
            //    {
            //        TempData["Mensaje"] = "Ocurrió un error al procesar la acreditación.";
            //    }
            //}

            return RedirectToAction("Acreditacion");
        }
        private async Task<string> GuardarArchivo(IFormFile archivo, int Id_Equipo, int Correlativo, int Id_013_TipoArchivo, string extensionesPermitidas)
        {
            if (archivo == null || archivo.Length == 0)
            {
                return null;
            }

            var NombreTipoArchivo = await _tiposService.TipoArchivo_Descripcion(Id_013_TipoArchivo);
            string[] allowedExtensions = extensionesPermitidas.Split(',').Select(e => e.Trim().ToLower()).ToArray();
            string fileExtension = Path.GetExtension(archivo.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new InvalidOperationException($"Extensión {fileExtension} no permitida para el tipo de archivo {NombreTipoArchivo}.");
            }

            // Limpiar archivos existentes antes de guardar el nuevo
            await LimpiarArchivosExistentes(Id_Equipo, Correlativo, Id_013_TipoArchivo, allowedExtensions);

            // Nueva ruta de almacenamiento
            string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Archivos", Id_Equipo.ToString(), "Comprobantes");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Nuevo nombre del archivo basado en el Id_Equipo y Correlativo
            string fileName = $"{Id_Equipo}_Comprobante{Correlativo}{fileExtension}";
            string filePath = Path.Combine(folderPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await archivo.CopyToAsync(fileStream);
            }

            return Path.Combine("Archivos", Id_Equipo.ToString(), "Comprobantes", fileName);
        }
        private async Task LimpiarArchivosExistentes(int Id_Equipo, int Correlativo, int Id_013_TipoArchivo, string[] extensionesPermitidas)
        {
            var NombreTipoArchivo = await _tiposService.TipoArchivo_Descripcion(Id_013_TipoArchivo);
            string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Archivos", Id_Equipo.ToString(), "Comprobantes");

            if (Directory.Exists(folderPath))
            {
                string baseFileName = $"{Id_Equipo}_Comprobante{Correlativo}";
                var archivosExistentes = Directory.GetFiles(folderPath)
                    .Where(f => Path.GetFileNameWithoutExtension(f) == baseFileName &&
                                extensionesPermitidas.Contains(Path.GetExtension(f).ToLower()));

                foreach (var archivo in archivosExistentes)
                {
                    try
                    {
                        System.IO.File.Delete(archivo);
                    }
                    catch (Exception ex)
                    {
                        // Log del error si es necesario
                    }
                }
            }
        }

    }
}
