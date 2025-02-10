using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebFPRTest.Areas.Externo.Interface.Equipo;
using WebFPRTest.Areas.Externo.Models.Equipo;

namespace WebFPRTest.Areas.Externo.Controllers
{
    [Authorize] // Requiere autenticación
    [Area("Externo")]
    public class EquipoController:Controller
    {
        private readonly IEquipoService _equipoService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EquipoController(IEquipoService equipoService, IWebHostEnvironment webHostEnvironment)
        {
            _equipoService = equipoService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Equipo()
        {
            var tipoUsuario = User.FindFirstValue("Id_011_TipoUsuario");
            if (tipoUsuario == null || tipoUsuario != "409")
            {
                return RedirectToAction("Login", "Login");
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
            await _equipoService.AsociarUsuarioEquipo(equipo.Id_Equipo, Id_Usuario);
            equipo.Horarios.Id_Equipo = equipo.Id_Equipo;
            await _equipoService.HorariosEntrenamientos_Insertar(equipo.Horarios,Id_Usuario);
            var rutaExistente = await _equipoService.Archivo_RutaLogo(equipo.Id_Equipo, 0, 417);
            if (!string.IsNullOrEmpty(rutaExistente))
            {
                EliminarArchivo(rutaExistente);
            }

            if (equipo.Logo != null && equipo.Logo.Length > 0)
            {
                // Extensiones permitidas
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
                string fileExtension = Path.GetExtension(equipo.Logo.FileName).ToLower();

                // Comprobar si la extensión es válida
                if (!allowedExtensions.Contains(fileExtension))
                {
                    // Agregar un error al modelo si la extensión no es válida
                    ModelState.AddModelError("Logo", "Solo se permiten archivos de imagen (.jpg, .jpeg, .png).");
                    return View(equipo); // Retorna con el error
                }

                // Obtener la ruta base donde se almacenarán los archivos
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Archivos", equipo.Id_Equipo.ToString());

                // Crear la carpeta si no existe
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }
                // Crear un nombre de archivo único y agregar la extensión
                string fileName = $"{equipo.Id_Equipo}_Logo{fileExtension}";
                // Ruta completa donde se guardará el archivo
                string filePath = Path.Combine(uploadFolder, fileName);
                // Guardar el archivo en la ruta especificada
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await equipo.Logo.CopyToAsync(fileStream);
                }
                // Asignar la ruta final del archivo al modelo
                equipo.RutaLogo = Path.Combine("Archivos", equipo.Id_Equipo.ToString(), fileName);
                await _equipoService.Archivo_Insertar(equipo.Id_Equipo, 0, 417, equipo.RutaLogo, Id_Usuario);
            }
            return RedirectToAction("Equipo");
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

        public void EliminarArchivo(string rutaArchivo)
        {
            try
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, rutaArchivo);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath); // Elimina el archivo existente
                }
                else
                {
                    throw new FileNotFoundException("El archivo no fue encontrado para eliminar.", filePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ocurrió un error al eliminar el archivo: {rutaArchivo}", ex);
            }
        }
        
    }
}
