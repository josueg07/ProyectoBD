namespace ProyectoBD.Presentacion
{
    partial class IngresoBodega
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
            label1 = new Label(); groupBox1 = new GroupBox(); label2 = new Label(); comboBox1 = new ComboBox(); label3 = new Label(); comboBox2 = new ComboBox(); label4 = new Label(); dateTimePicker1 = new DateTimePicker(); button1 = new Button(); button2 = new Button(); button3 = new Button(); button4 = new Button(); button5 = new Button(); label5 = new Label(); dataGridView1 = new DataGridView();
            groupBox1.SuspendLayout(); ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            label1.Location = new Point(20, 12);
label1.Name = "label1";
label1.Size = new Size(1060, 50);
label1.TabIndex = 0;
label1.Text = "Ingreso a bodega";
label1.AutoSize = false;
label1.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
label1.Font = new Font("Segoe UI", 24F, FontStyle.Bold | FontStyle.Italic);
groupBox1.Location = new Point(20, 80);
groupBox1.Name = "groupBox1";
groupBox1.Size = new Size(1060, 170);
groupBox1.TabIndex = 1;
groupBox1.Text = "Datos de la recepción";
groupBox1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
label2.Location = new Point(16, 36);
label2.Name = "label2";
label2.Size = new Size(150, 28);
label2.TabIndex = 2;
label2.Text = "Orden de compra";
label2.AutoSize = false;
label2.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
comboBox1.Location = new Point(171, 34);
comboBox1.Name = "comboBox1";
comboBox1.Size = new Size(270, 32);
comboBox1.TabIndex = 3;
comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
label3.Location = new Point(550, 36);
label3.Name = "label3";
label3.Size = new Size(150, 28);
label3.TabIndex = 4;
label3.Text = "Ubicación";
label3.AutoSize = false;
label3.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
comboBox2.Location = new Point(705, 34);
comboBox2.Name = "comboBox2";
comboBox2.Size = new Size(320, 32);
comboBox2.TabIndex = 5;
comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
label4.Location = new Point(16, 94);
label4.Name = "label4";
label4.Size = new Size(150, 28);
label4.TabIndex = 6;
label4.Text = "Fecha";
label4.AutoSize = false;
label4.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
dateTimePicker1.Location = new Point(171, 92);
dateTimePicker1.Name = "dateTimePicker1";
dateTimePicker1.Size = new Size(270, 32);
dateTimePicker1.TabIndex = 7;
dateTimePicker1.Format = DateTimePickerFormat.Custom;
dateTimePicker1.CustomFormat = "dd/MM/yyyy HH:mm:ss";
button1.Location = new Point(20, 275);
button1.Name = "button1";
button1.Size = new Size(180, 42);
button1.TabIndex = 8;
button1.Text = "Cargar pendientes";
button1.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
button1.UseVisualStyleBackColor = true;
button1.Click += button1_Click;
button2.Location = new Point(215, 275);
button2.Name = "button2";
button2.Size = new Size(155, 42);
button2.TabIndex = 9;
button2.Text = "Recibir todo";
button2.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
button2.UseVisualStyleBackColor = true;
button2.Click += button2_Click;
button3.Location = new Point(385, 275);
button3.Name = "button3";
button3.Size = new Size(180, 42);
button3.TabIndex = 10;
button3.Text = "Registrar ingreso";
button3.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
button3.UseVisualStyleBackColor = true;
button3.Click += button3_Click;
button4.Location = new Point(580, 275);
button4.Name = "button4";
button4.Size = new Size(180, 42);
button4.TabIndex = 11;
button4.Text = "Actualizar órdenes";
button4.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
button4.UseVisualStyleBackColor = true;
button4.Click += button4_Click;
button5.Location = new Point(920, 275);
button5.Name = "button5";
button5.Size = new Size(160, 42);
button5.TabIndex = 12;
button5.Text = "Volver";
button5.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
button5.UseVisualStyleBackColor = true;
button5.Click += button5_Click;
label5.Location = new Point(20, 338);
label5.Name = "label5";
label5.Size = new Size(1060, 40);
label5.TabIndex = 13;
label5.Text = "Complete la cantidad a recibir y el costo. El número de lote es opcional; una cantidad de cero omite la línea.";
label5.AutoSize = false;
label5.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
dataGridView1.Location = new Point(20, 390);
dataGridView1.Name = "dataGridView1";
dataGridView1.Size = new Size(1060, 310);
dataGridView1.TabIndex = 14;
dataGridView1.AllowUserToAddRows = false;
dataGridView1.AllowUserToDeleteRows = false;
dataGridView1.ReadOnly = true;
dataGridView1.MultiSelect = false;
dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
dataGridView1.BackgroundColor = Color.WhiteSmoke;
dataGridView1.RowHeadersVisible = false;
dataGridView1.ReadOnly = false;
dataGridView1.DataError += dataGridView1_DataError;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DarkCyan;
            Font = new Font("Segoe UI", 10F);
            ClientSize = new Size(1100, 720);
            Name = "IngresoBodega";
            Text = "Ingreso a bodega";
            FormBorderStyle = FormBorderStyle.None;
            ControlBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            KeyPreview = true;
            Controls.Add(label1); Controls.Add(groupBox1); groupBox1.Controls.Add(label2); groupBox1.Controls.Add(comboBox1); groupBox1.Controls.Add(label3); groupBox1.Controls.Add(comboBox2); groupBox1.Controls.Add(label4); groupBox1.Controls.Add(dateTimePicker1); Controls.Add(button1); Controls.Add(button2); Controls.Add(button3); Controls.Add(button4); Controls.Add(button5); Controls.Add(label5); Controls.Add(dataGridView1);
            Load += IngresoBodega_Load;
            groupBox1.ResumeLayout(false); ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }
        #endregion

        private Label label1;
private GroupBox groupBox1;
private Label label2;
private ComboBox comboBox1;
private Label label3;
private ComboBox comboBox2;
private Label label4;
private DateTimePicker dateTimePicker1;
private Button button1;
private Button button2;
private Button button3;
private Button button4;
private Button button5;
private Label label5;
private DataGridView dataGridView1;
    }
}
