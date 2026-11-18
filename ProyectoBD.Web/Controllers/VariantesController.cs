using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;
using ProyectoBD.Web.Models;

namespace ProyectoBD.Web.Controllers
{
    [Authorize(Roles = "Digitador")]
    public class VariantesController : Controller
    {
        private readonly ProductoNegocio negocio;
        private readonly PresentacionNegocio presentaciones;
        private readonly ColorProductoNegocio colores;
        private readonly ILogger<VariantesController> logger;

        public VariantesController(ProductoNegocio negocio, PresentacionNegocio presentaciones,
            ColorProductoNegocio colores, ILogger<VariantesController> logger)
        {
            this.negocio = negocio;
            this.presentaciones = presentaciones;
            this.colores = colores;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Index(int? producto)
        {
            ViewBag.Producto = producto;
            ViewBag.Productos = new List<SelectListItem>();
            try
            {
                var productos = negocio.Listar();
                if (producto.HasValue && !productos.Any(p => p.ID_Producto == producto.Value)) return NotFound();
                ViewBag.Productos = new SelectList(productos, "ID_Producto", "NombreProducto", producto);
                var nombresProductos = productos.ToDictionary(p => p.ID_Producto, p => p.NombreProducto);
                var nombresPresentaciones = presentaciones.Listar().ToDictionary(p => p.ID_Presentacion, p => p.NombrePresentacion);
                var nombresColores = colores.Listar().ToDictionary(c => c.ID_Color, c => c.NombreColor);
                var listado = negocio.ListarVariantes(producto).Select(v => new VarianteListadoViewModel
                {
                    ID_ProductoV = v.ID_ProductoV,
                    ID_Producto = v.ID_Producto,
                    Producto = nombresProductos.GetValueOrDefault(v.ID_Producto, "No disponible"),
                    Presentacion = nombresPresentaciones.GetValueOrDefault(v.ID_Presentacion, "No disponible"),
                    Color = v.ID_Color.HasValue ? nombresColores.GetValueOrDefault(v.ID_Color.Value, "No disponible") : "Sin color",
                    PrecioVenta = v.PrecioVenta
                }).ToList();
                return View(listado);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al listar variantes.");
                ViewBag.Error = "No se pudo cargar el listado. Intente nuevamente.";
                return View(new List<VarianteListadoViewModel>());
            }
        }

        [HttpGet]
        public IActionResult Crear(int? producto)
        {
            var modelo = new ProductoVarianteViewModel { ID_Producto = producto ?? 0 };
            CargarCatalogos(modelo, false);
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(ProductoVarianteViewModel modelo)
        {
            CargarCatalogos(modelo, true);
            if (!ModelState.IsValid) return View(modelo);
            try
            {
                ProductoVariante variante = new ProductoVariante
                {
                    ID_Producto = modelo.ID_Producto,
                    ID_Presentacion = modelo.ID_Presentacion,
                    ID_Color = modelo.ID_Color,
                    PrecioVenta = modelo.PrecioVenta
                };
                if (!negocio.GuardarVariante(variante, true))
                    throw new InvalidOperationException("No se pudo guardar la variante.");
                TempData["Mensaje"] = "Variante creada correctamente.";
                return RedirectToAction(nameof(Index), new { producto = modelo.ID_Producto });
            }
            catch (ArgumentException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
            catch (InvalidOperationException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al crear variante.");
                ModelState.AddModelError(string.Empty, "No se pudo guardar la variante. Intente nuevamente.");
            }
            return View(modelo);
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            try
            {
                ProductoVariante? variante = negocio.ListarVariantes().FirstOrDefault(v => v.ID_ProductoV == id);
                if (variante == null) return NotFound();
                var modelo = new ProductoVarianteViewModel
                {
                    ID_ProductoV = variante.ID_ProductoV,
                    ID_Producto = variante.ID_Producto,
                    ID_Presentacion = variante.ID_Presentacion,
                    ID_Color = variante.ID_Color,
                    PrecioVenta = variante.PrecioVenta
                };
                CargarCatalogos(modelo, false);
                return View(modelo);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar variante.");
                TempData["Error"] = "No se pudo cargar la variante. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, ProductoVarianteViewModel modelo)
        {
            if (id <= 0 || id != modelo.ID_ProductoV) return BadRequest();
            CargarCatalogos(modelo, true);
            try
            {
                ProductoVariante? actual = negocio.ListarVariantes().FirstOrDefault(v => v.ID_ProductoV == id);
                if (actual == null) return NotFound();
                if (modelo.ID_Producto != actual.ID_Producto) return BadRequest();
                if (!ModelState.IsValid) return View(modelo);
                ProductoVariante variante = new ProductoVariante
                {
                    ID_ProductoV = id,
                    ID_Producto = actual.ID_Producto,
                    ID_Presentacion = modelo.ID_Presentacion,
                    ID_Color = modelo.ID_Color,
                    PrecioVenta = modelo.PrecioVenta
                };
                if (!negocio.GuardarVariante(variante, false)) return NotFound();
                TempData["Mensaje"] = "Variante actualizada correctamente.";
                return RedirectToAction(nameof(Index), new { producto = actual.ID_Producto });
            }
            catch (ArgumentException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
            catch (InvalidOperationException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al editar variante.");
                ModelState.AddModelError(string.Empty, "No se pudo actualizar la variante. Intente nuevamente.");
            }
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            if (id <= 0) return BadRequest();
            int? producto = null;
            try
            {
                ProductoVariante? variante = negocio.ListarVariantes().FirstOrDefault(v => v.ID_ProductoV == id);
                if (variante == null)
                {
                    TempData["Error"] = "La variante no existe o ya fue eliminada.";
                    return RedirectToAction(nameof(Index));
                }
                producto = variante.ID_Producto;
                if (negocio.EliminarVariante(id)) TempData["Mensaje"] = "Variante eliminada correctamente.";
                else TempData["Error"] = "La variante no existe o ya fue eliminada.";
            }
            catch (InvalidOperationException)
            {
                TempData["Error"] = "No se puede eliminar porque el registro está siendo utilizado por otras operaciones.";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al eliminar variante.");
                TempData["Error"] = "No se pudo eliminar la variante. Intente nuevamente.";
            }
            return RedirectToAction(nameof(Index), new { producto });
        }

        private void CargarCatalogos(ProductoVarianteViewModel modelo, bool validar)
        {
            try
            {
                var productos = negocio.Listar();
                var listaPresentaciones = presentaciones.Listar();
                var listaColores = colores.Listar();
                modelo.Productos = new SelectList(productos, "ID_Producto", "NombreProducto");
                modelo.Presentaciones = new SelectList(listaPresentaciones, "ID_Presentacion", "NombrePresentacion");
                modelo.Colores = new SelectList(listaColores, "ID_Color", "NombreColor");
                if (validar && !productos.Any(p => p.ID_Producto == modelo.ID_Producto))
                    ModelState.AddModelError(nameof(modelo.ID_Producto), "Seleccione un producto disponible.");
                if (validar && !listaPresentaciones.Any(p => p.ID_Presentacion == modelo.ID_Presentacion))
                    ModelState.AddModelError(nameof(modelo.ID_Presentacion), "Seleccione una presentación disponible.");
                if (validar && modelo.ID_Color.HasValue && !listaColores.Any(c => c.ID_Color == modelo.ID_Color.Value))
                    ModelState.AddModelError(nameof(modelo.ID_Color), "Seleccione un color disponible o Sin color.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al cargar catálogos de variantes.");
                ModelState.AddModelError(string.Empty, "No se pudieron cargar los catálogos. Intente nuevamente.");
            }
        }
    }
}
