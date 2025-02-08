using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebFPRTest.Areas.Externo.Models.Jugador;
using WebFPRTest.Interface;

namespace WebFPRTest.Areas.Externo.Controllers
{
    [Authorize] // Requiere autenticación
    [Area("Externo")]
    public class JugadorController : Controller
    {
        private readonly ITiposService _tiposService;
        public JugadorController(ITiposService tiposService)
        { 
            _tiposService = tiposService;
        }
        [HttpGet]
        public async Task<IActionResult> Index() 
        {
            var tipoUsuario = User.FindFirstValue("Id_011_TipoUsuario");
            if (tipoUsuario == null || tipoUsuario != "409")
            {
                return RedirectToAction("Login", "Login");
            }
            JugadorFiltroViewModel jugadorFiltro = new JugadorFiltroViewModel();
            jugadorFiltro.ListaDivisiones = await _tiposService.ParametroTipo_Listar(7);
            return View(jugadorFiltro);
        }
        [HttpPost]
        public async Task<IActionResult> CargarExcel(IFormFile archivoExcel)
        {
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

                        if (filaValida)
                        {
                            listaJugadores.Add(jugador);
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

            var logPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "logs", fileName);

            if (!System.IO.File.Exists(logPath))
            {
                return NotFound("El archivo de log no se encontró.");
            }

            // Leer el archivo
            var fileBytes = System.IO.File.ReadAllBytes(logPath);

            // Eliminar el archivo después de leerlo
            System.IO.File.Delete(logPath);

            // Devolver el archivo para la descarga
            return File(fileBytes, "text/plain", fileName);
        }

    }
}
