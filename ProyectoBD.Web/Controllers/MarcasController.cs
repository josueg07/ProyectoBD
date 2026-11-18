using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;
using ProyectoBD.Web.Models;

namespace ProyectoBD.Web.Controllers
{
    [Authorize(Roles = "Digitador")]
    public class MarcasController : Controller
    {
        private readonly MarcaNegocio negocio;
        private readonly ILogger<MarcasController> logger;

        public MarcasController(MarcaNegocio negocio, ILogger<MarcasController> logger)
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
                logger.LogError(ex, "Error al listar marcas.");
                ViewBag.Error = "No se pudo cargar el listado. Intente nuevamente.";
                return View(new List<Marca>());
            }
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View(new MarcaViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(MarcaViewModel modelo)
        {
            if (!ModelState.IsValid) return View(modelo);
            try
            {
                Marca registro = new Marca
                {
                    NombreMarca = modelo.NombreMarca
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
                logger.LogError(ex, "Error al crear marca.");
                ModelState.AddModelError(string.Empty, "No se pudo guardar el registro. Intente nuevamente.");
            }
            return View(modelo);
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            try
            {
                Marca? registro = negocio.Listar().FirstOrDefault(x => x.ID_Marca == id);
                if (registro == null) return NotFound();
                return View(new MarcaViewModel
                {
                    ID_Marca = registro.ID_Marca,
                    NombreMarca = registro.NombreMarca
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar marca.");
                TempData["Error"] = "No se pudo cargar el registro. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, MarcaViewModel modelo)
        {
            if (id <= 0 || id != modelo.ID_Marca) return BadRequest();
            if (!ModelState.IsValid) return View(modelo);
            try
            {
                if (!negocio.Listar().Any(x => x.ID_Marca == id)) return NotFound();
                Marca registro = new Marca
                {
                    ID_Marca = id,
                    NombreMarca = modelo.NombreMarca
                };
                if (!negocio.Actualizar(registro)) return NotFound();
                TempData["Mensaje"] = "Registro actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
            catch (InvalidOperationException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al editar marca.");
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
                logger.LogError(ex, "Error al eliminar marca.");
                TempData["Error"] = "No se pudo eliminar el registro. Intente nuevamente.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
