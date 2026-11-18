namespace ProyectoBD.Presentacion
{
    partial class Pagos
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
            label1 = new Label(); groupBox1 = new GroupBox(); label2 = new Label(); comboBox1 = new ComboBox(); label3 = new Label(); numericUpDown1 = new NumericUpDown(); label4 = new Label(); textBox1 = new TextBox(); button1 = new Button(); button2 = new Button(); button3 = new Button(); button4 = new Button(); label5 = new Label(); label6 = new Label(); dataGridView1 = new DataGridView(); label7 = new Label(); dataGridView2 = new DataGridView();
            groupBox1.SuspendLayout(); ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit(); ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit(); ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit();
            SuspendLayout();
            label1.Location = new Point(20, 12);
label1.Name = "label1";
label1.Size = new Size(960, 50);
label1.TabIndex = 0;
label1.Text = "Cobro de factura";
label1.AutoSize = false;
label1.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
label1.Font = new Font("Segoe UI", 24F, FontStyle.Bold | FontStyle.Italic);
groupBox1.Location = new Point(20, 80);
groupBox1.Name = "groupBox1";
groupBox1.Size = new Size(960, 175);
groupBox1.TabIndex = 1;
groupBox1.Text = "Datos del pago";
groupBox1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
label2.Location = new Point(16, 36);
label2.Name = "label2";
label2.Size = new Size(150, 28);
label2.TabIndex = 2;
label2.Text = "Método de pago";
label2.AutoSize = false;
label2.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
comboBox1.Location = new Point(171, 34);
comboBox1.Name = "comboBox1";
comboBox1.Size = new Size(270, 32);
comboBox1.TabIndex = 3;
comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
label3.Location = new Point(500, 36);
label3.Name = "label3";
label3.Size = new Size(150, 28);
label3.TabIndex = 4;
label3.Text = "Monto Q";
label3.AutoSize = false;
label3.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
numericUpDown1.Location = new Point(655, 34);
numericUpDown1.Name = "numericUpDown1";
numericUpDown1.Size = new Size(260, 32);
numericUpDown1.TabIndex = 5;
numericUpDown1.DecimalPlaces = 2;
numericUpDown1.Maximum = 9999999999.99M;
numericUpDown1.Minimum = 0;
label4.Location = new Point(16, 95);
label4.Name = "label4";
label4.Size = new Size(150, 28);
label4.TabIndex = 6;
label4.Text = "Referencia";
label4.AutoSize = false;
label4.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
textBox1.Location = new Point(171, 93);
textBox1.Name = "textBox1";
textBox1.Size = new Size(730, 32);
textBox1.TabIndex = 7;
textBox1.MaxLength = 100;
button1.Location = new Point(20, 275);
button1.Name = "button1";
button1.Size = new Size(170, 42);
button1.TabIndex = 8;
button1.Text = "Agregar pago";
button1.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
button1.UseVisualStyleBackColor = true;
button1.Click += button1_Click;
button2.Location = new Point(205, 275);
button2.Name = "button2";
button2.Size = new Size(160, 42);
button2.TabIndex = 9;
button2.Text = "Quitar pago";
button2.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
button2.UseVisualStyleBackColor = true;
button2.Click += button2_Click;
button3.Location = new Point(380, 275);
button3.Name = "button3";
button3.Size = new Size(190, 42);
button3.TabIndex = 10;
button3.Text = "Registrar pagos";
button3.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
button3.UseVisualStyleBackColor = true;
button3.Click += button3_Click;
button4.Location = new Point(820, 275);
button4.Name = "button4";
button4.Size = new Size(160, 42);
button4.TabIndex = 11;
button4.Text = "Volver";
button4.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
button4.UseVisualStyleBackColor = true;
button4.Click += button4_Click;
label5.Location = new Point(20, 335);
label5.Name = "label5";
label5.Size = new Size(960, 35);
label5.TabIndex = 12;
label5.Text = "Saldo Q0.00";
label5.AutoSize = false;
label5.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
label6.Location = new Point(20, 380);
label6.Name = "label6";
label6.Size = new Size(960, 28);
label6.TabIndex = 13;
label6.Text = "Pagos por registrar";
label6.AutoSize = false;
label6.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
dataGridView1.Location = new Point(20, 415);
dataGridView1.Name = "dataGridView1";
dataGridView1.Size = new Size(960, 110);
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
label7.Location = new Point(20, 545);
label7.Name = "label7";
label7.Size = new Size(960, 28);
label7.TabIndex = 15;
label7.Text = "Pagos ya registrados";
label7.AutoSize = false;
label7.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
dataGridView2.Location = new Point(20, 580);
dataGridView2.Name = "dataGridView2";
dataGridView2.Size = new Size(960, 120);
dataGridView2.TabIndex = 16;
dataGridView2.AllowUserToAddRows = false;
dataGridView2.AllowUserToDeleteRows = false;
dataGridView2.ReadOnly = true;
dataGridView2.MultiSelect = false;
dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
dataGridView2.BackgroundColor = Color.WhiteSmoke;
dataGridView2.RowHeadersVisible = false;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DarkCyan;
            Font = new Font("Segoe UI", 10F);
            ClientSize = new Size(1000, 720);
            Name = "Pagos";
            Text = "Cobro de factura";
            FormBorderStyle = FormBorderStyle.None;
            ControlBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            KeyPreview = true;
            Controls.Add(label1); Controls.Add(groupBox1); groupBox1.Controls.Add(label2); groupBox1.Controls.Add(comboBox1); groupBox1.Controls.Add(label3); groupBox1.Controls.Add(numericUpDown1); groupBox1.Controls.Add(label4); groupBox1.Controls.Add(textBox1); Controls.Add(button1); Controls.Add(button2); Controls.Add(button3); Controls.Add(button4); Controls.Add(label5); Controls.Add(label6); Controls.Add(dataGridView1); Controls.Add(label7); Controls.Add(dataGridView2);
            Load += Pagos_Load;
            groupBox1.ResumeLayout(false); ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit(); ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit(); ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit();
            ResumeLayout(false);
        }
        #endregion

        private Label label1;
private GroupBox groupBox1;
private Label label2;
private ComboBox comboBox1;
private Label label3;
private NumericUpDown numericUpDown1;
private Label label4;
private TextBox textBox1;
private Button button1;
private Button button2;
private Button button3;
private Button button4;
private Label label5;
private Label label6;
private DataGridView dataGridView1;
private Label label7;
private DataGridView dataGridView2;
    }
}
