using System.Data;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;

namespace ProyectoBD.Presentacion
{
    public partial class ConsultaFactura : Form
    {
        private ReporteNegocio? negocio;
        private int numeroConsultado;
        private bool permitirAnulacion;

        public ConsultaFactura()
        {
            InitializeComponent();
        }
        public ConsultaFactura(UsuarioSesion sesion, bool anular = false) : this()
        {
            PermisosNegocio.Exigir(sesion, "Gerente");
            negocio = new ReporteNegocio(sesion);
            permitirAnulacion = anular;
            button2.Visible = anular;
            label1.Text = anular ? "Anulación de factura" : "Consulta de factura";
            Text = label1.Text;
        }
        private void Limpiar()
        {
            numeroConsultado = 0;
            button2.Enabled = false;
            label2.Text = "Busque una factura por su número.";
            label3.Text = "";
            label4.Text = "";
            label5.Text = "";
            dataGridView1.DataSource = null;
            dataGridView2.DataSource = null;
            dataGridView3.DataSource = null;
        }
        private void textBox1_TextChanged(object? sender, EventArgs e)
        {
            Limpiar();
        }
        private void button1_Click(object? sender, EventArgs e)
        {
            try
            {
                Limpiar();
                if (!int.TryParse(textBox1.Text.Trim(), out int numero) || numero <= 0)
                    throw new ArgumentException("Ingrese un número de factura entero y positivo.");
                CargarFactura(numero);
            }
            catch (Exception ex) { MostrarError(ex); }
        }
        private void CargarFactura(int numero)
        {
            if (negocio == null) throw new InvalidOperationException("Abra esta consulta desde el menú del Gerente.");
            DataSet resultado = negocio.BuscarFactura(numero);
            if (resultado.Tables[0].Rows.Count == 0)
                throw new InvalidOperationException("No se encontró la factura indicada.");
            DataRow factura = resultado.Tables[0].Rows[0];
            string estado = Convert.ToString(factura["Estado"]) ?? "";
            label2.Text = "Factura N.º " + factura["NumeroFactura"] + " | Fecha: " + Convert.ToDateTime(factura["FechaFactura"]).ToString("dd/MM/yyyy HH:mm")
                + " | Estado: " + estado;
            label3.Text = "Cliente: " + factura["Cliente"] + "\nEmitida por: " + factura["Empleado"] + " (" + factura["Usuario"] + ")";
            label4.Text = "Total actual Q" + Convert.ToDecimal(factura["Total"]).ToString("N2")
                + " | Total original Q" + Convert.ToDecimal(factura["TotalOriginal"]).ToString("N2")
                + " | Pagos históricos Q" + Convert.ToDecimal(factura["PagosHistoricos"]).ToString("N2")
                + " | Saldo Q" + Convert.ToDecimal(factura["SaldoPorCobrar"]).ToString("N2");
            label5.Text = estado == "Anulada" ? "FACTURA ANULADA. Detalles y pagos se conservan como historial; no generan ventas ni saldo por cobrar." : "";
            dataGridView1.DataSource = resultado.Tables[1];
            dataGridView2.DataSource = resultado.Tables[2];
            dataGridView3.DataSource = resultado.Tables[3];
            Reportes.FormatearTabla(dataGridView1);
            Reportes.FormatearTabla(dataGridView2);
            Reportes.FormatearTabla(dataGridView3);
            numeroConsultado = numero;
            button2.Enabled = permitirAnulacion && estado != "Anulada";
        }
        private void button2_Click(object? sender, EventArgs e)
        {
            try
            {
                if (negocio == null || numeroConsultado <= 0 || !permitirAnulacion)
                    throw new InvalidOperationException("Busque una factura antes de anular.");
                DialogResult respuesta = MessageBox.Show(this,
                    "¿Desea anular la factura N.º " + numeroConsultado + "?\nSe reintegrará el inventario y su total quedará en Q0.00. Los pagos se conservarán como historial.",
                    "Confirmar anulación", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (respuesta != DialogResult.Yes) return;
                int numero = numeroConsultado;
                button2.Enabled = false;
                negocio.AnularFactura(numero);
                CargarFactura(numero);
                MessageBox.Show(this, "Factura anulada. Inventario y auditoría registrados.", "Anulación", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                button2.Enabled = false;
                MostrarError(ex);
            }
        }
        private void button3_Click(object? sender, EventArgs e)
        {
            Close();
        }
        private void MostrarError(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            string mensaje = "No se pudo completar la operación. Revise la conexión y vuelva a buscar la factura.";
            if (ex is ArgumentException || ex is InvalidOperationException) mensaje = ex.Message;
            MessageBox.Show(this, mensaje, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
