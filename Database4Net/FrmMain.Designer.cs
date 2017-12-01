namespace Database4Net
{
    partial class FrmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label6 = new System.Windows.Forms.Label();
            this.txtNamespace = new System.Windows.Forms.TextBox();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.grpFile = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.chkIDentifierconversion = new System.Windows.Forms.CheckBox();
            this.btnCreateModel = new System.Windows.Forms.Button();
            this.btnCreateDictionary = new System.Windows.Forms.Button();
            this.prgCreateModel = new System.Windows.Forms.ProgressBar();
            this.lblCreateModel = new System.Windows.Forms.Label();
            this.cboDatabaseType = new System.Windows.Forms.ComboBox();
            this.lblDatabaseType = new System.Windows.Forms.Label();
            this.lblDatabaseConnectionString = new System.Windows.Forms.Label();
            this.txtDatabaseConnectionString = new System.Windows.Forms.TextBox();
            this.grpDatabase = new System.Windows.Forms.GroupBox();
            this.grpFile.SuspendLayout();
            this.grpDatabase.SuspendLayout();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(54, 128);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "模型命名空间";
            // 
            // txtNamespace
            // 
            this.txtNamespace.Location = new System.Drawing.Point(137, 125);
            this.txtNamespace.Name = "txtNamespace";
            this.txtNamespace.Size = new System.Drawing.Size(100, 20);
            this.txtNamespace.TabIndex = 11;
            // 
            // txtFilePath
            // 
            this.txtFilePath.Location = new System.Drawing.Point(137, 154);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(100, 20);
            this.txtFilePath.TabIndex = 12;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(54, 157);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "文件输出路径";
            // 
            // grpFile
            // 
            this.grpFile.Controls.Add(this.label8);
            this.grpFile.Controls.Add(this.chkIDentifierconversion);
            this.grpFile.Controls.Add(this.label6);
            this.grpFile.Controls.Add(this.txtNamespace);
            this.grpFile.Controls.Add(this.label7);
            this.grpFile.Controls.Add(this.txtFilePath);
            this.grpFile.Location = new System.Drawing.Point(318, 13);
            this.grpFile.Name = "grpFile";
            this.grpFile.Size = new System.Drawing.Size(300, 325);
            this.grpFile.TabIndex = 15;
            this.grpFile.TabStop = false;
            this.grpFile.Text = "文件";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(56, 185);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(67, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "标识符转换";
            // 
            // chkIDentifierconversion
            // 
            this.chkIDentifierconversion.AutoSize = true;
            this.chkIDentifierconversion.Checked = true;
            this.chkIDentifierconversion.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIDentifierconversion.Location = new System.Drawing.Point(137, 184);
            this.chkIDentifierconversion.Name = "chkIDentifierconversion";
            this.chkIDentifierconversion.Size = new System.Drawing.Size(50, 17);
            this.chkIDentifierconversion.TabIndex = 14;
            this.chkIDentifierconversion.Text = "开启";
            this.chkIDentifierconversion.UseVisualStyleBackColor = true;
            // 
            // btnCreateModel
            // 
            this.btnCreateModel.Location = new System.Drawing.Point(624, 135);
            this.btnCreateModel.Name = "btnCreateModel";
            this.btnCreateModel.Size = new System.Drawing.Size(128, 25);
            this.btnCreateModel.TabIndex = 16;
            this.btnCreateModel.Text = "创建模型";
            this.btnCreateModel.UseVisualStyleBackColor = true;
            this.btnCreateModel.Click += new System.EventHandler(this.btnCreateModel_Click);
            // 
            // btnCreateDictionary
            // 
            this.btnCreateDictionary.Location = new System.Drawing.Point(624, 167);
            this.btnCreateDictionary.Name = "btnCreateDictionary";
            this.btnCreateDictionary.Size = new System.Drawing.Size(128, 25);
            this.btnCreateDictionary.TabIndex = 17;
            this.btnCreateDictionary.Text = "创建数据库字典";
            this.btnCreateDictionary.UseVisualStyleBackColor = true;
            this.btnCreateDictionary.Click += new System.EventHandler(this.btnCreateDictionary_Click);
            // 
            // prgCreateModel
            // 
            this.prgCreateModel.BackColor = System.Drawing.Color.Green;
            this.prgCreateModel.Location = new System.Drawing.Point(12, 345);
            this.prgCreateModel.Name = "prgCreateModel";
            this.prgCreateModel.Size = new System.Drawing.Size(776, 25);
            this.prgCreateModel.TabIndex = 10;
            this.prgCreateModel.Visible = false;
            // 
            // lblCreateModel
            // 
            this.lblCreateModel.AutoSize = true;
            this.lblCreateModel.BackColor = System.Drawing.Color.Transparent;
            this.lblCreateModel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblCreateModel.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCreateModel.ForeColor = System.Drawing.Color.Black;
            this.lblCreateModel.Location = new System.Drawing.Point(374, 351);
            this.lblCreateModel.Name = "lblCreateModel";
            this.lblCreateModel.Size = new System.Drawing.Size(0, 12);
            this.lblCreateModel.TabIndex = 18;
            this.lblCreateModel.Visible = false;
            // 
            // cboDatabaseType
            // 
            this.cboDatabaseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDatabaseType.FormattingEnabled = true;
            this.cboDatabaseType.Items.AddRange(new object[] {
            "MSSQL",
            "MySQL",
            "Oracle"});
            this.cboDatabaseType.Location = new System.Drawing.Point(79, 125);
            this.cboDatabaseType.Name = "cboDatabaseType";
            this.cboDatabaseType.Size = new System.Drawing.Size(121, 21);
            this.cboDatabaseType.TabIndex = 0;
            // 
            // lblDatabaseType
            // 
            this.lblDatabaseType.AutoSize = true;
            this.lblDatabaseType.Location = new System.Drawing.Point(6, 132);
            this.lblDatabaseType.Name = "lblDatabaseType";
            this.lblDatabaseType.Size = new System.Drawing.Size(64, 13);
            this.lblDatabaseType.TabIndex = 1;
            this.lblDatabaseType.Text = "数据库类型";
            // 
            // lblDatabaseConnectionString
            // 
            this.lblDatabaseConnectionString.AutoSize = true;
            this.lblDatabaseConnectionString.Location = new System.Drawing.Point(6, 157);
            this.lblDatabaseConnectionString.Name = "lblDatabaseConnectionString";
            this.lblDatabaseConnectionString.Size = new System.Drawing.Size(67, 13);
            this.lblDatabaseConnectionString.TabIndex = 10;
            this.lblDatabaseConnectionString.Text = "连接字符串";
            // 
            // txtDatabaseConnectionString
            // 
            this.txtDatabaseConnectionString.Location = new System.Drawing.Point(79, 157);
            this.txtDatabaseConnectionString.Name = "txtDatabaseConnectionString";
            this.txtDatabaseConnectionString.Size = new System.Drawing.Size(215, 20);
            this.txtDatabaseConnectionString.TabIndex = 11;
            // 
            // grpDatabase
            // 
            this.grpDatabase.Controls.Add(this.txtDatabaseConnectionString);
            this.grpDatabase.Controls.Add(this.lblDatabaseConnectionString);
            this.grpDatabase.Controls.Add(this.lblDatabaseType);
            this.grpDatabase.Controls.Add(this.cboDatabaseType);
            this.grpDatabase.Location = new System.Drawing.Point(12, 13);
            this.grpDatabase.Name = "grpDatabase";
            this.grpDatabase.Size = new System.Drawing.Size(300, 325);
            this.grpDatabase.TabIndex = 14;
            this.grpDatabase.TabStop = false;
            this.grpDatabase.Text = "数据库";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 519);
            this.Controls.Add(this.lblCreateModel);
            this.Controls.Add(this.prgCreateModel);
            this.Controls.Add(this.btnCreateDictionary);
            this.Controls.Add(this.btnCreateModel);
            this.Controls.Add(this.grpFile);
            this.Controls.Add(this.grpDatabase);
            this.Name = "FrmMain";
            this.Text = "主页";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.grpFile.ResumeLayout(false);
            this.grpFile.PerformLayout();
            this.grpDatabase.ResumeLayout(false);
            this.grpDatabase.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtNamespace;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox grpFile;
        private System.Windows.Forms.Button btnCreateModel;
        private System.Windows.Forms.Button btnCreateDictionary;
        private System.Windows.Forms.ProgressBar prgCreateModel;
        private System.Windows.Forms.Label lblCreateModel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox chkIDentifierconversion;
        private System.Windows.Forms.ComboBox cboDatabaseType;
        private System.Windows.Forms.Label lblDatabaseType;
        private System.Windows.Forms.Label lblDatabaseConnectionString;
        private System.Windows.Forms.TextBox txtDatabaseConnectionString;
        private System.Windows.Forms.GroupBox grpDatabase;
    }
}

