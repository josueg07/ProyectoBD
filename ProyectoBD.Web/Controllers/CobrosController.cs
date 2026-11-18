using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;
using ProyectoBD.Web.Helpers;
using ProyectoBD.Web.Models;

namespace ProyectoBD.Web.Controllers;

[Authorize(Roles = "Cajero")]
public class CobrosController : Controller
{
    private readonly ILogger<CobrosController> logger;
    public CobrosController(ILogger<CobrosController> logger) { this.logger = logger; }
    private ProcesoNegocio Proceso() => new(User.ObtenerSesion());
    [HttpGet]
    public IActionResult Index()
    {
        try { return View(FacturaWeb.Listar(Proceso()).Where(f => f.Estado == "Pendiente" && f.Saldo > 0).ToList()); }
        catch (Exception ex) { logger.LogError(ex, "Error al listar cobros."); ViewBag.Error = "No se pudieron cargar las facturas pendientes."; return View(new List<FacturaWebViewModel>()); }
    }
    [HttpGet]
    public IActionResult Detalle(int id)
    {
        try { var f = FacturaWeb.Detalle(Proceso(), id); return f == null ? NotFound() : View(f); }
        catch (Exception ex) { logger.LogError(ex, "Error al consultar cobro."); TempData["Error"] = "No se pudo consultar la factura."; return RedirectToAction(nameof(Index)); }
    }
    [HttpGet]
    public IActionResult Cobrar(int id)
    {
        var modelo = new CobroViewModel { ID_Factura = id, Pagos = new() { new() } };
        if (!Cargar(modelo)) return NotFound();
        return View(modelo);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Cobrar(int id, CobroViewModel modelo, string? accion = "guardar")
    {
        if (id <= 0 || id != modelo.ID_Factura) return BadRequest();
        modelo.Pagos ??= new();
        if (!Cargar(modelo)) return NotFound();
        if (accion == "agregar") { if (modelo.Pagos.Count < 30) modelo.Pagos.Add(new()); ModelState.Clear(); return View(modelo); }
        if (accion?.StartsWith("quitar:") == true && int.TryParse(accion[7..], out int indice))
        { if (indice < 0 || indice >= modelo.Pagos.Count) return BadRequest(); modelo.Pagos.RemoveAt(indice); ModelState.Clear(); return View(modelo); }
        if (accion != "guardar") return BadRequest();
        if (modelo.Factura.Estado != "Pendiente" || modelo.Factura.Saldo <= 0) ModelState.AddModelError("", "La factura no está pendiente de pago.");
        if (modelo.Pagos.Any(p => !modelo.Metodos.Any(m => m.Value == p.ID_MetodoP.ToString()))) ModelState.AddModelError("", "Seleccione métodos de pago disponibles.");
        if (modelo.TotalPagos > modelo.Factura.Saldo) ModelState.AddModelError("", "Los pagos superan el saldo pendiente.");
        if (!ModelState.IsValid) return View(modelo);
        try
        {
            Proceso().Pagar(id, modelo.Pagos.Select(p => new PagoFactura { ID_MetodoP = p.ID_MetodoP, Monto = p.Monto, Referencia = p.Referencia?.Trim() ?? "" }).ToList());
            TempData["Mensaje"] = "Pago registrado correctamente.";
            return RedirectToAction(nameof(Detalle), new { id });
        }
        catch (ArgumentException ex) { ModelState.AddModelError("", ex.Message); }
        catch (InvalidOperationException ex) { ModelState.AddModelError("", ex.Message); }
        catch (Exception ex) { logger.LogError(ex, "Error al registrar cobro."); ModelState.AddModelError("", "No se pudo registrar el pago. Consulte el saldo e intente nuevamente."); }
        Cargar(modelo);
        return View(modelo);
    }
    private bool Cargar(CobroViewModel modelo)
    {
        try
        {
            var factura = FacturaWeb.Detalle(Proceso(), modelo.ID_Factura);
            if (factura == null) return false;
            modelo.Factura = factura;
            modelo.Metodos = new SelectList(new MetodoPagoNegocio().Listar(), "ID_MetodoP", "NombreMetodo");
        }
        catch (Exception ex) { logger.LogError(ex, "Error al cargar datos de cobro."); ModelState.AddModelError("", "No se pudieron cargar los datos del cobro."); }
        return true;
    }
}
