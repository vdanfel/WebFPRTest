using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebFPRTest.Areas.Externo.Interface.Equipo;
using WebFPRTest.Areas.Externo.Models.Equipo;
using WebFPRTest.Interface;

namespace WebFPRTest.Areas.Externo.Controllers
{
    [Authorize] // Requiere autenticación
    [Area("Externo")]
    public class EquipoController:Controller
    {
        private readonly IEquipoService _equipoService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ITiposService _tiposService;

        public EquipoController(IEquipoService equipoService, IWebHostEnvironment webHostEnvironment, ITiposService tiposService)
        {
            _equipoService = equipoService;
            _webHostEnvironment = webHostEnvironment;
            _tiposService = tiposService;
        }

        [HttpGet]
        public async Task<IActionResult> Equipo()
        {
            int Id_Controlador = 1;
            int tipoUsuario = int.TryParse(User.FindFirstValue("Id_011_TipoUsuario"), out int result) ? result : 0;

            var acceso = await _tiposService.ControladorTipoUsuario(Id_Controlador,tipoUsuario);

            if (!acceso) 
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }
            var idEquipoClaim = User.Claims.FirstOrDefault(c => c.Type == "Id_Equipo")?.Value;
            int Id_Equipo = (string.IsNullOrEmpty(idEquipoClaim) || !int.TryParse(idEquipoClaim, out int idEquipoTemp)) ? 0 : idEquipoTemp;


            EquipoViewModel equipo = new EquipoViewModel();
            HorariosEntrenamientoModel horarios = new HorariosEntrenamientoModel();
            equipo.Horarios = horarios;
            if (Id_Equipo > 0)
            {
                equipo = await _equipoService.Equipo_Listar(Id_Equipo);
                equipo.RutaLogo = await _equipoService.Archivo_RutaLogo(Id_Equipo, 0, 417);
                equipo.Horarios = await _equipoService.HorariosEntrenamiento_Listar(Id_Equipo);
                HttpContext.Session.SetString("NombreEquipo", equipo.Nombre);
            }
            return View(equipo);
        }
        [HttpPost]
        public async Task<IActionResult> Equipo(EquipoViewModel equipo)
        {
            // Obtener Id_Usuario del Claim
            var idUsuarioStr = User.FindFirst("Id_Usuario")?.Value ?? "0";
            var Id_Usuario = int.Parse(idUsuarioStr);
            
            if (Id_Usuario == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            int existe = await _equipoService.Equipo_Existe(equipo.Nombre, equipo.RUC);
            if (existe == 0)
            {
                equipo.Id_Equipo = await _equipoService.Equipo_Insertar(equipo, Id_Usuario);
                await ActualizarClaimIdEquipo(equipo.Id_Equipo);
                TempData["Mensaje"] = "Equipo registrado con éxito";
            }
            else if (existe == 1)
            {
                TempData["Mensaje"] = "El nombre del Equipo ya está siendo utilizado por otro equipo";
                return View(equipo);
            }
            else if (existe == 2)
            {
                await _equipoService.Equipo_Actualizar(equipo, Id_Usuario);
                TempData["Mensaje"] = "Equipo actualizado con éxito";
            }
            HttpContext.Session.SetString("NombreEquipo", equipo.Nombre);
            await _equipoService.AsociarUsuarioEquipo(equipo.Id_Equipo, Id_Usuario);
            equipo.Horarios.Id_Equipo = equipo.Id_Equipo;
            await _equipoService.HorariosEntrenamientos_Insertar(equipo.Horarios,Id_Usuario);

            if (equipo.Logo != null)
            {
                string nuevaRutaLogo = await GuardarArchivo(
                    equipo.Logo,
                    equipo.Id_Equipo,
                    0,
                    417,
                     ".jpg, .jpeg, .png"
                    );
                if (!string.IsNullOrEmpty(nuevaRutaLogo))
                {
                    await _equipoService.Archivo_Insertar(equipo.Id_Equipo, 0, 417, nuevaRutaLogo, Id_Usuario);
                }
            }

            return RedirectToAction("Equipo");
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

            string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Archivos", Id_Equipo.ToString());

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Usar siempre la extensión del archivo original para mantener el formato correcto
            string fileName = $"{Id_Equipo}_{NombreTipoArchivo}{fileExtension}";
            string filePath = Path.Combine(folderPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await archivo.CopyToAsync(fileStream);
            }
            return Path.Combine("Archivos", Id_Equipo.ToString(), fileName);
        }


        private async Task ActualizarClaimIdEquipo(int nuevoIdEquipo)
        {
            var identity = (ClaimsIdentity)User.Identity;

            // Remover el Claim anterior si existe
            var claimExistente = identity.FindFirst("Id_Equipo");
            if (claimExistente != null)
            {
                identity.RemoveClaim(claimExistente);
            }

            // Agregar el nuevo Claim
            identity.AddClaim(new Claim("Id_Equipo", nuevoIdEquipo.ToString()));

            // Crear una nueva instancia de ClaimsPrincipal con la identidad actualizada
            var nuevoPrincipal = new ClaimsPrincipal(identity);

            // Actualizar la autenticación del usuario
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, nuevoPrincipal);
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

    }
}
