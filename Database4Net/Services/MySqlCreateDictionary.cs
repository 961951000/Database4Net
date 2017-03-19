using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Dapper;
using Database4Net.Models;
using Database4Net.Util;
using MySql.Data.MySqlClient;

namespace Database4Net.Services
{
    /// <summary>
    /// MySQL创建数据库字典
    /// </summary>
    public class MySqlCreateDictionary : DatabaseCreateDictionary
    {
        /// <summary>
        /// 文件输出路径
        /// </summary>
        private string _path;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">文件输出路径</param>
        public MySqlCreateDictionary(string path)
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
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
                using (var db = new MySqlConnection { ConnectionString = connectionString })
                {
                    var database = string.IsNullOrEmpty(db.Database) ? Regex.Match(connectionString, @"database=([^;]+)").Groups[1].Value : db.Database;
                    var sql = $"select table_name 'TableName',table_comment 'TableComment' from information_schema.tables where table_schema = '{database}'";
                    var tables = db.Query<Table>(sql).ToArray();
                    action(_progressCount / 2, tables.Length);
                    foreach (var table in tables)
                    {
                        sql = $"select column_name 'ColumnName',column_key 'ConstraintType',data_type 'DataType',numeric_precision 'DataLength',numeric_scale 'DataScale',case when is_nullable = 'yes' then 'Y' else 'N' end 'Nullable',column_default 'DataDefault',column_comment 'Comments' from information_schema.columns where table_schema = '{database}' and table_name = '{table.TableName}'";
                        table.TableColumns = db.Query<TableColumn>(sql).Distinct(new TableColumnNoComparer()).ToArray();
                        sql = $"select c.referenced_table_name 主键表名称,c.referenced_column_name 主键列名,c.table_name 外键表名称,c.column_name 外键列名,c.constraint_name 约束名,t.table_comment 表注释,r.update_rule 级联更新,r.delete_rule 级联删除 from information_schema.key_column_usage c join information_schema. tables t on t.table_name = c.table_name join information_schema.referential_constraints r on r.table_name = c.table_name and r.constraint_name = c.constraint_name and r.referenced_table_name = c.referenced_table_name where c.referenced_table_name is not null and c.table_schema = '{database}' and c.table_name = '{table.TableName}'";
                        var foreignKeyArr = db.Query<ForeignKey>(sql).ToArray();
                        foreach (var tableColumn in table.TableColumns)
                        {
                            tableColumn.ConstraintType = ModelConvertTool.ConvertMySqlConstraintType(tableColumn.ConstraintType);
                            foreach (var item in foreignKeyArr)
                            {
                                if (tableColumn.ColumnName == item.外键列名)
                                {
                                    tableColumn.ConstraintType = $"外键（主键表名称：{item.主键表名称}；主键列名：{item.主键列名}）";
                                }
                            }
                        }
                        _progressCount++;
                        action(_progressCount / 2, tables.Length);
                    }
                    db.Dispose();
                    #region 设置路径           
                    if (string.IsNullOrEmpty(_path) || !BaseTool.IsValidPath(_path))
                    {
                        _path = AppDomain.CurrentDomain.BaseDirectory;
                    }
                    _path = Path.Combine(_path, $"{database}_mysql.xlsx");
                    #endregion
                    return CreateDictionary(_path, tables, () =>
                    {
                        _progressCount++;
                        action(_progressCount / 2, tables.Length);
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
