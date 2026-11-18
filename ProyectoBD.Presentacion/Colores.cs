using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;

namespace ProyectoBD.Presentacion
{
    public partial class Colores : Form
    {
        private readonly ColorProductoNegocio negocio = new ColorProductoNegocio();
        private int id;
        public Colores()
        {
            InitializeComponent();
        }

        private void Colores_Load(object? sender, EventArgs e)
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
                ColorProducto? item = dataGridView1.CurrentRow.DataBoundItem as ColorProducto;
                if (item == null)
                    return;
                id = item.ID_Color;
                textBox1.Text = item.NombreColor;

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
            textBox1.Focus();
        }
        private void Cargar()
        {
            dataGridView1.DataSource = negocio.Listar();
            Nuevo();
        }
        private void Guardar(bool nuevo)
        {
            ColorProducto item = new ColorProducto
            {
                ID_Color = id,
                NombreColor = textBox1.Text
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
