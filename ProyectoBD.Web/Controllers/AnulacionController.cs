using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoBD.Negocio;
using ProyectoBD.Web.Helpers;
using ProyectoBD.Web.Models;
using ProyectoBD.Web.Services;

namespace ProyectoBD.Web.Controllers;

[Authorize(Roles = "Gerente")]
public class AnulacionController : Controller
{
    private readonly ILogger<AnulacionController> logger;
    private readonly ReportesMemoria memoria;
    public AnulacionController(ILogger<AnulacionController> logger, ReportesMemoria memoria) { this.logger = logger; this.memoria = memoria; }
    [HttpGet]
    public IActionResult Index(int? numeroFactura)
    {
        var modelo = new ConsultaFacturaViewModel { NumeroFactura = numeroFactura, Anulacion = true };
        if (!numeroFactura.HasValue) return View("~/Views/Reportes/Consulta.cshtml", modelo);
        if (numeroFactura <= 0 || !ModelState.IsValid) ModelState.AddModelError("", "Ingrese un número de factura entero y positivo.");
        else try
        {
            var sesion = User.ObtenerSesion();
            var resultado = new ReporteNegocio(sesion).BuscarFactura(numeroFactura.Value);
            modelo.Reporte = ReporteWeb.Factura(resultado, numeroFactura.Value);
            modelo.Estado = (string)resultado.Tables[0].Rows[0]["Estado"];
            memoria.Guardar(sesion.ID_Usuario, modelo.Reporte);
        }
        catch (ArgumentException ex) { ModelState.AddModelError("", ex.Message); }
        catch (Exception ex) { logger.LogError(ex, "Error al buscar factura para anulación."); ModelState.AddModelError("", "No se pudo consultar la factura."); }
        return View("~/Views/Reportes/Consulta.cshtml", modelo);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Anular(int numeroFactura, bool confirmar = false)
    {
        if (!ModelState.IsValid || numeroFactura <= 0) return BadRequest();
        if (!confirmar) { TempData["Error"] = "La factura no se anuló. Debe confirmar expresamente la operación."; return RedirectToAction(nameof(Index), new { numeroFactura }); }
        try
        {
            new ReporteNegocio(User.ObtenerSesion()).AnularFactura(numeroFactura);
            TempData["Mensaje"] = "Factura anulada. Total Q0.00 e inventario restaurado; se conserva el historial.";
        }
        catch (ArgumentException ex) { TempData["Error"] = ex.Message; }
        catch (InvalidOperationException ex) { TempData["Error"] = ex.Message; }
        catch (Exception ex) { logger.LogError(ex, "Error al anular factura."); TempData["Error"] = "No se pudo anular la factura. Vuelva a consultar su estado."; }
        return RedirectToAction(nameof(Index), new { numeroFactura });
    }
}
