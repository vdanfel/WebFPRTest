using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public JugadorController(ITiposService tiposService, IJugadorService jugadorService)
        { 
            _tiposService = tiposService;
            _jugadorService = jugadorService;
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
            JugadorFiltroViewModel jugadorFiltro = new JugadorFiltroViewModel();
            jugadorFiltro.Id_Equipo = Id_Equipo;
            jugadorFiltro.ListaDivisiones = await _tiposService.ParametroTipo_Listar(7);
            jugadorFiltro.ListaJugadores = await _jugadorService.Jugador_Bandeja(jugadorFiltro);
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
                { "Correo", "Correo" }
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
                        int? Id_Persona = await _jugadorService.Persona_Existe(idTipoDocumento, nroDocumento);

                        if (Id_Persona == null)
                        {
                            var persona = new PersonaModel
                            {
                                Paterno = jugador.ContainsKey("Paterno") ? jugador["Paterno"] : null,
                                Materno = jugador.ContainsKey("Materno") ? jugador["Materno"] : null,
                                Nombres = jugador.ContainsKey("Nombres") ? jugador["Nombres"] : null,
                                Id_001_TipoDocumento = idTipoDocumento,
                                Documento = nroDocumento,
                                FechaNacimiento = jugador.ContainsKey("FechaNacimiento") && DateTime.TryParse(jugador["FechaNacimiento"], out DateTime fechaNacimiento) ? fechaNacimiento : default,
                                Celular = jugador.ContainsKey("Celular") ? jugador["Celular"] : null,
                                Correo = jugador.ContainsKey("Correo") ? jugador["Correo"] : null
                            };
                            // Insertar persona en la BD
                            Id_Persona = await _jugadorService.Persona_Insertar(persona, Id_Usuario);
                        }

                        if (Id_Persona != null)
                        {
                            var jExiste = await _jugadorService.Jugador_Existe((int)Id_Persona, Id_Equipo);
                            if (jExiste == 0)
                            {
                                var jugadorModel = new JugadorModel
                                {
                                    Id_Persona = (int)Id_Persona,
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
        public IActionResult GuardarJugadorSeleccionado(int Id_Jugador)
        {
            var tipoUsuario = User.FindFirstValue("Id_011_TipoUsuario");
            if (tipoUsuario == null || tipoUsuario != "409")
            {
                return RedirectToAction("Login", "Login");
            }
            TempData["Id_Jugador"] = Id_Jugador;
            return RedirectToAction("Jugador", "Jugador", new { area = "Externo" });
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
            JugadorViewModel jugadorViewModel = new JugadorViewModel();
            jugadorViewModel.TipoDocumentos = await _tiposService.ParametroTipo_Listar(1);
            jugadorViewModel.Paises = await _tiposService.ParametroTipo_Listar(3);
            jugadorViewModel.Nacionalidades = await _tiposService.ParametroTipo_Listar(4);
            jugadorViewModel.Sexos = await _tiposService.ParametroTipo_Listar(2);
            jugadorViewModel.TipoSeguros = await _tiposService.ParametroTipo_Listar(5);
            jugadorViewModel.TipoVehiculos = await _tiposService.ParametroTipo_Listar(6);
            jugadorViewModel.DivisionList = await _tiposService.ParametroTipo_Listar(7);
            jugadorViewModel.SituacionList = await _tiposService.ParametroTipo_Listar(8);
            return View(jugadorViewModel);
        }
    }
}
