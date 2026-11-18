namespace ProyectoBD.Presentacion
{
    partial class Cotizacion
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
            label1 = new Label(); groupBox1 = new GroupBox(); groupBox2 = new GroupBox(); comboBox1 = new ComboBox();
            numericUpDown1 = new NumericUpDown();
            numericUpDown2 = new NumericUpDown();
            dataGridView1 = new DataGridView();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            button1 = new Button();
            button2 = new Button(); dataGridView2 = new DataGridView(); dataGridView3 = new DataGridView(); label6 = new Label(); label7 = new Label(); button3 = new Button(); label8 = new Label(); comboBox2 = new ComboBox(); button4 = new Button(); button5 = new Button(); button6 = new Button(); button7 = new Button(); button8 = new Button();
            groupBox1.SuspendLayout(); groupBox2.SuspendLayout(); ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit(); ((System.ComponentModel.ISupportInitialize)dataGridView3).BeginInit();
            SuspendLayout();
            label1.Location = new Point(20, 12);
label1.Name = "label1";
label1.Size = new Size(1060, 50);
label1.TabIndex = 0;
label1.Text = "Cotizaciones";
label1.AutoSize = false;
label1.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
label1.Font = new Font("Segoe UI", 24F, FontStyle.Bold | FontStyle.Italic);
groupBox1.Location = new Point(20, 75);
groupBox1.Name = "groupBox1";
groupBox1.Size = new Size(1060, 350);
groupBox1.TabIndex = 1;
groupBox1.Text = "Nueva cotización";
groupBox1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
groupBox2.Location = new Point(20, 445);
groupBox2.Name = "groupBox2";
groupBox2.Size = new Size(1060, 235);
groupBox2.TabIndex = 2;
groupBox2.Text = "Historial de cotizaciones";
groupBox2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
dataGridView2.Location = new Point(16, 100);
dataGridView2.Name = "dataGridView2";
dataGridView2.Size = new Size(498, 116);
dataGridView2.TabIndex = 4;
dataGridView2.AllowUserToAddRows = false;
dataGridView2.AllowUserToDeleteRows = false;
dataGridView2.ReadOnly = true;
dataGridView2.MultiSelect = false;
dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
dataGridView2.BackgroundColor = Color.WhiteSmoke;
dataGridView2.RowHeadersVisible = false;
dataGridView2.SelectionChanged += dataGridView2_SelectionChanged;
dataGridView3.Location = new Point(530, 100);
dataGridView3.Name = "dataGridView3";
dataGridView3.Size = new Size(514, 116);
dataGridView3.TabIndex = 5;
dataGridView3.AllowUserToAddRows = false;
dataGridView3.AllowUserToDeleteRows = false;
dataGridView3.ReadOnly = true;
dataGridView3.MultiSelect = false;
dataGridView3.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
dataGridView3.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
dataGridView3.BackgroundColor = Color.WhiteSmoke;
dataGridView3.RowHeadersVisible = false;
label6.Location = new Point(16, 70);
label6.Name = "label6";
label6.Size = new Size(498, 24);
label6.TabIndex = 6;
label6.Text = "Documentos guardados";
label6.AutoSize = false;
label6.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
label7.Location = new Point(530, 70);
label7.Name = "label7";
label7.Size = new Size(514, 24);
label7.TabIndex = 7;
label7.Text = "Detalle del documento seleccionado";
label7.AutoSize = false;
label7.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
button3.Location = new Point(930, 700);
button3.Name = "button3";
button3.Size = new Size(150, 42);
button3.TabIndex = 8;
button3.Text = "Volver";
button3.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
button3.UseVisualStyleBackColor = true;
button3.Click += button3_Click;
label8.Location = new Point(16, 35);
label8.Name = "label8";
label8.Size = new Size(150, 28);
label8.TabIndex = 9;
label8.Text = "Cliente";
label8.AutoSize = false;
label8.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
comboBox2.Location = new Point(171, 33);
comboBox2.Name = "comboBox2";
comboBox2.Size = new Size(440, 32);
comboBox2.TabIndex = 10;
comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
button4.Location = new Point(650, 27);
button4.Name = "button4";
button4.Size = new Size(135, 42);
button4.TabIndex = 11;
button4.Text = "Nuevo";
button4.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
button4.UseVisualStyleBackColor = true;
button4.Click += button4_Click;
button5.Location = new Point(800, 27);
button5.Name = "button5";
button5.Size = new Size(135, 42);
button5.TabIndex = 12;
button5.Text = "Guardar";
button5.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
button5.UseVisualStyleBackColor = true;
button5.Click += button5_Click;
button6.Location = new Point(16, 23);
button6.Name = "button6";
button6.Size = new Size(190, 42);
button6.TabIndex = 13;
button6.Text = "Confirmar";
button6.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
button6.UseVisualStyleBackColor = true;
button6.Click += button6_Click;
button7.Location = new Point(220, 23);
button7.Name = "button7";
button7.Size = new Size(190, 42);
button7.TabIndex = 14;
button7.Text = "Cancelar cotización";
button7.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
button7.UseVisualStyleBackColor = true;
button7.Click += button7_Click;
button8.Location = new Point(424, 23);
button8.Name = "button8";
button8.Size = new Size(190, 42);
button8.TabIndex = 15;
button8.Text = "Actualizar";
button8.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
button8.UseVisualStyleBackColor = true;
button8.Click += button8_Click;
            
label2.Name = "label2";
label2.Location = new Point(16, 86);
label2.Size = new Size(480, 26);
label2.Text = "Producto / presentación / color";
label2.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);

