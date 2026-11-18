using System.Data;
using System.Drawing.Printing;
using ProyectoBD.Negocio;
namespace ProyectoBD.Presentacion;

internal static class ImpresionFactura
{
    public static void Previsualizar(IWin32Window propietario, ProcesoNegocio negocio, int id)
    {
        using var documento = CrearDocumento(negocio, id);
        using var preview = new PrintPreviewDialog { Document = documento, Width = 1000, Height = 750 };
        preview.ShowDialog(propietario);
    }
    internal static PrintDocument CrearDocumento(ProcesoNegocio negocio, int id)
    {
        var f = negocio.Facturas().AsEnumerable().SingleOrDefault(r => r.Field<int>("ID_Factura") == id) ?? throw new ArgumentException("La factura no existe.");
        var lineas = new List<string> { $"FACTURA No. {f["NumeroFactura"]}", $"Fecha: {Convert.ToDateTime(f["FechaFactura"]):dd/MM/yyyy HH:mm}", $"Cliente: {f["Cliente"]}", $"Responsable: {f["Usuario"]}", $"Estado: {f["Estado"]}", "", "PRODUCTO | CANTIDAD | PRECIO Q | SUBTOTAL Q" };
        foreach (DataRow r in negocio.DetalleFactura(id).Rows)
            lineas.Add($"{r["Producto"]} | {r["Cantidad"]} | {Convert.ToDecimal(r["PrecioUnitario"]):N2} | {Convert.ToDecimal(r["Subtotal"]):N2}");
        lineas.Add("");
        lineas.Add($"TOTAL Q{Convert.ToDecimal(f["Total"]):N2}");
        lineas.Add("MEDIOS DE PAGO");
        var pagos = negocio.Pagos(id);
        foreach (DataRow r in pagos.Rows)
            lineas.Add($"{r["NombreMetodo"]}: Q{Convert.ToDecimal(r["Monto"]):N2}  Ref. {r["Referencia"]}");
        if (pagos.Rows.Count == 0)
            lineas.Add("Sin pagos registrados.");
        lineas.Add($"Saldo Q{Convert.ToDecimal(f["Saldo"]):N2}");
        var documento = new PrintDocument { DocumentName = $"Factura-{f["NumeroFactura"]}" };
        documento.DefaultPageSettings.Margins = new Margins(50, 50, 50, 50);
        int indice = 0, pagina = 0;
        List<string>? envueltas = null;
        documento.BeginPrint += (_, _) => { indice = 0; pagina = 0; envueltas = null; };
        documento.PrintPage += (_, e) =>
        {
            if (e.Graphics == null)
                return;
            using var fuente = new Font("Segoe UI", 10);
            using var titulo = new Font("Segoe UI", 12, FontStyle.Bold);
            float alto = fuente.GetHeight(e.Graphics) + 5;
            float y = e.MarginBounds.Top;
            e.Graphics.DrawString($"Factura {f["NumeroFactura"]} — Página {++pagina}", titulo, Brushes.Black, e.MarginBounds.Left, y);
            y += alto * 2;
            // Envuelve antes de paginar para no truncar nombres largos.
            envueltas ??= Envolver(lineas, e.Graphics, fuente, e.MarginBounds.Width);
            while (indice < envueltas.Count && y + alto <= e.MarginBounds.Bottom)
            {
                e.Graphics.DrawString(envueltas[indice++], fuente, Brushes.Black, e.MarginBounds.Left, y);
                y += alto;
            }
            e.HasMorePages = indice < envueltas.Count;
        };
        return documento;
    }
    private static List<string> Envolver(List<string> textos, Graphics g, Font fuente, int ancho)
    {
        var resultado = new List<string>();
        foreach (string texto in textos)
        {
            if (texto.Length == 0)
            {
                resultado.Add("");
                continue;
            }
            string linea = "";
            foreach (char c in texto)
            {
                if (linea.Length > 0 && g.MeasureString(linea + c, fuente).Width > ancho)
                {
                    resultado.Add(linea);
                    linea = "";
                }
                linea += c;
            }
            resultado.Add(linea);
        }
        return resultado;
    }
}
