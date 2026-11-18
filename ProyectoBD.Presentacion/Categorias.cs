using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;

namespace ProyectoBD.Presentacion
{
    public partial class Categorias : Form
    {
        private readonly CategoriaNegocio negocio = new CategoriaNegocio();
        private int id;
        public Categorias()
        {
            InitializeComponent();
        }

        private void Categorias_Load(object? sender, EventArgs e)
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
                Categoria? item = dataGridView1.CurrentRow.DataBoundItem as Categoria;
                if (item == null)
                    return;
                id = item.ID_Categoria;
                textBox1.Text = item.NombreCategoria;
                textBox2.Text = item.Descripcion;

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
            Categoria item = new Categoria
            {
                ID_Categoria = id,
                NombreCategoria = textBox1.Text,
                Descripcion = textBox2.Text
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
