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
public class FacturasController : Controller
{
    private readonly ILogger<FacturasController> logger;
    public FacturasController(ILogger<FacturasController> logger) { this.logger = logger; }
    private ProcesoNegocio Proceso() => new(User.ObtenerSesion());
    [HttpGet]
    public IActionResult Index()
    {
        try { return View(FacturaWeb.Listar(Proceso())); }
        catch (Exception ex) { logger.LogError(ex, "Error al listar facturas."); ViewBag.Error = "No se pudieron cargar las facturas."; return View(new List<FacturaWebViewModel>()); }
    }
    [HttpGet]
    public IActionResult Detalle(int id)
    {
        try { var f = FacturaWeb.Detalle(Proceso(), id); return f == null ? NotFound() : View(f); }
        catch (Exception ex) { logger.LogError(ex, "Error al consultar factura."); TempData["Error"] = "No se pudo consultar la factura."; return RedirectToAction(nameof(Index)); }
    }
    [HttpGet]
    public IActionResult Crear(int? cotizacion)
    {
        var modelo = new FacturaCrearViewModel { ID_Cotizacion = cotizacion, Detalle = new() { new() } };
        Cargar(modelo);
        return View(modelo);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Crear(FacturaCrearViewModel modelo, string? accion = "guardar")
    {
        modelo.Detalle ??= new();
        Cargar(modelo);
        if (!modelo.ID_Cotizacion.HasValue && accion == "agregar")
        { if (modelo.Detalle.Count < modelo.Variantes.Count) modelo.Detalle.Add(new()); ModelState.Clear(); Cargar(modelo); return View(modelo); }
        if (!modelo.ID_Cotizacion.HasValue && accion?.StartsWith("quitar:") == true && int.TryParse(accion[7..], out int indice))
        { if (indice < 0 || indice >= modelo.Detalle.Count) return BadRequest(); modelo.Detalle.RemoveAt(indice); ModelState.Clear(); Cargar(modelo); return View(modelo); }
        if (accion != "guardar") return BadRequest();
        if (!modelo.ID_Cotizacion.HasValue)
        {
            if (!modelo.Clientes.Any(c => c.Value == modelo.ID_Cliente.ToString())) ModelState.AddModelError("", "Seleccione un cliente disponible.");
            if (modelo.Detalle.Count == 0) ModelState.AddModelError("", "Agregue al menos una línea.");
            if (modelo.Detalle.Any(l => !modelo.Variantes.Any(v => v.ID_ProductoV == l.ID_ProductoV))) ModelState.AddModelError("", "Seleccione variantes disponibles.");
        }
        if (!ModelState.IsValid) return View(modelo);
        try
        {
            int id = Proceso().Facturar(new Factura { ID_Cliente = modelo.ID_Cliente, ID_Cotizacion = modelo.ID_Cotizacion,
                Detalle = modelo.ID_Cotizacion.HasValue ? new() : modelo.Detalle.Select(l => new LineaDocumento { ID_ProductoV = l.ID_ProductoV, Cantidad = l.Cantidad, PrecioUnitario = l.PrecioUnitario }).ToList() });
            TempData["Mensaje"] = "Factura registrada. Inventario actualizado por PEPS.";
            return RedirectToAction(nameof(Detalle), new { id });
        }
        catch (ArgumentException ex) { ModelState.AddModelError("", ex.Message); }
        catch (InvalidOperationException ex) { ModelState.AddModelError("", ex.Message); }
        catch (Exception ex) { logger.LogError(ex, "Error al facturar."); ModelState.AddModelError("", "No se pudo registrar la factura. Revise los datos e intente nuevamente."); }
        Cargar(modelo);
        return View(modelo);
    }
    private void Cargar(FacturaCrearViewModel modelo)
    {
        try
        {
            modelo.Clientes = new SelectList(new ClienteNegocio().Listar(), "ID_Cliente", "NombreRazonSocial");
            modelo.Variantes = new ProductoNegocio().ListarVariantes();
            var confirmadas = Proceso().Cotizaciones().AsEnumerable().Where(r => r.Field<string>("Estado") == "Confirmada").ToList();
            modelo.Cotizaciones = confirmadas.Select(r => new SelectListItem($"Cotización {r["ID_Cotizacion"]} · {r["NombreRazonSocial"]} · Q{r.Field<decimal>("Total"):N2}", r["ID_Cotizacion"].ToString(), r.Field<int>("ID_Cotizacion") == modelo.ID_Cotizacion));
            if (!modelo.ID_Cotizacion.HasValue) return;
            // El cliente y los importes de una cotización siempre se obtienen nuevamente del servidor.
            foreach (string key in ModelState.Keys.Where(k => k.StartsWith("Detalle[") || k == nameof(modelo.ID_Cliente)).ToList()) ModelState.Remove(key);
            var cotizacion = confirmadas.FirstOrDefault(r => r.Field<int>("ID_Cotizacion") == modelo.ID_Cotizacion);
            if (cotizacion == null) { ModelState.AddModelError("", "Seleccione una cotización confirmada y sin facturar."); return; }
            modelo.ID_Cliente = cotizacion.Field<int>("ID_Cliente");
            modelo.ClienteCotizacion = cotizacion.Field<string>("NombreRazonSocial")!;
            modelo.DetalleConfirmado = Proceso().DetalleCotizacion(modelo.ID_Cotizacion.Value);
        }
        catch (Exception ex) { logger.LogError(ex, "Error al cargar datos de facturación."); ModelState.AddModelError("", "No se pudieron cargar los datos de facturación."); }
    }
}
