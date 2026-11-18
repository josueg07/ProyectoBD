using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;

namespace ProyectoBD.Presentacion
{
    public partial class Pagos : Form
    {
        private ProcesoNegocio negocio = new ProcesoNegocio(new UsuarioSesion());
        private int idFactura;
        private readonly BindingList<PagoFactura> pagos = new BindingList<PagoFactura>();
        public Pagos()
        {
            InitializeComponent();
        }
        public Pagos(UsuarioSesion sesion, int idFactura) : this()
        {
            PermisosNegocio.Exigir(sesion, "Digitador", "Cajero");
            negocio = new ProcesoNegocio(sesion);
            this.idFactura = idFactura;
            label1.Text = "Cobro de factura " + idFactura;
        }
        private void Pagos_Load(object? sender, EventArgs e)
        {
            if (DesignMode)
                return;
            try
            {
                dataGridView1.DataSource = pagos;
                comboBox1.DisplayMember = "NombreMetodo";
                comboBox1.ValueMember = "ID_MetodoP";
                comboBox1.DataSource = new MetodoPagoNegocio().Listar();
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
                MetodoPago? metodo = comboBox1.SelectedItem as MetodoPago;
                if (metodo == null || numericUpDown1.Value <= 0)
                {
                    MessageBox.Show("Seleccione método y monto positivo.");
                    return;
                }
                PagoFactura pago = new PagoFactura();
                pago.ID_MetodoP = metodo.ID_MetodoP;
                pago.Metodo = metodo.NombreMetodo;
                pago.Monto = numericUpDown1.Value;
                pago.Referencia = textBox1.Text.Trim();
                pagos.Add(pago);
                ActualizarSaldo();
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
                if (dataGridView1.CurrentRow == null)
                    return;
                PagoFactura? pago = dataGridView1.CurrentRow.DataBoundItem as PagoFactura;
                if (pago != null)
                    pagos.Remove(pago);
                ActualizarSaldo();
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
                negocio.Pagar(idFactura, new List<PagoFactura>(pagos));
                pagos.Clear();
                Cargar();
                MessageBox.Show("Pagos registrados. La factura se marca Pagada únicamente al cubrir el total.");
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }
        private void button4_Click(object? sender, EventArgs e)
        {
            Close();
        }
        private decimal pendiente;

        private void Cargar()

        {

            var f = negocio.Facturas().Select($"ID_Factura = {idFactura}").Single();

            pendiente = Convert.ToDecimal(f["Saldo"]);

            dataGridView2.DataSource = negocio.Pagos(idFactura);

            ActualizarSaldo();

        }

        private void ActualizarSaldo()
        {
            decimal suma = 0;
            foreach (PagoFactura pago in pagos)
            {
                suma += pago.Monto;
            }
            label5.Text = "Saldo Q" + pendiente.ToString("N2") + " | Pagos por registrar Q" + suma.ToString("N2");
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
