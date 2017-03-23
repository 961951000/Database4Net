using System;
using System.Configuration;
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
        /// 进度计数
        /// </summary>
        private int _progressCount = 0;
        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="action">进度条委托</param>
        /// <returns>创建字典表数量</returns>
        public int Start(Action<int, int> action)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["OracleConnection"].ConnectionString;
            using (var db = new OracleConnection { ConnectionString = connectionString })
            {
                var database = string.IsNullOrEmpty(db.Database) ? Regex.Match(connectionString, @"User Id=([^;]+)").Groups[1].Value : db.Database;
                var sql = "select table_name TableName,comments TableComment from user_tab_comments order by table_name  order by table_name";
                var tables = db.Query<Table>(sql).ToArray();
                action(_progressCount / 2, tables.Length);
                foreach (var table in tables)
                {
                    sql = $"select a.column_name ColumnName,a.data_type DataType,a.data_length DataLength,a.data_precision DataPrecision,a.data_scale DataScale,a.nullable Nullable,a.data_default DataDefault,b.comments Comments,d.constraint_type ConstraintType from user_tab_columns a left join user_col_comments b on a.table_name = b.table_name and a.column_name = b.column_name left join user_cons_columns c on a.table_name = c.table_name and a.column_name = c.column_name left join user_constraints d on c.constraint_name = d.constraint_name where a.table_name = '{table.TableName}' order by column_id";
                    table.TableColumns = db.Query<TableColumn>(sql).Distinct(new TableColumnNoComparer()).ToArray();
                    foreach (var colum in table.TableColumns)
                    {
                        colum.ConstraintType = ModelConvertTool.ConvertOracleConstraintType(colum.ConstraintType);
                    }
                    _progressCount++;
                    action(_progressCount / 2, tables.Length);
                }
                db.Dispose();
                #region 设置路径           
                if (!BaseTool.IsValidPath(_path))//替换非法目录
                {
                    _path = AppDomain.CurrentDomain.BaseDirectory;
                }
                _path = BaseTool.ReservedWordsReplace(Path.Combine(_path, $"{database}.xlsx"));
                #endregion
                if (tables.Length < 1)
                {
                    Loger.Debug($"数据库中不包含用户可见的数据表：数据库名 = {database}");
                    return 0;
                }
                else
                {
                    return CreateDictionary(_path, tables, (x) =>
                    {
                        action((_progressCount + x) / 2, tables.Length);
                    });
                }
            }
        }
    }
}
