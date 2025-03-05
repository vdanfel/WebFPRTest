using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebFPRTest.Areas.Externo.Interface.Inscripcion;
using WebFPRTest.Areas.Externo.Models.Inscripcion;
using WebFPRTest.Areas.Externo.Models.Jugador;
using WebFPRTest.Interface;

namespace WebFPRTest.Areas.Externo.Controllers
{
    [Authorize] // Requiere autenticación
    [Area("Externo")]
    public class InscripcionController:Controller
    {
        private readonly ITiposService _tiposService;
        private readonly IInscripcionService _inscripcionService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public InscripcionController(ITiposService tiposService, IInscripcionService inscripcionService, IWebHostEnvironment webHostEnvironment)
        { 
            _tiposService = tiposService;
            _inscripcionService = inscripcionService;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public async Task<IActionResult> Inscripcion()
        {
            var tipoUsuario = User.FindFirstValue("Id_011_TipoUsuario");
            if (tipoUsuario == null || tipoUsuario != "409")
            {
                return RedirectToAction("Login", "Login");
            }
            var idEquipoStr = User.FindFirst("Id_Equipo")?.Value ?? "0";
            var Id_Equipo = int.Parse(idEquipoStr);
            if (Id_Equipo == 0)
            {
                TempData["Mensaje"] = "Debe registrar a su equipo primero";
                return RedirectToAction("Equipo", "Equipo");
            }
            int Id_Jugador = TempData.Peek("Id_Jugador") as int? ?? 0;
            var inscripcionViewModel = await _inscripcionService.Jugador_Select(Id_Jugador);
            inscripcionViewModel.TipoDocumentos = await _tiposService.ParametroTipo_Listar(1);
            var archivos = await _inscripcionService.ArchivosInscripcion(Id_Equipo, Id_Jugador);
            if (archivos != null)
            {
                inscripcionViewModel.RutaActaMedica = archivos.RutaActaMedica;
                inscripcionViewModel.FechaRegistroActaMedica = archivos.FechaRegistroActaMedica;
                inscripcionViewModel.FechaVencimientoActaMedica = archivos.FechaVencimientoActaMedica;
                inscripcionViewModel.RutaRugbyReady = archivos.RutaRugbyReady;
                inscripcionViewModel.FechaRegistroRugbyReady = archivos.FechaRegistroRugbyReady;
                inscripcionViewModel.FechaVencimientoRugbyReady = archivos.FechaVencimientoRugbyReady;
                inscripcionViewModel.RutaRugbyLaws = archivos.RutaRugbyLaws;
                inscripcionViewModel.FechaRegistroRugbyLaws = archivos.FechaRegistroRugbyLaws;
                inscripcionViewModel.FechaVencimientoRugbyLaws = archivos.FechaVencimientoRugbyLaws;
                inscripcionViewModel.RutaKeepRugbyClean = archivos.RutaKeepRugbyClean;
                inscripcionViewModel.FechaRegistroKeepRugbyClean = archivos.FechaRegistroKeepRugbyClean;
                inscripcionViewModel.FechaVencimientoKeepRugbyClean = archivos.FechaVencimientoKeepRugbyClean;
                inscripcionViewModel.RutaPrimerosAuxilios = archivos.RutaPrimerosAuxilios;
                inscripcionViewModel.FechaRegistroPrimerosAuxilios = archivos.FechaRegistroPrimerosAuxilios;
                inscripcionViewModel.FechaVencimientoPrimerosAuxilios = archivos.FechaVencimientoPrimerosAuxilios;
                inscripcionViewModel.RutaConmocionCerebral = archivos.RutaConmocionCerebral;
                inscripcionViewModel.FechaRegistroConmocionCerebral = archivos.FechaRegistroConmocionCerebral;
                inscripcionViewModel.FechaVencimientoConmocionCerebral = archivos.FechaVencimientoConmocionCerebral;
            }
            return View(inscripcionViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Inscripcion(InscripcionViewModel inscripcionViewModel)
        {
            var idUsuarioStr = User.FindFirst("Id_Usuario")?.Value ?? "0";
            var Id_Usuario = int.Parse(idUsuarioStr);
            if (Id_Usuario == 0)
            {
                return RedirectToAction("Login", "Account");
            }
            if (inscripcionViewModel.ActaMedica != null)
            {
                int Id_013_TipoArchivo = 456;
                string nuevaRutaActaMedica = await GuardarArchivo(
                    inscripcionViewModel.ActaMedica,
                    inscripcionViewModel.Id_Equipo,
                    inscripcionViewModel.Id_Jugador,
                    Id_013_TipoArchivo,
                    ".pdf"
                );

                if (!string.IsNullOrEmpty(nuevaRutaActaMedica))
                {
                    await _inscripcionService.Archivo_Insertar(inscripcionViewModel.Id_Equipo, inscripcionViewModel.Id_Jugador, Id_013_TipoArchivo, nuevaRutaActaMedica,
                        inscripcionViewModel.FechaRegistroActaMedica,inscripcionViewModel.FechaVencimientoActaMedica, Id_Usuario);
                }
            }
            if (inscripcionViewModel.RugbyReady != null)
            {
                int Id_013_TipoArchivo = 457;
                string nuevaRutaRugbyReady = await GuardarArchivo(
                    inscripcionViewModel.RugbyReady,
                    inscripcionViewModel.Id_Equipo,
                    inscripcionViewModel.Id_Jugador,
                    Id_013_TipoArchivo,
                    ".pdf"
                );

                if (!string.IsNullOrEmpty(nuevaRutaRugbyReady))
                {
                    await _inscripcionService.Archivo_Insertar(inscripcionViewModel.Id_Equipo, inscripcionViewModel.Id_Jugador, Id_013_TipoArchivo, nuevaRutaRugbyReady,
                        inscripcionViewModel.FechaRegistroRugbyReady, inscripcionViewModel.FechaVencimientoRugbyReady, Id_Usuario);
                }
            }
            if (inscripcionViewModel.RugbyLaws != null)
            {
                int Id_013_TipoArchivo = 458;
                string nuevaRutaRugbyLaws = await GuardarArchivo(
                    inscripcionViewModel.RugbyLaws,
                    inscripcionViewModel.Id_Equipo,
                    inscripcionViewModel.Id_Jugador,
                    Id_013_TipoArchivo,
                    ".pdf"
                );

                if (!string.IsNullOrEmpty(nuevaRutaRugbyLaws))
                {
                    await _inscripcionService.Archivo_Insertar(inscripcionViewModel.Id_Equipo, inscripcionViewModel.Id_Jugador, Id_013_TipoArchivo, nuevaRutaRugbyLaws,
                        inscripcionViewModel.FechaRegistroRugbyLaws, inscripcionViewModel.FechaVencimientoRugbyLaws, Id_Usuario);
                }
            }
            if (inscripcionViewModel.KeepRugbyClean != null)
            {
                int Id_013_TipoArchivo = 459;
                string nuevaRutaKeepRugbyClean = await GuardarArchivo(
                    inscripcionViewModel.KeepRugbyClean,
                    inscripcionViewModel.Id_Equipo,
                    inscripcionViewModel.Id_Jugador,
                    Id_013_TipoArchivo,
                    ".pdf"
                );

                if (!string.IsNullOrEmpty(nuevaRutaKeepRugbyClean))
                {
                    await _inscripcionService.Archivo_Insertar(inscripcionViewModel.Id_Equipo, inscripcionViewModel.Id_Jugador, Id_013_TipoArchivo, nuevaRutaKeepRugbyClean,
                        inscripcionViewModel.FechaRegistroKeepRugbyClean, inscripcionViewModel.FechaVencimientoKeepRugbyClean, Id_Usuario);
                }
            }
            if (inscripcionViewModel.PrimerosAuxilios != null)
            {
                int Id_013_TipoArchivo = 460;
                string nuevaRutaPrimerosAuxilios = await GuardarArchivo(
                    inscripcionViewModel.PrimerosAuxilios,
                    inscripcionViewModel.Id_Equipo,
                    inscripcionViewModel.Id_Jugador,
                    Id_013_TipoArchivo,
                    ".pdf"
                );

                if (!string.IsNullOrEmpty(nuevaRutaPrimerosAuxilios))
                {
                    await _inscripcionService.Archivo_Insertar(inscripcionViewModel.Id_Equipo, inscripcionViewModel.Id_Jugador, Id_013_TipoArchivo, nuevaRutaPrimerosAuxilios,
                        inscripcionViewModel.FechaRegistroPrimerosAuxilios, inscripcionViewModel.FechaVencimientoPrimerosAuxilios, Id_Usuario);
                }
            }
            if (inscripcionViewModel.ConmocionCerebral != null)
            {
                int Id_013_TipoArchivo = 461;
                string nuevaRutaConmocionCerebral = await GuardarArchivo(
                    inscripcionViewModel.ConmocionCerebral,
                    inscripcionViewModel.Id_Equipo,
                    inscripcionViewModel.Id_Jugador,
                    Id_013_TipoArchivo,
                    ".pdf"
                );

                if (!string.IsNullOrEmpty(nuevaRutaConmocionCerebral))
                {
                    await _inscripcionService.Archivo_Insertar(inscripcionViewModel.Id_Equipo, inscripcionViewModel.Id_Jugador, Id_013_TipoArchivo, nuevaRutaConmocionCerebral,
                        inscripcionViewModel.FechaRegistroConmocionCerebral, inscripcionViewModel.FechaVencimientoConmocionCerebral, Id_Usuario);
                }
            }

            await _inscripcionService.Jugador_CambioEstado445(inscripcionViewModel.Id_Equipo, inscripcionViewModel.Id_Jugador, Id_Usuario);

            return RedirectToAction("GuardarJugadorSeleccionado", "Jugador", new { Id_Jugador = inscripcionViewModel.Id_Jugador, Pagina = 2 });
        }
        private async Task LimpiarArchivosExistentes(int Id_Equipo, int Id_Jugador, int Id_013_TipoArchivo, string[] extensionesPermitidas)
        {
            var NombreTipoArchivo = await _tiposService.TipoArchivo_Descripcion(Id_013_TipoArchivo);
            string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Archivos", Id_Equipo.ToString(), Id_Jugador.ToString());

            if (Directory.Exists(folderPath))
            {
                string baseFileName = $"{Id_Jugador}_{NombreTipoArchivo}";
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
        private async Task<string> GuardarArchivo(IFormFile archivo, int Id_Equipo, int Id_Jugador, int Id_013_TipoArchivo, string extensionesPermitidas)
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
            await LimpiarArchivosExistentes(Id_Equipo, Id_Jugador, Id_013_TipoArchivo, allowedExtensions);

            string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Archivos", Id_Equipo.ToString(), Id_Jugador.ToString());
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Usar siempre la extensión del archivo original para mantener el formato correcto
            string fileName = $"{Id_Jugador}_{NombreTipoArchivo}{fileExtension}";
            string filePath = Path.Combine(folderPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await archivo.CopyToAsync(fileStream);
            }

            return Path.Combine("Archivos", Id_Equipo.ToString(), Id_Jugador.ToString(), fileName);
        }
    }
}
