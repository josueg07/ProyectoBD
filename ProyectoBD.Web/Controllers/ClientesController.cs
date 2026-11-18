using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;
using ProyectoBD.Web.Models;

namespace ProyectoBD.Web.Controllers
{
    [Authorize(Roles = "Digitador")]
    public class ClientesController : Controller
    {
        private readonly ClienteNegocio negocio;
        private readonly ILogger<ClientesController> logger;

        public ClientesController(ClienteNegocio negocio, ILogger<ClientesController> logger)
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
                logger.LogError(ex, "Error al listar clientes.");
                ViewBag.Error = "No se pudo cargar el listado. Intente nuevamente.";
                return View(new List<Cliente>());
            }
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View(new ClienteViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(ClienteViewModel modelo)
        {
            if (!ModelState.IsValid) return View(modelo);
            try
            {
                Cliente registro = new Cliente
                {
                    TipoCliente = modelo.TipoCliente,
                    NombreRazonSocial = modelo.NombreRazonSocial
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
                logger.LogError(ex, "Error al crear cliente.");
                ModelState.AddModelError(string.Empty, "No se pudo guardar el registro. Intente nuevamente.");
            }
            return View(modelo);
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            try
            {
                Cliente? registro = negocio.Listar().FirstOrDefault(x => x.ID_Cliente == id);
                if (registro == null) return NotFound();
                return View(new ClienteViewModel
                {
                    ID_Cliente = registro.ID_Cliente,
                    TipoCliente = registro.TipoCliente,
                    NombreRazonSocial = registro.NombreRazonSocial
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar cliente.");
                TempData["Error"] = "No se pudo cargar el registro. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, ClienteViewModel modelo)
        {
            if (id <= 0 || id != modelo.ID_Cliente) return BadRequest();
            if (!ModelState.IsValid) return View(modelo);
            try
            {
                if (!negocio.Listar().Any(x => x.ID_Cliente == id)) return NotFound();
                Cliente registro = new Cliente
                {
                    ID_Cliente = id,
                    TipoCliente = modelo.TipoCliente,
                    NombreRazonSocial = modelo.NombreRazonSocial
                };
                if (!negocio.Actualizar(registro)) return NotFound();
                TempData["Mensaje"] = "Registro actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
            catch (InvalidOperationException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al editar cliente.");
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
                logger.LogError(ex, "Error al eliminar cliente.");
                TempData["Error"] = "No se pudo eliminar el registro. Intente nuevamente.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
