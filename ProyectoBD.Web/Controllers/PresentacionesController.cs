using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;
using ProyectoBD.Web.Models;

namespace ProyectoBD.Web.Controllers
{
    [Authorize(Roles = "Digitador")]
    public class PresentacionesController : Controller
    {
        private readonly PresentacionNegocio negocio;
        private readonly ILogger<PresentacionesController> logger;

        public PresentacionesController(PresentacionNegocio negocio, ILogger<PresentacionesController> logger)
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
                logger.LogError(ex, "Error al listar presentaciones.");
                ViewBag.Error = "No se pudo cargar el listado. Intente nuevamente.";
                return View(new List<Presentacion>());
            }
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View(new PresentacionViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(PresentacionViewModel modelo)
        {
            if (!ModelState.IsValid) return View(modelo);
            try
            {
                Presentacion registro = new Presentacion
                {
                    NombrePresentacion = modelo.NombrePresentacion,
                    UnidadMedida = modelo.UnidadMedida
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
                logger.LogError(ex, "Error al crear presentación.");
                ModelState.AddModelError(string.Empty, "No se pudo guardar el registro. Intente nuevamente.");
            }
            return View(modelo);
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            try
            {
                Presentacion? registro = negocio.Listar().FirstOrDefault(x => x.ID_Presentacion == id);
                if (registro == null) return NotFound();
                return View(new PresentacionViewModel
                {
                    ID_Presentacion = registro.ID_Presentacion,
                    NombrePresentacion = registro.NombrePresentacion,
                    UnidadMedida = registro.UnidadMedida
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar presentación.");
                TempData["Error"] = "No se pudo cargar el registro. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, PresentacionViewModel modelo)
        {
            if (id <= 0 || id != modelo.ID_Presentacion) return BadRequest();
            if (!ModelState.IsValid) return View(modelo);
            try
            {
                if (!negocio.Listar().Any(x => x.ID_Presentacion == id)) return NotFound();
                Presentacion registro = new Presentacion
                {
                    ID_Presentacion = id,
                    NombrePresentacion = modelo.NombrePresentacion,
                    UnidadMedida = modelo.UnidadMedida
                };
                if (!negocio.Actualizar(registro)) return NotFound();
                TempData["Mensaje"] = "Registro actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
            catch (InvalidOperationException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al editar presentación.");
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
                logger.LogError(ex, "Error al eliminar presentación.");
                TempData["Error"] = "No se pudo eliminar el registro. Intente nuevamente.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
