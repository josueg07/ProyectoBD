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
public class IngresosBodegaController : Controller
{
    private readonly UbicacionNegocio ubicaciones;
    private readonly ILogger<IngresosBodegaController> logger;
    public IngresosBodegaController(UbicacionNegocio ubicaciones, ILogger<IngresosBodegaController> logger)
    {
        this.ubicaciones = ubicaciones;
        this.logger = logger;
    }
    private ProcesoNegocio Proceso()
    {
        UsuarioSesion sesion = User.ObtenerSesion();
        return new ProcesoNegocio(sesion);
    }
    private List<IngresoBodegaResumenViewModel> Listar(int? orden = null)
    {
        return Proceso().Ingresos().AsEnumerable().Where(r => !orden.HasValue || r.Field<int>("ID_OrdenC") == orden.Value)
            .GroupBy(r => r.Field<int>("ID_IngresoB")).Select(g => new IngresoBodegaResumenViewModel
            {
                ID_IngresoB = g.Key, ID_OrdenC = g.First().Field<int>("ID_OrdenC"), Fecha = g.First().Field<DateTime>("FechaIngreso"),
                Usuario = g.First().Field<string>("Usuario")!, CantidadRecibida = g.Sum(r => (long)r.Field<int>("CantidadRecibida")),
                CostoTotal = g.Sum(r => r.Field<int>("CantidadRecibida") * r.Field<decimal>("CostoUnitario"))
            }).ToList();
    }
    [HttpGet]
    public IActionResult Index(int? orden)
    {
        ViewBag.Orden = orden;
        try { return View(Listar(orden)); }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al consultar ingresos de bodega.");
            ViewBag.Error = "No se pudieron cargar los ingresos. Intente nuevamente.";
            return View(new List<IngresoBodegaResumenViewModel>());
        }
    }
    [HttpGet]
    public IActionResult Detalle(int id)
    {
        try
        {
            var modelo = Listar().FirstOrDefault(i => i.ID_IngresoB == id);
            if (modelo == null) return NotFound();
            var pendientes = Proceso().PendienteOrden(modelo.ID_OrdenC).AsEnumerable()
                .ToDictionary(r => r.Field<int>("ID_DetalleOC"), r => r.Field<string>("Producto")!);
            modelo.Detalle = Proceso().Ingresos().AsEnumerable().Where(r => r.Field<int>("ID_IngresoB") == id)
                .Select(r => new IngresoBodegaDetalleViewModel
                {
                    Producto = pendientes.GetValueOrDefault(r.Field<int>("ID_DetalleOC"), "No disponible"),
                    Cantidad = r.Field<int>("CantidadRecibida"), CostoUnitario = r.Field<decimal>("CostoUnitario")
                }).ToList();
            return View(modelo);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al consultar ingreso de bodega.");
            TempData["Error"] = "No se pudo consultar el ingreso. Intente nuevamente.";
            return RedirectToAction(nameof(Index));
        }
    }
    [HttpGet]
    public IActionResult Crear(int? orden)
    {
        var modelo = new IngresoBodegaCrearViewModel { ID_OrdenC = orden ?? 0, FechaIngreso = DateTime.Now };
        Cargar(modelo, false);
        return View(modelo);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(IngresoBodegaCrearViewModel modelo)
    {
        modelo.Detalle ??= new();
        Cargar(modelo, true);
        if (!ModelState.IsValid) return View(modelo);
        try
        {
            var ingreso = new IngresoBodega
            {
                ID_OrdenC = modelo.ID_OrdenC, ID_UbicacionA = modelo.ID_UbicacionA, FechaIngreso = modelo.FechaIngreso!.Value,
                Detalle = modelo.Detalle.Where(l => l.CantidadRecibida > 0).Select(l => new LineaIngreso
                {
                    ID_DetalleOC = l.ID_DetalleOC, CantidadRecibida = l.CantidadRecibida, CostoUnitario = l.CostoUnitario ?? 0,
                    NumeroLote = string.IsNullOrWhiteSpace(l.NumeroLote) ? null : l.NumeroLote.Trim()
                }).ToList()
            };
            int id = Proceso().Ingresar(ingreso);
            TempData["Mensaje"] = "Ingreso registrado. Las cantidades pendientes, los lotes y los movimientos fueron actualizados.";
            return RedirectToAction(nameof(Detalle), new { id });
        }
        catch (ArgumentException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
        catch (InvalidOperationException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al registrar ingreso de bodega.");
            ModelState.AddModelError(string.Empty, "No se pudo registrar el ingreso. Consulte nuevamente los pendientes antes de reintentar.");
        }
        // Si otra recepción cambió los pendientes, volver a consultarlos sin perder lo ingresado.
        Cargar(modelo, true);
        return View(modelo);
    }
    private void Cargar(IngresoBodegaCrearViewModel modelo, bool validar)
    {
        try
        {
            var proceso = Proceso();
            var ordenes = proceso.Ordenes().AsEnumerable().ToList();
            modelo.Ordenes = ordenes.Where(r => r.Field<string>("Estado") is "Pendiente" or "Parcial")
                .Select(r => new SelectListItem($"Orden {r["ID_OrdenC"]} — {r["Nombre_Proveedor"]} ({r["Estado"]})", r["ID_OrdenC"].ToString(), r.Field<int>("ID_OrdenC") == modelo.ID_OrdenC)).ToList();
            var listaUbicaciones = ubicaciones.Listar();
            modelo.Ubicaciones = new SelectList(listaUbicaciones, "ID_UbicacionA", "NombreUbicacion");
            var cabecera = ordenes.FirstOrDefault(r => r.Field<int>("ID_OrdenC") == modelo.ID_OrdenC);
            if (cabecera == null)
            {
                if (validar || modelo.ID_OrdenC != 0) ModelState.AddModelError(nameof(modelo.ID_OrdenC), "Seleccione una orden disponible.");
                modelo.Detalle = new();
                return;
            }
            modelo.Proveedor = cabecera.Field<string>("Nombre_Proveedor")!;
            modelo.Estado = cabecera.Field<string>("Estado")!;
            var lineasOrden = proceso.PendienteOrden(modelo.ID_OrdenC).AsEnumerable().ToList();
            var recibidas = (modelo.Detalle ?? new()).ToList();
            var errores = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? "Revise los valores ingresados." : e.ErrorMessage).ToList();
            if (validar)
            {
                if (recibidas.GroupBy(l => l.ID_DetalleOC).Any(g => g.Count() > 1)) errores.Add("No repita detalles de la orden.");
                if (recibidas.Any(l => !lineasOrden.Any(r => r.Field<int>("ID_DetalleOC") == l.ID_DetalleOC)))
                    errores.Add("El detalle contiene líneas que no pertenecen a esta orden. Vuelva a consultar los pendientes.");
            }
            modelo.Detalle = lineasOrden.Select(r =>
            {
                var entrada = recibidas.FirstOrDefault(l => l.ID_DetalleOC == r.Field<int>("ID_DetalleOC"));
                return new LineaRecepcionViewModel
                {
                    ID_DetalleOC = r.Field<int>("ID_DetalleOC"), Producto = r.Field<string>("Producto")!,
                    Solicitada = r.Field<int>("CantidadSolicitada"), RecibidaAnterior = r.Field<int>("CantidadRecibida"), Pendiente = r.Field<int>("Pendiente"),
                    CantidadRecibida = entrada?.CantidadRecibida ?? 0, CostoUnitario = entrada?.CostoUnitario, NumeroLote = entrada?.NumeroLote
                };
            }).ToList();
            if (!validar) return;
            // Los nombres y pendientes son del servidor; sólo se conservan cantidades, costos y lotes del formulario.
            ModelState.Clear();
            foreach (string error in errores) ModelState.AddModelError(string.Empty, error);
            TryValidateModel(modelo);
            if (modelo.Estado is not ("Pendiente" or "Parcial")) ModelState.AddModelError(string.Empty, "La orden está completa y ya no admite ingresos.");
            if (!listaUbicaciones.Any(u => u.ID_UbicacionA == modelo.ID_UbicacionA)) ModelState.AddModelError(nameof(modelo.ID_UbicacionA), "Seleccione una ubicación disponible.");
            if (modelo.FechaIngreso > DateTime.Now) ModelState.AddModelError(nameof(modelo.FechaIngreso), "La fecha de ingreso no puede ser futura.");
            if (!modelo.Detalle.Any(l => l.CantidadRecibida > 0)) ModelState.AddModelError(string.Empty, "Ingrese al menos una cantidad a recibir mayor que cero.");
            for (int i = 0; i < modelo.Detalle.Count; i++)
            {
                var l = modelo.Detalle[i];
                if (l.CantidadRecibida > l.Pendiente) ModelState.AddModelError($"Detalle[{i}].CantidadRecibida", "La cantidad supera lo pendiente actual.");
                if (l.CantidadRecibida > 0 && (!l.CostoUnitario.HasValue || l.CostoUnitario <= 0 || l.CostoUnitario > 9999999999.99m || decimal.Round(l.CostoUnitario.Value, 2) != l.CostoUnitario))
                    ModelState.AddModelError($"Detalle[{i}].CostoUnitario", "Ingrese un costo positivo con hasta dos decimales.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al cargar pendientes de recepción.");
            ModelState.AddModelError(string.Empty, "No se pudieron cargar las órdenes, ubicaciones y pendientes. Intente nuevamente.");
        }
    }
}
