using ClosedXML.Excel;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Globalization;
using System.Security.Claims;
using WebFPRTest.Areas.Externo.Interface.Jugador;
using WebFPRTest.Areas.Externo.Models.Jugador;
using WebFPRTest.Interface;
using WebFPRTest.Models;

namespace WebFPRTest.Areas.Externo.Controllers
{
    [Authorize] // Requiere autenticación
    [Area("Externo")]
    public class JugadorController : Controller
    {
        private readonly ITiposService _tiposService;
        private readonly IJugadorService _jugadorService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public JugadorController(ITiposService tiposService, IJugadorService jugadorService, IWebHostEnvironment webHostEnvironment)
        { 
            _tiposService = tiposService;
            _jugadorService = jugadorService;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public async Task<IActionResult> Index() 
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
            JugadorFiltroViewModel jugadorFiltro = new JugadorFiltroViewModel();
            jugadorFiltro.Id_Equipo = Id_Equipo;
            jugadorFiltro.ListaDivisiones = await _tiposService.ParametroTipo_Listar(7);
            jugadorFiltro.ListaJugadores = await _jugadorService.Jugador_Bandeja(jugadorFiltro);
            TempData.Remove("Id_001_TipoDocumento");
            TempData.Remove("Documento");
            return View(jugadorFiltro);
        }
        [HttpPost]
        public async Task<IActionResult> Index(JugadorFiltroViewModel jugadorFiltroViewModel)
        {
            // Obtener Id_Usuario del Claim
            var idUsuarioStr = User.FindFirst("Id_Usuario")?.Value ?? "0";
            var Id_Usuario = int.Parse(idUsuarioStr);
            if (Id_Usuario == 0)
            {
                return RedirectToAction("Login", "Account");
            }
            jugadorFiltroViewModel.ListaDivisiones = await _tiposService.ParametroTipo_Listar(7);
            jugadorFiltroViewModel.ListaJugadores = await _jugadorService.Jugador_Bandeja(jugadorFiltroViewModel);
            return View(jugadorFiltroViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> CargarExcel(IFormFile archivoExcel)
        {
            // Obtener Id_Usuario del Claim
            var idUsuarioStr = User.FindFirst("Id_Usuario")?.Value ?? "0";
            var Id_Usuario = int.Parse(idUsuarioStr);
            var idEquipoStr = User.FindFirst("Id_Equipo")?.Value ?? "0";
            var Id_Equipo = int.Parse(idEquipoStr);
            if (Id_Usuario == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            if (archivoExcel == null || archivoExcel.Length == 0)
            {
                return Json(new { success = false, message = "No se seleccionó un archivo." });
            }

            var columnasMapeadas = new Dictionary<string, string>
            {
                { "Ape. Paterno", "Paterno" },
                { "Ape. Materno", "Materno" },
                { "Nombres", "Nombres" },
                { "Tipo Documento", "Id_001_TipoDocumento" },
                { "Nro Documento", "Documento" },
                { "Fecha Nacimiento", "FechaNacimiento" },
                { "Celular", "Celular" },
                { "Correo", "Correo" },
                { "Sexo", "Sexo" }
            };

            var listaJugadores = new List<Dictionary<string, string>>();
            var errores = new List<string>();

            using (var stream = new MemoryStream())
            {
                await archivoExcel.CopyToAsync(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1); // Primera hoja

                    if (worksheet == null)
                    {
                        return Json(new { success = false, message = "El archivo no tiene hojas válidas." });
                    }

                    int colCount = worksheet.LastColumnUsed().ColumnNumber();
                    int rowCount = worksheet.LastRowUsed().RowNumber();

                    // Leer los nombres de las columnas en la primera fila
                    var nombresColumnas = new Dictionary<int, string>();

                    for (int col = 1; col <= colCount; col++)
                    {
                        string nombreColumna = worksheet.Cell(1, col).GetString().Trim();
                        if (columnasMapeadas.ContainsKey(nombreColumna))
                        {
                            nombresColumnas[col] = columnasMapeadas[nombreColumna]; // Usar el nombre mapeado
                        }
                    }

                    // Leer los datos del Excel (desde la fila 2 en adelante)
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var jugador = new Dictionary<string, string>();
                        bool filaValida = true;
                        int idTipoDocumento = 0;
                        string nroDocumento = "";

                        foreach (var col in nombresColumnas.Keys)
                        {
                            string valorCelda = worksheet.Cell(row, col).GetString().Trim();

                            if (string.IsNullOrEmpty(valorCelda))
                            {
                                errores.Add($"Fila {row}: La columna '{nombresColumnas[col]}' está vacía.");
                                filaValida = false;
                            }
                            else
                            {
                                jugador[nombresColumnas[col]] = valorCelda;
                            }
                        }

                        // Verificamos si la fila es válida antes de continuar
                        if (!filaValida) continue;

                        // Obtener el tipo de documento y el número de documento desde el diccionario
                        if (jugador.ContainsKey("Id_001_TipoDocumento"))
                        {
                            int.TryParse(jugador["Id_001_TipoDocumento"], out idTipoDocumento);
                        }

                        if (jugador.ContainsKey("Documento"))
                        {
                            nroDocumento = jugador["Documento"];
                        }

                        // Validar si la persona existe en la BD
                        var persona = await _jugadorService.Persona_Existe(idTipoDocumento, nroDocumento);
                        if (persona == null)
                        {
                            persona = new PersonaModel();
                        }
                        if (persona.Id_Persona == 0)
                        {

                            persona.Paterno = jugador.ContainsKey("Paterno") ? jugador["Paterno"] : null;
                            persona.Materno = jugador.ContainsKey("Materno") ? jugador["Materno"] : null;
                            persona.Nombres = jugador.ContainsKey("Nombres") ? jugador["Nombres"] : null;
                            persona.Id_001_TipoDocumento = idTipoDocumento;
                            persona.Documento = nroDocumento;
                            if (jugador.ContainsKey("FechaNacimiento"))
                            {
                                var fechaValor = jugador["FechaNacimiento"];

                                if (DateTime.TryParse(fechaValor, out DateTime fechaNacimiento))
                                {
                                    persona.FechaNacimiento = fechaNacimiento;
                                }
                                else
                                {
                                    Console.WriteLine($"Error al convertir la fecha: {fechaValor}");
                                    persona.FechaNacimiento = null; // o alguna otra lógica de manejo de errores
                                }
                            }

                            persona.Celular = jugador.ContainsKey("Celular") ? jugador["Celular"] : null;
                            persona.Correo = jugador.ContainsKey("Correo") ? jugador["Correo"] : null;
                            persona.Id_002_Sexo = jugador.ContainsKey("Sexo") && int.TryParse(jugador["Sexo"], out int sexo) ? sexo : 0;

                            // Insertar persona en la BD
                            persona.Id_Persona = await _jugadorService.Persona_Insertar(persona, Id_Usuario);
                        }

                        if (persona.Id_Persona != null)
                        {
                            var jExiste = await _jugadorService.Jugador_Existe(persona.Id_Persona, Id_Equipo);
                            if (jExiste == 0)
                            {
                                var jugadorModel = new JugadorModel
                                {
                                    Id_Persona = persona.Id_Persona,
                                    Id_Equipo = Id_Equipo
                                };
                                var Id_Jugador = await _jugadorService.Jugador_Insertar(jugadorModel, Id_Usuario);
                            }
                            else if (jExiste == 1)
                            {
                                errores.Add("El jugador " + jugador["Nombres"]+" ya había sido registrado");
                            }
                            else if (jExiste == 2)
                            {
                                errores.Add("El jugador " + jugador["Nombres"] + " está registrado en otro equipo");
                            }
                        }
                    }
                }
            }
            
            // Si hay errores, generamos un archivo de log para descargar
            if (errores.Count > 0)
            {
                var logPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "logs");
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }

                string logFileName = $"LogErrores_{System.DateTime.Now:yyyyMMddHHmmss}.txt";
                string logFilePath = Path.Combine(logPath, logFileName);
                await System.IO.File.WriteAllLinesAsync(logFilePath, errores);

                return Json(new { success = false, message = "Errores en la carga. Descarga el log.", logFile = $"/logs/{logFileName}" });
            }

