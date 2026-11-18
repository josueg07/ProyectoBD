using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Fonts;
using PdfSharp.Drawing;
using ProyectoBD.Web.Models;

namespace ProyectoBD.Web.Services;

public static class ReportePdf
{
    private static readonly object Bloqueo = new();
    public static byte[] Crear(ReporteVistaViewModel reporte)
    {
        lock (Bloqueo)
        {
            GlobalFontSettings.FontResolver ??= new FuentesReporte();
            var documento = new Document();
            documento.Info.Title = reporte.Titulo;
            documento.Info.Author = "Comercializadora de Pinturas de Occidente";
            var normal = documento.Styles[StyleNames.Normal]!;
            normal.Font.Name = "Noto Sans"; normal.Font.Size = 9;
            normal.ParagraphFormat.SpaceAfter = Unit.FromPoint(5);
            var seccion = documento.AddSection();
            seccion.PageSetup.PageFormat = PageFormat.A4;
            seccion.PageSetup.Orientation = Orientation.Landscape;
            seccion.PageSetup.LeftMargin = seccion.PageSetup.RightMargin = Unit.FromCentimeter(1.5);
            seccion.PageSetup.TopMargin = Unit.FromCentimeter(1.5);
            seccion.PageSetup.BottomMargin = Unit.FromCentimeter(1.8);
            seccion.PageSetup.FooterDistance = Unit.FromCentimeter(0.7);
            var marca = seccion.AddParagraph("COMERCIALIZADORA DE PINTURAS DE OCCIDENTE");
            marca.Format.Font.Size = 9; marca.Format.Font.Color = Colors.DarkSlateGray;
            var titulo = seccion.AddParagraph(reporte.Titulo); titulo.Format.Font.Size = 18; titulo.Format.Font.Bold = true;
            seccion.AddParagraph("Generado: " + reporte.Generado.ToString("dd/MM/yyyy HH:mm:ss"));
            seccion.AddParagraph(reporte.Parametros);
            if (reporte.Nota.Length > 0) seccion.AddParagraph(reporte.Nota);
            var pie = seccion.Footers.Primary.AddParagraph(); pie.Format.Alignment = ParagraphAlignment.Right;
            pie.AddText("Página "); pie.AddPageField(); pie.AddText(" de "); pie.AddNumPagesField();
            foreach (var datos in reporte.Tablas)
            {
                var subtitulo = seccion.AddParagraph(datos.Titulo); subtitulo.Format.Font.Size = 12; subtitulo.Format.Font.Bold = true;
                subtitulo.Format.SpaceBefore = Unit.FromPoint(10); subtitulo.Format.KeepWithNext = true;
                if (datos.Filas.Count == 0) { seccion.AddParagraph("No hay resultados para esta consulta."); continue; }
                if (datos.Titulo == "Auditoría")
                {
                    // Los JSON históricos pueden ocupar varias páginas; párrafos permiten
                    // paginarlos sin encerrar todo un evento en una fila indivisible.
                    foreach (var evento in datos.Filas)
                        for (int c = 0; c < datos.Columnas.Count; c++)
                        {
                            var dato = seccion.AddParagraph();
                            dato.AddFormattedText(datos.Columnas[c] + ": ", TextFormat.Bold);
                            AgregarTexto(dato, evento[c], Unit.FromCentimeter(26.7).Point - 8, 9);
                        }
                    continue;
                }
                var tabla = seccion.AddTable(); tabla.Borders.Width = 0.4; tabla.Borders.Color = Colors.LightGray;
                tabla.Format.Font.Size = datos.Columnas.Count >= 6 ? 8 : 9;
                tabla.TopPadding = tabla.BottomPadding = Unit.FromPoint(4);
                tabla.LeftPadding = tabla.RightPadding = Unit.FromPoint(3);
                // Producto y textos de auditoría requieren mayor anchura que cantidades e importes.
                var pesos = datos.Columnas.Select(c => c is "Producto" or "Datos anteriores" or "Datos nuevos" or "Valor" ? 2.5 : c is "Cantidad" or "Disponible" or "Cantidad vendida" ? 0.8 : 1.3).ToList();
                foreach (double peso in pesos) tabla.AddColumn(Unit.FromCentimeter(26.7 * peso / pesos.Sum()));
                var cabecera = tabla.AddRow(); cabecera.HeadingFormat = true; cabecera.Shading.Color = Colors.DarkSlateGray; cabecera.Format.Font.Color = Colors.White; cabecera.Format.Font.Bold = true;
                for (int c = 0; c < datos.Columnas.Count; c++) cabecera.Cells[c].AddParagraph(datos.Columnas[c]);
                foreach (var fila in datos.Filas)
                {
                    var renglon = tabla.AddRow();
                    for (int c = 0; c < fila.Count; c++)
                    {
                        var parrafo = renglon.Cells[c].AddParagraph();
                        // MigraDoc no divide identificadores o JSON sin espacios: medir y partir
                        // también esas cadenas evita que invadan la columna siguiente.
                        double ancho = Unit.FromCentimeter(26.7 * pesos[c] / pesos.Sum()).Point - 8;
                        AgregarTexto(parrafo, fila[c], ancho, datos.Columnas.Count >= 6 ? 8 : 9);
                    }
                }
                if (datos.Totales.Length > 0) { var total = seccion.AddParagraph(datos.Totales); total.Format.Font.Bold = true; }
            }
            var renderer = new PdfDocumentRenderer { Document = documento };
            renderer.RenderDocument();
            using var memoria = new MemoryStream(); renderer.PdfDocument.Save(memoria, false); return memoria.ToArray();
        }
    }
    private static void AgregarTexto(Paragraph parrafo, string texto, double ancho, double tamano)
    {
        using var medida = XGraphics.CreateMeasureContext(new XSize(1000, 1000), XGraphicsUnit.Point, XPageDirection.Downwards);
        var fuente = new XFont("Noto Sans", tamano);
        string[] lineas = texto.Replace("\r", "").Split('\n');
        for (int i = 0; i < lineas.Length; i++)
        {
            string pendiente = lineas[i];
            while (medida.MeasureString(pendiente, fuente).Width > ancho)
            {
                int corte = 1;
                while (corte < pendiente.Length && medida.MeasureString(pendiente[..(corte + 1)], fuente).Width <= ancho) corte++;
                int espacio = pendiente.LastIndexOf(' ', corte - 1, corte);
                if (espacio > 0) corte = espacio;
                parrafo.AddText(pendiente[..corte]); parrafo.AddLineBreak();
                pendiente = pendiente[corte..].TrimStart(' ');
            }
            parrafo.AddText(pendiente);
            if (i < lineas.Length - 1) parrafo.AddLineBreak();
        }
    }
    private sealed class FuentesReporte : IFontResolver
    {
        public FontResolverInfo ResolveTypeface(string familyName, bool bold, bool italic) => new(bold ? "NotoSans-Bold.ttf" : "NotoSans-Regular.ttf", false, italic);
        public byte[] GetFont(string faceName)
        {
            using var recurso = typeof(ReportePdf).Assembly.GetManifestResourceStream("ProyectoBD.Web.Resources.Fonts." + faceName)
                ?? throw new InvalidOperationException("No se encontró la fuente del reporte.");
            using var memoria = new MemoryStream(); recurso.CopyTo(memoria); return memoria.ToArray();
        }
    }
}
