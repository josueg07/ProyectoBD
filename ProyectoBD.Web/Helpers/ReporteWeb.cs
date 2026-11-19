using System.Data;
using System.Globalization;
using ProyectoBD.Negocio;
using ProyectoBD.Web.Models;
using ProyectoBD.FormatoVisual;

namespace ProyectoBD.Web.Helpers;

// Sólo adapta las tablas existentes a HTML/PDF. No consulta SQL ni reproduce cálculos de negocio.
public static class ReporteWeb
{
    public static readonly string[] Titulos = { "", "Facturación y formas de pago", "Productos con mayores ingresos", "Productos más vendidos", "Inventario actual", "Productos con menos ventas", "Productos sin existencias", "Consulta detallada de factura" };
    private static readonly Dictionary<string, string> Encabezados = new()
    {
        ["TotalFacturado"] = "Total facturado", ["TotalPagado"] = "Total pagado", ["SaldoPorCobrar"] = "Saldo por cobrar",
        ["MetodoPago"] = "Método de pago", ["CantidadVendida"] = "Cantidad vendida", ["IngresoTotal"] = "Ingreso generado",
        ["Presentacion"] = "Presentación", ["Ubicacion"] = "Ubicación", ["FechaIngreso"] = "Fecha de ingreso", ["CantidadDisponible"] = "Disponible",
        ["NumeroFactura"] = "Número de factura", ["FechaFactura"] = "Fecha", ["TotalOriginal"] = "Total original", ["PagosHistoricos"] = "Pagos históricos",
        ["PrecioUnitario"] = "Precio unitario", ["FechaHora"] = "Fecha y hora", ["Accion"] = "Acción", ["DatosAnteriores"] = "Datos anteriores", ["DatosNuevos"] = "Datos nuevos"
    };
    public static ReporteVistaViewModel Generar(ReporteNegocio negocio, ReporteParametrosViewModel p)
    {
        var vista = new ReporteVistaViewModel { Titulo = Titulos[p.Tipo], Parametros = p.Tipo is 1 or 2 or 3 or 5 ? $"Desde {p.Inicial:dd/MM/yyyy} hasta {p.Final:dd/MM/yyyy}" : "Existencias al momento de generar" };
        switch (p.Tipo)
        {
            case 1:
                var facturacion = negocio.ReporteFacturacion(p.Inicial!.Value, p.Final!.Value);
                vista.Tablas.Add(Tabla("Resumen del período", facturacion.Tables[0]));
                vista.Tablas.Add(Tabla("Pagos por método", facturacion.Tables[1]));
                vista.Nota = "Pagos históricos de facturas emitidas en el período. Se excluyen facturas anuladas; los pagos no tienen fecha propia.";
                break;
            case 2: vista.Tablas.Add(Tabla("Ingresos por variante", negocio.ReporteProductosMayoresIngresos(p.Inicial!.Value, p.Final!.Value))); break;
            case 3: vista.Tablas.Add(Tabla("Ventas por cantidad", negocio.ReporteProductosMasVendidos(p.Inicial!.Value, p.Final!.Value))); break;
            case 4: vista.Tablas.Add(Tabla("Lotes disponibles", negocio.ReporteInventario())); break;
            case 5: vista.Tablas.Add(Tabla("Ventas ascendentes, incluidas variantes sin ventas", negocio.ReporteProductosMenosVendidos(p.Inicial!.Value, p.Final!.Value))); break;
            case 6: vista.Tablas.Add(Tabla("Variantes sin existencias", negocio.ReporteProductosSinExistencia())); break;
            case 7: return Factura(negocio.BuscarFactura(p.NumeroFactura!.Value), p.NumeroFactura.Value);
        }
        return vista;
    }
    public static ReporteVistaViewModel Factura(DataSet resultado, int numero)
    {
        if (resultado.Tables[0].Rows.Count == 0) throw new ArgumentException("No se encontró la factura indicada.");
        var vista = new ReporteVistaViewModel { Titulo = "Factura N.º " + numero, Parametros = "Consulta por número de factura: " + numero };
        // El encabezado vertical deja visibles todos sus datos sin una tabla excesivamente ancha.
        var encabezado = new TablaReporteViewModel { Titulo = "Encabezado", Columnas = new() { "Dato", "Valor" } };
        foreach (DataColumn col in resultado.Tables[0].Columns)
            encabezado.Filas.Add(new() { Etiqueta(col.ColumnName), Valor(resultado.Tables[0].Rows[0][col]) });
        vista.Tablas.Add(encabezado);
        vista.Tablas.Add(Tabla("Detalle de productos", resultado.Tables[1]));
        vista.Tablas.Add(Tabla("Pagos históricos", resultado.Tables[2]));
        vista.Tablas.Add(Tabla("Auditoría", resultado.Tables[3]));
        if (resultado.Tables[0].Rows[0].Field<string>("Estado") == "Anulada")
            vista.Nota = "FACTURA ANULADA. Los detalles y pagos se conservan como historial; no generan ventas ni saldo por cobrar.";
        return vista;
    }
    private static string Etiqueta(string columna) => Encabezados.GetValueOrDefault(columna, columna);
    private static string Valor(object dato) => dato switch
    {
        DBNull => "—", DateTime fecha => fecha.ToString("dd/MM/yyyy HH:mm"),
        decimal monto => "Q" + monto.ToString("N2", CultureInfo.GetCultureInfo("es-GT")),
        _ => Convert.ToString(dato, CultureInfo.GetCultureInfo("es-GT")) ?? ""
    };
    private static TablaReporteViewModel Tabla(string titulo, DataTable datos)
    {
        var tabla = new TablaReporteViewModel { Titulo = titulo, Columnas = datos.Columns.Cast<DataColumn>().Select(c => Etiqueta(c.ColumnName)).ToList() };
        foreach (DataRow fila in datos.Rows)
            tabla.Filas.Add(datos.Columns.Cast<DataColumn>().Select(c =>
                c.ColumnName is "DatosAnteriores" or "DatosNuevos"
                    ? FormatoAuditoria.Mostrar(fila[c]) : Valor(fila[c])).ToList());
        var sumas = new List<string>();
        foreach (DataColumn col in datos.Columns)
        {
            if (col.DataType == typeof(decimal) && col.ColumnName != "PrecioUnitario") sumas.Add(Etiqueta(col.ColumnName) + ": " + Valor(datos.AsEnumerable().Sum(r => r.Field<decimal?>(col) ?? 0)));
            else if (col.ColumnName is "Cantidad" or "CantidadVendida" or "CantidadDisponible")
                sumas.Add(Etiqueta(col.ColumnName) + ": " + datos.AsEnumerable().Sum(r => Convert.ToInt64(r[col])).ToString("N0"));
        }
        tabla.Totales = string.Join(" · ", sumas);
        return tabla;
    }
}
