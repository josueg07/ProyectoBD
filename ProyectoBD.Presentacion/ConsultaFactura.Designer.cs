namespace ProyectoBD.Presentacion
{
    partial class ConsultaFactura
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            textBox1 = new TextBox();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            dataGridView1 = new DataGridView();
            dataGridView2 = new DataGridView();
            dataGridView3 = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView3).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Font = new Font("Segoe UI", 24F, FontStyle.Bold | FontStyle.Italic);
            label1.Location = new Point(20, 10);
            label1.Name = "label1";
            label1.Size = new Size(1060, 55);
            label1.TabIndex = 0;
            label1.Text = "Consulta de factura";
            // 
            // label2
            // 
            label2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label2.Location = new Point(20, 115);
            label2.Name = "label2";
            label2.Size = new Size(1060, 25);
            label2.TabIndex = 1;
            label2.Text = "Busque una factura por su número.";
            // 
            // label3
            // 
            label3.Location = new Point(20, 143);
            label3.Name = "label3";
            label3.Size = new Size(1060, 50);
            label3.TabIndex = 2;
            // 
            // label4
            // 
            label4.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label4.Location = new Point(20, 196);
            label4.Name = "label4";
            label4.Size = new Size(1060, 25);
            label4.TabIndex = 3;
            // 
            // label5
            // 
            label5.ForeColor = Color.White;
            label5.Location = new Point(20, 224);
            label5.Name = "label5";
            label5.Size = new Size(1060, 25);
            label5.TabIndex = 4;
            // 
            // label6
            // 
            label6.Location = new Point(20, 252);
            label6.Name = "label6";
            label6.Size = new Size(1060, 25);
            label6.TabIndex = 5;
            label6.Text = "Detalle de productos";
            // 
            // label7
            // 
            label7.Location = new Point(20, 397);
            label7.Name = "label7";
            label7.Size = new Size(1060, 25);
            label7.TabIndex = 6;
            label7.Text = "Pagos históricos";
            // 
            // label8
            // 
            label8.Location = new Point(20, 527);
            label8.Name = "label8";
            label8.Size = new Size(1060, 25);
            label8.TabIndex = 7;
            label8.Text = "Historial de operaciones y auditoría";
            // 
            // label9
            // 
            label9.Location = new Point(20, 75);
            label9.Name = "label9";
            label9.Size = new Size(165, 27);
            label9.TabIndex = 8;
            label9.Text = "Número de factura";
            // 
            // textBox1
            // 
            textBox1.AccessibleName = "Número de factura";
            textBox1.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox1.Location = new Point(190, 72);
            textBox1.MaxLength = 10;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(197, 31);
            textBox1.TabIndex = 0;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // button1
            // 
            button1.Location = new Point(393, 67);
            button1.Name = "button1";
            button1.Size = new Size(140, 40);
            button1.TabIndex = 1;
            button1.Text = "Buscar";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Enabled = false;
            button2.Location = new Point(550, 67);
            button2.Name = "button2";
            button2.Size = new Size(180, 40);
            button2.TabIndex = 2;
            button2.Text = "Anular factura";
            button2.UseVisualStyleBackColor = true;
            button2.Visible = false;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(930, 660);
            button3.Name = "button3";
            button3.Size = new Size(150, 42);
            button3.TabIndex = 3;
            button3.Text = "Volver";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.AccessibleName = "Detalle de productos";
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.BackgroundColor = Color.WhiteSmoke;
            dataGridView1.ColumnHeadersHeight = 29;
            dataGridView1.Location = new Point(20, 280);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(1060, 110);
            dataGridView1.TabIndex = 9;
            // 
            // dataGridView2
            // 
            dataGridView2.AccessibleName = "Pagos históricos";
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.AllowUserToDeleteRows = false;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView2.BackgroundColor = Color.WhiteSmoke;
            dataGridView2.ColumnHeadersHeight = 29;
            dataGridView2.Location = new Point(20, 425);
            dataGridView2.Name = "dataGridView2";
            dataGridView2.ReadOnly = true;
            dataGridView2.RowHeadersVisible = false;
            dataGridView2.RowHeadersWidth = 51;
            dataGridView2.Size = new Size(1060, 95);
            dataGridView2.TabIndex = 10;
            // 
            // dataGridView3
            // 
            dataGridView3.AccessibleName = "Historial de operaciones";
            dataGridView3.AllowUserToAddRows = false;
            dataGridView3.AllowUserToDeleteRows = false;
            dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView3.BackgroundColor = Color.WhiteSmoke;
            dataGridView3.ColumnHeadersHeight = 29;
            dataGridView3.Location = new Point(20, 555);
            dataGridView3.Name = "dataGridView3";
            dataGridView3.ReadOnly = true;
            dataGridView3.RowHeadersVisible = false;
            dataGridView3.RowHeadersWidth = 51;
            dataGridView3.Size = new Size(1060, 85);
            dataGridView3.TabIndex = 11;
            // 
            // ConsultaFactura
            // 
            AcceptButton = button1;
            AutoScaleDimensions = new SizeF(9F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DarkCyan;
            CancelButton = button3;
            ClientSize = new Size(1100, 720);
            ControlBox = false;
            Controls.Add(label1);
            Controls.Add(label2);
            Controls.Add(label3);
            Controls.Add(label4);
            Controls.Add(label5);
            Controls.Add(label6);
            Controls.Add(label7);
            Controls.Add(label8);
            Controls.Add(label9);
            Controls.Add(textBox1);
            Controls.Add(button1);
            Controls.Add(button2);
            Controls.Add(button3);
            Controls.Add(dataGridView1);
            Controls.Add(dataGridView2);
            Controls.Add(dataGridView3);
            Font = new Font("Segoe UI", 10F);
            FormBorderStyle = FormBorderStyle.None;
            Name = "ConsultaFactura";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Consulta de factura";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView3).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private TextBox textBox1;
        private Button button1;
        private Button button2;
        private Button button3;
        private DataGridView dataGridView1;
        private DataGridView dataGridView2;
        private DataGridView dataGridView3;
    }
}
