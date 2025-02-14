﻿using ClosedXML.Excel;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System.Data;
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
                                Correo = jugador.ContainsKey("Correo") ? jugador["Correo"] : null,
                                Id_002_Sexo = jugador.ContainsKey("Sexo") && int.TryParse(jugador["Sexo"], out int sexo) ? sexo : 0
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
            jugadorViewModel.Id_Equipo = Id_Equipo;
            jugadorViewModel.Id_Jugador = TempData.Peek("Id_Jugador") as int? ?? 0;
            if (jugadorViewModel.Id_Jugador > 0)
            {
                jugadorViewModel = await _jugadorService.Jugador_Select(jugadorViewModel.Id_Jugador);
                jugadorViewModel.DatosApoderado = await _jugadorService.Apoderado_Select(jugadorViewModel.Id_Persona);
            }
            jugadorViewModel.TipoDocumentos = await _tiposService.ParametroTipo_Listar(1);
            jugadorViewModel.Paises = await _tiposService.ParametroTipo_Listar(3);
            jugadorViewModel.Nacionalidades = await _tiposService.ParametroTipo_Listar(4);
            jugadorViewModel.Sexos = await _tiposService.ParametroTipo_Listar(2);
            jugadorViewModel.TipoSeguros = await _tiposService.ParametroTipo_Listar(5);
            jugadorViewModel.TipoVehiculos = await _tiposService.ParametroTipo_Listar(6);
            jugadorViewModel.DivisionList = await _tiposService.ParametroTipo_Listar(7);
            jugadorViewModel.SituacionList = await _tiposService.ParametroTipo_Listar(8);
            jugadorViewModel.TipoSangre = await _tiposService.ParametroTipo_Listar(14);
            jugadorViewModel.RutaFoto = await _jugadorService.Archivo_Ruta(jugadorViewModel.Id_Equipo, jugadorViewModel.Id_Jugador, 431);
            jugadorViewModel.RutaDeslinde = await _jugadorService.Archivo_Ruta(jugadorViewModel.Id_Equipo, jugadorViewModel.Id_Jugador, 432);
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
            int? existeP = await _jugadorService.Persona_Existe(jugadorViewModel.Id_001_TipoDocumento, jugadorViewModel.Documento);
            if (existeP == null)
            { 
                PersonaModel personaModel = new PersonaModel()
                { 
                    Id_001_TipoDocumento = jugadorViewModel.Id_001_TipoDocumento,
                    Paterno = jugadorViewModel.Paterno,
                    Materno = jugadorViewModel.Materno,
                    Nombres = jugadorViewModel.Nombres,
                    FechaNacimiento = (DateTime)jugadorViewModel.FechaNacimiento,
                    Id_003_Pais = jugadorViewModel.Id_003_Pais,
                    Id_004_Nacionalidad = jugadorViewModel.Id_004_Nacionalidad,
                    Id_002_Sexo = jugadorViewModel.Id_002_Sexo,
                    Celular = jugadorViewModel.Celular,
                    Id_014_TipoSangre = jugadorViewModel.Id_014_TipoSangre,
                    Correo = jugadorViewModel.Correo,
                    Id_005_TipoSeguro = jugadorViewModel.Id_005_TipoSeguro,
                    NumeroPoliza = jugadorViewModel.NumeroPoliza,
                    FechaPoliza = (DateTime)jugadorViewModel.FechaPoliza,
                    FechaVencimientoPoliza = (DateTime)jugadorViewModel.FechaVencimientoPoliza,
                    Id_006_TipoVehiculo = (int)jugadorViewModel.Id_006_TipoVehiculo,
                    NumeroPlaca = jugadorViewModel.NumeroPlaca
                };
                jugadorViewModel.Id_Persona = await _jugadorService.Persona_Insertar(personaModel, Id_Usuario);
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
                    Id_009_EstadoJugador = jugadorViewModel.Id_009_EstadoJugador
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
            if (jugadorViewModel.DatosApoderado.ApoderadoNombres.Length > 0)
            { 
                await _jugadorService.Apoderado_Insertar(jugadorViewModel, Id_Usuario);
            }
            //var rutaExisteFoto = await _jugadorService.Archivo_Ruta(jugadorViewModel.Id_Equipo, jugadorViewModel.Id_Jugador, 431);
            //if (!string.IsNullOrEmpty(rutaExisteFoto))
            //{
            //    EliminarArchivo(rutaExisteFoto);
            //}
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

            //var rutaExisteDeslinde = await _jugadorService.Archivo_Ruta(jugadorViewModel.Id_Equipo, jugadorViewModel.Id_Jugador, 432);
            //if (!string.IsNullOrEmpty(rutaExisteDeslinde))
            //{
            //    EliminarArchivo(rutaExisteDeslinde);
            //}
            if (jugadorViewModel.Deslinde != null)
            {
                string nuevaRutaDeslinde = await GuardarArchivo(
                    jugadorViewModel.Deslinde,
                    jugadorViewModel.Id_Equipo,
                    jugadorViewModel.Id_Jugador,
                    432,
                    ".pdf, .doc, .docx"
                );

                if (!string.IsNullOrEmpty(nuevaRutaDeslinde))
                {
                    await _jugadorService.Archivo_Insertar(jugadorViewModel.Id_Equipo, jugadorViewModel.Id_Jugador, 432, nuevaRutaDeslinde, Id_Usuario);
                }
            }

            return RedirectToAction("Jugador");
        }
        private async Task<string> GuardarArchivo(IFormFile archivo, int Id_Equipo, int Id_Jugador, int Id_013_TipoArchivo,  string extensionesPermitidas)
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
            string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Archivos", Id_Equipo.ToString(), Id_Jugador.ToString());
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string fileName = $"{Id_Jugador}_{NombreTipoArchivo}{fileExtension}";
            string filePath = Path.Combine(folderPath, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await archivo.CopyToAsync(fileStream);
            }
            return Path.Combine("Archivos", Id_Equipo.ToString(), Id_Jugador.ToString(), fileName);
        }
        //public void EliminarArchivo(string rutaArchivo)
        //{
        //    try
        //    {
        //        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, rutaArchivo);
        //        if (System.IO.File.Exists(filePath))
        //        {
        //            System.IO.File.Delete(filePath); // Elimina el archivo existente
        //        }
        //        else
        //        {
        //            throw new FileNotFoundException("El archivo no fue encontrado para eliminar.", filePath);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Ocurrió un error al eliminar el archivo: {rutaArchivo}", ex);
        //    }
        //}
        [HttpGet]
        public async Task<IActionResult> ValidarPersona(int idTipoDocumento, string documento)
        {
            var idEquipoStr = User.FindFirst("Id_Equipo")?.Value ?? "0";
            var Id_Equipo = int.Parse(idEquipoStr);

            int Id_Jugador = await _jugadorService.Jugador_BuscarPorDocumento(Id_Equipo, idTipoDocumento, documento);

            if (Id_Jugador < 0)
            {
                return Json(new { success = false, message = "El jugador ya está registrado en otro equipo." });
            }
            if (Id_Jugador > 0)
            {
                return Json(new { success = true, redirectUrl = Url.Action("GuardarJugadorSeleccionado", "Jugador", new { Id_Jugador }) });
            }

            // Para el caso en que Id_Jugador == 0
            return Json(new { success = true });
        }

    }
}
