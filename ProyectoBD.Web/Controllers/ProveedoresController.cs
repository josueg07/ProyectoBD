using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;
using ProyectoBD.Web.Models;

namespace ProyectoBD.Web.Controllers
{
    [Authorize(Roles = "Digitador")]
    public class ProveedoresController : Controller
    {
        private readonly ProveedorNegocio negocio;
        private readonly ILogger<ProveedoresController> logger;

        public ProveedoresController(ProveedorNegocio negocio, ILogger<ProveedoresController> logger)
        {
            this.negocio = negocio;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try { return View(negocio.Listar()); }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al listar proveedores.");
                ViewBag.Error = "No se pudo cargar el listado. Intente nuevamente.";
                return View(new List<Proveedor>());
            }
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View(new ProveedorViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(ProveedorViewModel modelo)
        {
            if (!ModelState.IsValid) return View(modelo);
            try
            {
                Proveedor registro = new Proveedor
                {
                    Nombre_Proveedor = modelo.Nombre_Proveedor
                };
                if (!negocio.Guardar(registro))
                    throw new InvalidOperationException("No se pudo guardar el registro.");
                TempData["Mensaje"] = "Registro creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
            catch (InvalidOperationException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al crear proveedor.");
                ModelState.AddModelError(string.Empty, "No se pudo guardar el registro. Intente nuevamente.");
            }
            return View(modelo);
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            try
            {
                Proveedor? registro = negocio.Listar().FirstOrDefault(x => x.ID_Proveedor == id);
                if (registro == null) return NotFound();
                return View(new ProveedorViewModel
                {
                    ID_Proveedor = registro.ID_Proveedor,
                    Nombre_Proveedor = registro.Nombre_Proveedor
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar proveedor.");
                TempData["Error"] = "No se pudo cargar el registro. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, ProveedorViewModel modelo)
        {
            if (id <= 0 || id != modelo.ID_Proveedor) return BadRequest();
            if (!ModelState.IsValid) return View(modelo);
            try
            {
                if (!negocio.Listar().Any(x => x.ID_Proveedor == id)) return NotFound();
                Proveedor registro = new Proveedor
                {
                    ID_Proveedor = id,
                    Nombre_Proveedor = modelo.Nombre_Proveedor
                };
                if (!negocio.Actualizar(registro)) return NotFound();
                TempData["Mensaje"] = "Registro actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
            catch (InvalidOperationException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al editar proveedor.");
                ModelState.AddModelError(string.Empty, "No se pudo actualizar el registro. Intente nuevamente.");
            }
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            if (id <= 0) return BadRequest();
            try
            {
                if (negocio.Eliminar(id)) TempData["Mensaje"] = "Registro eliminado correctamente.";
                else TempData["Error"] = "El registro no existe o ya fue eliminado.";
            }
            catch (InvalidOperationException)
            {
                TempData["Error"] = "No se puede eliminar porque el registro está siendo utilizado por otras operaciones.";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al eliminar proveedor.");
                TempData["Error"] = "No se pudo eliminar el registro. Intente nuevamente.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
