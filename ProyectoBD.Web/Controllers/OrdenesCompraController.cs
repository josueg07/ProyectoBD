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
public class OrdenesCompraController : Controller
{
    private readonly ProveedorNegocio proveedores;
    private readonly ProductoNegocio productos;
    private readonly ILogger<OrdenesCompraController> logger;
    public OrdenesCompraController(ProveedorNegocio proveedores, ProductoNegocio productos, ILogger<OrdenesCompraController> logger)
    {
        this.proveedores = proveedores;
        this.productos = productos;
        this.logger = logger;
    }
    private ProcesoNegocio Proceso()
    {
        UsuarioSesion sesion = User.ObtenerSesion();
        return new ProcesoNegocio(sesion);
    }
    private List<OrdenCompraResumenViewModel> Listar()
    {
        return Proceso().Ordenes().AsEnumerable().Select(r => new OrdenCompraResumenViewModel
        {
            ID_OrdenC = r.Field<int>("ID_OrdenC"), Fecha = r.Field<DateTime>("FechaOrden"),
            Proveedor = r.Field<string>("Nombre_Proveedor")!, Estado = r.Field<string>("Estado")!
        }).ToList();
    }
    [HttpGet]
    public IActionResult Index()
    {
        try { return View(Listar()); }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al listar órdenes de compra.");
            ViewBag.Error = "No se pudieron cargar las órdenes. Intente nuevamente.";
            return View(new List<OrdenCompraResumenViewModel>());
        }
    }
    [HttpGet]
    public IActionResult Detalle(int id)
    {
        try
        {
            var modelo = Listar().FirstOrDefault(o => o.ID_OrdenC == id);
            if (modelo == null) return NotFound();
            modelo.Detalle = Proceso().PendienteOrden(id).AsEnumerable().Select(r => new LineaPendienteViewModel
            {
                ID_DetalleOC = r.Field<int>("ID_DetalleOC"), Producto = r.Field<string>("Producto")!,
                Solicitada = r.Field<int>("CantidadSolicitada"), Recibida = r.Field<int>("CantidadRecibida"), Pendiente = r.Field<int>("Pendiente")
            }).ToList();
            return View(modelo);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al consultar orden de compra.");
            TempData["Error"] = "No se pudo consultar la orden. Intente nuevamente.";
            return RedirectToAction(nameof(Index));
        }
    }
    [HttpGet]
    public IActionResult Crear()
    {
        var modelo = new OrdenCompraCrearViewModel { Detalle = new() { new() } };
        Cargar(modelo, false);
        return View(modelo);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(OrdenCompraCrearViewModel modelo, string accion = "guardar")
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
            var orden = new OrdenCompra
            {
                ID_Proveedor = modelo.ID_Proveedor,
                Detalle = modelo.Detalle.Select(l => new LineaDocumento { ID_ProductoV = l.ID_ProductoV, Cantidad = l.Cantidad }).ToList()
            };
            int id = Proceso().GuardarOrden(orden);
            TempData["Mensaje"] = "Orden de compra registrada como Pendiente.";
            return RedirectToAction(nameof(Detalle), new { id });
        }
        catch (ArgumentException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
        catch (InvalidOperationException ex) { ModelState.AddModelError(string.Empty, ex.Message); }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al guardar orden de compra.");
            ModelState.AddModelError(string.Empty, "No se pudo guardar la orden. Revise los datos e intente nuevamente.");
        }
        return View(modelo);
    }
    private void Cargar(OrdenCompraCrearViewModel modelo, bool validar)
    {
        try
        {
            var lista = proveedores.Listar();
            modelo.Proveedores = new SelectList(lista, "ID_Proveedor", "Nombre_Proveedor");
            modelo.Variantes = productos.ListarVariantes();
            if (!validar) return;
            if (!lista.Any(p => p.ID_Proveedor == modelo.ID_Proveedor)) ModelState.AddModelError(nameof(modelo.ID_Proveedor), "Seleccione un proveedor disponible.");
            for (int i = 0; i < modelo.Detalle.Count; i++)
                if (!modelo.Variantes.Any(v => v.ID_ProductoV == modelo.Detalle[i].ID_ProductoV))
                    ModelState.AddModelError($"Detalle[{i}].ID_ProductoV", "Seleccione una variante disponible.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al cargar catálogos de compra.");
            ModelState.AddModelError(string.Empty, "No se pudieron cargar los proveedores y variantes. Intente nuevamente.");
        }
    }
}
