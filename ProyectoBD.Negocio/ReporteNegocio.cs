using System.Data;
using ProyectoBD.Datos;
using ProyectoBD.Entidades;

namespace ProyectoBD.Negocio
{
    public class ReporteNegocio
    {
        private readonly ReporteDatos datos = new ReporteDatos();
        private readonly UsuarioSesion sesion;

        public ReporteNegocio(UsuarioSesion sesion)
        {
            this.sesion = sesion;
        }
        private void ValidarPermiso()
        {
            PermisosNegocio.Exigir(sesion, "Gerente");
        }
        private void ValidarFechas(DateTime inicial, DateTime final)
        {
            ValidarPermiso();
            if (inicial.Date > final.Date)
                throw new ArgumentException("La fecha inicial no puede ser mayor que la fecha final.");
        }
        public DataSet ReporteFacturacion(DateTime inicial, DateTime final)
        {
            ValidarFechas(inicial, final);
            return datos.ReporteFacturacion(inicial, final);
        }
        public DataTable ReporteProductosMayoresIngresos(DateTime inicial, DateTime final)
        {
            ValidarFechas(inicial, final);
            return datos.ReporteProductosMayoresIngresos(inicial, final);
        }
        public DataTable ReporteProductosMasVendidos(DateTime inicial, DateTime final)
        {
            ValidarFechas(inicial, final);
            return datos.ReporteProductosMasVendidos(inicial, final);
        }
        public DataTable ReporteInventario()
        {
            ValidarPermiso();
            return datos.ReporteInventario();
        }
        public DataTable ReporteProductosMenosVendidos(DateTime inicial, DateTime final)
        {
            ValidarFechas(inicial, final);
            return datos.ReporteProductosMenosVendidos(inicial, final);
        }
        public DataTable ReporteProductosSinExistencia()
        {
            ValidarPermiso();
            return datos.ReporteProductosSinExistencia();
        }
        private void ValidarNumero(int numero)
        {
            ValidarPermiso();
            if (numero <= 0)
                throw new ArgumentException("Ingrese un número de factura entero y positivo.");
        }
        public DataSet BuscarFactura(int numero)
        {
            ValidarNumero(numero);
            return datos.BuscarFactura(numero);
        }
        public void AnularFactura(int numero)
        {
            ValidarNumero(numero);
            datos.AnularFactura(numero, sesion.ID_Usuario);
        }
    }
}
