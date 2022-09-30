namespace MachineBoxingManagement.WinForm
{
    partial class FormTakeOut
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.buttonCloseOut = new System.Windows.Forms.Button();
            this.buttonTakeout = new System.Windows.Forms.Button();
            this.buttonExport = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(22, 13);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 27;
            this.dataGridView1.Size = new System.Drawing.Size(847, 401);
            this.dataGridView1.TabIndex = 0;
            // 
            // buttonCloseOut
            // 
            this.buttonCloseOut.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonCloseOut.Location = new System.Drawing.Point(0, 491);
            this.buttonCloseOut.Margin = new System.Windows.Forms.Padding(2);
            this.buttonCloseOut.Name = "buttonCloseOut";
            this.buttonCloseOut.Size = new System.Drawing.Size(883, 36);
            this.buttonCloseOut.TabIndex = 1;
            this.buttonCloseOut.Text = "關閉";
            this.buttonCloseOut.UseVisualStyleBackColor = true;
            // 
            // buttonTakeout
            // 
            this.buttonTakeout.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonTakeout.Location = new System.Drawing.Point(0, 455);
            this.buttonTakeout.Margin = new System.Windows.Forms.Padding(2);
            this.buttonTakeout.Name = "buttonTakeout";
            this.buttonTakeout.Size = new System.Drawing.Size(883, 36);
            this.buttonTakeout.TabIndex = 2;
            this.buttonTakeout.Text = "取出機台";
            this.buttonTakeout.UseVisualStyleBackColor = true;
            // 
            // buttonExport
            // 
            this.buttonExport.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonExport.Location = new System.Drawing.Point(0, 419);
            this.buttonExport.Margin = new System.Windows.Forms.Padding(2);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(883, 36);
            this.buttonExport.TabIndex = 3;
            this.buttonExport.Text = "輸出Excel";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // FormTakeOut
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(883, 527);
            this.Controls.Add(this.buttonExport);
            this.Controls.Add(this.buttonTakeout);
            this.Controls.Add(this.buttonCloseOut);
            this.Controls.Add(this.dataGridView1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormTakeOut";
            this.Text = "FormTakeOut";
            this.Load += new System.EventHandler(this.FormTakeOut_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button buttonCloseOut;
        private System.Windows.Forms.Button buttonTakeout;
        private System.Windows.Forms.Button buttonExport;
    }
}