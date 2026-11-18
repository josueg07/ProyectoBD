using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace ProyectoBD.Datos
{
    public class ConexionBD
    {
        private const string VariableConexion =
            "PROYECTOBD_CONNECTION_STRING";
        private readonly string cadenaConexion =
           @"Server=(localdb)\MSSQLLocalDB;
              Database=Proyecto_BD;
              Integrated Security=True;
              TrustServerCertificate=True;";

        public SqlConnection CrearConexion()
        {
            // Docker, Dokploy o Kubernetes
            string? cadenaEntorno =
                Environment.GetEnvironmentVariable(
                    VariableConexion
                );

            if (!string.IsNullOrWhiteSpace(cadenaEntorno))
            {
                return new SqlConnection(cadenaEntorno);
            }

            // Desarrollo local en Windows
            SqlConnectionStringBuilder cadena =
                new SqlConnectionStringBuilder(cadenaConexion);

            string servidor = ObtenerCanalLocalDB();

            if (!string.IsNullOrEmpty(servidor))
            {
                cadena.DataSource = servidor;
            }

            return new SqlConnection(
                cadena.ConnectionString
            );
        }

        private static string ObtenerCanalLocalDB()
        {
            try
            {
                string programa = "SqlLocalDB.exe";
                string carpetaSQL = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Microsoft SQL Server");
                foreach (string version in new[] { "170", "160", "150", "140", "130", "120" })
                {
                    string ruta = System.IO.Path.Combine(carpetaSQL, version, "Tools", "Binn", "SqlLocalDB.exe");
                    if (System.IO.File.Exists(ruta))
                    {
                        programa = ruta;
                        break;
                    }
                }

                string informacion = EjecutarLocalDB(programa, "info");
                string canal = BuscarCanal(informacion);
                if (string.IsNullOrEmpty(canal))
                {
                    // Inicia sólo la instancia existente, sin crear otra base ni modificar sus archivos.
                    EjecutarLocalDB(programa, "start");
                    canal = BuscarCanal(EjecutarLocalDB(programa, "info"));
                }
                return canal;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                RegistrarDiagnostico(ex.ToString());
                // En otros equipos se conserva la conexión normal por nombre de instancia.
                return string.Empty;
            }
        }

        private static string EjecutarLocalDB(string programa, string accion)
        {
            ProcessStartInfo inicio = new ProcessStartInfo();
            inicio.FileName = programa;
            inicio.ArgumentList.Add(accion);
            inicio.ArgumentList.Add("MSSQLLocalDB");
            inicio.UseShellExecute = false;
            inicio.CreateNoWindow = true;
            inicio.RedirectStandardOutput = true;
            inicio.RedirectStandardError = true;
            using (Process? proceso = Process.Start(inicio))
            {
                if (proceso == null) return string.Empty;
                Task<string> salida = proceso.StandardOutput.ReadToEndAsync();
                Task<string> error = proceso.StandardError.ReadToEndAsync();
                if (!proceso.WaitForExit(15000))
                {
                    proceso.Kill();
                    throw new TimeoutException("LocalDB tardó demasiado en responder.");
                }
                if (proceso.ExitCode != 0)
                {
                    RegistrarDiagnostico(programa + " " + accion + " | " + proceso.ExitCode + " | " + salida.GetAwaiter().GetResult() + " | " + error.GetAwaiter().GetResult());
                    return string.Empty;
                }
                return salida.GetAwaiter().GetResult();
            }
        }

        private static void RegistrarDiagnostico(string mensaje)
        {
            try
            {
                string carpeta = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ProyectoBD");
                System.IO.Directory.CreateDirectory(carpeta);
                System.IO.File.AppendAllText(System.IO.Path.Combine(carpeta, "localdb.log"), DateTime.Now.ToString("s") + " | " + mensaje + Environment.NewLine);
            }
            catch { }
        }

        private static string BuscarCanal(string informacion)
        {
            foreach (string linea in informacion.Split('\n'))
            {
                int posicion = linea.IndexOf(@"np:\\.\pipe\", StringComparison.OrdinalIgnoreCase);
                if (posicion >= 0) return linea.Substring(posicion).Trim();
            }
            return string.Empty;
        }

        public bool ProbarConexion()
        {
            try
            {
                using (SqlConnection conexion = CrearConexion())
                {
                    conexion.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
