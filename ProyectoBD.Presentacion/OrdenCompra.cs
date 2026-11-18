using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;

namespace ProyectoBD.Presentacion
{
    public partial class OrdenCompra : Form
    {
        private BindingList<LineaDocumento> lineas = new BindingList<LineaDocumento>();
        private ProcesoNegocio negocio = new ProcesoNegocio(new UsuarioSesion());
        public OrdenCompra()
        {
            InitializeComponent();
        }
        public OrdenCompra(UsuarioSesion sesion) : this()
        {
            PermisosNegocio.Exigir(sesion, "Digitador");
            negocio = new ProcesoNegocio(sesion);
        }
        private void OrdenCompra_Load(object? sender, EventArgs e)
        {
            if (DesignMode)
                return;
            try
            {
                comboBox2.DisplayMember = "Nombre_Proveedor";
                comboBox2.ValueMember = "ID_Proveedor";
                comboBox2.DataSource = new ProveedorNegocio().Listar();
                CargarProductos();
                Cargar();
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
                EstablecerDetalle(new List<LineaDocumento>());
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }
        private void button3_Click(object? sender, EventArgs e)
        {
            Close();
        }
        private void button6_Click(object? sender, EventArgs e)
        {
            try
            {
                Cargar();
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
                ProyectoBD.Entidades.OrdenCompra orden = new ProyectoBD.Entidades.OrdenCompra();
                orden.ID_Proveedor = Convert.ToInt32(comboBox2.SelectedValue);
                orden.Detalle = new List<LineaDocumento>(lineas);
                int creado = negocio.GuardarOrden(orden);
                EstablecerDetalle(new List<LineaDocumento>());
                Cargar();
                MessageBox.Show("Orden " + creado + " guardada.");
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
                int idOrden = Convert.ToInt32(dataGridView2.CurrentRow.Cells["ID_OrdenC"].Value);
                dataGridView3.DataSource = negocio.PendienteOrden(idOrden);
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }
        private void Cargar()
        {
            dataGridView2.DataSource = negocio.Ordenes();
        }


        private void CargarProductos()
        {
            dataGridView1.DataSource = lineas;
            comboBox1.DisplayMember = "DescripcionVariante";
            comboBox1.ValueMember = "ID_ProductoV";
            ProductoNegocio productoNegocio = new ProductoNegocio();
            comboBox1.DataSource = productoNegocio.ListarVariantes();
            ConfigurarDetalle();
        }
        private void EstablecerDetalle(List<LineaDocumento> detalle, bool editable = true)
        {
            lineas = new BindingList<LineaDocumento>(detalle);
            dataGridView1.DataSource = lineas;
            comboBox1.Enabled = editable;
            numericUpDown1.Enabled = editable;
            numericUpDown2.Enabled = editable;
            button1.Enabled = editable;
            button2.Enabled = editable;
            ConfigurarDetalle();
            ActualizarTotal();
        }
        private void ConfigurarDetalle()
        {
            if (dataGridView1.Columns.Contains("ID_ProductoV"))
                dataGridView1.Columns["ID_ProductoV"].Visible = false;
            if (dataGridView1.Columns.Contains("PrecioUnitario"))
                dataGridView1.Columns["PrecioUnitario"].Visible = false;
            if (dataGridView1.Columns.Contains("Subtotal"))
                dataGridView1.Columns["Subtotal"].Visible = false;
        }
        private void ActualizarTotal()
        {
            decimal suma = 0;
            foreach (LineaDocumento linea in lineas)
            {
                suma += linea.Subtotal;
            }
            label5.Text = "Total Q" + suma.ToString("N2");
        }
        private void comboBox1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            ProductoVariante? variante = comboBox1.SelectedItem as ProductoVariante;
            if (variante != null)
                numericUpDown2.Value = variante.PrecioVenta;
        }
        private void button1_Click(object? sender, EventArgs e)
        {
            try
            {
                ProductoVariante? variante = comboBox1.SelectedItem as ProductoVariante;
                if (variante == null || numericUpDown1.Value < 1)
                {
                    MessageBox.Show("Seleccione una variante y complete los datos del detalle.");
                    return;
                }
                foreach (LineaDocumento linea in lineas)
                {
                    if (linea.ID_ProductoV == variante.ID_ProductoV)
                    {
                        MessageBox.Show("La variante ya está agregada. Quite la línea para cambiarla.");
                        return;
                    }
                }
                LineaDocumento nuevaLinea = new LineaDocumento();
                nuevaLinea.ID_ProductoV = variante.ID_ProductoV;
                nuevaLinea.Producto = variante.DescripcionVariante;
                nuevaLinea.Cantidad = (int)numericUpDown1.Value;
                lineas.Add(nuevaLinea);
                ActualizarTotal();
            }
            catch (Exception ex) { MostrarError(ex); }
        }
        private void button2_Click(object? sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
                return;
            LineaDocumento? linea = dataGridView1.CurrentRow.DataBoundItem as LineaDocumento;
            if (linea != null)
                lineas.Remove(linea);
            ActualizarTotal();
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
