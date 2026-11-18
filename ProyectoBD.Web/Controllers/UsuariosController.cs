using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoBD.Negocio;
using ProyectoBD.Entidades;
using ProyectoBD.Web.Helpers;
using ProyectoBD.Web.Models;

namespace ProyectoBD.Web.Controllers
{
    [Authorize(Roles = "Gerente")]
    public class UsuariosController : Controller
    {
        private readonly UsuarioNegocio usuarioNegocio;
        private readonly RolNegocio rolNegocio;

        public UsuariosController(
            UsuarioNegocio usuarioNegocio,
            RolNegocio rolNegocio)
        {
            this.usuarioNegocio = usuarioNegocio;
            this.rolNegocio = rolNegocio;
        }

        public IActionResult Index()
        {
            try
            {
                UsuarioSesion sesion = User.ObtenerSesion();

                List<Usuario> usuarios =
                    usuarioNegocio.Listar(sesion);

                return View(usuarios);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;

                return View(new List<Usuario>());
            }
        }

        [HttpGet]
        public IActionResult Crear()
        {
            CargarRoles();

            return View(new UsuarioCrearViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(UsuarioCrearViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                CargarRoles();
                return View(modelo);
            }

            try
            {
                UsuarioSesion sesion = User.ObtenerSesion();

                Usuario usuario = new Usuario
                {
                    Nombres = modelo.Nombres,
                    Apellidos = modelo.Apellidos,
                    NombreUsuario = modelo.NombreUsuario,
                    ID_Rol = modelo.ID_Rol
                };

                usuarioNegocio.Guardar(
                    usuario,
                    modelo.Password,
                    sesion,
                    true
                );

                TempData["Mensaje"] =
                    "Usuario creado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);

                ModelState.AddModelError(
                    string.Empty,
                    "No se pudo crear el usuario. " +
                    "Verifique que el nombre de usuario no esté repetido."
                );
            }

            CargarRoles();

            return View(modelo);
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            try
            {
                UsuarioSesion sesion = User.ObtenerSesion();

                Usuario? usuario = usuarioNegocio
                    .Listar(sesion)
                    .FirstOrDefault(u => u.ID_Usuario == id);

                if (usuario == null)
                {
                    return NotFound();
                }

                UsuarioEditarViewModel modelo =
                    new UsuarioEditarViewModel
                    {
                        ID_Usuario = usuario.ID_Usuario,
                        Nombres = usuario.Nombres,
                        Apellidos = usuario.Apellidos,
                        NombreUsuario = usuario.NombreUsuario,
                        ID_Rol = usuario.ID_Rol
                    };

                CargarRoles();

                return View(modelo);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return RedirectToAction(nameof(Index));
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(UsuarioEditarViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                CargarRoles();

                return View(modelo);
            }

            try
            {
                UsuarioSesion sesion = User.ObtenerSesion();

                Usuario usuario = new Usuario
                {
                    ID_Usuario = modelo.ID_Usuario,
                    Nombres = modelo.Nombres,
                    Apellidos = modelo.Apellidos,
                    NombreUsuario = modelo.NombreUsuario,
                    ID_Rol = modelo.ID_Rol
                };

                usuarioNegocio.Guardar(
                    usuario,
                    string.Empty,
                    sesion,
                    false
                );

                TempData["Mensaje"] =
                    "Usuario actualizado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);

                ModelState.AddModelError(
                    string.Empty,
                    "No se pudo actualizar el usuario. " +
                    "Verifique que el nombre de usuario no esté repetido."
                );
            }

            CargarRoles();

            return View(modelo);
        }

        [HttpGet]
        public IActionResult CambiarPassword(int id)
        {
            try
            {
                UsuarioSesion sesion = User.ObtenerSesion();

                Usuario? usuario = usuarioNegocio
                    .Listar(sesion)
                    .FirstOrDefault(u => u.ID_Usuario == id);

                if (usuario == null)
                {
                    return NotFound();
                }

                CambiarPasswordViewModel modelo =
                    new CambiarPasswordViewModel
                    {
                        ID_Usuario = usuario.ID_Usuario,
                        NombreUsuario = usuario.NombreUsuario
                    };

                return View(modelo);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return RedirectToAction(nameof(Index));
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarPassword(
            CambiarPasswordViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            try
            {
                UsuarioSesion sesion = User.ObtenerSesion();

                usuarioNegocio.CambiarPassword(
                    modelo.ID_Usuario,
                    modelo.NuevaPassword,
                    sesion
                );

                TempData["Mensaje"] =
                    "Contraseña actualizada correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);

                ModelState.AddModelError(
                    string.Empty,
                    "No se pudo cambiar la contraseña."
                );
            }

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            try
            {
                UsuarioSesion sesion = User.ObtenerSesion();

                usuarioNegocio.Eliminar(
                    id,
                    sesion
                );

                TempData["Mensaje"] =
                    "Usuario eliminado correctamente.";
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);

                TempData["Error"] =
                    "No se pudo eliminar el usuario. " +
                    "Es posible que tenga registros relacionados en el sistema.";
            }

            return RedirectToAction(nameof(Index));
        }
        private void CargarRoles()
        {
            ViewBag.Roles = new SelectList(
                rolNegocio.ListarRoles(),
                "ID_Rol",
                "NombreRol"
            );
        }
    }
}
