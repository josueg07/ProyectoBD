namespace ProyectoBD.Presentacion
{
    partial class Marcas
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
            label1 = new Label(); groupBox1 = new GroupBox(); label2 = new Label(); textBox1 = new TextBox(); button1 = new Button(); button2 = new Button(); button3 = new Button(); button4 = new Button(); button5 = new Button(); dataGridView1 = new DataGridView();
            groupBox1.SuspendLayout(); ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            label1.Location = new Point(20, 12);
label1.Name = "label1";
label1.Size = new Size(860, 50);
label1.TabIndex = 0;
label1.Text = "Mantenimiento de marcas";
label1.AutoSize = false;
label1.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
label1.Font = new Font("Segoe UI", 24F, FontStyle.Bold | FontStyle.Italic);
groupBox1.Location = new Point(20, 80);
groupBox1.Name = "groupBox1";
groupBox1.Size = new Size(860, 155);
groupBox1.TabIndex = 1;
groupBox1.Text = "Datos del registro";
groupBox1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
label2.Location = new Point(20, 40);
label2.Name = "label2";
label2.Size = new Size(150, 28);
label2.TabIndex = 2;
label2.Text = "Marca";
label2.AutoSize = false;
label2.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
textBox1.Location = new Point(175, 38);
textBox1.Name = "textBox1";
textBox1.Size = new Size(590, 32);
textBox1.TabIndex = 3;
textBox1.MaxLength = 45;
button1.Location = new Point(20, 255);
button1.Name = "button1";
button1.Size = new Size(162, 42);
button1.TabIndex = 4;
button1.Text = "Nuevo";
button1.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
button1.UseVisualStyleBackColor = true;
button1.Click += button1_Click;
button2.Location = new Point(194, 255);
button2.Name = "button2";
button2.Size = new Size(162, 42);
button2.TabIndex = 5;
button2.Text = "Guardar";
button2.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
button2.UseVisualStyleBackColor = true;
button2.Click += button2_Click;
button3.Location = new Point(368, 255);
button3.Name = "button3";
button3.Size = new Size(162, 42);
button3.TabIndex = 6;
button3.Text = "Editar";
button3.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
button3.UseVisualStyleBackColor = true;
button3.Click += button3_Click;
button4.Location = new Point(542, 255);
button4.Name = "button4";
button4.Size = new Size(162, 42);
button4.TabIndex = 7;
button4.Text = "Eliminar";
button4.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
button4.UseVisualStyleBackColor = true;
button4.Click += button4_Click;
button5.Location = new Point(716, 255);
button5.Name = "button5";
button5.Size = new Size(162, 42);
button5.TabIndex = 8;
button5.Text = "Volver";
button5.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
button5.UseVisualStyleBackColor = true;
button5.Click += button5_Click;
dataGridView1.Location = new Point(20, 320);
dataGridView1.Name = "dataGridView1";
dataGridView1.Size = new Size(860, 260);
dataGridView1.TabIndex = 9;
dataGridView1.AllowUserToAddRows = false;
dataGridView1.AllowUserToDeleteRows = false;
dataGridView1.ReadOnly = true;
dataGridView1.MultiSelect = false;
dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
dataGridView1.BackgroundColor = Color.WhiteSmoke;
dataGridView1.RowHeadersVisible = false;
dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DarkCyan;
            Font = new Font("Segoe UI", 10F);
            ClientSize = new Size(900, 600);
            Name = "Marcas";
            Text = "Mantenimiento de marcas";
            FormBorderStyle = FormBorderStyle.None;
            ControlBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            KeyPreview = true;
            Controls.Add(label1); Controls.Add(groupBox1); groupBox1.Controls.Add(label2); groupBox1.Controls.Add(textBox1); Controls.Add(button1); Controls.Add(button2); Controls.Add(button3); Controls.Add(button4); Controls.Add(button5); Controls.Add(dataGridView1);
            Load += Marcas_Load;
            groupBox1.ResumeLayout(false); ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }
        #endregion

        private Label label1;
private GroupBox groupBox1;
private Label label2;
private TextBox textBox1;
private Button button1;
private Button button2;
private Button button3;
private Button button4;
private Button button5;
private DataGridView dataGridView1;
    }
}
