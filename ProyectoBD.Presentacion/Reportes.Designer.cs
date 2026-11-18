namespace ProyectoBD.Presentacion
{
    partial class Reportes
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms
        private void InitializeComponent()
        {
            label1 = new Label(); dataGridView1 = new DataGridView(); button1 = new Button();
            groupBox1 = new GroupBox();
            comboBox1 = new ComboBox();
            dateTimePicker1 = new DateTimePicker();
            dateTimePicker2 = new DateTimePicker();
            button2 = new Button();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            groupBox1.Name = "groupBox1";
            groupBox1.Text = "Reportes gerenciales";
            groupBox1.Location = new Point(20, 70);
            groupBox1.Size = new Size(1060, 165);
            groupBox1.Visible = true;
            groupBox1.TabIndex = 0;
            label2.Name = "label2";
            label2.Text = "Reporte";
            label2.Location = new Point(15, 25);
            label2.Size = new Size(100, 23);
            comboBox1.Name = "comboBox1";
            comboBox1.Location = new Point(15, 50);
            comboBox1.Size = new Size(425, 28);
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.AccessibleName = "Tipo de reporte";
            comboBox1.TabIndex = 0;
            comboBox1.Items.AddRange(new object[] { "1. Total facturado por forma de pago", "2. Productos con mayores ingresos", "3. Productos más vendidos por cantidad", "4. Inventario actual por lote", "5. Productos con menos ventas", "6. Productos sin existencias", "7. Detalle de factura por número" });
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            label3.Name = "label3";
            label3.Text = "Fecha inicial";
            label3.Location = new Point(460, 25);
            label3.Size = new Size(150, 23);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Format = DateTimePickerFormat.Short;
            dateTimePicker1.Location = new Point(460, 50);
            dateTimePicker1.Size = new Size(155, 28);
            dateTimePicker1.TabIndex = 1;
            dateTimePicker1.AccessibleName = "Fecha inicial";
            label4.Name = "label4";
            label4.Text = "Fecha final";
            label4.Location = new Point(635, 25);
            label4.Size = new Size(150, 23);
            dateTimePicker2.Name = "dateTimePicker2";
            dateTimePicker2.Format = DateTimePickerFormat.Short;
            dateTimePicker2.Location = new Point(635, 50);
            dateTimePicker2.Size = new Size(155, 28);
            dateTimePicker2.TabIndex = 2;
            dateTimePicker2.AccessibleName = "Fecha final";
            button2.Name = "button2";
            button2.Text = "Generar";
            button2.Location = new Point(815, 43);
            button2.Size = new Size(225, 42);
            button2.UseVisualStyleBackColor = true;
            button2.TabIndex = 3;
            button2.Click += button2_Click;
            label5.Name = "label5";
            label5.Location = new Point(15, 95);
            label5.Size = new Size(1025, 60);
            label5.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            groupBox1.Controls.AddRange(new Control[] { label2, label3, label4, label5, comboBox1, dateTimePicker1, dateTimePicker2, button2 });
            Controls.Add(groupBox1);
            label1.Location = new Point(20, 12);
label1.Name = "label1";
label1.Size = new Size(1060, 50);
label1.TabIndex = 0;
label1.Text = "Consulta de reportes";
label1.AutoSize = false;
label1.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
label1.Font = new Font("Segoe UI", 24F, FontStyle.Bold | FontStyle.Italic);
dataGridView1.Location = new Point(20, 250);
dataGridView1.Name = "dataGridView1";
dataGridView1.Size = new Size(1060, 375);
dataGridView1.TabIndex = 1;
dataGridView1.AllowUserToAddRows = false;
dataGridView1.AllowUserToDeleteRows = false;
dataGridView1.ReadOnly = true;
dataGridView1.MultiSelect = false;
dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
dataGridView1.BackgroundColor = Color.WhiteSmoke;
dataGridView1.RowHeadersVisible = false;
button1.Location = new Point(930, 650);
button1.Name = "button1";
button1.Size = new Size(150, 42);
button1.TabIndex = 2;
button1.Text = "Volver";
button1.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
button1.UseVisualStyleBackColor = true;
button1.Click += button1_Click;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DarkCyan;
            Font = new Font("Segoe UI", 10F);
            ClientSize = new Size(1100, 720);
            Name = "Reportes";
            Text = "Consulta de reportes";
            FormBorderStyle = FormBorderStyle.None;
            ControlBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            KeyPreview = true;
            Controls.Add(label1); Controls.Add(dataGridView1); Controls.Add(button1);
            
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }
        #endregion

        private Label label1;
private DataGridView dataGridView1;
private Button button1;
        private GroupBox groupBox1;
        private ComboBox comboBox1;
        private DateTimePicker dateTimePicker1;
        private DateTimePicker dateTimePicker2;
        private Button button2;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
    }
}
