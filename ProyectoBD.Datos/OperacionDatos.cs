using System.Data;
using Microsoft.Data.SqlClient;
namespace ProyectoBD.Datos;
// Auxiliar ADO.NET compartido; sólo la capa Datos contiene comandos SQL.
internal static class OperacionDatos
{
    internal static SqlParameter Parametro(string nombre, object? valor)
    {
        var p = new SqlParameter { ParameterName = nombre, Value = valor ?? DBNull.Value };
        switch (valor)
        {
            case int:
                p.SqlDbType = SqlDbType.Int;
                break;
            case decimal:
                p.SqlDbType = SqlDbType.Decimal;
                p.Precision = 12;
                p.Scale = 2;
                break;
            case DateTime:
                p.SqlDbType = SqlDbType.DateTime2;
                break;
            default:
                p.SqlDbType = SqlDbType.NVarChar;
                p.Size = -1;
                break;
        }
        return p;
    }
    internal static DataTable Tabla(string sql, params SqlParameter[] parametros)
    {
        using var c = new ConexionBD().CrearConexion();
        c.Open();
        using var cmd = new SqlCommand(sql, c);
        cmd.Parameters.AddRange(parametros);
        using var lector = cmd.ExecuteReader();
        var tabla = new DataTable();
        tabla.Load(lector);
        return tabla;
    }
    internal static int Ejecutar(string sql, params SqlParameter[] parametros)
    {
        try
        {
            using var c = new ConexionBD().CrearConexion();
            c.Open();
            using var cmd = new SqlCommand(sql, c);
            cmd.Parameters.AddRange(parametros);
            return cmd.ExecuteNonQuery();
        }
        catch (SqlException ex) when (ex.Number == 547) { throw new InvalidOperationException("No se puede eliminar o guardar el registro porque está relacionado con otros datos.", ex); }
        catch (SqlException ex) when (ex.Number is 2601 or 2627) { throw new InvalidOperationException("Ya existe un registro con ese nombre o número.", ex); }
    }
}
