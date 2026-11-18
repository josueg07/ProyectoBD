using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;
using ProyectoBD.Web.Models;

namespace ProyectoBD.Web.Controllers
{
    [Authorize(Roles = "Gerente")]
    public class RolesController : Controller
    {
        private readonly RolNegocio rolNegocio;

        public RolesController(RolNegocio rolNegocio)
        {
            this.rolNegocio = rolNegocio;
        }


        public IActionResult Index()
        {
            try
            {
                List<Rol> roles =
                    rolNegocio.ListarRoles();

                return View(roles);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;

                return View(new List<Rol>());
            }
        }


        [HttpGet]
        public IActionResult Crear()
        {
            return View(new RolViewModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(RolViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            try
            {
                Rol rol = new Rol
                {
                    NombreRol = modelo.NombreRol
                };

                bool guardado =
                    rolNegocio.Guardar(rol);

                if (!guardado)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "No se pudo guardar el rol."
                    );

                    return View(modelo);
                }

                TempData["Mensaje"] =
                    "Rol creado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message
                );

                return View(modelo);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);

                ModelState.AddModelError(
                    string.Empty,
                    "No se pudo crear el rol."
                );

                return View(modelo);
            }
        }


        [HttpGet]
        public IActionResult Editar(int id)
        {
            try
            {
                Rol? rol = rolNegocio
                    .ListarRoles()
                    .FirstOrDefault(r => r.ID_Rol == id);

                if (rol == null)
                {
                    return NotFound();
                }

                if (EsRolBase(rol.NombreRol))
                {
                    TempData["Error"] =
                        "Los roles Digitador, Cajero y Gerente " +
                        "son roles base del sistema y no pueden renombrarse.";

                    return RedirectToAction(nameof(Index));
                }

                RolViewModel modelo =
                    new RolViewModel
                    {
                        ID_Rol = rol.ID_Rol,
                        NombreRol = rol.NombreRol
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
        public IActionResult Editar(RolViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            try
            {
                Rol? actual = rolNegocio
                    .ListarRoles()
                    .FirstOrDefault(
                        r => r.ID_Rol == modelo.ID_Rol
                    );

                if (actual == null)
                {
                    return NotFound();
                }

                if (EsRolBase(actual.NombreRol))
                {
                    TempData["Error"] =
                        "Los roles base del sistema no pueden renombrarse.";

                    return RedirectToAction(nameof(Index));
                }

                Rol rol = new Rol
                {
                    ID_Rol = modelo.ID_Rol,
                    NombreRol = modelo.NombreRol
                };

                bool actualizado =
                    rolNegocio.Actualizar(rol);

                if (!actualizado)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "No se pudo actualizar el rol."
                    );

                    return View(modelo);
                }

                TempData["Mensaje"] =
                    "Rol actualizado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message
                );

                return View(modelo);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);

                ModelState.AddModelError(
                    string.Empty,
                    "No se pudo actualizar el rol."
                );

                return View(modelo);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            try
            {
                Rol? rol = rolNegocio
                    .ListarRoles()
                    .FirstOrDefault(r => r.ID_Rol == id);

                if (rol == null)
                {
                    TempData["Error"] =
                        "El rol no existe.";

                    return RedirectToAction(nameof(Index));
                }

                if (EsRolBase(rol.NombreRol))
                {
                    TempData["Error"] =
                        "Los roles Digitador, Cajero y Gerente " +
                        "son obligatorios y no pueden eliminarse.";

                    return RedirectToAction(nameof(Index));
                }

                bool eliminado =
                    rolNegocio.Eliminar(id);

                if (eliminado)
                {
                    TempData["Mensaje"] =
                        "Rol eliminado correctamente.";
                }
                else
                {
                    TempData["Error"] =
                        "No se pudo eliminar el rol.";
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] =
                    ex.Message;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);

                TempData["Error"] =
                    "No se pudo eliminar el rol.";
            }

            return RedirectToAction(nameof(Index));
        }


        private static bool EsRolBase(string nombreRol)
        {
            return
                nombreRol.Equals(
                    "Digitador",
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                nombreRol.Equals(
                    "Cajero",
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                nombreRol.Equals(
                    "Gerente",
                    StringComparison.OrdinalIgnoreCase
                );
        }
    }
}
