using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoBD.Negocio;
using ProyectoBD.Web.Helpers;
using ProyectoBD.Web.Models;
using ProyectoBD.Web.Services;

namespace ProyectoBD.Web.Controllers;

[Authorize(Roles = "Gerente")]
public class ReportesController : Controller
{
    private readonly ReportesMemoria memoria;
    private readonly ILogger<ReportesController> logger;
    public ReportesController(ReportesMemoria memoria, ILogger<ReportesController> logger) { this.memoria = memoria; this.logger = logger; }
    [HttpGet] public IActionResult Index() => View(new ReporteParametrosViewModel());
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Generar(ReporteParametrosViewModel modelo)
    {
        if (modelo.Tipo is 1 or 2 or 3 or 5)
        {
            if (!modelo.Inicial.HasValue || !modelo.Final.HasValue || modelo.Inicial > modelo.Final)
                ModelState.AddModelError("", "Ingrese un período válido, con fecha inicial menor o igual a la final.");
        }
        if (modelo.Tipo == 7 && (!modelo.NumeroFactura.HasValue || modelo.NumeroFactura <= 0)) ModelState.AddModelError("", "Ingrese un número de factura entero y positivo.");
        if (!ModelState.IsValid) return View("Index", modelo);
        try
        {
            var sesion = User.ObtenerSesion();
            var vista = ReporteWeb.Generar(new ReporteNegocio(sesion), modelo);
            memoria.Guardar(sesion.ID_Usuario, vista);
            return RedirectToAction(nameof(Previsualizar), new { token = vista.Token });
        }
        catch (ArgumentException ex) { ModelState.AddModelError("", ex.Message); }
        catch (Exception ex) { logger.LogError(ex, "Error al generar reporte."); ModelState.AddModelError("", "No se pudo generar el reporte. Revise los parámetros e intente nuevamente."); }
        return View("Index", modelo);
    }
    [HttpGet, ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public IActionResult Previsualizar(string token)
    {
        var vista = memoria.Obtener(User.ObtenerSesion().ID_Usuario, token);
        if (vista == null) { TempData["Error"] = "La previsualización venció o no está disponible. Genere nuevamente el reporte."; return RedirectToAction(nameof(Index)); }
        return View(vista);
    }
    [HttpGet, ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public IActionResult Pdf(string token)
    {
        var vista = memoria.Obtener(User.ObtenerSesion().ID_Usuario, token);
        if (vista == null) return NotFound("La previsualización venció. Genere nuevamente el reporte.");
        try { return File(ReportePdf.Crear(vista), "application/pdf", $"Reporte_{vista.Generado:yyyy-MM-dd}_{vista.Token[..8]}.pdf"); }
        catch (Exception ex) { logger.LogError(ex, "Error al generar PDF."); TempData["Error"] = "No se pudo generar el PDF. Intente nuevamente."; return RedirectToAction(nameof(Previsualizar), new { token }); }
    }
    [HttpGet]
    public IActionResult Consulta(int? numeroFactura)
    {
        var modelo = new ConsultaFacturaViewModel { NumeroFactura = numeroFactura };
        if (!numeroFactura.HasValue) return View(modelo);
        if (numeroFactura <= 0 || !ModelState.IsValid) { ModelState.AddModelError("", "Ingrese un número de factura entero y positivo."); return View(modelo); }
        try
        {
            var sesion = User.ObtenerSesion();
            modelo.Reporte = ReporteWeb.Factura(new ReporteNegocio(sesion).BuscarFactura(numeroFactura.Value), numeroFactura.Value);
            memoria.Guardar(sesion.ID_Usuario, modelo.Reporte);
        }
        catch (ArgumentException ex) { ModelState.AddModelError("", ex.Message); }
        catch (Exception ex) { logger.LogError(ex, "Error al consultar factura por número."); ModelState.AddModelError("", "No se pudo consultar la factura."); }
        return View(modelo);
    }
}
