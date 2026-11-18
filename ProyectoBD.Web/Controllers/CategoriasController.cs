using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;
using ProyectoBD.Web.Models;

namespace ProyectoBD.Web.Controllers
{
    [Authorize(Roles = "Digitador")]
    public class CategoriasController : Controller
    {
        private readonly CategoriaNegocio negocio;
        private readonly ILogger<CategoriasController> logger;

        public CategoriasController(CategoriaNegocio negocio, ILogger<CategoriasController> logger)
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
                logger.LogError(ex, "Error al listar categorías.");
                ViewBag.Error = "No se pudo cargar el listado. Intente nuevamente.";
                return View(new List<Categoria>());
            }
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View(new CategoriaViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(CategoriaViewModel modelo)
        {
            if (!ModelState.IsValid) return View(modelo);
            try
            {
                Categoria registro = new Categoria
                {
                    NombreCategoria = modelo.NombreCategoria,
                    Descripcion = modelo.Descripcion
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
                logger.LogError(ex, "Error al crear categoría.");
                ModelState.AddModelError(string.Empty, "No se pudo guardar el registro. Intente nuevamente.");
            }
            return View(modelo);
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            try
            {
                Categoria? registro = negocio.Listar().FirstOrDefault(x => x.ID_Categoria == id);
                if (registro == null) return NotFound();
                return View(new CategoriaViewModel
                {
                    ID_Categoria = registro.ID_Categoria,
                    NombreCategoria = registro.NombreCategoria,
                    Descripcion = registro.Descripcion
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar categoría.");
                TempData["Error"] = "No se pudo cargar el registro. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, CategoriaViewModel modelo)
        {
            if (id <= 0 || id != modelo.ID_Categoria) return BadRequest();
            if (!ModelState.IsValid) return View(modelo);
            try
            {
                if (!negocio.Listar().Any(x => x.ID_Categoria == id)) return NotFound();
                Categoria registro = new Categoria
                {
                    ID_Categoria = id,
                    NombreCategoria = modelo.NombreCategoria,
                    Descripcion = modelo.Descripcion
                };
                if (!negocio.Actualizar(registro)) return NotFound();
                TempData["Mensaje"] = "Registro actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
            catch (InvalidOperationException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al editar categoría.");
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
                logger.LogError(ex, "Error al eliminar categoría.");
                TempData["Error"] = "No se pudo eliminar el registro. Intente nuevamente.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
