using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebFPRTest.Areas.Interno.Interface.CartaPase;
using WebFPRTest.Areas.Interno.Models.CartaPase;
using WebFPRTest.Interface;

namespace WebFPRTest.Areas.Interno.Controllers
{
    [Area("Interno")]
    [Authorize]
    public class CartaPaseController:Controller
    {

        private readonly ITiposService _tiposService;
        private readonly ICartaPaseService _cartaPaseService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CartaPaseController(ITiposService tiposService, ICartaPaseService cartaPaseService, IWebHostEnvironment webHostEnvironment)
        {
            _tiposService = tiposService;
            _cartaPaseService = cartaPaseService;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public IActionResult GuardarJugadorSeleccionado(int Id_Jugador, int Id_Equipo)
        {
            var tipoUsuario = User.FindFirstValue("Id_011_TipoUsuario");
            if (tipoUsuario == null || (tipoUsuario != "406" && tipoUsuario != "407" && tipoUsuario != "408"))
            {
                return RedirectToAction("Login", "Login");
            }
            TempData["Id_Jugador"] = Id_Jugador;
            TempData["Id_Equipo"] = Id_Equipo;
            return RedirectToAction("CartaPase", "CartaPase", new { area = "Interno" });
        }
        [HttpGet]
        public async Task<IActionResult> CartaPase()
        {
            int Id_Controlador = 6;
            int tipoUsuario = int.TryParse(User.FindFirstValue("Id_011_TipoUsuario"), out int result) ? result : 0;

            var acceso = await _tiposService.ControladorTipoUsuario(Id_Controlador, tipoUsuario);

            if (!acceso)
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }
            int Id_Jugador = TempData.Peek("Id_Jugador") as int? ?? 0;
            int Id_Equipo = TempData.Peek("Id_Equipo") as int? ?? 0;
            CartaPaseViewModel cartaPaseViewModel = new CartaPaseViewModel();
            cartaPaseViewModel.TipoDocumentos = await _tiposService.ParametroTipo_Listar(1);
            cartaPaseViewModel.EquipoList = await _tiposService.Equipo_Listar();
            var jugador = await _cartaPaseService.Jugador_Datos(Id_Jugador);
            cartaPaseViewModel.Paterno = jugador.Paterno;
            cartaPaseViewModel.Materno = jugador.Materno;
            cartaPaseViewModel.Nombres = jugador.Nombres;
            cartaPaseViewModel.Id_001_TipoDocumento = jugador.Id_001_TipoDocumento;
            cartaPaseViewModel.NombreEquipo = jugador.NombreEquipo;
            cartaPaseViewModel.Documento = jugador.Documento;
            cartaPaseViewModel.Correo = jugador.Correo;
            cartaPaseViewModel.Id_JugadorActual = Id_Jugador;
            cartaPaseViewModel.Id_EquipoActual = Id_Equipo;
            return View(cartaPaseViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> CartaPase(CartaPaseViewModel cartaPaseViewModel)
        {
            var idUsuarioStr = User.FindFirst("Id_Usuario")?.Value ?? "0";
            var Id_Usuario = int.Parse(idUsuarioStr);

            if (Id_Usuario == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            if (cartaPaseViewModel.JugadorLibre == true)
            {
                bool estado = await _cartaPaseService.JugadorLibre(cartaPaseViewModel.Id_JugadorActual);

                if (estado == false)
                {
                    TempData["Mensaje"] = "Jugador aun no puede pasar libremente, no ha completado los 2 años";
                    return RedirectToAction("GuardarJugadorSeleccionado", "CartaPase", new { area = "Interno", Id_Jugador = cartaPaseViewModel.Id_JugadorActual, Id_Equipo = cartaPaseViewModel.Id_EquipoActual });
                }
                else
                {
                    cartaPaseViewModel.RutaArchivos = await _cartaPaseService.Archivos_RutasJugador(cartaPaseViewModel.Id_JugadorActual, cartaPaseViewModel.Id_EquipoActual);

                    TempData["Mensaje"] = "Jugador transferido correctamente";
                    cartaPaseViewModel.Id_JugadorNuevo = await _cartaPaseService.Jugador_CartaPase(cartaPaseViewModel.Id_JugadorActual, cartaPaseViewModel.Id_EquipoNuevo, Id_Usuario);
                    await MoverArchivos(cartaPaseViewModel.RutaArchivos, cartaPaseViewModel.Id_EquipoActual, cartaPaseViewModel.Id_EquipoNuevo, cartaPaseViewModel.Id_JugadorActual, cartaPaseViewModel.Id_JugadorNuevo);

                    return RedirectToAction("GuardarJugadorSeleccionado", "CartaPase", new { area = "Interno", Id_Jugador = cartaPaseViewModel.Id_JugadorNuevo, Id_Equipo = cartaPaseViewModel.Id_EquipoNuevo});
                }
            }
            else
            {
                if (cartaPaseViewModel.CartaPase != null)
                {
                    cartaPaseViewModel.Id_JugadorNuevo = await _cartaPaseService.Jugador_CartaPase(cartaPaseViewModel.Id_JugadorActual, cartaPaseViewModel.Id_EquipoNuevo, Id_Usuario);
                    cartaPaseViewModel.RutaArchivos = await _cartaPaseService.Archivos_RutasJugador(cartaPaseViewModel.Id_JugadorActual, cartaPaseViewModel.Id_EquipoActual);
                    await MoverArchivos(cartaPaseViewModel.RutaArchivos, cartaPaseViewModel.Id_EquipoActual, cartaPaseViewModel.Id_EquipoNuevo, cartaPaseViewModel.Id_JugadorActual, cartaPaseViewModel.Id_JugadorNuevo);

                    int Id_013_TipoArchivo = 462;
                    string nuevaRutaCartaPase = await GuardarArchivo(
                        cartaPaseViewModel.CartaPase,
                        cartaPaseViewModel.Id_EquipoNuevo,
                        cartaPaseViewModel.Id_JugadorNuevo,
                        Id_013_TipoArchivo,
                        ".pdf"
                    );

                    if (!string.IsNullOrEmpty(nuevaRutaCartaPase))
                    {
                        await _cartaPaseService.Archivo_Insertar(cartaPaseViewModel.Id_EquipoNuevo, cartaPaseViewModel.Id_JugadorNuevo, Id_013_TipoArchivo, nuevaRutaCartaPase, Id_Usuario);
                        TempData["Mensaje"] = "Jugador transferido correctamente";
                        return RedirectToAction("GuardarJugadorSeleccionado", "CartaPase", new { area = "Interno", Id_Jugador = cartaPaseViewModel.Id_JugadorNuevo, Id_Equipo = cartaPaseViewModel.Id_EquipoNuevo });
                    }
                    else
                    {
                        TempData["Mensaje"] = "Error al guardar la Carta Pase";
                        return RedirectToAction("GuardarJugadorSeleccionado", "CartaPase", new { area = "Interno", Id_Jugador = cartaPaseViewModel.Id_JugadorActual, Id_Equipo = cartaPaseViewModel.Id_EquipoActual });
                    }
                }
                else
                {
                    TempData["Mensaje"] = "Jugador no ha seleccionado documento de Carta Pase";
                    return RedirectToAction("GuardarJugadorSeleccionado", "CartaPase", new { area = "Interno", Id_Jugador = cartaPaseViewModel.Id_JugadorActual, Id_Equipo = cartaPaseViewModel.Id_EquipoActual });
                }
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
        private async Task MoverArchivos(List<RutasArchivos> rutaArchivos, int Id_EquipoActual, int Id_EquipoNuevo, int Id_JugadorActual, int Id_JugadorNuevo)
        {
            string nuevaCarpeta = Path.Combine(_webHostEnvironment.WebRootPath, "Archivos", Id_EquipoNuevo.ToString(), Id_JugadorNuevo.ToString());

            // Crear la nueva carpeta si no existe
            if (!Directory.Exists(nuevaCarpeta))
            {
                Directory.CreateDirectory(nuevaCarpeta);
            }

            foreach (var archivo in rutaArchivos)
            {
                if (archivo == null || string.IsNullOrEmpty(archivo.RutaArchivo))
                {
                    continue; // Saltar rutas vacías
                }

                // Obtener el nombre del tipo de archivo desde el servicio
                string NombreTipoArchivo = await _tiposService.TipoArchivo_Descripcion(archivo.Id_013_TipoArchivo);

                // Construir la ruta original del archivo
                string rutaOriginal = Path.Combine(_webHostEnvironment.WebRootPath, "Archivos", Id_EquipoActual.ToString(), Id_JugadorActual.ToString(), Path.GetFileName(archivo.RutaArchivo));

                // Verificar si el archivo original existe
                if (!System.IO.File.Exists(rutaOriginal))
                {
                    Console.WriteLine($"Archivo no encontrado: {rutaOriginal}");
                    continue;
                }

                // Obtener la extensión original del archivo
                string fileExtension = Path.GetExtension(rutaOriginal);

                // Crear el nuevo nombre del archivo
                string nuevoNombreArchivo = $"{Id_JugadorNuevo}_{NombreTipoArchivo}{fileExtension}";
                string nuevaRuta = Path.Combine(nuevaCarpeta, nuevoNombreArchivo);

                try
                {
                    // Mover el archivo a la nueva ubicación con el nuevo nombre
                    System.IO.File.Move(rutaOriginal, nuevaRuta);
                }
                catch (Exception ex)
                {
                    // Manejo de errores (puedes loguearlo)
                    Console.WriteLine($"Error moviendo archivo {rutaOriginal} -> {nuevaRuta}: {ex.Message}");
                }
            }
        }
    }
}
