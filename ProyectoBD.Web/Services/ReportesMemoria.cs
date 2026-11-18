using Microsoft.Extensions.Caching.Memory;
using ProyectoBD.Web.Models;

namespace ProyectoBD.Web.Services;

public class ReportesMemoria
{
    private readonly IMemoryCache cache;
    public ReportesMemoria(IMemoryCache cache) { this.cache = cache; }
    public void Guardar(int usuario, ReporteVistaViewModel reporte)
    {
        reporte.Token = Guid.NewGuid().ToString("N");
        cache.Set($"reporte:{usuario}:{reporte.Token}", reporte, TimeSpan.FromMinutes(20));
    }
    public ReporteVistaViewModel? Obtener(int usuario, string token)
    {
        if (!Guid.TryParseExact(token, "N", out _)) return null;
        return cache.Get<ReporteVistaViewModel>($"reporte:{usuario}:{token}");
    }
}
