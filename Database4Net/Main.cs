using System;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Database4Net.Services;
using Database4Net.Util;

namespace Database4Net
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            cboDatabaseType.SelectedIndex = 0;
            var onnectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
            txtDataSource.Text = Regex.Match(onnectionString, @"server=([^;]+)").Groups[1].Value;
            txtInitialCatalog.Text = Regex.Match(onnectionString, @"database=([^;]+)").Groups[1].Value;
            txtUserId.Text = Regex.Match(onnectionString, @"uid=([^;]+)").Groups[1].Value;
            txtPassword.Text = Regex.Match(onnectionString, @"pwd=([^;]+)").Groups[1].Value;
            txtNamespace.Text = ConfigurationManager.AppSettings["Namespace"];
            txtFilePath.Text = ConfigurationManager.AppSettings["FilePath"];
        }

        private void cboDatabaseType_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtInitialCatalog.ReadOnly = cboDatabaseType.SelectedIndex == 2;
                switch (cboDatabaseType.SelectedIndex)
                {
                    case 0:
                        {
                            var connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
                            txtDataSource.Text = Regex.Match(connectionString, @"server=([^;]+)").Groups[1].Value;
                            txtInitialCatalog.Text = Regex.Match(connectionString, @"database=([^;]+)").Groups[1].Value;
                            txtUserId.Text = Regex.Match(connectionString, @"uid=([^;]+)").Groups[1].Value;
                            txtPassword.Text = Regex.Match(connectionString, @"pwd=([^;]+)").Groups[1].Value;
                        }
                        break;
                    case 1:
                        {

                            var connectionString = ConfigurationManager.ConnectionStrings["MSSQLConnection"].ConnectionString;
                            txtDataSource.Text = Regex.Match(connectionString, @"Data Source=([^;]+)").Groups[1].Value;
                            txtInitialCatalog.Text = Regex.Match(connectionString, @"Initial Catalog=([^;]+)").Groups[1].Value;
                            txtUserId.Text = Regex.Match(connectionString, @"User Id=([^;]+)").Groups[1].Value;
                            txtPassword.Text = Regex.Match(connectionString, @"Password=([^;]+)").Groups[1].Value;
                        }
                        break;
                    case 2:
                        {

                            var connectionString = ConfigurationManager.ConnectionStrings["OracleConnection"].ConnectionString;
                            txtDataSource.Text = Regex.Match(connectionString, @"Data Source=([^;]+)").Groups[1].Value;
                            txtInitialCatalog.Text = string.Empty;
                            txtUserId.Text = Regex.Match(connectionString, @"User Id=([^;]+)").Groups[1].Value;
                            txtPassword.Text = Regex.Match(connectionString, @"Password=([^;]+)").Groups[1].Value;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }
        /// <summary>
        /// 生成模型
        /// </summary>
        private void btnCreateModel_Click(object sender, EventArgs e)
        {
            btnCreateModel.Enabled = false;
            try
            {
                switch (cboDatabaseType.SelectedIndex)
                {
                    case 0:
                        {
                            SetMySqlConnection();
                            var item = new MySqlCreateModel(ConfigurationManager.AppSettings["FilePath"], ConfigurationManager.AppSettings["Namespace"]);
                            lblCreateModel.Visible = true;
                            prgCreateModel.Visible = true;
                            prgCreateModel.Value = 0;
                            var count = item.Start((x, y) =>
                            {
                                prgCreateModel.Maximum = y;
                                prgCreateModel.Value = x;
                                SetPos(x);
                            });
                            lblCreateModel.Text = @"100%";
                            MessageBox.Show(count > 0 ? $"操作成功，新建{count}个模型！" : "操作失败，详细异常信息请查看错误日志！");
                        }
                        break;
                    case 1:
                        {
                            SetMsSqlConnection();
                            var item = new MsSqlCreateModel(ConfigurationManager.AppSettings["FilePath"], ConfigurationManager.AppSettings["Namespace"]);
                            lblCreateModel.Visible = true;
                            prgCreateModel.Visible = true;
                            prgCreateModel.Value = 0;
                            var count = item.Start((x, y) =>
                                {
                                    prgCreateModel.Maximum = y;
                                    prgCreateModel.Value = x;
                                    SetPos(x);
                                }
                            );
                            lblCreateModel.Text = @"100%";
                            MessageBox.Show(count > 0 ? $"操作成功，新建{count}个模型！" : "操作失败，详细异常信息请查看错误日志！");
                        }
                        break;
                    case 2:
                        {
                            SetOracleConnection();
                            var item = new OracleCreateModel(ConfigurationManager.AppSettings["FilePath"], ConfigurationManager.AppSettings["Namespace"]);
                            lblCreateModel.Visible = true;
                            prgCreateModel.Visible = true;
                            prgCreateModel.Value = 0;
                            var count = item.Start((x, y) =>
                                {
                                    prgCreateModel.Maximum = y;
                                    prgCreateModel.Value = x;
                                    SetPos(x);
                                }
                            );
                            lblCreateModel.Text = @"100%";
                            MessageBox.Show(count > 0 ? $"操作成功，新建{count}个模型！" : "操作失败，详细异常信息请查看错误日志！");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                MessageBox.Show(@"操作失败，详细异常信息请查看错误日志！");
            }
            finally
            {
                lblCreateModel.Visible = false;
                prgCreateModel.Visible = false;
                lblCreateModel.Text = string.Empty;
            }
            btnCreateModel.Enabled = true;
        }



        /// <summary>
        /// 生成数据库字典
        /// </summary>
        private void btnCreateDictionary_Click(object sender, EventArgs e)
        {
            btnCreateDictionary.Enabled = false; try
            {
                switch (cboDatabaseType.SelectedIndex)
                {
                    case 0:
                        {
                            SetMySqlConnection();
                            var item = new MySqlCreateDictionary(ConfigurationManager.AppSettings["FilePath"]);
                            lblCreateModel.Visible = true;
                            prgCreateModel.Visible = true;
                            prgCreateModel.Value = 0;
                            var count = item.Start((x, y) =>
                            {
                                prgCreateModel.Maximum = y;
                                prgCreateModel.Value = x;
                                SetPos(x);
                            });
                            lblCreateModel.Text = @"100%";
                            MessageBox.Show(count > 0 ? "操作成功！" : "操作失败，详细异常信息请查看错误日志！");
                        }
                        break;
                    case 1:
                        {
                            SetMsSqlConnection();
                            var item = new MsSqlCreateDictionary(ConfigurationManager.AppSettings["FilePath"]);
                            lblCreateModel.Visible = true;
                            prgCreateModel.Visible = true;
                            prgCreateModel.Value = 0;
                            var count = item.Start((x, y) =>
                            {
                                prgCreateModel.Maximum = y;
                                prgCreateModel.Value = x;
                                SetPos(x);
                            });
                            lblCreateModel.Text = @"100%";
                            MessageBox.Show(count > 0 ? "操作成功！" : "操作失败，详细异常信息请查看错误日志！");
                        }
                        break;
                    case 2:
                        {
                            SetOracleConnection();
                            var item = new OracleCreateDictionary(ConfigurationManager.AppSettings["FilePath"]);
                            lblCreateModel.Visible = true;
                            prgCreateModel.Visible = true;
                            prgCreateModel.Value = 0;
                            var count = item.Start((x, y) =>
                            {
                                prgCreateModel.Maximum = y;
                                prgCreateModel.Value = x;
                                SetPos(x);
                            });
                            lblCreateModel.Text = @"100%";
                            MessageBox.Show(count > 0 ? "操作成功！" : "操作失败，详细异常信息请查看错误日志！");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                MessageBox.Show(@"操作失败，详细异常信息请查看错误日志！");
            }
            finally
            {
                lblCreateModel.Visible = false;
                prgCreateModel.Visible = false;
                lblCreateModel.Text = string.Empty;
            }
            btnCreateDictionary.Enabled = true;
        }
        /// <summary>
        /// 重置MySQL连接字符串
        /// </summary>
        /// <returns></returns>
        private void SetMySqlConnection()
        {
            var arr = new[]
                    {
                            "Convert Zero Datetime=True",
                            "Allow Zero Datetime=True",
                            $"server={txtDataSource.Text}",
                            $"database={txtInitialCatalog.Text}",
                            $"uid={txtUserId.Text}",
                            $"pwd={txtPassword.Text}"
                        };
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            UpdateConnectionStringsConfig(config, "MySQLConnection", string.Join(";", arr), "MySql.Data.MySqlClient");
            config.AppSettings.Settings["Namespace"].Value = txtNamespace.Text;
            config.AppSettings.Settings["FilePath"].Value = txtFilePath.Text;
            //一定要记得保存，写不带参数的config.Save()也可以
            config.Save(ConfigurationSaveMode.Modified);
            //刷新，否则程序读取的还是之前的值（可能已装入内存）
            ConfigurationManager.RefreshSection("appSettings");
        }
        /// <summary>
        /// 重置MsSql连接字符串
        /// </summary>
        private void SetMsSqlConnection()
        {
            var arr = new[]
                    {
                           $"Initial Catalog={txtInitialCatalog.Text}",
                           $" User Id={txtUserId.Text}",
                           $" Password={txtPassword.Text}",
                           $" Data Source={txtDataSource.Text}"
                    };
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            UpdateConnectionStringsConfig(config, "MSSQLConnection", string.Join(";", arr), "System.Data.SqlClient");
            config.AppSettings.Settings["Namespace"].Value = txtNamespace.Text;
            config.AppSettings.Settings["FilePath"].Value = txtFilePath.Text;
            //一定要记得保存，写不带参数的config.Save()也可以
            config.Save(ConfigurationSaveMode.Modified);
            //刷新，否则程序读取的还是之前的值（可能已装入内存）
            ConfigurationManager.RefreshSection("appSettings");
        }
        /// <summary>
        /// 重置Oracle连接字符串
        /// </summary>
        private void SetOracleConnection()
        {
            string[] arr;
            if (txtUserId.Text.ToLower() == @"sys" || txtUserId.Text.ToLower() == "system")
            {
                arr = new[]
                  {        "DBA Privilege=SYSDBA",
                           $" User Id={txtUserId.Text}",
                           $" Password={txtPassword.Text}",
                           $" Data Source={txtDataSource.Text}"
                    };
            }
            else
            {
                arr = new[]
                  {
                           $" User Id={txtUserId.Text}",
                           $" Password={txtPassword.Text}",
                           $" Data Source={txtDataSource.Text}"
                    };
            }

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            UpdateConnectionStringsConfig(config, "OracleConnection", string.Join(";", arr), "Oracle.ManagedDataAccess.Client");
            config.AppSettings.Settings["Namespace"].Value = txtNamespace.Text;
            config.AppSettings.Settings["FilePath"].Value = txtFilePath.Text;
            //一定要记得保存，写不带参数的config.Save()也可以
            config.Save(ConfigurationSaveMode.Modified);
            //刷新，否则程序读取的还是之前的值（可能已装入内存）
            ConfigurationManager.RefreshSection("appSettings");
        }
        ///<summary> 
        ///更新连接字符串  
        ///</summary> 
        ///<param name="config">Configuration实例</param>
        ///<param name="name">连接字符串名称</param> 
        ///<param name="connectionString">连接字符串内容</param> 
        ///<param name="providerName">数据提供程序名称</param> 
        private void UpdateConnectionStringsConfig(Configuration config, string name, string connectionString, string providerName)
        {
            var isModified = config.ConnectionStrings.ConnectionStrings[name] != null;
            //新建一个连接字符串实例      
            var settings = new ConnectionStringSettings(name, connectionString, providerName);
            // 如果连接串已存在，首先删除它      
            if (isModified)
            {
                config.ConnectionStrings.ConnectionStrings.Remove(name);
            }
            // 将新的连接串添加到配置文件中.      
            config.ConnectionStrings.ConnectionStrings.Add(settings);
            // 保存对配置文件所作的更改      
            config.Save(ConfigurationSaveMode.Modified);
            //刷新，否则程序读取的还是之前的值（可能已装入内存）
            ConfigurationManager.RefreshSection("connectionStrings");
        }
        /// <summary>
        /// 设置进度条当前进度值
        /// </summary>
        /// <param name="value">进度条当前进度值</param>
        private void SetPos(int value)//设置进度条当前进度值
        {
            if (value < prgCreateModel.Maximum)//如果值有效
            {
                prgCreateModel.Value = value;//设置进度值
                //lblCreateModel.Text = $@"{(value * 100 / prgCreateModel.Maximum)}%";//显示百分比
                lblCreateModel.Text = $@"{value} / {prgCreateModel.Maximum}";//显示百分比
            }
            Application.DoEvents();//重点，必须加上，否则父子窗体都假死
        }
    }
}
