﻿using Microsoft.AspNetCore.Authorization;
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
            var tipoUsuario = User.FindFirstValue("Id_011_TipoUsuario");
            if (tipoUsuario == null || tipoUsuario != "407")
            {
                return RedirectToAction("Login", "Login");
            }
            UsuarioFiltroViewModel index = new UsuarioFiltroViewModel();
            index.ListaUsuarios = await _usuarioService.Usuario_Bandeja(index);
            return View(index);
        }
        [HttpPost]
        public async Task<IActionResult> Index(UsuarioFiltroViewModel index)
        {
            var idUsuarioStr = User.FindFirst("Id_Usuario")?.Value ?? "0";
            var Id_Usuario = int.Parse(idUsuarioStr);

            if (Id_Usuario == 0)
            {
                return RedirectToAction("Login", "Account");
            }
            index.ListaUsuarios = await _usuarioService.Usuario_Bandeja(index);
            return View(index);
        }
        public IActionResult GuardarUsuarioSeleccionado(int id_Usuario)
        {
            // Opción 1: Usando TempData
            TempData["UsuarioSeleccionado"] = id_Usuario;
            return RedirectToAction("Usuario", "Usuario");
        }
        [HttpGet]
        public async Task<IActionResult> Usuario()
        {
            var tipoUsuario = User.FindFirstValue("Id_011_TipoUsuario");
            if (tipoUsuario == null || tipoUsuario != "407")
            {
                return RedirectToAction("Login", "Login");
            }

            int Id_Usuario = TempData["UsuarioSeleccionado"] as int? ?? 0;
            UsuarioViewModel usuario = new UsuarioViewModel();
            
            if (Id_Usuario > 0)
            {
                usuario = await _usuarioService.Usuario_BuscarId_Usuario(Id_Usuario);
            }
            usuario.TipoUsuarioList = await _tiposService.ParametroTipo_Listar(11); 
            usuario.TipoDocumentoList = await _tiposService.ParametroTipo_Listar(1);
            return View(usuario);
        }
        [HttpPost]
        public async Task<IActionResult> Usuario(UsuarioViewModel usuario)
        {
            var idUsuarioStr = User.FindFirst("Id_Usuario")?.Value ?? "0";
            var Id_Usuario = int.Parse(idUsuarioStr);

            if (Id_Usuario == 0)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(usuario);
        }
        [HttpGet]
        public async Task<IActionResult> ValidarPersona(int idTipoDocumento, string documento)
        {
            var persona = await _usuarioService.Usuario_ValidarPersona(idTipoDocumento, documento);

            return Json(persona);
        }

    }
}