            return Json(new { success = true, message = "Archivo cargado exitosamente." });
        }
        [HttpGet]
        public IActionResult DescargarLog(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return NotFound("El archivo no existe.");
            }

            // Asegurar que fileName no tenga rutas parciales como "/logs/"
            fileName = fileName.Replace("/logs/", "").Replace("\\logs\\", "");

            // Construir la ruta correcta
            var logPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "logs", fileName);
            Console.WriteLine($"Buscando archivo en: {logPath}"); // Depurar ruta

            if (!System.IO.File.Exists(logPath))
            {
                return NotFound($"El archivo de log no se encontró. Ruta: {logPath}");
            }

            // Leer el archivo
            var fileBytes = System.IO.File.ReadAllBytes(logPath);

            // Eliminar el archivo después de leerlo
            System.IO.File.Delete(logPath);

            // Devolver el archivo para la descarga
            return File(fileBytes, "text/plain", fileName);
        }
        public IActionResult GuardarJugadorSeleccionado(int Id_Jugador, int Pagina)
        {
            var tipoUsuario = User.FindFirstValue("Id_011_TipoUsuario");
            if (tipoUsuario == null || tipoUsuario != "409")
            {
                return RedirectToAction("Login", "Login");
            }
            TempData["Id_Jugador"] = Id_Jugador;
            if (Pagina == 1)
            {
                return RedirectToAction("Jugador", "Jugador", new { area = "Externo" });
            }
            else
            {
                return RedirectToAction("Inscripcion", "Inscripcion", new { area = "Externo" });
            }
            
        }
        [HttpGet]
        public async Task<IActionResult> Jugador()
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
            JugadorViewModel jugadorViewModel = new JugadorViewModel();
            jugadorViewModel.Id_Equipo = Id_Equipo;
            jugadorViewModel.Id_Jugador = TempData.Peek("Id_Jugador") as int? ?? 0;
            jugadorViewModel.TipoDocumentos = await _tiposService.ParametroTipo_Listar(1);
            jugadorViewModel.Paises = await _tiposService.ParametroTipo_Listar(3);
            jugadorViewModel.Nacionalidades = await _tiposService.ParametroTipo_Listar(4);
            jugadorViewModel.Sexos = await _tiposService.ParametroTipo_Listar(2);
            jugadorViewModel.TipoSeguros = await _tiposService.ParametroTipo_Listar(5);
            jugadorViewModel.TipoVehiculos = await _tiposService.ParametroTipo_Listar(6);
            jugadorViewModel.DivisionList = await _tiposService.ParametroTipo_Listar(7);
            jugadorViewModel.SituacionList = await _tiposService.ParametroTipo_Listar(8);
            jugadorViewModel.TipoSangre = await _tiposService.ParametroTipo_Listar(14);
            
            if (jugadorViewModel.Id_Jugador > 0)
            { 
                var jugador = await _jugadorService.Jugador_Select(jugadorViewModel.Id_Jugador);
                jugadorViewModel.Id_Persona = jugador.Id_Persona;
                jugadorViewModel.Paterno = jugador.Paterno;
                jugadorViewModel.Materno = jugador.Materno;
                jugadorViewModel.Nombres = jugador.Nombres;
                jugadorViewModel.DatosApoderado = await _jugadorService.Apoderado_Select(jugadorViewModel.Id_Persona);
                jugadorViewModel.Id_001_TipoDocumento = jugador.Id_001_TipoDocumento;
                jugadorViewModel.Documento = jugador.Documento;
                jugadorViewModel.FechaNacimiento = jugador.FechaNacimiento;
                jugadorViewModel.Id_003_Pais = jugador.Id_003_Pais;
                jugadorViewModel.Id_004_Nacionalidad = jugador.Id_004_Nacionalidad;
                jugadorViewModel.Id_002_Sexo = jugador.Id_002_Sexo;
                jugadorViewModel.Celular = jugador.Celular;
                jugadorViewModel.Correo = jugador.Correo;
                jugadorViewModel.Id_005_TipoSeguro = jugador.Id_005_TipoSeguro;
                jugadorViewModel.NumeroPoliza = jugador.NumeroPoliza;
                jugadorViewModel.FechaPoliza = jugador.FechaPoliza;
                jugadorViewModel.FechaVencimientoPoliza = jugador.FechaVencimientoPoliza;
                jugadorViewModel.Id_006_TipoVehiculo = jugador.Id_006_TipoVehiculo;
                jugadorViewModel.NumeroPlaca = jugador.NumeroPlaca;
                jugadorViewModel.Id_007_Division = jugador.Id_007_Division;
                jugadorViewModel.Id_008_Situacion = jugador.Id_008_Situacion;
                jugadorViewModel.Id_009_EstadoJugador = jugador.Id_009_EstadoJugador;
                jugadorViewModel.Observacion = jugador.Observacion;
                jugadorViewModel.MotivoAnulacion = jugador.MotivoAnulacion;
                jugadorViewModel.Id_014_TipoSangre = jugador.Id_014_TipoSangre;
                jugadorViewModel.RutaFoto = await _jugadorService.Archivo_Ruta(jugadorViewModel.Id_Equipo, jugadorViewModel.Id_Jugador, 431);
                jugadorViewModel.RutaDeslinde = await _jugadorService.Archivo_Ruta(jugadorViewModel.Id_Equipo, jugadorViewModel.Id_Jugador, 432);
                jugadorViewModel.MotivoAnulacion = jugador.MotivoAnulacion;
            }
            else
            {
                var Id_001_TipoDocumento = TempData.Peek("Id_001_TipoDocumento") as int? ?? 0;
                var Documento = TempData.Peek("Documento") as string;
                if (Id_001_TipoDocumento > 0)
                {
                    var persona = await _jugadorService.Persona_Existe(Id_001_TipoDocumento, Documento);
                    if (persona == null)
                    {
                        persona = new PersonaModel();
                    }
                    jugadorViewModel.Id_Persona = persona.Id_Persona;
                    jugadorViewModel.Paterno = persona.Paterno ?? string.Empty;
                    jugadorViewModel.Materno = persona.Materno ?? string.Empty;
                    jugadorViewModel.Nombres = persona.Nombres ?? string.Empty;
                    jugadorViewModel.FechaNacimiento = persona.FechaNacimiento;
                    jugadorViewModel.Id_003_Pais = persona.Id_003_Pais;
                    jugadorViewModel.Id_004_Nacionalidad = persona.Id_004_Nacionalidad;
                    jugadorViewModel.Id_002_Sexo = persona.Id_002_Sexo;
                    jugadorViewModel.Celular = persona.Celular ?? string.Empty;
                    jugadorViewModel.Id_014_TipoSangre = persona.Id_014_TipoSangre;
                    jugadorViewModel.Correo = persona.Correo ?? string.Empty;
                    jugadorViewModel.Id_005_TipoSeguro = persona.Id_005_TipoSeguro;
                    jugadorViewModel.NumeroPoliza = persona.NumeroPoliza ?? string.Empty;
                    jugadorViewModel.FechaPoliza = persona.FechaPoliza;
                    jugadorViewModel.FechaVencimientoPoliza = persona.FechaVencimientoPoliza;
                    jugadorViewModel.Id_006_TipoVehiculo = persona.Id_006_TipoVehiculo;
                    jugadorViewModel.NumeroPlaca = persona.NumeroPlaca ?? string.Empty;
                    jugadorViewModel.Id_001_TipoDocumento = Id_001_TipoDocumento;
                    jugadorViewModel.Documento = Documento ?? string.Empty;
                    var apoderado = await _jugadorService.Apoderado_Select(jugadorViewModel.Id_Persona);
                    jugadorViewModel.DatosApoderado = apoderado ?? new JugadorApoderado();

                }
            }
            return View(jugadorViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Jugador(JugadorViewModel jugadorViewModel)
        {
            var idUsuarioStr = User.FindFirst("Id_Usuario")?.Value ?? "0";
            var Id_Usuario = int.Parse(idUsuarioStr);
            if (Id_Usuario == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var persona = await _jugadorService.Persona_Existe(jugadorViewModel.Id_001_TipoDocumento, jugadorViewModel.Documento);
            if (persona == null)
            {
                PersonaModel personaModel = new PersonaModel()
                {
                    Id_001_TipoDocumento = jugadorViewModel.Id_001_TipoDocumento,
                    Documento = jugadorViewModel.Documento,
                    Paterno = jugadorViewModel.Paterno,
                    Materno = jugadorViewModel.Materno,
                    Nombres = jugadorViewModel.Nombres,
                    FechaNacimiento = jugadorViewModel.FechaNacimiento ?? DateTime.MinValue, // Si es null, usa una fecha por defecto
                    Id_003_Pais = jugadorViewModel.Id_003_Pais,
                    Id_004_Nacionalidad = jugadorViewModel.Id_004_Nacionalidad,
                    Id_002_Sexo = jugadorViewModel.Id_002_Sexo,
                    Celular = jugadorViewModel.Celular,
                    Id_014_TipoSangre = jugadorViewModel.Id_014_TipoSangre,
                    Correo = jugadorViewModel.Correo,
                    Id_005_TipoSeguro = jugadorViewModel.Id_005_TipoSeguro,
                    NumeroPoliza = jugadorViewModel.NumeroPoliza,
                    FechaPoliza = jugadorViewModel.FechaPoliza ?? DateTime.MinValue, // Evita null en fechas
                    FechaVencimientoPoliza = jugadorViewModel.FechaVencimientoPoliza ?? DateTime.MinValue, // Evita null en fechas
                    Id_006_TipoVehiculo = jugadorViewModel.Id_006_TipoVehiculo ?? 0, // Si es null, usa 0
                    NumeroPlaca = jugadorViewModel.NumeroPlaca ?? "" // Si es null, usa string vacío
                };
                jugadorViewModel.Id_Persona = await _jugadorService.Persona_Insertar(personaModel, Id_Usuario);
                
            }
            else
            {
                await _jugadorService.Persona_Actualizar(jugadorViewModel, Id_Usuario);
            }
            if (jugadorViewModel.DatosApoderado.ApoderadoDocumento != null)
            {
                await _jugadorService.Apoderado_Insertar(jugadorViewModel, Id_Usuario);
            }
            int existe = await _jugadorService.Jugador_Existe(jugadorViewModel.Id_Persona, jugadorViewModel.Id_Equipo);
            if (existe == 0)
            {
                var jugadorModel = new JugadorModel
                {
                    Id_Persona = jugadorViewModel.Id_Persona,
                    Id_Equipo = jugadorViewModel.Id_Equipo,
                    Id_007_Division = jugadorViewModel.Id_007_Division,
                    Id_008_Situacion = jugadorViewModel.Id_008_Situacion,
                    Id_009_EstadoJugador = 401
                };
                jugadorViewModel.Id_Jugador = await _jugadorService.Jugador_Insertar(jugadorModel, Id_Usuario);
                TempData["Mensaje"] = "Jugador registrado con éxito";
            }
            else if (existe == 1)
            {
                await _jugadorService.Persona_Actualizar(jugadorViewModel, Id_Usuario);
                await _jugadorService.Jugador_Actualizar(jugadorViewModel, Id_Usuario);
                TempData["Mensaje"] = "Jugador actualizado con éxito";
            }
            else if (existe == 2)
            {
                TempData["Mensaje"] = "Jugador está en otro equipo";
                return RedirectToAction("Jugador");
            }
            if (!jugadorViewModel.DatosApoderado.ApoderadoNombres.IsNullOrEmpty())
            {
                await _jugadorService.Apoderado_Insertar(jugadorViewModel, Id_Usuario);
            }

            if (jugadorViewModel.Foto != null)
            {
                string nuevaRutaFoto = await GuardarArchivo(
                    jugadorViewModel.Foto,
                    jugadorViewModel.Id_Equipo,
                    jugadorViewModel.Id_Jugador,
                    431,
                    ".jpg, .jpeg, .png"
                );

                if (!string.IsNullOrEmpty(nuevaRutaFoto))
                {
                    await _jugadorService.Archivo_Insertar(jugadorViewModel.Id_Equipo, jugadorViewModel.Id_Jugador, 431, nuevaRutaFoto, Id_Usuario);
                }
            }

            if (jugadorViewModel.Deslinde != null)
            {
                string nuevaRutaDeslinde = await GuardarArchivo(
                    jugadorViewModel.Deslinde,
                    jugadorViewModel.Id_Equipo,
                    jugadorViewModel.Id_Jugador,
                    432,
                    ".pdf"
                );

                if (!string.IsNullOrEmpty(nuevaRutaDeslinde))
                {
                    await _jugadorService.Archivo_Insertar(jugadorViewModel.Id_Equipo, jugadorViewModel.Id_Jugador, 432, nuevaRutaDeslinde, Id_Usuario);
                }
            }
            await _jugadorService.Jugador_CambioEstado441(jugadorViewModel.Id_Equipo, jugadorViewModel.Id_Jugador, Id_Usuario);
            return RedirectToAction("GuardarJugadorSeleccionado", "Jugador", new {Id_Jugador = jugadorViewModel.Id_Jugador, Pagina = 1 });
        }
        [HttpGet]
        public async Task<IActionResult> ValidarPersona(int idTipoDocumento, string documento)
        {
            var idEquipoStr = User.FindFirst("Id_Equipo")?.Value ?? "0";
            var Id_Equipo = int.Parse(idEquipoStr);

            TempData["Id_001_TipoDocumento"] = idTipoDocumento;
            TempData["Documento"] = documento;

            int Id_Jugador = await _jugadorService.Jugador_BuscarPorDocumento(Id_Equipo, idTipoDocumento, documento);

            if (Id_Jugador < 0)
            {
                return Json(new { success = false, message = "El jugador ya está registrado en otro equipo." });
            }
            else if (Id_Jugador > 0)
            {
                return Json(new { success = true, redirectUrl = Url.Action("GuardarJugadorSeleccionado", "Jugador", new { Id_Jugador = Id_Jugador, Pagina = 1 }) });
            }
            else
            {
                return Json(new { success = true, redirectUrl = Url.Action("Jugador", "Jugador") });
            }
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
