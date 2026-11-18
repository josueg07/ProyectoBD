using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;
using ProyectoBD.Web.Models;
using System.Security.Claims;

namespace ProyectoBD.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UsuarioNegocio _usuarioNegocio;

        public AccountController(UsuarioNegocio usuarioNegocio)
        {
            _usuarioNegocio = usuarioNegocio;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            UsuarioSesion? sesion =
                _usuarioNegocio.IniciarSesion(
                    modelo.Usuario,
                    modelo.Password
                );

            if (sesion == null)
            {
                ModelState.AddModelError(
                    "",
                    "Usuario o contraseña incorrectos."
                );

                return View(modelo);
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    sesion.ID_Usuario.ToString()
                ),

                new Claim(
                    ClaimTypes.Name,
                    sesion.NombreCompleto
                ),

                new Claim(
                    ClaimTypes.Role,
                    sesion.NombreRol
                ),

                new Claim(
                    "NombreUsuario",
                    sesion.NombreUsuario
                ),

                new Claim(
                    "ID_Rol",
                    sesion.ID_Rol.ToString()
                )
            };

            ClaimsIdentity identidad =
                new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );

            ClaimsPrincipal principal =
                new ClaimsPrincipal(identidad);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            return RedirectToAction(
                "Index",
                "Home"
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return RedirectToAction("Login");
        }

        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }
}