comboBox1.Name = "comboBox1";
comboBox1.Location = new Point(16, 116);
comboBox1.Size = new Size(480, 32);

label3.Name = "label3";
label3.Location = new Point(516, 86);
label3.Size = new Size(100, 26);
label3.Text = "Cantidad";
label3.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);

numericUpDown1.Name = "numericUpDown1";
numericUpDown1.Location = new Point(516, 116);
numericUpDown1.Size = new Size(100, 32);

label4.Name = "label4";
label4.Location = new Point(636, 86);
label4.Size = new Size(120, 26);
label4.Text = "Precio Q";
label4.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);

numericUpDown2.Name = "numericUpDown2";
numericUpDown2.Location = new Point(636, 116);
numericUpDown2.Size = new Size(120, 32);

button1.Name = "button1";
button1.Location = new Point(770, 110);
button1.Size = new Size(110, 42);
button1.Text = "Agregar";
button1.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
button1.UseVisualStyleBackColor = true;
button1.Click += button1_Click;

button2.Name = "button2";
button2.Location = new Point(894, 110);
button2.Size = new Size(148, 42);
button2.Text = "Quitar línea";
button2.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
button2.UseVisualStyleBackColor = true;
button2.Click += button2_Click;

dataGridView1.Name = "dataGridView1";
dataGridView1.Location = new Point(16, 170);
dataGridView1.Size = new Size(1028, 128);

label5.Name = "label5";
label5.Location = new Point(16, 310);
label5.Size = new Size(1028, 28);
label5.Text = "Total Q0.00";
label5.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
 comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
 numericUpDown1.Minimum = 1;
 numericUpDown1.Maximum = 1000000;
 numericUpDown1.Value = 1;
 numericUpDown2.DecimalPlaces = 2;
 numericUpDown2.Maximum = 9999999999.99M;
 dataGridView1.ReadOnly = true;
 dataGridView1.AllowUserToAddRows = false;
 dataGridView1.AllowUserToDeleteRows = false;
 dataGridView1.MultiSelect = false;
 dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
 dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
 dataGridView1.BackgroundColor = Color.WhiteSmoke;
 dataGridView1.RowHeadersVisible = false;

AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DarkCyan;
            Font = new Font("Segoe UI", 10F);
            ClientSize = new Size(1100, 760);
            Name = "Cotizacion";
            Text = "Cotizaciones";
            FormBorderStyle = FormBorderStyle.None;
            ControlBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            KeyPreview = true;
            Controls.Add(label1); Controls.Add(groupBox1); Controls.Add(groupBox2); groupBox1.Controls.Add(comboBox1);
            groupBox1.Controls.Add(numericUpDown1);
            groupBox1.Controls.Add(numericUpDown2);
            groupBox1.Controls.Add(dataGridView1);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(button1);
            groupBox1.Controls.Add(button2); groupBox2.Controls.Add(dataGridView2); groupBox2.Controls.Add(dataGridView3); groupBox2.Controls.Add(label6); groupBox2.Controls.Add(label7); Controls.Add(button3); groupBox1.Controls.Add(label8); groupBox1.Controls.Add(comboBox2); groupBox1.Controls.Add(button4); groupBox1.Controls.Add(button5); groupBox2.Controls.Add(button6); groupBox2.Controls.Add(button7); groupBox2.Controls.Add(button8);
            Load += Cotizacion_Load;
            groupBox1.ResumeLayout(false); groupBox2.ResumeLayout(false); ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit(); ((System.ComponentModel.ISupportInitialize)dataGridView3).EndInit();
            ResumeLayout(false);
        }
        #endregion

        private Label label1;
private GroupBox groupBox1;
private GroupBox groupBox2;
private ComboBox comboBox1;
        private NumericUpDown numericUpDown1;
        private NumericUpDown numericUpDown2;
        private DataGridView dataGridView1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Button button1;
        private Button button2;
private DataGridView dataGridView2;
private DataGridView dataGridView3;
private Label label6;
private Label label7;
private Button button3;
private Label label8;
private ComboBox comboBox2;
private Button button4;
private Button button5;
private Button button6;
private Button button7;
private Button button8;
    }
}
