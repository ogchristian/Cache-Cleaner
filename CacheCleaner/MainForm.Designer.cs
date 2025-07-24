namespace PcTools
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dataGridView1 = new DataGridView();
            button1 = new Button();
            button2 = new Button();
            checkedListBox1 = new CheckedListBox();
            label1 = new Label();
            label2 = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(0, -1);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 102;
            dataGridView1.Size = new Size(2026, 1432);
            dataGridView1.TabIndex = 0;
            // 
            // button1
            // 
            button1.Location = new Point(2032, 1341);
            button1.Name = "button1";
            button1.Size = new Size(188, 74);
            button1.TabIndex = 1;
            button1.Text = "Fetch data";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(2707, 1341);
            button2.Name = "button2";
            button2.Size = new Size(188, 71);
            button2.TabIndex = 2;
            button2.Text = "Clean";
            button2.UseVisualStyleBackColor = true;
            // 
            // checkedListBox1
            // 
            checkedListBox1.CheckOnClick = true;
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Items.AddRange(new object[] { "Internet Cache", "Cookies", "Internet History", "Download History", "Last Download ", "Location ", "Session", "Saved Form Information", "Saved Passwords", "Compact Databases", "Metrics Temp Files", "Thumbnail Cache", "Recycle Bin", "System Temporary Files", "Windows Log Files", "Widows Event Trace Logs", "Windows Web Cache" });
            checkedListBox1.Location = new Point(2032, 23);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(863, 796);
            checkedListBox1.TabIndex = 10;
            checkedListBox1.SelectedIndexChanged += checkedListBox1_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.FromArgb(255, 128, 0);
            label1.Location = new Point(2032, 1257);
            label1.Name = "label1";
            label1.Size = new Size(131, 54);
            label1.TabIndex = 11;
            label1.Text = "Status";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.Red;
            label2.Location = new Point(2032, 882);
            label2.Name = "label2";
            label2.Size = new Size(305, 54);
            label2.TabIndex = 12;
            label2.Text = "Problems found";
            // 
            // CacheCleaner
            // 
            AutoScaleDimensions = new SizeF(17F, 41F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(2907, 1427);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(checkedListBox1);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(dataGridView1);
            Name = "CacheCleaner";
            Text = "CacheCleaner";
            Load += CacheCleaner_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView1;
        private Button button1;
        private Button button2;
        private CheckedListBox checkedListBox1;
        private Label label1;
        private Label label2;
    }
}