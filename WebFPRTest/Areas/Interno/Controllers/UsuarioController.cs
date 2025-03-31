using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using WebFPRTest.Areas.Interno.Interface.Usuario;
using WebFPRTest.Areas.Interno.Models.Usuario;
using WebFPRTest.Areas.Interno.Service.Usuario;
using WebFPRTest.Interface;
using WebFPRTest.Result;
using WebFPRTest.Service;

namespace WebFPRTest.Areas.Interno.Controllers
{
    [Area("Interno")]
    [Authorize]
    public class UsuarioController:Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ITiposService _tiposService;
        public UsuarioController(IUsuarioService usuarioService, ITiposService tiposService)
        { 
            _usuarioService = usuarioService;
            _tiposService = tiposService;
        }

        [HttpGet]
        public async Task<IActionResult> Index() 
        {
            int Id_Controlador = 10;
            int tipoUsuario = int.TryParse(User.FindFirstValue("Id_011_TipoUsuario"), out int result) ? result : 0;

            var acceso = await _tiposService.ControladorTipoUsuario(Id_Controlador, tipoUsuario);

            if (!acceso)
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }
            //var tipoUsuario = User.FindFirstValue("Id_011_TipoUsuario");
            //if (tipoUsuario == null || (tipoUsuario != "406" && tipoUsuario != "407" && tipoUsuario != "408"))
            //{
            //    return RedirectToAction("Login", "Login");
            //}
            UsuarioFiltroViewModel indexView = new UsuarioFiltroViewModel();
            indexView.ListaUsuarios = await _usuarioService.Usuario_Bandeja(indexView);
            return View(indexView);
        }
        [HttpPost]
        public async Task<IActionResult> Index(UsuarioFiltroViewModel indexView)
        {
            var idUsuarioStr = User.FindFirst("Id_Usuario")?.Value ?? "0";
            var Id_Usuario = int.Parse(idUsuarioStr);

            if (Id_Usuario == 0)
            {
                return RedirectToAction("Login", "Account");
            }
            indexView.ListaUsuarios = await _usuarioService.Usuario_Bandeja(indexView);
            return View(indexView);
        }
        public IActionResult GuardarUsuarioSeleccionado(int Id_Usuario, int Id_Persona)
        {
            var tipoUsuario = User.FindFirstValue("Id_011_TipoUsuario");

            if (tipoUsuario == null || (tipoUsuario != "406" && tipoUsuario != "407"))
            {
                TempData["Mensaje"] = "Usted no tiene permisos para entrar a esta ventana.";
                return RedirectToAction("Index", "Usuario", new { area = "Interno" });
            }

            // Guardar en TempData el usuario seleccionado
            TempData["Id_Usuario"] = Id_Usuario;
            TempData["Id_Persona"] = Id_Persona;
            return RedirectToAction("Usuario", "Usuario", new { area = "Interno" });
        }
        [HttpGet]
        public async Task<IActionResult> Usuario_Eliminar(int Id_Usuario)
        {
            int Id_Controlador = 11;
            int tipoUsuario = int.TryParse(User.FindFirstValue("Id_011_TipoUsuario"), out int result) ? result : 0;
            var acceso = await _tiposService.ControladorTipoUsuario(Id_Controlador, tipoUsuario);
            if (!acceso)
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }
            await _usuarioService.Usuario_Eliminar(Id_Usuario);
            TempData["Mensaje"] = "Usuario Eliminado con éxito";
            return RedirectToAction("Index", "Usuario", new { area = "Interno" });
        }
        [HttpGet]
        public async Task<IActionResult> Usuario()
        {
            int Id_Controlador = 12;
            int tipoUsuario = int.TryParse(User.FindFirstValue("Id_011_TipoUsuario"), out int result) ? result : 0;
            var acceso = await _tiposService.ControladorTipoUsuario(Id_Controlador, tipoUsuario);
            if (!acceso)
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }
            UsuarioViewModel usuario = new UsuarioViewModel();
            usuario.Id_Usuario = TempData.Peek("Id_Usuario") as int? ?? 0;
            usuario.Id_Persona = TempData.Peek("Id_Persona") as int? ?? 0;
            if (usuario.Id_Usuario > 0)
            {
                usuario = await _usuarioService.Usuario_BuscarId_Usuario(usuario.Id_Usuario);
            }
            usuario.TipoUsuarioList = await _tiposService.ParametroTipo_Listar(11); 
            usuario.TipoDocumentoList = await _tiposService.ParametroTipo_Listar(1);
            usuario.EquiposList = await _tiposService.Equipo_Listar();
            return View(usuario);
        }
        [HttpPost]
        public async Task<IActionResult> Usuario(UsuarioViewModel usuarioView)
        {
            var idUsuarioStr = User.FindFirst("Id_Usuario")?.Value ?? "0";
            var IdUsuario = int.Parse(idUsuarioStr);
            if (IdUsuario == 0)
            {
                return RedirectToAction("Login", "Login");
            }

            int existe = await _usuarioService.Usuario_Existe(usuarioView.Usuario, usuarioView.Id_Persona);
            if (existe == 0)
            {
                if (usuarioView.ClaveConfirmacion == null)
                {
                    TempData["Mensaje"] = "Debe colocar una clave";
                    return RedirectToAction("GuardarUsuarioSeleccionado", "Usuario", new { area = "Interno", Id_Usuario = usuarioView.Id_Usuario });
                }
                else
                {
                    if (usuarioView.Id_Persona == 0)
                    {
                        usuarioView.Id_Persona = await _usuarioService.Persona_Insertar(usuarioView, IdUsuario);
                    }
                    else
                    {
                        await _usuarioService.Persona_Actualizar(usuarioView, IdUsuario);
                    }
                    usuarioView.ClaveHash = usuarioView.ClaveConfirmacion;
                    usuarioView.Id_Usuario = await _usuarioService.Usuario_Insertar(usuarioView, IdUsuario);
                    TempData["Mensaje"] = "Usuario registrado con éxito";
                    return RedirectToAction("GuardarUsuarioSeleccionado", "Usuario", new { area = "Interno", Id_Usuario = usuarioView.Id_Usuario });
                }
            }
            else if (existe == 1)
            {
                TempData["Mensaje"] = "El nombre del Usuario ya está siendo utilizado por otro usuario";
            }
            else if (existe == 2)
            {
                await _usuarioService.Persona_Actualizar(usuarioView, IdUsuario);
                await _usuarioService.Usuario_Actualizar(usuarioView, IdUsuario);
                TempData["Mensaje"] = "Usuario actualizado con éxito";
            }
            return RedirectToAction("GuardarUsuarioSeleccionado", "Usuario", new { area = "Interno" ,Id_Usuario = usuarioView.Id_Usuario});
        }
        [HttpGet]
        public async Task<IActionResult> ValidarPersona(int idTipoDocumento, string documento)
        {
            var persona = await _usuarioService.Usuario_ValidarPersona(idTipoDocumento, documento);
            return Json(persona);
        }

    }
}
