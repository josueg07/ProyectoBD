using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;
using ProyectoBD.FormatoVisual;

namespace ProyectoBD.Presentacion
{
    public partial class Reportes : Form
    {
        private UsuarioSesion sesion = new UsuarioSesion();
        private ReporteNegocio? negocio;

        public Reportes()
        {
            InitializeComponent();
        }
        public Reportes(string titulo, DataTable datos) : this()
        {
            label1.Text = titulo;
            Text = titulo;
            groupBox1.Visible = false;
            dataGridView1.Top = groupBox1.Top;
            dataGridView1.Height = button1.Top - dataGridView1.Top - 25;
            dataGridView1.DataSource = datos;
            if (datos.Columns.Contains("DatosAnteriores") || datos.Columns.Contains("DatosNuevos"))
                FormatearTabla(dataGridView1);
        }
        public Reportes(UsuarioSesion sesion) : this()
        {
            PermisosNegocio.Exigir(sesion, "Gerente");
            this.sesion = sesion;
            negocio = new ReporteNegocio(sesion);
            groupBox1.Visible = true;
            dateTimePicker1.Value = new DateTime(DateTime.Today.Year, 1, 1);
            dateTimePicker2.Value = DateTime.Today;
            comboBox1.SelectedIndex = 0;
        }
        private void comboBox1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            int reporte = comboBox1.SelectedIndex;
            bool fechas = reporte == 0 || reporte == 1 || reporte == 2 || reporte == 4;
            dateTimePicker1.Enabled = fechas;
            dateTimePicker2.Enabled = fechas;
            button2.Text = reporte == 6 ? "Consultar factura" : "Generar";
            dataGridView1.DataSource = null;
            label5.Text = reporte == 0 ? "Pagos de facturas emitidas en el período; excluye anuladas." : "";
        }
        private void button2_Click(object? sender, EventArgs e)
        {
            try
            {
                if (negocio == null) return;
                DateTime inicial = dateTimePicker1.Value.Date;
                DateTime final = dateTimePicker2.Value.Date;
                DataTable datos;
                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        DataSet resultado = negocio.ReporteFacturacion(inicial, final);
                        DataRow resumen = resultado.Tables[0].Rows[0];
                        label5.Text = "Facturado Q" + Convert.ToDecimal(resumen["TotalFacturado"]).ToString("N2")
                            + " | Pagado Q" + Convert.ToDecimal(resumen["TotalPagado"]).ToString("N2")
                            + " | Por cobrar Q" + Convert.ToDecimal(resumen["SaldoPorCobrar"]).ToString("N2")
                            + "\nPagos históricos de facturas emitidas en el período; excluye anuladas.";
                        datos = resultado.Tables[1];
                        break;
                    case 1: datos = negocio.ReporteProductosMayoresIngresos(inicial, final); break;
                    case 2: datos = negocio.ReporteProductosMasVendidos(inicial, final); break;
                    case 3: datos = negocio.ReporteInventario(); break;
                    case 4: datos = negocio.ReporteProductosMenosVendidos(inicial, final); break;
                    case 5: datos = negocio.ReporteProductosSinExistencia(); break;
                    case 6:
                        using (ConsultaFactura formulario = new ConsultaFactura(sesion))
                        {
                            Hide();
                            formulario.ShowDialog();
                        }
                        Show();
                        return;
                    default: throw new ArgumentException("Seleccione un reporte.");
                }
                dataGridView1.DataSource = datos;
                FormatearTabla(dataGridView1);
                if (comboBox1.SelectedIndex != 0)
                    label5.Text = datos.Rows.Count == 0 ? "No hay resultados para esta consulta." : "Registros: " + datos.Rows.Count;
            }
            catch (Exception ex)
            {
                Show();
                MostrarError(ex);
            }
        }
        internal static void FormatearTabla(DataGridView tabla)
        {
            tabla.CellFormatting -= FormatearAuditoria;
            tabla.CellFormatting += FormatearAuditoria;
            tabla.CellDoubleClick -= VerAuditoria;
            tabla.CellDoubleClick += VerAuditoria;
            foreach (DataGridViewColumn columna in tabla.Columns)
            {
                if (columna.Name is "DatosAnteriores" or "DatosNuevos")
                {
                    columna.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    columna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
                    columna.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    columna.Width = 360;
                    columna.ToolTipText = "Doble clic en una celda para leer el detalle completo.";
                    tabla.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
                }
                switch (columna.Name)
                {
                    case "PrecioUnitario": columna.HeaderText = "Precio unitario"; break;
                    case "IngresoTotal": columna.HeaderText = "Ingreso total"; break;
                    case "CantidadVendida": columna.HeaderText = "Cantidad vendida"; break;
                    case "CantidadDisponible": columna.HeaderText = "Disponible"; break;
                    case "FechaIngreso": columna.HeaderText = "Fecha de ingreso"; break;
                    case "MetodoPago": columna.HeaderText = "Método de pago"; break;
                    case "TotalPagado": columna.HeaderText = "Total pagado"; break;
                    case "Presentacion": columna.HeaderText = "Presentación"; break;
                    case "Ubicacion": columna.HeaderText = "Ubicación"; break;
                    case "FechaHora": columna.HeaderText = "Fecha y hora"; break;
                    case "Accion": columna.HeaderText = "Acción"; break;
                    case "DatosAnteriores": columna.HeaderText = "Datos anteriores"; break;
                    case "DatosNuevos": columna.HeaderText = "Datos nuevos"; break;
                }
                if (columna.ValueType == typeof(decimal))
                {
                    columna.DefaultCellStyle.Format = "'Q'#,##0.00";
                    columna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                if (columna.ValueType == typeof(DateTime))
                    columna.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            }
        }
        private static void FormatearAuditoria(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (sender is DataGridView tabla && e.ColumnIndex >= 0 &&
                tabla.Columns[e.ColumnIndex].Name is "DatosAnteriores" or "DatosNuevos")
            {
                e.Value = FormatoAuditoria.Mostrar(e.Value);
                e.FormattingApplied = true;
            }
        }
        private static void VerAuditoria(object? sender, DataGridViewCellEventArgs e)
        {
            if (sender is not DataGridView tabla || e.RowIndex < 0 || e.ColumnIndex < 0 ||
                tabla.Columns[e.ColumnIndex].Name is not ("DatosAnteriores" or "DatosNuevos")) return;
            using var ventana = new Form
            {
                Text = tabla.Columns[e.ColumnIndex].HeaderText, Width = 800, Height = 550,
                StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog,
                ControlBox = false
            };
            var textBox1 = new TextBox
            {
                Multiline = true, ReadOnly = true, WordWrap = true, ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Fill, Text = FormatoAuditoria.Mostrar(tabla.Rows[e.RowIndex].Cells[e.ColumnIndex].Value)
            };
            var button1 = new Button { Text = "Volver", Dock = DockStyle.Bottom, Height = 38, DialogResult = DialogResult.Cancel };
            ventana.Controls.Add(textBox1);
            ventana.Controls.Add(button1);
            ventana.CancelButton = button1;
            ventana.ShowDialog(tabla.FindForm());
        }
        private void button1_Click(object? sender, EventArgs e)
        {
            Close();
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
