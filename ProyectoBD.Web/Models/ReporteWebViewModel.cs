using System.ComponentModel.DataAnnotations;
namespace ProyectoBD.Web.Models;

public class ReporteParametrosViewModel
{
    [Range(1, 7, ErrorMessage = "Seleccione uno de los siete reportes.")]
    public int Tipo { get; set; } = 1;
    [DataType(DataType.Date)] public DateTime? Inicial { get; set; } = new DateTime(DateTime.Today.Year, 1, 1);
    [DataType(DataType.Date)] public DateTime? Final { get; set; } = DateTime.Today;
    public int? NumeroFactura { get; set; }
}
public class ReporteVistaViewModel
{
    public string Token { get; set; } = "";
    public string Titulo { get; set; } = "";
    public string Parametros { get; set; } = "";
    public DateTime Generado { get; set; } = DateTime.Now;
    public string Nota { get; set; } = "";
    public List<TablaReporteViewModel> Tablas { get; set; } = new();
}
public class TablaReporteViewModel
{
    public string Titulo { get; set; } = "";
    public List<string> Columnas { get; set; } = new();
    public List<List<string>> Filas { get; set; } = new();
    public string Totales { get; set; } = "";
}
public class ConsultaFacturaViewModel
{
    public int? NumeroFactura { get; set; }
    public bool Anulacion { get; set; }
    public string Estado { get; set; } = "";
    public ReporteVistaViewModel? Reporte { get; set; }
}
