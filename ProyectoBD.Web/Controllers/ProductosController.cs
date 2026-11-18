using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;
using ProyectoBD.Web.Models;

namespace ProyectoBD.Web.Controllers
{
    [Authorize(Roles = "Digitador")]
    public class ProductosController : Controller
    {
        private readonly ProductoNegocio negocio;
        private readonly CategoriaNegocio categorias;
        private readonly MarcaNegocio marcas;
        private readonly ILogger<ProductosController> logger;

        public ProductosController(ProductoNegocio negocio, CategoriaNegocio categorias,
            MarcaNegocio marcas, ILogger<ProductosController> logger)
        {
            this.negocio = negocio;
            this.categorias = categorias;
            this.marcas = marcas;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                var nombresCategorias = categorias.Listar().ToDictionary(c => c.ID_Categoria, c => c.NombreCategoria);
                var nombresMarcas = marcas.Listar().ToDictionary(m => m.ID_Marca, m => m.NombreMarca);
                var listado = negocio.Listar().Select(p => new ProductoListadoViewModel
                {
                    ID_Producto = p.ID_Producto,
                    NombreProducto = p.NombreProducto,
                    Descripcion = p.Descripcion,
                    Categoria = nombresCategorias.GetValueOrDefault(p.ID_CategoriaP, "No disponible"),
                    Marca = nombresMarcas.GetValueOrDefault(p.ID_Marca, "No disponible")
                }).ToList();
                return View(listado);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al listar productos.");
                ViewBag.Error = "No se pudo cargar el listado. Intente nuevamente.";
                return View(new List<ProductoListadoViewModel>());
            }
        }

        [HttpGet]
        public IActionResult Crear()
        {
            var modelo = new ProductoViewModel();
            CargarCatalogos(modelo, false);
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(ProductoViewModel modelo)
        {
            CargarCatalogos(modelo, true);
            if (!ModelState.IsValid) return View(modelo);
            try
            {
                Producto producto = new Producto
                {
                    NombreProducto = modelo.NombreProducto,
                    Descripcion = modelo.Descripcion,
                    ID_CategoriaP = modelo.ID_CategoriaP,
                    ID_Marca = modelo.ID_Marca
                };
                if (!negocio.Guardar(producto, true))
                    throw new InvalidOperationException("No se pudo guardar el producto.");
                TempData["Mensaje"] = "Producto creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
            catch (InvalidOperationException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al crear producto.");
                ModelState.AddModelError(string.Empty, "No se pudo guardar el producto. Intente nuevamente.");
            }
            return View(modelo);
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            try
            {
                Producto? producto = negocio.Listar().FirstOrDefault(p => p.ID_Producto == id);
                if (producto == null) return NotFound();
                var modelo = new ProductoViewModel
                {
                    ID_Producto = producto.ID_Producto,
                    NombreProducto = producto.NombreProducto,
                    Descripcion = producto.Descripcion,
                    ID_CategoriaP = producto.ID_CategoriaP,
                    ID_Marca = producto.ID_Marca
                };
                CargarCatalogos(modelo, false);
                return View(modelo);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar producto.");
                TempData["Error"] = "No se pudo cargar el producto. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, ProductoViewModel modelo)
        {
            if (id <= 0 || id != modelo.ID_Producto) return BadRequest();
            CargarCatalogos(modelo, true);
            if (!ModelState.IsValid) return View(modelo);
            try
            {
                if (!negocio.Listar().Any(p => p.ID_Producto == id)) return NotFound();
                Producto producto = new Producto
                {
                    ID_Producto = id,
                    NombreProducto = modelo.NombreProducto,
                    Descripcion = modelo.Descripcion,
                    ID_CategoriaP = modelo.ID_CategoriaP,
                    ID_Marca = modelo.ID_Marca
                };
                if (!negocio.Guardar(producto, false)) return NotFound();
                TempData["Mensaje"] = "Producto actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
            catch (InvalidOperationException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al editar producto.");
                ModelState.AddModelError(string.Empty, "No se pudo actualizar el producto. Intente nuevamente.");
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
                if (negocio.Eliminar(id)) TempData["Mensaje"] = "Producto eliminado correctamente.";
                else TempData["Error"] = "El producto no existe o ya fue eliminado.";
            }
            catch (InvalidOperationException)
            {
                TempData["Error"] = "No se puede eliminar porque el registro está siendo utilizado por otras operaciones.";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al eliminar producto.");
                TempData["Error"] = "No se pudo eliminar el producto. Intente nuevamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        private void CargarCatalogos(ProductoViewModel modelo, bool validar)
        {
            try
            {
                var listaCategorias = categorias.Listar();
                var listaMarcas = marcas.Listar();
                modelo.Categorias = new SelectList(listaCategorias, "ID_Categoria", "NombreCategoria");
                modelo.Marcas = new SelectList(listaMarcas, "ID_Marca", "NombreMarca");
                if (validar && !listaCategorias.Any(c => c.ID_Categoria == modelo.ID_CategoriaP))
                    ModelState.AddModelError(nameof(modelo.ID_CategoriaP), "Seleccione una categoría disponible.");
                if (validar && !listaMarcas.Any(m => m.ID_Marca == modelo.ID_Marca))
                    ModelState.AddModelError(nameof(modelo.ID_Marca), "Seleccione una marca disponible.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al cargar categorías y marcas.");
                ModelState.AddModelError(string.Empty, "No se pudieron cargar las categorías y marcas. Intente nuevamente.");
            }
        }
    }
}
