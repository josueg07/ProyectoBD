using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;
using ProyectoBD.Web.Models;

namespace ProyectoBD.Web.Controllers
{
    [Authorize(Roles = "Cajero")]
    public class MediosPagoController : Controller
    {
        private readonly MetodoPagoNegocio negocio;
        private readonly ILogger<MediosPagoController> logger;

        public MediosPagoController(MetodoPagoNegocio negocio, ILogger<MediosPagoController> logger)
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
                logger.LogError(ex, "Error al listar medio de pagos.");
                ViewBag.Error = "No se pudo cargar el listado. Intente nuevamente.";
                return View(new List<MetodoPago>());
            }
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View(new MedioPagoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(MedioPagoViewModel modelo)
        {
            if (!ModelState.IsValid) return View(modelo);
            try
            {
                MetodoPago registro = new MetodoPago
                {
                    NombreMetodo = modelo.NombreMetodo
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
                logger.LogError(ex, "Error al crear medio de pago.");
                ModelState.AddModelError(string.Empty, "No se pudo guardar el registro. Intente nuevamente.");
            }
            return View(modelo);
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            try
            {
                MetodoPago? registro = negocio.Listar().FirstOrDefault(x => x.ID_MetodoP == id);
                if (registro == null) return NotFound();
                return View(new MedioPagoViewModel
                {
                    ID_MetodoP = registro.ID_MetodoP,
                    NombreMetodo = registro.NombreMetodo
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar medio de pago.");
                TempData["Error"] = "No se pudo cargar el registro. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, MedioPagoViewModel modelo)
        {
            if (id <= 0 || id != modelo.ID_MetodoP) return BadRequest();
            if (!ModelState.IsValid) return View(modelo);
            try
            {
                if (!negocio.Listar().Any(x => x.ID_MetodoP == id)) return NotFound();
                MetodoPago registro = new MetodoPago
                {
                    ID_MetodoP = id,
                    NombreMetodo = modelo.NombreMetodo
                };
                if (!negocio.Actualizar(registro)) return NotFound();
                TempData["Mensaje"] = "Registro actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
            catch (InvalidOperationException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al editar medio de pago.");
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
                logger.LogError(ex, "Error al eliminar medio de pago.");
                TempData["Error"] = "No se pudo eliminar el registro. Intente nuevamente.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

