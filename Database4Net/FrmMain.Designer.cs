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
            this.cboDatabaseType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDataSource = new System.Windows.Forms.TextBox();
            this.txtInitialCatalog = new System.Windows.Forms.TextBox();
            this.txtUserId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtNamespace = new System.Windows.Forms.TextBox();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.grpDatabase = new System.Windows.Forms.GroupBox();
            this.grpFile = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.chkIDentifierconversion = new System.Windows.Forms.CheckBox();
            this.btnCreateModel = new System.Windows.Forms.Button();
            this.btnCreateDictionary = new System.Windows.Forms.Button();
            this.prgCreateModel = new System.Windows.Forms.ProgressBar();
            this.lblCreateModel = new System.Windows.Forms.Label();
            this.grpDatabase.SuspendLayout();
            this.grpFile.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboDatabaseType
            // 
            this.cboDatabaseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDatabaseType.FormattingEnabled = true;
            this.cboDatabaseType.Items.AddRange(new object[] {
            "MySQL",
            "MSSQL",
            "Oracle"});
            this.cboDatabaseType.Location = new System.Drawing.Point(133, 82);
            this.cboDatabaseType.Name = "cboDatabaseType";
            this.cboDatabaseType.Size = new System.Drawing.Size(121, 20);
            this.cboDatabaseType.TabIndex = 0;
            this.cboDatabaseType.SelectedValueChanged += new System.EventHandler(this.cboDatabaseType_SelectedValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(62, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "数据库类型";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(62, 112);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "服务器名";
            // 
            // txtDataSource
            // 
            this.txtDataSource.Location = new System.Drawing.Point(133, 109);
            this.txtDataSource.Name = "txtDataSource";
            this.txtDataSource.Size = new System.Drawing.Size(100, 21);
            this.txtDataSource.TabIndex = 3;
            // 
            // txtInitialCatalog
            // 
            this.txtInitialCatalog.Location = new System.Drawing.Point(133, 137);
            this.txtInitialCatalog.Name = "txtInitialCatalog";
            this.txtInitialCatalog.Size = new System.Drawing.Size(100, 21);
            this.txtInitialCatalog.TabIndex = 4;
            // 
            // txtUserId
            // 
            this.txtUserId.Location = new System.Drawing.Point(133, 165);
            this.txtUserId.Name = "txtUserId";
            this.txtUserId.Size = new System.Drawing.Size(100, 21);
            this.txtUserId.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(62, 140);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "数据库名";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(62, 168);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "用户";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(133, 193);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(100, 21);
            this.txtPassword.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(62, 196);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "密码";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(54, 118);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 12);
            this.label6.TabIndex = 10;
            this.label6.Text = "模型命名空间";
            // 
            // txtNamespace
            // 
            this.txtNamespace.Location = new System.Drawing.Point(137, 115);
            this.txtNamespace.Name = "txtNamespace";
            this.txtNamespace.Size = new System.Drawing.Size(100, 21);
            this.txtNamespace.TabIndex = 11;
            // 
            // txtFilePath
            // 
            this.txtFilePath.Location = new System.Drawing.Point(137, 142);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(100, 21);
            this.txtFilePath.TabIndex = 12;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(54, 145);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 12);
            this.label7.TabIndex = 13;
            this.label7.Text = "文件输出路径";
            // 
            // grpDatabase
            // 
            this.grpDatabase.Controls.Add(this.label1);
            this.grpDatabase.Controls.Add(this.cboDatabaseType);
            this.grpDatabase.Controls.Add(this.label2);
            this.grpDatabase.Controls.Add(this.txtDataSource);
            this.grpDatabase.Controls.Add(this.txtInitialCatalog);
            this.grpDatabase.Controls.Add(this.txtUserId);
            this.grpDatabase.Controls.Add(this.label5);
            this.grpDatabase.Controls.Add(this.label3);
            this.grpDatabase.Controls.Add(this.txtPassword);
            this.grpDatabase.Controls.Add(this.label4);
            this.grpDatabase.Location = new System.Drawing.Point(12, 12);
            this.grpDatabase.Name = "grpDatabase";
            this.grpDatabase.Size = new System.Drawing.Size(300, 300);
            this.grpDatabase.TabIndex = 14;
            this.grpDatabase.TabStop = false;
            this.grpDatabase.Text = "数据库";
            // 
            // grpFile
            // 
            this.grpFile.Controls.Add(this.label8);
            this.grpFile.Controls.Add(this.chkIDentifierconversion);
            this.grpFile.Controls.Add(this.label6);
            this.grpFile.Controls.Add(this.txtNamespace);
            this.grpFile.Controls.Add(this.label7);
            this.grpFile.Controls.Add(this.txtFilePath);
            this.grpFile.Location = new System.Drawing.Point(318, 12);
            this.grpFile.Name = "grpFile";
            this.grpFile.Size = new System.Drawing.Size(300, 300);
            this.grpFile.TabIndex = 15;
            this.grpFile.TabStop = false;
            this.grpFile.Text = "文件";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(56, 171);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 15;
            this.label8.Text = "标识符转换";
            // 
            // chkIDentifierconversion
            // 
            this.chkIDentifierconversion.AutoSize = true;
            this.chkIDentifierconversion.Checked = true;
            this.chkIDentifierconversion.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIDentifierconversion.Location = new System.Drawing.Point(137, 170);
            this.chkIDentifierconversion.Name = "chkIDentifierconversion";
            this.chkIDentifierconversion.Size = new System.Drawing.Size(48, 16);
            this.chkIDentifierconversion.TabIndex = 14;
            this.chkIDentifierconversion.Text = "开启";
            this.chkIDentifierconversion.UseVisualStyleBackColor = true;
            // 
            // btnCreateModel
            // 
            this.btnCreateModel.Location = new System.Drawing.Point(624, 125);
            this.btnCreateModel.Name = "btnCreateModel";
            this.btnCreateModel.Size = new System.Drawing.Size(128, 23);
            this.btnCreateModel.TabIndex = 16;
            this.btnCreateModel.Text = "创建模型";
            this.btnCreateModel.UseVisualStyleBackColor = true;
            this.btnCreateModel.Click += new System.EventHandler(this.btnCreateModel_Click);
            // 
            // btnCreateDictionary
            // 
            this.btnCreateDictionary.Location = new System.Drawing.Point(624, 154);
            this.btnCreateDictionary.Name = "btnCreateDictionary";
            this.btnCreateDictionary.Size = new System.Drawing.Size(128, 23);
            this.btnCreateDictionary.TabIndex = 17;
            this.btnCreateDictionary.Text = "创建数据库字典";
            this.btnCreateDictionary.UseVisualStyleBackColor = true;
            this.btnCreateDictionary.Click += new System.EventHandler(this.btnCreateDictionary_Click);
            // 
            // prgCreateModel
            // 
            this.prgCreateModel.BackColor = System.Drawing.Color.Green;
            this.prgCreateModel.Location = new System.Drawing.Point(12, 318);
            this.prgCreateModel.Name = "prgCreateModel";
            this.prgCreateModel.Size = new System.Drawing.Size(776, 23);
            this.prgCreateModel.TabIndex = 10;
            this.prgCreateModel.Visible = false;
            // 
            // lblCreateModel
            // 
            this.lblCreateModel.AutoSize = true;
            this.lblCreateModel.BackColor = System.Drawing.Color.Transparent;
            this.lblCreateModel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblCreateModel.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCreateModel.ForeColor = System.Drawing.Color.Black;
            this.lblCreateModel.Location = new System.Drawing.Point(374, 324);
            this.lblCreateModel.Name = "lblCreateModel";
            this.lblCreateModel.Size = new System.Drawing.Size(0, 12);
            this.lblCreateModel.TabIndex = 18;
            this.lblCreateModel.Visible = false;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 479);
            this.Controls.Add(this.lblCreateModel);
            this.Controls.Add(this.prgCreateModel);
            this.Controls.Add(this.btnCreateDictionary);
            this.Controls.Add(this.btnCreateModel);
            this.Controls.Add(this.grpFile);
            this.Controls.Add(this.grpDatabase);
            this.Name = "FrmMain";
            this.Text = "主页";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.grpDatabase.ResumeLayout(false);
            this.grpDatabase.PerformLayout();
            this.grpFile.ResumeLayout(false);
            this.grpFile.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboDatabaseType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDataSource;
        private System.Windows.Forms.TextBox txtInitialCatalog;
        private System.Windows.Forms.TextBox txtUserId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtNamespace;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox grpDatabase;
        private System.Windows.Forms.GroupBox grpFile;
        private System.Windows.Forms.Button btnCreateModel;
        private System.Windows.Forms.Button btnCreateDictionary;
        private System.Windows.Forms.ProgressBar prgCreateModel;
        private System.Windows.Forms.Label lblCreateModel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox chkIDentifierconversion;
    }
}

