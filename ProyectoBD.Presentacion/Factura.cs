using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;

namespace ProyectoBD.Presentacion
{
    public partial class Factura : Form
    {
        private BindingList<LineaDocumento> lineas = new BindingList<LineaDocumento>();
        private ProcesoNegocio negocio = new ProcesoNegocio(new UsuarioSesion());
        private UsuarioSesion sesion = new UsuarioSesion();
        public Factura()
        {
            InitializeComponent();
        }
        public Factura(UsuarioSesion sesion) : this()
        {
            PermisosNegocio.Exigir(sesion, "Digitador", "Cajero");
            this.sesion = sesion;
            negocio = new ProcesoNegocio(sesion);
        }
        private void Factura_Load(object? sender, EventArgs e)
        {
            if (DesignMode)
                return;
            try
            {
                comboBox3.DisplayMember = "NombreRazonSocial";
                comboBox3.ValueMember = "ID_Cliente";
                comboBox3.DataSource = new ClienteNegocio().Listar();
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
                comboBox2.SelectedValue = 0;
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
        private void button8_Click(object? sender, EventArgs e)
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
                int idCotizacion = Convert.ToInt32(comboBox2.SelectedValue);
                ProyectoBD.Entidades.Factura factura = new ProyectoBD.Entidades.Factura();
                factura.ID_Cliente = Convert.ToInt32(comboBox3.SelectedValue);
                if (idCotizacion > 0)
                    factura.ID_Cotizacion = idCotizacion;
                factura.Detalle = new List<LineaDocumento>(lineas);
                int creada = negocio.Facturar(factura);
                EstablecerDetalle(new List<LineaDocumento>());
                Cargar();
                foreach (DataGridViewRow fila in dataGridView2.Rows)
                {
                    if (Convert.ToInt32(fila.Cells["ID_Factura"].Value) == creada)
                    {
                        dataGridView2.CurrentCell = fila.Cells[0];
                        break;
                    }
                }
                MessageBox.Show("Factura " + creada + " registrada. Inventario actualizado por PEPS. Registre los pagos en Cobrar.");
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
                int idFactura = Seleccionada();
                using (Pagos formularioPagos = new Pagos(sesion, idFactura))
                {
                    formularioPagos.ShowDialog(this);
                }
                CargarHistorial();
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
                ImpresionFactura.Previsualizar(this, negocio, Seleccionada());
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
                if (dataGridView2.CurrentRow != null)
                    dataGridView3.DataSource = negocio.DetalleFactura(Seleccionada());
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }
        private void comboBox2_SelectedIndexChanged(object? sender, EventArgs e)
        {
            try
            {
                DataRowView? fila = comboBox2.SelectedItem as DataRowView;
                if (fila == null)
                    return;
                int idCotizacion = Convert.ToInt32(fila["ID_Cotizacion"]);
                if (idCotizacion > 0)
                {
                    comboBox3.Enabled = false;
                    comboBox3.SelectedValue = Convert.ToInt32(fila["ID_Cliente"]);
                    EstablecerDetalle(negocio.DetalleCotizacion(idCotizacion), false);
                }
                else
                {
                    comboBox3.Enabled = true;
                    EstablecerDetalle(new List<LineaDocumento>());
                }
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }
        private int Seleccionada()

        {

            if (dataGridView2.CurrentRow == null)

                throw new ArgumentException("Seleccione una factura.");

            return Convert.ToInt32(dataGridView2.CurrentRow.Cells["ID_Factura"].Value);

        }

        private void CargarHistorial()
        {
            dataGridView2.DataSource = negocio.Facturas();
        }

        private void Cargar()

        {

            var t = negocio.Cotizaciones();

            var confirmadas = t.Clone();

            confirmadas.Rows.Add(0, DateTime.Today, "Sin cotización", 0m, 0, "Venta directa");

            foreach (DataRow r in t.Rows)

                if ((string)r["Estado"] == "Confirmada")

                    confirmadas.ImportRow(r);

            confirmadas.Columns.Add("Seleccion", typeof(string));

            foreach (DataRow r in confirmadas.Rows)

                r["Seleccion"] = Convert.ToInt32(r["ID_Cotizacion"]) == 0 ? "Sin cotización" : $"{r["ID_Cotizacion"]} - {r["NombreRazonSocial"]}";

            comboBox2.DisplayMember = "Seleccion";

            comboBox2.ValueMember = "ID_Cotizacion";

            comboBox2.DataSource = confirmadas;

            CargarHistorial();

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
                if (variante == null || numericUpDown1.Value < 1 || numericUpDown2.Value <= 0)
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
                nuevaLinea.PrecioUnitario = numericUpDown2.Value;
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
