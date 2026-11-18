using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;
using ProyectoBD.Web.Helpers;
using ProyectoBD.Web.Models;

namespace ProyectoBD.Web.Controllers;

[Authorize(Roles = "Digitador")]
public class CotizacionesController : Controller
{
    private readonly ClienteNegocio clientes;
    private readonly ProductoNegocio productos;
    private readonly ILogger<CotizacionesController> logger;
    public CotizacionesController(ClienteNegocio clientes, ProductoNegocio productos, ILogger<CotizacionesController> logger)
    {
        this.clientes = clientes;
        this.productos = productos;
        this.logger = logger;
    }
    private ProcesoNegocio Proceso()
    {
        UsuarioSesion sesion = User.ObtenerSesion();
        return new ProcesoNegocio(sesion);
    }
    private List<CotizacionResumenViewModel> Listar()
    {
        return Proceso().Cotizaciones().AsEnumerable().Select(r => new CotizacionResumenViewModel
        {
            ID_Cotizacion = r.Field<int>("ID_Cotizacion"), Fecha = r.Field<DateTime>("FechaCotizacion"),
            Cliente = r.Field<string>("NombreRazonSocial")!, Estado = r.Field<string>("Estado")!, Total = r.Field<decimal>("Total")
        }).ToList();
    }
    [HttpGet]
    public IActionResult Index()
    {
        try { return View(Listar()); }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al consultar cotizaciones.");
            ViewBag.Error = "No se pudieron cargar las cotizaciones. Intente nuevamente.";
            return View(new List<CotizacionResumenViewModel>());
        }
    }
    [HttpGet]
    public IActionResult Detalle(int id)
    {
        try
        {
            var modelo = Listar().FirstOrDefault(c => c.ID_Cotizacion == id);
            if (modelo == null) return NotFound();
            modelo.Detalle = Proceso().DetalleCotizacion(id);
            return View(modelo);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al consultar cotización.");
            TempData["Error"] = "No se pudo consultar la cotización. Intente nuevamente.";
            return RedirectToAction(nameof(Index));
        }
    }
    [HttpGet]
    public IActionResult Crear()
    {
        var modelo = new CotizacionCrearViewModel { Detalle = new() { new() } };
        Cargar(modelo, false);
        return View(modelo);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(CotizacionCrearViewModel modelo, string accion = "guardar")
    {
        modelo.Detalle ??= new();
        Cargar(modelo, false);
        if (accion == "agregar")
        {
            if (modelo.Detalle.Count < modelo.Variantes.Count) modelo.Detalle.Add(new());
            ModelState.Clear();
            return View(modelo);
        }
        if (accion.StartsWith("quitar:") && int.TryParse(accion[7..], out int indice))
        {
            if (indice < 0 || indice >= modelo.Detalle.Count) return BadRequest();
            modelo.Detalle.RemoveAt(indice);
            ModelState.Clear();
            return View(modelo);
        }
        if (accion != "guardar") return BadRequest();
        Cargar(modelo, true);
        if (!ModelState.IsValid) return View(modelo);
        try
        {
            var cotizacion = new Cotizacion
            {
                ID_Cliente = modelo.ID_Cliente,
                Detalle = modelo.Detalle.Select(l => new LineaDocumento
                { ID_ProductoV = l.ID_ProductoV, Cantidad = l.Cantidad, PrecioUnitario = l.PrecioUnitario }).ToList()
            };
            int id = Proceso().GuardarCotizacion(cotizacion);
            TempData["Mensaje"] = "Cotización registrada como Pendiente.";
            return RedirectToAction(nameof(Detalle), new { id });
        }
        catch (ArgumentException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
        catch (InvalidOperationException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al guardar cotización.");
            ModelState.AddModelError(string.Empty, "No se pudo guardar la cotización. Revise los datos e intente nuevamente.");
        }
        return View(modelo);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Confirmar(int id) => CambiarEstado(id, "Confirmada");
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Cancelar(int id) => CambiarEstado(id, "Cancelada");
    private IActionResult CambiarEstado(int id, string estado)
    {
        if (id <= 0) return BadRequest();
        try
        {
            if (!Listar().Any(c => c.ID_Cotizacion == id)) return NotFound();
            Proceso().EstadoCotizacion(id, estado);
            TempData["Mensaje"] = "Estado de la cotización: " + estado + ".";
        }
        catch (ArgumentException ex) { TempData["Error"] = ex.Message; }
        catch (InvalidOperationException ex) { TempData["Error"] = ex.Message; }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al cambiar estado de cotización.");
            TempData["Error"] = "No se pudo cambiar el estado de la cotización.";
        }
        return RedirectToAction(nameof(Detalle), new { id });
    }
    private void Cargar(CotizacionCrearViewModel modelo, bool validar)
    {
        try
        {
            var lista = clientes.Listar();
            modelo.Clientes = new SelectList(lista, "ID_Cliente", "NombreRazonSocial");
            modelo.Variantes = productos.ListarVariantes();
            if (!validar) return;
            if (!lista.Any(c => c.ID_Cliente == modelo.ID_Cliente)) ModelState.AddModelError(nameof(modelo.ID_Cliente), "Seleccione un cliente disponible.");
            for (int i = 0; i < modelo.Detalle.Count; i++)
                if (!modelo.Variantes.Any(v => v.ID_ProductoV == modelo.Detalle[i].ID_ProductoV))
                    ModelState.AddModelError($"Detalle[{i}].ID_ProductoV", "Seleccione una variante disponible.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al cargar catálogos de cotización.");
            ModelState.AddModelError(string.Empty, "No se pudieron cargar los clientes y variantes. Intente nuevamente.");
        }
    }
}
