using System.Data;
using Microsoft.Data.SqlClient;

namespace ProyectoBD.Datos
{
    public class ReporteDatos
    {
        private DataSet Consultar(string procedimiento, params SqlParameter[] parametros)
        {
            using (SqlConnection conexion = new ConexionBD().CrearConexion())
            using (SqlCommand comando = new SqlCommand(procedimiento, conexion))
            {
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddRange(parametros);
                using (SqlDataAdapter adaptador = new SqlDataAdapter(comando))
                {
                    DataSet resultado = new DataSet();
                    adaptador.Fill(resultado);
                    return resultado;
                }
            }
        }

        private DataSet ConsultarPeriodo(string procedimiento, DateTime inicial, DateTime final)
        {
            return Consultar(procedimiento,
                new SqlParameter("@FechaInicial", SqlDbType.Date) { Value = inicial.Date },
                new SqlParameter("@FechaFinal", SqlDbType.Date) { Value = final.Date });
        }

        public DataSet ReporteFacturacion(DateTime inicial, DateTime final)
        {
            return ConsultarPeriodo("dbo.usp_ReporteFacturacionFormasPago", inicial, final);
        }
        public DataTable ReporteProductosMayoresIngresos(DateTime inicial, DateTime final)
        {
            return ConsultarPeriodo("dbo.usp_ReporteProductosMayoresIngresos", inicial, final).Tables[0];
        }
        public DataTable ReporteProductosMasVendidos(DateTime inicial, DateTime final)
        {
            return ConsultarPeriodo("dbo.usp_ReporteProductosMasVendidos", inicial, final).Tables[0];
        }
        public DataTable ReporteInventario()
        {
            return Consultar("dbo.usp_ReporteInventarioActual").Tables[0];
        }
        public DataTable ReporteProductosMenosVendidos(DateTime inicial, DateTime final)
        {
            return ConsultarPeriodo("dbo.usp_ReporteProductosMenosVendidos", inicial, final).Tables[0];
        }
        public DataTable ReporteProductosSinExistencia()
        {
            return Consultar("dbo.usp_ReporteProductosSinExistencia").Tables[0];
        }
        public DataSet BuscarFactura(int numero)
        {
            return Consultar("dbo.usp_ReporteDetalleFactura", new SqlParameter("@NumeroFactura", SqlDbType.Int) { Value = numero });
        }
        public void AnularFactura(int numero, int usuario)
        {
            try
            {
                using (SqlConnection conexion = new ConexionBD().CrearConexion())
                using (SqlCommand comando = new SqlCommand("dbo.usp_AnularFactura", conexion))
                {
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.Parameters.Add("@NumeroFactura", SqlDbType.Int).Value = numero;
                    comando.Parameters.Add("@ID_Usuario", SqlDbType.Int).Value = usuario;
                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
            catch (SqlException ex) when (ex.Number >= 50000)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }
    }
}
