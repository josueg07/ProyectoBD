using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;

namespace ProyectoBD.Presentacion
{
    public partial class Productos : Form
    {
        private readonly ProductoNegocio negocio = new ProductoNegocio();
        private int idProducto;
        private int idVariante;
        public Productos()
        {
            InitializeComponent();
        }

        private void Productos_Load(object? sender, EventArgs e)
        {
            if (DesignMode)
                return;
            try
            {
                Enlazar(comboBox1, new CategoriaNegocio().Listar(), "NombreCategoria", "ID_Categoria");
                Enlazar(comboBox2, new MarcaNegocio().Listar(), "NombreMarca", "ID_Marca");
                Enlazar(comboBox3, new PresentacionNegocio().Listar(), "NombrePresentacion", "ID_Presentacion");
                List<ColorProducto> colores = new ColorProductoNegocio().Listar();
                colores.Insert(0, new ColorProducto { ID_Color = 0, NombreColor = "Sin color" });
                Enlazar(comboBox4, colores, "NombreColor", "ID_Color");
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
                idProducto = 0;
                dataGridView1.ClearSelection();
                textBox1.Clear();
                textBox2.Clear();
                CargarVariantes();
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
        private void button9_Click(object? sender, EventArgs e)
        {
            Close();
        }
        private void button5_Click(object? sender, EventArgs e)
        {
            try
            {
                idVariante = 0;
                dataGridView2.ClearSelection();
                numericUpDown1.Value = 0;
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }
        private void button6_Click(object? sender, EventArgs e)
        {
            try
            {
                GuardarVariante(true);
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }
        private void button7_Click(object? sender, EventArgs e)
        {
            try
            {
                GuardarVariante(false);
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
                if (idProducto <= 0)
                {
                    MessageBox.Show("Seleccione el registro que desea eliminar.");
                    return;
                }
                if (MessageBox.Show("¿Eliminar el registro seleccionado?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    negocio.Eliminar(idProducto);
                    Cargar();
                }
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }
        private void button8_Click(object? sender, EventArgs e)
        {
            try
            {
                if (idVariante <= 0)
                {
                    MessageBox.Show("Seleccione el registro que desea eliminar.");
                    return;
                }
                if (MessageBox.Show("¿Eliminar el registro seleccionado?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    negocio.EliminarVariante(idVariante);
                    CargarVariantes();
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
                Producto? item = dataGridView1.CurrentRow.DataBoundItem as Producto;
                if (item == null)
                    return;
                idProducto = item.ID_Producto;
                textBox1.Text = item.NombreProducto;
                textBox2.Text = item.Descripcion;
                comboBox1.SelectedValue = item.ID_CategoriaP;
                comboBox2.SelectedValue = item.ID_Marca;
                CargarVariantes();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }
        private void dataGridView2_SelectionChanged(object? sender, EventArgs e)
        {
            try
            {
                if (dataGridView2.CurrentRow == null)
                    return;
                ProductoVariante? item = dataGridView2.CurrentRow.DataBoundItem as ProductoVariante;
                if (item == null)
                    return;
                idVariante = item.ID_ProductoV;
                comboBox3.SelectedValue = item.ID_Presentacion;
                comboBox4.SelectedValue = item.ID_Color ?? 0;
                numericUpDown1.Value = item.PrecioVenta;
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }
        private static void Enlazar(ComboBox c, object data, string nombre, string id)

        {

            c.DisplayMember = nombre;

            c.ValueMember = id;

            c.DataSource = data;

        }

        private void Cargar()

        {

            dataGridView1.DataSource = negocio.Listar();

            if (dataGridView1.Rows.Count == 0)

            {

                idProducto = 0;

                textBox1.Clear();

                textBox2.Clear();

                CargarVariantes();

            }

        }

        private void CargarVariantes()

        {

            idVariante = 0;

            dataGridView2.DataSource = negocio.ListarVariantes(idProducto);

            dataGridView2.ClearSelection();

        }

        private void Guardar(bool nuevo)

        {

            if (!negocio.Guardar(new Producto { ID_Producto = idProducto, NombreProducto = textBox1.Text, Descripcion = textBox2.Text, ID_CategoriaP = Convert.ToInt32(comboBox1.SelectedValue), ID_Marca = Convert.ToInt32(comboBox2.SelectedValue) }, nuevo))

                throw new InvalidOperationException("El producto ya no existe.");

            Cargar();

        }

        private void GuardarVariante(bool nuevo)

        {

            int color = Convert.ToInt32(comboBox4.SelectedValue);

            if (!negocio.GuardarVariante(new ProductoVariante { ID_ProductoV = idVariante, ID_Producto = idProducto, ID_Presentacion = Convert.ToInt32(comboBox3.SelectedValue), ID_Color = color == 0 ? null : color, PrecioVenta = numericUpDown1.Value }, nuevo))

                throw new InvalidOperationException("La variante ya no existe.");

            CargarVariantes();

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
