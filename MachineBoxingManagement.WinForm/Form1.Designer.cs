namespace MachineBoxingManagement.WinForm
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxOperator = new System.Windows.Forms.TextBox();
            this.tabControlFunc = new System.Windows.Forms.TabControl();
            this.tabPageBoxIn = new System.Windows.Forms.TabPage();
            this.textBoxSeries = new System.Windows.Forms.TextBox();
            this.checkBoxTempSave = new System.Windows.Forms.CheckBox();
            this.buttonTempSave = new System.Windows.Forms.Button();
            this.buttonBatchSave = new System.Windows.Forms.Button();
            this.buttonTempModify = new System.Windows.Forms.Button();
            this.labelPnDesc = new System.Windows.Forms.Label();
            this.comboBoxMStyle = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.numericUpDownTurtleLevel = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownBoxNumber = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.comboBoxMOption = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBoxWarehouse = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxMIBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxModel = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxSSN = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxPN = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPageBoxOut = new System.Windows.Forms.TabPage();
            this.buttonQuery = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.textBoxModelQuery = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textBoxPNQuery = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.checkBoxTopMost = new System.Windows.Forms.CheckBox();
            this.tabControlFunc.SuspendLayout();
            this.tabPageBoxIn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTurtleLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBoxNumber)).BeginInit();
            this.tabPageBoxOut.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "操作者:";
            // 
            // textBoxOperator
            // 
            this.textBoxOperator.Location = new System.Drawing.Point(95, 30);
            this.textBoxOperator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBoxOperator.Name = "textBoxOperator";
            this.textBoxOperator.Size = new System.Drawing.Size(116, 25);
            this.textBoxOperator.TabIndex = 1;
            // 
            // tabControlFunc
            // 
            this.tabControlFunc.Controls.Add(this.tabPageBoxIn);
            this.tabControlFunc.Controls.Add(this.tabPageBoxOut);
            this.tabControlFunc.Location = new System.Drawing.Point(21, 92);
            this.tabControlFunc.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabControlFunc.Name = "tabControlFunc";
            this.tabControlFunc.SelectedIndex = 0;
            this.tabControlFunc.Size = new System.Drawing.Size(896, 556);
            this.tabControlFunc.TabIndex = 2;
            // 
            // tabPageBoxIn
            // 
            this.tabPageBoxIn.Controls.Add(this.textBoxSeries);
            this.tabPageBoxIn.Controls.Add(this.checkBoxTempSave);
            this.tabPageBoxIn.Controls.Add(this.buttonTempSave);
            this.tabPageBoxIn.Controls.Add(this.buttonBatchSave);
            this.tabPageBoxIn.Controls.Add(this.buttonTempModify);
            this.tabPageBoxIn.Controls.Add(this.labelPnDesc);
            this.tabPageBoxIn.Controls.Add(this.comboBoxMStyle);
            this.tabPageBoxIn.Controls.Add(this.label11);
            this.tabPageBoxIn.Controls.Add(this.numericUpDownTurtleLevel);
            this.tabPageBoxIn.Controls.Add(this.numericUpDownBoxNumber);
            this.tabPageBoxIn.Controls.Add(this.label10);
            this.tabPageBoxIn.Controls.Add(this.comboBoxMOption);
            this.tabPageBoxIn.Controls.Add(this.label9);
            this.tabPageBoxIn.Controls.Add(this.comboBoxWarehouse);
            this.tabPageBoxIn.Controls.Add(this.label8);
            this.tabPageBoxIn.Controls.Add(this.textBoxMIBox);
            this.tabPageBoxIn.Controls.Add(this.label7);
            this.tabPageBoxIn.Controls.Add(this.label6);
            this.tabPageBoxIn.Controls.Add(this.label5);
            this.tabPageBoxIn.Controls.Add(this.textBoxModel);
            this.tabPageBoxIn.Controls.Add(this.label4);
            this.tabPageBoxIn.Controls.Add(this.textBoxSSN);
            this.tabPageBoxIn.Controls.Add(this.label3);
            this.tabPageBoxIn.Controls.Add(this.textBoxPN);
            this.tabPageBoxIn.Controls.Add(this.label2);
            this.tabPageBoxIn.Location = new System.Drawing.Point(4, 25);
            this.tabPageBoxIn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPageBoxIn.Name = "tabPageBoxIn";
            this.tabPageBoxIn.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPageBoxIn.Size = new System.Drawing.Size(888, 527);
            this.tabPageBoxIn.TabIndex = 0;
            this.tabPageBoxIn.Text = "裝箱維護";
            this.tabPageBoxIn.UseVisualStyleBackColor = true;
            // 
            // textBoxSeries
            // 
            this.textBoxSeries.Location = new System.Drawing.Point(101, 224);
            this.textBoxSeries.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBoxSeries.Name = "textBoxSeries";
            this.textBoxSeries.Size = new System.Drawing.Size(224, 25);
            this.textBoxSeries.TabIndex = 26;
            // 
            // checkBoxTempSave
            // 
            this.checkBoxTempSave.AutoSize = true;
            this.checkBoxTempSave.Location = new System.Drawing.Point(352, 480);
            this.checkBoxTempSave.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBoxTempSave.Name = "checkBoxTempSave";
            this.checkBoxTempSave.Size = new System.Drawing.Size(104, 19);
            this.checkBoxTempSave.TabIndex = 25;
            this.checkBoxTempSave.Text = "立即暫存？";
            this.checkBoxTempSave.UseVisualStyleBackColor = true;
            // 
            // buttonTempSave
            // 
            this.buttonTempSave.Location = new System.Drawing.Point(480, 472);
            this.buttonTempSave.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonTempSave.Name = "buttonTempSave";
            this.buttonTempSave.Size = new System.Drawing.Size(111, 32);
            this.buttonTempSave.TabIndex = 24;
            this.buttonTempSave.Text = "暫存資料";
            this.buttonTempSave.UseVisualStyleBackColor = true;
            this.buttonTempSave.Click += new System.EventHandler(this.buttonTempSave_Click);
            // 
            // buttonBatchSave
            // 
            this.buttonBatchSave.Location = new System.Drawing.Point(712, 472);
            this.buttonBatchSave.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonBatchSave.Name = "buttonBatchSave";
            this.buttonBatchSave.Size = new System.Drawing.Size(132, 32);
            this.buttonBatchSave.TabIndex = 23;
            this.buttonBatchSave.Text = "批次儲存";
            this.buttonBatchSave.UseVisualStyleBackColor = true;
            this.buttonBatchSave.Click += new System.EventHandler(this.buttonBatchSave_Click);
            // 
            // buttonTempModify
            // 
            this.buttonTempModify.Location = new System.Drawing.Point(596, 472);
            this.buttonTempModify.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonTempModify.Name = "buttonTempModify";
            this.buttonTempModify.Size = new System.Drawing.Size(111, 32);
            this.buttonTempModify.TabIndex = 22;
            this.buttonTempModify.Text = "查看暫存";
            this.buttonTempModify.UseVisualStyleBackColor = true;
            this.buttonTempModify.Click += new System.EventHandler(this.buttonTempModify_Click);
            // 
            // labelPnDesc
            // 
            this.labelPnDesc.AutoSize = true;
            this.labelPnDesc.Location = new System.Drawing.Point(349, 96);
            this.labelPnDesc.Name = "labelPnDesc";
            this.labelPnDesc.Size = new System.Drawing.Size(52, 15);
            this.labelPnDesc.TabIndex = 9;
            this.labelPnDesc.Text = "PNDesc";
            // 
            // comboBoxMStyle
            // 
            this.comboBoxMStyle.FormattingEnabled = true;
            this.comboBoxMStyle.Location = new System.Drawing.Point(669, 21);
            this.comboBoxMStyle.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBoxMStyle.Name = "comboBoxMStyle";
            this.comboBoxMStyle.Size = new System.Drawing.Size(191, 23);
            this.comboBoxMStyle.TabIndex = 21;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(593, 24);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(71, 15);
            this.label11.TabIndex = 20;
            this.label11.Text = "樣式選項:";
            // 
            // numericUpDownTurtleLevel
            // 
            this.numericUpDownTurtleLevel.Location = new System.Drawing.Point(108, 348);
            this.numericUpDownTurtleLevel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.numericUpDownTurtleLevel.Name = "numericUpDownTurtleLevel";
            this.numericUpDownTurtleLevel.Size = new System.Drawing.Size(89, 25);
            this.numericUpDownTurtleLevel.TabIndex = 19;
            // 
            // numericUpDownBoxNumber
            // 
            this.numericUpDownBoxNumber.Location = new System.Drawing.Point(108, 269);
            this.numericUpDownBoxNumber.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.numericUpDownBoxNumber.Name = "numericUpDownBoxNumber";
            this.numericUpDownBoxNumber.Size = new System.Drawing.Size(89, 25);
            this.numericUpDownBoxNumber.TabIndex = 18;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 358);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(86, 15);
            this.label10.TabIndex = 17;
            this.label10.Text = "烏龜車層數:";
            // 
            // comboBoxMOption
            // 
            this.comboBoxMOption.FormattingEnabled = true;
            this.comboBoxMOption.Location = new System.Drawing.Point(365, 21);
            this.comboBoxMOption.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBoxMOption.Name = "comboBoxMOption";
            this.comboBoxMOption.Size = new System.Drawing.Size(191, 23);
            this.comboBoxMOption.TabIndex = 16;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.Red;
            this.label9.Location = new System.Drawing.Point(281, 24);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(78, 15);
            this.label9.TabIndex = 15;
            this.label9.Text = "*機台選項:";
            // 
            // comboBoxWarehouse
            // 
            this.comboBoxWarehouse.FormattingEnabled = true;
            this.comboBoxWarehouse.Location = new System.Drawing.Point(69, 21);
            this.comboBoxWarehouse.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBoxWarehouse.Name = "comboBoxWarehouse";
            this.comboBoxWarehouse.Size = new System.Drawing.Size(191, 23);
            this.comboBoxWarehouse.TabIndex = 14;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.Red;
            this.label8.Location = new System.Drawing.Point(16, 24);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(48, 15);
            this.label8.TabIndex = 13;
            this.label8.Text = "*庫房:";
            // 
            // textBoxMIBox
            // 
            this.textBoxMIBox.Location = new System.Drawing.Point(108, 306);
            this.textBoxMIBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBoxMIBox.Name = "textBoxMIBox";
            this.textBoxMIBox.Size = new System.Drawing.Size(45, 25);
            this.textBoxMIBox.TabIndex = 12;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 316);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 15);
            this.label7.TabIndex = 11;
            this.label7.Text = "箱內數量:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 271);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 15);
            this.label6.TabIndex = 10;
            this.label6.Text = "箱號:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 228);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 15);
            this.label5.TabIndex = 8;
            this.label5.Text = "裝箱系列:";
            // 
            // textBoxModel
            // 
            this.textBoxModel.Location = new System.Drawing.Point(101, 176);
            this.textBoxModel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBoxModel.Name = "textBoxModel";
            this.textBoxModel.Size = new System.Drawing.Size(224, 25);
            this.textBoxModel.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 179);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "Model:";
            // 
            // textBoxSSN
            // 
            this.textBoxSSN.Location = new System.Drawing.Point(101, 134);
            this.textBoxSSN.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBoxSSN.Name = "textBoxSSN";
            this.textBoxSSN.Size = new System.Drawing.Size(224, 25);
            this.textBoxSSN.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 138);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "SSN:";
            // 
            // textBoxPN
            // 
            this.textBoxPN.Location = new System.Drawing.Point(101, 86);
            this.textBoxPN.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBoxPN.Name = "textBoxPN";
            this.textBoxPN.Size = new System.Drawing.Size(224, 25);
            this.textBoxPN.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "P/N:";
            // 
            // tabPageBoxOut
            // 
            this.tabPageBoxOut.Controls.Add(this.buttonQuery);
            this.tabPageBoxOut.Controls.Add(this.label15);
            this.tabPageBoxOut.Controls.Add(this.label16);
            this.tabPageBoxOut.Controls.Add(this.label17);
            this.tabPageBoxOut.Controls.Add(this.textBoxModelQuery);
            this.tabPageBoxOut.Controls.Add(this.label13);
            this.tabPageBoxOut.Controls.Add(this.textBoxPNQuery);
            this.tabPageBoxOut.Controls.Add(this.label14);
            this.tabPageBoxOut.Location = new System.Drawing.Point(4, 25);
            this.tabPageBoxOut.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPageBoxOut.Name = "tabPageBoxOut";
            this.tabPageBoxOut.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPageBoxOut.Size = new System.Drawing.Size(888, 527);
            this.tabPageBoxOut.TabIndex = 1;
            this.tabPageBoxOut.Text = "取出維護";
            this.tabPageBoxOut.UseVisualStyleBackColor = true;
            // 
            // buttonQuery
            // 
            this.buttonQuery.Location = new System.Drawing.Point(716, 486);
            this.buttonQuery.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonQuery.Name = "buttonQuery";
            this.buttonQuery.Size = new System.Drawing.Size(132, 32);
            this.buttonQuery.TabIndex = 24;
            this.buttonQuery.Text = "查詢資料";
            this.buttonQuery.UseVisualStyleBackColor = true;
            this.buttonQuery.Click += new System.EventHandler(this.buttonQuery_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(315, 129);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(71, 15);
            this.label15.TabIndex = 26;
            this.label15.Text = "樣式選項:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(149, 129);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(71, 15);
            this.label16.TabIndex = 24;
            this.label16.Text = "機台選項:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(8, 129);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(41, 15);
            this.label17.TabIndex = 22;
            this.label17.Text = "庫房:";
            // 
            // textBoxModelQuery
            // 
            this.textBoxModelQuery.Location = new System.Drawing.Point(120, 82);
            this.textBoxModelQuery.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBoxModelQuery.Name = "textBoxModelQuery";
            this.textBoxModelQuery.Size = new System.Drawing.Size(224, 25);
            this.textBoxModelQuery.TabIndex = 13;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(35, 86);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(48, 15);
            this.label13.TabIndex = 12;
            this.label13.Text = "Model:";
            // 
            // textBoxPNQuery
            // 
            this.textBoxPNQuery.Location = new System.Drawing.Point(120, 30);
            this.textBoxPNQuery.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBoxPNQuery.Name = "textBoxPNQuery";
            this.textBoxPNQuery.Size = new System.Drawing.Size(224, 25);
            this.textBoxPNQuery.TabIndex = 11;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(35, 32);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(33, 15);
            this.label14.TabIndex = 10;
            this.label14.Text = "P/N:";
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.ForeColor = System.Drawing.Color.MediumBlue;
            this.labelVersion.Location = new System.Drawing.Point(820, 656);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(51, 15);
            this.labelVersion.TabIndex = 14;
            this.labelVersion.Text = "Version";
            // 
            // checkBoxTopMost
            // 
            this.checkBoxTopMost.AutoSize = true;
            this.checkBoxTopMost.Location = new System.Drawing.Point(701, 655);
            this.checkBoxTopMost.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxTopMost.Name = "checkBoxTopMost";
            this.checkBoxTopMost.Size = new System.Drawing.Size(104, 19);
            this.checkBoxTopMost.TabIndex = 15;
            this.checkBoxTopMost.Text = "最上層顯示";
            this.checkBoxTopMost.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(932, 690);
            this.Controls.Add(this.checkBoxTopMost);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.tabControlFunc);
            this.Controls.Add(this.textBoxOperator);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControlFunc.ResumeLayout(false);
            this.tabPageBoxIn.ResumeLayout(false);
            this.tabPageBoxIn.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTurtleLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBoxNumber)).EndInit();
            this.tabPageBoxOut.ResumeLayout(false);
            this.tabPageBoxOut.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxOperator;
        private System.Windows.Forms.TabControl tabControlFunc;
        private System.Windows.Forms.TabPage tabPageBoxIn;
        private System.Windows.Forms.TabPage tabPageBoxOut;
        private System.Windows.Forms.TextBox textBoxPN;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxSSN;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxModel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxMIBox;
        private System.Windows.Forms.ComboBox comboBoxWarehouse;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBoxMOption;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown numericUpDownTurtleLevel;
        private System.Windows.Forms.NumericUpDown numericUpDownBoxNumber;
        private System.Windows.Forms.ComboBox comboBoxMStyle;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label labelPnDesc;
        private System.Windows.Forms.Button buttonTempModify;
        private System.Windows.Forms.Button buttonBatchSave;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox textBoxModelQuery;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBoxPNQuery;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button buttonQuery;
        private System.Windows.Forms.Button buttonTempSave;
        private System.Windows.Forms.CheckBox checkBoxTempSave;
        private System.Windows.Forms.TextBox textBoxSeries;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.CheckBox checkBoxTopMost;
    }
}

