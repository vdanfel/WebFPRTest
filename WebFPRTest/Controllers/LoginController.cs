using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebFPRTest.Models;
using WebFPRTest.Interface;

namespace WebFPRTest.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILoginService _loginService;
        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string mensaje)
        {
            await HttpContext.SignOutAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            string mensaje = "";
            var usuario = await _loginService.ValidarLogin(login);

            if (usuario != null)
            {
                // Crear la identidad del usuario con los Claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Usuario), // Nombre del usuario
                    new Claim("Id_Usuario", usuario.Id_Usuario.ToString()), // ID del usuario
                    new Claim("Id_011_TipoUsuario", usuario.Id_011_TipoUsuario.ToString()), // Tipo de usuario
                    new Claim("Id_Equipo",usuario.Id_Equipo.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, // Mantener sesión
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30) // Expira en 30 minutos
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                // Redirección según el tipo de usuario
                if (usuario.Id_011_TipoUsuario == 409)
                {
                    return RedirectToAction("Equipo", "Equipo", new { area = "Externo" });
                }
                else if (usuario.Id_011_TipoUsuario == 406 || usuario.Id_011_TipoUsuario == 407 || usuario.Id_011_TipoUsuario == 408)
                {
                    return RedirectToAction("ListJugadores", "ListJugadores", new { area = "Interno" });
                }
            }

            mensaje = "Usuario o Clave incorrecto";
            ViewBag.Mensaje = mensaje;
            return View(login);
        }
    }
}
