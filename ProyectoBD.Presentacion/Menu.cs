using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;

namespace ProyectoBD.Presentacion
{
    public partial class Menu : Form
    {
        private UsuarioSesion usuarioSesion = new UsuarioSesion();
        public Menu()
        {
            InitializeComponent();
        }
        public Menu(UsuarioSesion sesion) : this()
        {
            usuarioSesion = sesion;
            ConfigurarPermisos();
            label3.Text = "Usuario: " + usuarioSesion.NombreCompleto;
            label1.Text = "Rol: " + usuarioSesion.NombreRol;
        }
        private void ConfigurarPermisos()
        {
            toolStripMenuItem4.Visible = false;
            toolStripMenuItem5.Visible = false;
            toolStripMenuItem6.Visible = false;
            toolStripMenuItem26.Visible = false;
            toolStripMenuItem7.Visible = false;
            switch (usuarioSesion.NombreRol)
            {
                case "Digitador":
                    ConfigurarDigitador();
                    break;
                case "Cajero":
                    ConfigurarCajero();
                    break;
                case "Gerente":
                    ConfigurarGerente();
                    break;
                default:
                    MessageBox.Show("El usuario no tiene un rol válido.");
                    break;
            }
        }
        private void ConfigurarDigitador()
        {
            toolStripMenuItem4.Visible = true;
            toolStripMenuItem6.Visible = true;
        }
        private void ConfigurarCajero()
        {
            toolStripMenuItem26.Visible = true;
        }
        private void ConfigurarGerente()
        {
            toolStripMenuItem5.Visible = true;
            toolStripMenuItem7.Visible = true;
        }
        private void toolStripMenuItem10_Click(object? sender, EventArgs e)
        {
            try
            {
                using (Productos formulario = new Productos())
                {
                    Hide();
                    formulario.ShowDialog();
                }
                Show();
            }
            catch (Exception ex)
            {
                Show();
                MostrarError(ex);
            }
        }
        private void toolStripMenuItem11_Click(object? sender, EventArgs e)
        {
            try
            {
                using (Clientes formulario = new Clientes())
                {
                    Hide();
                    formulario.ShowDialog();
                }
                Show();
            }
            catch (Exception ex)
            {
                Show();
                MostrarError(ex);
            }
        }
        private void toolStripMenuItem12_Click(object? sender, EventArgs e)
        {
            try
            {
                using (Proveedores formulario = new Proveedores())
                {
                    Hide();
                    formulario.ShowDialog();
                }
                Show();
            }
            catch (Exception ex)
            {
                Show();
                MostrarError(ex);
            }
        }
        private void toolStripMenuItem13_Click(object? sender, EventArgs e)
        {
            try
            {
                using (Marcas formulario = new Marcas())
                {
                    Hide();
                    formulario.ShowDialog();
                }
                Show();
            }
            catch (Exception ex)
            {
                Show();
                MostrarError(ex);
            }
        }
        private void toolStripMenuItem14_Click(object? sender, EventArgs e)
        {
            try
            {
                using (Presentaciones formulario = new Presentaciones())
                {
                    Hide();
                    formulario.ShowDialog();
                }
                Show();
            }
            catch (Exception ex)
            {
                Show();
                MostrarError(ex);
            }
        }
        private void toolStripMenuItem15_Click(object? sender, EventArgs e)
        {
            try
            {
                using (Colores formulario = new Colores())
                {
                    Hide();
                    formulario.ShowDialog();
                }
                Show();
            }
            catch (Exception ex)
            {
                Show();
                MostrarError(ex);
            }
        }
        private void toolStripMenuItem25_Click(object? sender, EventArgs e)
        {
            try
            {
                using (Categorias formulario = new Categorias())
                {
                    Hide();
                    formulario.ShowDialog();
                }
                Show();
            }
            catch (Exception ex)
            {
                Show();
                MostrarError(ex);
            }
        }
        private void toolStripMenuItem16_Click(object? sender, EventArgs e)
        {
            try
            {
                using (Usuarios formulario = new Usuarios(usuarioSesion))
                {
                    Hide();
                    formulario.ShowDialog();
                }
                Show();
            }
            catch (Exception ex)
            {
                Show();
                MostrarError(ex);
            }
        }
        private void toolStripMenuItem17_Click(object? sender, EventArgs e)
        {
            try
            {
                using (Roles formulario = new Roles())
                {
                    Hide();
                    formulario.ShowDialog();
                }
                Show();
            }
            catch (Exception ex)
            {
                Show();
                MostrarError(ex);
            }
        }
        private void toolStripMenuItem18_Click(object? sender, EventArgs e)
        {
            try
            {
                using (Cotizacion formulario = new Cotizacion(usuarioSesion))
                {
                    Hide();
                    formulario.ShowDialog();
                }
                Show();
            }
            catch (Exception ex)
            {
                Show();
                MostrarError(ex);
            }
        }
        private void toolStripMenuItem20_Click(object? sender, EventArgs e)
        {
            try
            {
                using (OrdenCompra formulario = new OrdenCompra(usuarioSesion))
                {
                    Hide();
                    formulario.ShowDialog();
                }
                Show();
            }
            catch (Exception ex)
            {
                Show();
                MostrarError(ex);
            }
        }
        private void toolStripMenuItem19_Click(object? sender, EventArgs e)
        {
            try
            {
                using (Factura formulario = new Factura(usuarioSesion))
                {
                    Hide();
                    formulario.ShowDialog();
                }
                Show();
            }
            catch (Exception ex)
            {
                Show();
                MostrarError(ex);
            }
        }
        private void toolStripMenuItem27_Click(object? sender, EventArgs e)
        {
            try
            {
                using (Factura formulario = new Factura(usuarioSesion))
                {
                    Hide();
                    formulario.ShowDialog();
                }
                Show();
            }
            catch (Exception ex)
            {
                Show();
                MostrarError(ex);
            }
        }
        private void toolStripMenuItem28_Click(object? sender, EventArgs e)
        {
            try
            {
                using (MetodosPago formulario = new MetodosPago())
                {
                    Hide();
                    formulario.ShowDialog();
                }
                Show();
            }
            catch (Exception ex)
            {
                Show();
                MostrarError(ex);
            }
        }
        private void toolStripMenuItem1_Click(object? sender, EventArgs e)
        {
            try
            {
                using (Ubicaciones formulario = new Ubicaciones())
                {
                    Hide();
                    formulario.ShowDialog();
                }
                Show();
            }
            catch (Exception ex)
            {
                Show();
                MostrarError(ex);
            }
        }
        private void toolStripMenuItem21_Click(object? sender, EventArgs e)
        {
            try
            {
                using (IngresoBodega formulario = new IngresoBodega(usuarioSesion))
                {
                    Hide();
                    formulario.ShowDialog();
                }
                Show();
            }
            catch (Exception ex)
            {
                Show();
                MostrarError(ex);
            }
        }
        private void toolStripMenuItem22_Click(object? sender, EventArgs e)
        {
            try
            {
                ProcesoNegocio procesoNegocio = new ProcesoNegocio(usuarioSesion);
                DataTable datos = procesoNegocio.Facturas();
                using (Reportes formulario = new Reportes("Ventas", datos))
                {
                    Hide();
                    formulario.ShowDialog();
                }
                Show();
            }
            catch (Exception ex)
            {
                Show();
                MostrarError(ex);
            }
        }
        private void toolStripMenuItem23_Click(object? sender, EventArgs e)
        {
            try
            {
                ProcesoNegocio procesoNegocio = new ProcesoNegocio(usuarioSesion);
                DataTable datos = procesoNegocio.Inventario();
                using (Reportes formulario = new Reportes("Inventario por lote", datos))
                {
                    Hide();
                    formulario.ShowDialog();
                }
                Show();
            }
            catch (Exception ex)
            {
                Show();
                MostrarError(ex);
            }
        }
        private void toolStripMenuItem24_Click(object? sender, EventArgs e)
        {
            try
            {
                ProcesoNegocio procesoNegocio = new ProcesoNegocio(usuarioSesion);
                DataTable datos = procesoNegocio.Ingresos();
                using (Reportes formulario = new Reportes("Ingresos a bodega", datos))
                {
                    Hide();
                    formulario.ShowDialog();
                }
                Show();
            }
            catch (Exception ex)
            {
                Show();
                MostrarError(ex);
            }
        }
        private void toolStripMenuItem29_Click(object? sender, EventArgs e)
        {
            try
            {
                using (Reportes formulario = new Reportes(usuarioSesion))
                {
                    Hide();
                    formulario.ShowDialog();
                }
                Show();
            }
            catch (Exception ex) { Show(); MostrarError(ex); }
        }
        private void toolStripMenuItem30_Click(object? sender, EventArgs e)
        {
            try
            {
                using (ConsultaFactura formulario = new ConsultaFactura(usuarioSesion))
                {
                    Hide();
                    formulario.ShowDialog();
                }
                Show();
            }
            catch (Exception ex) { Show(); MostrarError(ex); }
        }
        private void toolStripMenuItem31_Click(object? sender, EventArgs e)
        {
            try
            {
                using (ConsultaFactura formulario = new ConsultaFactura(usuarioSesion, true))
                {
                    Hide();
                    formulario.ShowDialog();
                }
                Show();
            }
            catch (Exception ex) { Show(); MostrarError(ex); }
        }
        private void toolStripMenuItem3_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Retry;
            Close();
        }
        private void label2_Click(object? sender, EventArgs e)
        {

        }
        private void label3_Click(object? sender, EventArgs e)
        {

        }
        private void Menu_Load(object? sender, EventArgs e)
        {
            if (DesignMode)
                return;

        }
        private void toolStripMenuItem4_Click(object? sender, EventArgs e)
        {

        }


        private void MostrarError(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            string mensaje = "No se pudo completar la operación. Revise la conexión y los datos ingresados.";
            if (ex is ArgumentException || ex is InvalidOperationException)
            {
                mensaje = ex.Message;
            }
            MessageBox.Show(mensaje, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

    }
}
