using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;

namespace ProyectoBD.Presentacion
{
    public partial class Presentaciones : Form
    {
        private readonly PresentacionNegocio negocio = new PresentacionNegocio();
        private int id;
        public Presentaciones()
        {
            InitializeComponent();
        }

        private void Presentaciones_Load(object? sender, EventArgs e)
        {
            if (DesignMode)
                return;
            try
            {
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
        private void button5_Click(object? sender, EventArgs e)
        {
            Close();
        }
        private void button4_Click(object? sender, EventArgs e)
        {
            try
            {
                if (id <= 0)
                {
                    MessageBox.Show("Seleccione un registro.");
                    return;
                }
                if (MessageBox.Show("¿Eliminar el registro seleccionado?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    negocio.Eliminar(id);
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
                ProyectoBD.Entidades.Presentacion? item = dataGridView1.CurrentRow.DataBoundItem as ProyectoBD.Entidades.Presentacion;
                if (item == null)
                    return;
                id = item.ID_Presentacion;
                textBox1.Text = item.NombrePresentacion;
                textBox2.Text = item.UnidadMedida;

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
            textBox1.Focus();
        }
        private void Cargar()
        {
            dataGridView1.DataSource = negocio.Listar();
            Nuevo();
        }
        private void Guardar(bool nuevo)
        {
            ProyectoBD.Entidades.Presentacion item = new ProyectoBD.Entidades.Presentacion
            {
                ID_Presentacion = id,
                NombrePresentacion = textBox1.Text,
                UnidadMedida = textBox2.Text
            };
            bool guardado;
            if (nuevo)
                guardado = negocio.Guardar(item);
            else
                guardado = negocio.Actualizar(item);
            if (!guardado)
                throw new InvalidOperationException("El registro ya no existe.");
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
