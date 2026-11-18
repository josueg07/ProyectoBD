using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;

namespace ProyectoBD.Presentacion
{
    public partial class Usuarios : Form
    {
        private readonly UsuarioNegocio negocio = new UsuarioNegocio();
        private UsuarioSesion sesion = new UsuarioSesion();
        private int id;
        public Usuarios()
        {
            InitializeComponent();
        }
        public Usuarios(UsuarioSesion sesion) : this()
        {
            PermisosNegocio.Exigir(sesion, "Gerente");
            this.sesion = sesion;
        }
        private void Usuarios_Load(object? sender, EventArgs e)
        {
            if (DesignMode)
                return;
            try
            {
                comboBox1.DisplayMember = "NombreRol";
                comboBox1.ValueMember = "ID_Rol";
                comboBox1.DataSource = new RolNegocio().ListarRoles();
                Cargar();
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
                Nuevo();
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
                Guardar(true);
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
                Guardar(false);
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }
        private void button6_Click(object? sender, EventArgs e)
        {
            Close();
        }
        private void button4_Click(object? sender, EventArgs e)
        {
            try
            {
                negocio.CambiarPassword(id, textBox4.Text, sesion);
                textBox4.Clear();
                MessageBox.Show("Contraseña actualizada.");
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }
        private void button5_Click(object? sender, EventArgs e)
        {
            try
            {
                if (id <= 0)
                {
                    MessageBox.Show("Seleccione un usuario.");
                    return;
                }
                if (MessageBox.Show("¿Eliminar el usuario seleccionado?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    negocio.Eliminar(id, sesion);
                    Cargar();
                }
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }
        private void dataGridView1_SelectionChanged(object? sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow == null)
                    return;
                Usuario? usuario = dataGridView1.CurrentRow.DataBoundItem as Usuario;
                if (usuario == null)
                    return;
                id = usuario.ID_Usuario;
                textBox1.Text = usuario.Nombres;
                textBox2.Text = usuario.Apellidos;
                textBox3.Text = usuario.NombreUsuario;
                comboBox1.SelectedValue = usuario.ID_Rol;
                textBox4.Clear();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }
        private void Nuevo()

        {

            id = 0;

            dataGridView1.ClearSelection();

            textBox1.Clear();

            textBox2.Clear();

            textBox3.Clear();

            textBox4.Clear();

        }

        private void Cargar()

        {

            dataGridView1.DataSource = negocio.Listar(sesion);

            dataGridView1.Columns["PasswordHash"].Visible = false;

            dataGridView1.Columns["ID_Rol"].Visible = false;

            dataGridView1.Columns["Nombres"].Visible = false;

            dataGridView1.Columns["Apellidos"].Visible = false;

            Nuevo();

        }

        private void Guardar(bool nuevo)

        {

            negocio.Guardar(new Usuario { ID_Usuario = id, Nombres = textBox1.Text, Apellidos = textBox2.Text, NombreUsuario = textBox3.Text, ID_Rol = Convert.ToInt32(comboBox1.SelectedValue) }, textBox4.Text, sesion, nuevo);

            Cargar();

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
