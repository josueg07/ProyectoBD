using ProyectoBD.Negocio;
using ProyectoBD.Entidades;


namespace ProyectoBD.Presentacion
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string usuario = textBox1.Text.Trim();
            string password = textBox2.Text;

            if (string.IsNullOrWhiteSpace(usuario))
            {
                MessageBox.Show(
                    "Ingrese su nombre de usuario.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                textBox1.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(
                    "Ingrese su contraseña.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                textBox2.Focus();
                return;
            }

            string etapa = "validar las credenciales";
            try
            {
                UsuarioNegocio usuarioNegocio =
                    new UsuarioNegocio();

                UsuarioSesion? sesion =
                    usuarioNegocio.IniciarSesion(
                        usuario,
                        password
                    );

                if (sesion == null)
                {
                    MessageBox.Show(
                        "Usuario o contraseña incorrectos.",
                        "Acceso denegado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );

                    textBox2.Clear();
                    textBox2.Focus();

                    return;
                }

                MessageBox.Show(
                    $"Bienvenido {sesion.NombreCompleto}\n" +
                    $"Rol: {sesion.NombreRol}",
                    "Inicio de sesión",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                etapa = "abrir el menú";
                Menu menu = new Menu(sesion);
                this.Hide();
                menu.ShowDialog();
                if (menu.DialogResult == DialogResult.Retry) { textBox2.Clear(); this.Show(); textBox1.Focus(); }
                else this.Close();

            }
            catch (Exception ex)
            {
                this.Show();
                string? archivo = RegistrarErrorInicioSesion(ex, etapa);
                string mensaje = "No se pudo completar el inicio de sesión al " + etapa + ".";
                mensaje += Environment.NewLine + "Tipo de error: " + ex.GetType().Name;
                if (archivo != null)
                {
                    mensaje += Environment.NewLine + "Se guardó el detalle en: " + archivo;
                }
                MessageBox.Show(mensaje, "Error de inicio de sesión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string? RegistrarErrorInicioSesion(Exception ex, string etapa)
        {
            try
            {
                string carpeta = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ProyectoBD");
                System.IO.Directory.CreateDirectory(carpeta);
                string archivo = System.IO.Path.Combine(carpeta, "login.log");
                string detalle = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " | " + etapa;
                detalle += Environment.NewLine + "Directorio: " + AppContext.BaseDirectory;
                detalle += Environment.NewLine + "Proceso de 64 bits: " + Environment.Is64BitProcess;
                detalle += Environment.NewLine + "Runtime: " + System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
                detalle += Environment.NewLine + "Versión de datos: " + typeof(ProyectoBD.Datos.ConexionBD).Assembly.ManifestModule.ModuleVersionId;
                detalle += Environment.NewLine + ex.ToString() + Environment.NewLine + Environment.NewLine;
                System.IO.File.AppendAllText(archivo, detalle);
                System.Diagnostics.Debug.WriteLine(detalle);
                return archivo;
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return null;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
        }
    }
}
