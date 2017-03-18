using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Dapper;
using Database4Net.Models;
using Oracle.ManagedDataAccess.Client;
using Database4Net.Util;

namespace Database4Net.Services
{
    /// <summary>
    /// Oracle创建数据库字典
    /// </summary>
    public class OracleCreateDictionary : DatabaseCreateDictionary
    {
        /// <summary>
        /// 文件输出路径
        /// </summary>
        private string _path;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">文件输出路径</param>
        public OracleCreateDictionary(string path)
        {
            _path = path;
        }
        /// <summary>
        /// 程序启动
        /// </summary>
        public int Start()
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["OracleConnection"].ConnectionString;
                using (var db = new OracleConnection { ConnectionString = connectionString })
                {
                    var sql = "select table_name TableName,comments TableComment from user_tab_comments order by table_name";
                    var tables = db.Query<Table>(sql).ToArray();
                    foreach (var table in tables)
                    {
                        sql = $"select a.column_name ColumnName,a.data_type DataType,a.data_length DataLength,a.data_precision DataPrecision,a.data_scale DataScale,a.nullable Nullable,a.data_default DataDefault,b.comments Comments,d.constraint_type ConstraintType from user_tab_columns a left join user_col_comments b on a.table_name = b.table_name and a.column_name = b.column_name left join user_cons_columns c on a.table_name = c.table_name and a.column_name = c.column_name left join user_constraints d on c.constraint_name = d.constraint_name where a.table_name = '{table.TableName}' order by column_id";
                        table.TableColumns = db.Query<TableColumn>(sql).ToArray();
                        foreach (var colum in table.TableColumns)
                        {
                            colum.ConstraintType = ModelConvertTool.ConvertOracleConstraintType(colum.ConstraintType);
                        }
                    }
                    if (db.State == ConnectionState.Open)
                    {
                        db.Close();
                    }
                    db.Dispose();
                    #region 设置路径           
                    if (string.IsNullOrEmpty(_path) || !BaseTool.IsValidPath(_path))
                    {
                        _path = AppDomain.CurrentDomain.BaseDirectory;
                    }
                    _path = Path.Combine(_path, $"{(string.IsNullOrEmpty(db.Database) ? Regex.Match(connectionString, @"User Id=([^;]+)").Groups[1].Value.ToUpper() : db.Database)}.xlsx");
                    #endregion
                    return CreateDictionary(_path, tables);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return 0;
        }
    }
}
