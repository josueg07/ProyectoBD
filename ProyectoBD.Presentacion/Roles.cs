using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;

namespace ProyectoBD.Presentacion
{
    public partial class Roles : Form
    {
        private readonly RolNegocio rolNegocio = new RolNegocio();
        private int idRolSeleccionado;
        public Roles()
        {
            InitializeComponent();
        }

        private void Roles_Load(object? sender, EventArgs e)
        {
            if (DesignMode)
                return;
            try
            {
                CargarRoles();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }
        private void button1_Click(object? sender, EventArgs e)
        {
            try
            {
                idRolSeleccionado = 0;
                dataGridView1.ClearSelection();
                textBox1.Clear();
                textBox1.Focus();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }
        private void button2_Click(object? sender, EventArgs e)
        {
            try
            {
                Rol rol = new Rol();
                rol.NombreRol = textBox1.Text;
                if (!rolNegocio.Guardar(rol))
                {
                    MessageBox.Show("Ingrese un nombre de hasta 45 caracteres.");
                    return;
                }
                CargarRoles();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }
        private void button3_Click(object? sender, EventArgs e)
        {
            try
            {
                Rol rol = new Rol();
                rol.ID_Rol = idRolSeleccionado;
                rol.NombreRol = textBox1.Text;
                if (!rolNegocio.Actualizar(rol))
                {
                    MessageBox.Show("Seleccione un rol e ingrese su nombre.");
                    return;
                }
                CargarRoles();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }
        private void button4_Click(object? sender, EventArgs e)
        {
            try
            {
                if (idRolSeleccionado <= 0)
                {
                    MessageBox.Show("Seleccione un rol.");
                    return;
                }
                if (MessageBox.Show("¿Eliminar el rol seleccionado?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    rolNegocio.Eliminar(idRolSeleccionado);
                    CargarRoles();
                }
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }
        private void button5_Click(object? sender, EventArgs e)
        {
            Close();
        }
        private void dataGridView1_SelectionChanged(object? sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow == null)
                    return;
                Rol? rol = dataGridView1.CurrentRow.DataBoundItem as Rol;
                if (rol == null)
                    return;
                idRolSeleccionado = rol.ID_Rol;
                textBox1.Text = rol.NombreRol;
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }
        private void CargarRoles()
        {
            dataGridView1.DataSource = rolNegocio.ListarRoles();
            dataGridView1.ClearSelection();
            idRolSeleccionado = 0;
            textBox1.Clear();
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
