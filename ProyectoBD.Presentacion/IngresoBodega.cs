using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;

namespace ProyectoBD.Presentacion
{
    public partial class IngresoBodega : Form
    {
        private ProcesoNegocio negocio = new ProcesoNegocio(new UsuarioSesion());
        private DataTable pendientes = new DataTable();
        public IngresoBodega()
        {
            InitializeComponent();
        }
        public IngresoBodega(UsuarioSesion sesion) : this()
        {
            PermisosNegocio.Exigir(sesion, "Digitador");
            negocio = new ProcesoNegocio(sesion);
        }
        private void IngresoBodega_Load(object? sender, EventArgs e)
        {
            if (DesignMode)
                return;
            try
            {
                comboBox2.DisplayMember = "NombreUbicacion";
                comboBox2.ValueMember = "ID_UbicacionA";
                comboBox2.DataSource = new UbicacionNegocio().Listar();
                CargarOrdenes();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }
        private void comboBox1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            try
            {
                CargarDetalle();
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
                CargarDetalle();
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
                CargarOrdenes();
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
                Guardar();
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
        private void button2_Click(object? sender, EventArgs e)
        {
            try
            {
                foreach (DataRow fila in pendientes.Rows)
                {
                    fila["Recibir"] = fila["Pendiente"];
                }
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }
        private void dataGridView1_DataError(object? sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            MessageBox.Show("Ingrese una cantidad entera o un costo decimal válido.");
        }
        private void CargarOrdenes()

        {

            var t = negocio.Ordenes();

            t.DefaultView.RowFilter = "Estado IN ('Pendiente','Parcial')";

            comboBox1.DisplayMember = "ID_OrdenC";

            comboBox1.ValueMember = "ID_OrdenC";

            comboBox1.DataSource = t.DefaultView;

            CargarDetalle();

        }

        private void CargarDetalle()

        {

            if (comboBox1.SelectedValue is not int id)

            {

                dataGridView1.DataSource = null;

                pendientes = new();

                return;

            }

            pendientes = negocio.PendienteOrden(id);

            pendientes.Columns.Add("Recibir", typeof(int)).DefaultValue = 0;

            pendientes.Columns.Add("CostoUnitario", typeof(decimal)).DefaultValue = 0m;

            pendientes.Columns.Add("NumeroLote", typeof(string));

            foreach (DataRow r in pendientes.Rows)

            {

                r["Recibir"] = 0;

                r["CostoUnitario"] = 0m;

            }

            dataGridView1.DataSource = pendientes;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            foreach (DataGridViewColumn c in dataGridView1.Columns)

            {

                c.ReadOnly = c.Name != "Recibir" && c.Name != "CostoUnitario" && c.Name != "NumeroLote";

                c.FillWeight = 80;

            }

            dataGridView1.Columns["ID_DetalleOC"].Visible = false;

            dataGridView1.Columns["ID_ProductoV"].Visible = false;

            dataGridView1.Columns["Producto"].FillWeight = 200;

            dataGridView1.Columns["CantidadSolicitada"].HeaderText = "Solicitado";

            dataGridView1.Columns["CantidadRecibida"].HeaderText = "Recibido";

            dataGridView1.Columns["CostoUnitario"].HeaderText = "Costo Q";

            dataGridView1.Columns["NumeroLote"].HeaderText = "Lote (opcional)";

        }

        private void Guardar()

        {

            if (!dataGridView1.EndEdit())

                throw new ArgumentException("Corrija la celda antes de guardar.");

            BindingContext?[pendientes]?.EndCurrentEdit();

            var detalle = new List<LineaIngreso>();

            foreach (DataRow r in pendientes.Rows)

            {

                int cantidad = Convert.ToInt32(r["Recibir"]);

                if (cantidad < 0 || cantidad > Convert.ToInt32(r["Pendiente"]))

                    throw new ArgumentException("La cantidad debe estar entre cero y lo pendiente.");

                if (cantidad == 0)

                    continue;

                detalle.Add(new LineaIngreso { ID_DetalleOC = Convert.ToInt32(r["ID_DetalleOC"]), CantidadRecibida = cantidad, CostoUnitario = Convert.ToDecimal(r["CostoUnitario"]), NumeroLote = r["NumeroLote"] == DBNull.Value ? null : Convert.ToString(r["NumeroLote"]) });

            }

            int id = negocio.Ingresar(new ProyectoBD.Entidades.IngresoBodega { ID_OrdenC = Convert.ToInt32(comboBox1.SelectedValue), ID_UbicacionA = Convert.ToInt32(comboBox2.SelectedValue), FechaIngreso = dateTimePicker1.Value, Detalle = detalle });

            CargarOrdenes();

            MessageBox.Show($"Ingreso {id} registrado con lotes y movimientos.");

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
