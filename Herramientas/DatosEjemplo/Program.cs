using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using ProyectoBD.Datos;
using ProyectoBD.Negocio;

try
{
    using SqlConnection conexion = new ConexionBD().CrearConexion();
    conexion.Open();
    conexion.InfoMessage += (_, e) => Console.WriteLine(e.Message);
    if (!args.Contains("--resumen"))
    {
        string script = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, args.Contains("--actualizar-usuarios") ? "ActualizarUsuariosEjemplo.sql" : "DatosEjemplo.sql"));
        using SqlCommand comando = new SqlCommand(script, conexion);
        comando.CommandTimeout = 120;
        foreach (var cuenta in new[] { (Parametro: "@HashCajero", Password: "cajero123"), (Parametro: "@HashGerente", Password: "gerente 123") })
        {
            // Contraseña pública exclusivamente para las cuentas de ejemplo.
            // SQL Server sólo recibe el hash BCrypt, nunca el texto original.
            comando.Parameters.Add(cuenta.Parametro, SqlDbType.NVarChar, 255).Value = BCrypt.Net.BCrypt.HashPassword(cuenta.Password);
        }
        comando.ExecuteNonQuery();
    }
    using SqlCommand resumen = new SqlCommand("SELECT DatosNuevos FROM dbo.tbl_Auditoria WHERE TablaAfectada='DATOS_EJEMPLO' AND RegistroAfectado='BD2_EJEMPLO_V1' AND Accion='CARGA EJEMPLO'", conexion);
    string manifiesto = Convert.ToString(resumen.ExecuteScalar()) ?? "";
    if (string.IsNullOrEmpty(manifiesto)) throw new InvalidOperationException("No existe la carga de ejemplo.");
    using JsonDocument datos = JsonDocument.Parse(manifiesto);
    Console.WriteLine("\nIDENTIFICADORES DE LOS DATOS:");
    foreach (JsonProperty propiedad in datos.RootElement.EnumerateObject()) Console.WriteLine(propiedad.Name + ": " + propiedad.Value);
    File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "ManifiestoEjemplo.json"), manifiesto);
    if (!args.Contains("--resumen"))
    {
        UsuarioNegocio negocio = new UsuarioNegocio();
        foreach (var cuenta in new[] { (Usuario: "cajero", Password: "cajero123"), (Usuario: "gerente", Password: "gerente 123") })
        {
            var sesion = negocio.IniciarSesion(cuenta.Usuario, cuenta.Password);
            if (sesion == null) throw new InvalidOperationException("Falló la comprobación de acceso de " + cuenta.Usuario);
            Console.WriteLine("Acceso BCrypt verificado: " + cuenta.Usuario + " / " + sesion.NombreRol);
        }
    }
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine("No se pudo completar la carga: " + ex.Message);
    return 1;
}
