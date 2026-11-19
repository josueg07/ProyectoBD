using System.Data;
using System.Text;
using ProyectoBD.FormatoVisual;
using ProyectoBD.Web.Helpers;
using ProyectoBD.Web.Services;

internal static class PruebasAuditoria
{
    public static void Ejecutar()
    {
        const string anterior = """{"NumeroFactura":1,"Estado":"Pagada","Total":7200.00,"Lotes":[{"ID_Lote":41,"Cantidad":50,"DisponibleAntes":0},{"ID_Lote":42,"Cantidad":10,"DisponibleAntes":30}]}""";
        const string nuevo = """{"NumeroFactura":1,"Estado":"Anulada","Total":0.00,"Lotes":[{"ID_Lote":41,"Reintegrada":50,"DisponibleDespues":50},{"ID_Lote":42,"Reintegrada":10,"DisponibleDespues":40}]}""";
        string texto = FormatoAuditoria.Mostrar(anterior);
        Exigir(texto.Contains("Número de factura: 1") && texto.Contains("Total: Q7200.00") &&
            texto.Contains("Registro 2:") && texto.Contains("Disponible antes: 30") &&
            !texto.Contains("{\"") && !texto.Contains("[{"), "nested audit is readable without losing values");
        Exigir(FormatoAuditoria.Mostrar(DBNull.Value) == "Sin datos registrados", "missing audit data is explicit");
        Exigir(FormatoAuditoria.Mostrar("{ }") == "Sin datos" && FormatoAuditoria.Mostrar("[]") == "Sin registros", "empty objects and arrays are explicit");
        Exigir(FormatoAuditoria.Mostrar("Texto histórico") == "Texto histórico", "legacy plain text is preserved");
        Exigir(FormatoAuditoria.Mostrar("{incompleto") == "{incompleto", "malformed historical data is preserved");
        string arreglo = FormatoAuditoria.Mostrar("""[{"ID_MetodoP":15,"Monto":12.50,"Referencia":"Abono ñ"},{"Monto":2.125,"Activo":true,"Observacion":null}]""");
        Exigir(arreglo.Contains("Monto: Q12.50") && arreglo.Contains("Monto: Q2.125") &&
            arreglo.Contains("Activo: Sí") && arreglo.Contains("Observacion: Sin dato") &&
            arreglo.Contains("Abono ñ"), "arrays, exact numeric precision, accents, booleans and nulls");

        var resultado = new DataSet();
        var encabezado = resultado.Tables.Add();
        encabezado.Columns.Add("Estado");
        encabezado.Columns.Add("Total", typeof(decimal));
        encabezado.Rows.Add("Anulada", 0m);
        resultado.Tables.Add().Columns.Add("Producto");
        resultado.Tables.Add().Columns.Add("MetodoPago");
        var auditoria = resultado.Tables.Add();
        auditoria.Columns.Add("Accion");
        auditoria.Columns.Add("DatosAnteriores");
        auditoria.Columns.Add("DatosNuevos");
        auditoria.Rows.Add("ANULAR FACTURA", anterior, nuevo);
        var reporte = ReporteWeb.Factura(resultado, 1);
        var fila = reporte.Tablas[^1].Filas[0];
        Exigir(fila[1].Contains("Estado: Pagada") && fila[2].Contains("Estado: Anulada"), "Web and PDF receive formatted before/after audit values");
        Exigir((string)auditoria.Rows[0]["DatosAnteriores"] == anterior &&
            (string)auditoria.Rows[0]["DatosNuevos"] == nuevo, "source DataSet and historical JSON are unchanged");
        byte[] pdf = ReportePdf.Crear(reporte);
        Exigir(Encoding.ASCII.GetString(pdf, 0, 5) == "%PDF-", "actual report PDF renders with formatted audit data");
        string? destino = Environment.GetEnvironmentVariable("PROYECTOBD_TEST_PDF_PATH");
        if (!string.IsNullOrEmpty(destino))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(destino))!);
            File.WriteAllBytes(destino, pdf);
        }
    }

    private static void Exigir(bool condicion, string nombre)
    {
        if (!condicion) throw new Exception("FAIL " + nombre);
        Console.WriteLine("PASS " + nombre);
    }
}
