using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Dapper;
using Database4Net.Models;
using Database4Net.Util;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Database4Net.Services
{
    /// <summary>
    /// MySQL自动创建Model
    /// </summary>
    public class MySqlCreateModel : DatabaseCreateModel
    {
        /// <summary>
        /// 文件输出路径
        /// </summary>
        private string _path;
        /// <summary>
        /// 模型命名空间
        /// </summary>
        private string _space;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">文件输出路径</param>
        /// <param name="space">模型命名空间</param>
        public MySqlCreateModel(string path, string space)
        {
            _path = path;
            _space = space;
        }
        /// <summary>
        /// 进度计数
        /// </summary>
        private int _progressCount = 0;
        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="identifierConversion">是否开启标识符转换</param>
        /// <param name="action">进度条委托</param>
        /// <returns>创建模型数量</returns>
        public int Start(bool identifierConversion, Action<int, int> action)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
            using (var db = new MySqlConnection { ConnectionString = connectionString })
            {
                var database = string.IsNullOrEmpty(db.Database) ? Regex.Match(connectionString, @"database=([^;]+)").Groups[1].Value : db.Database;
                var sql = $"select table_name 'TableName',table_comment 'TableComment' from information_schema.tables where table_schema = '{database}' order by table_name";
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
                if (tables.Length < 1)
                {
                    Loger.Debug($"数据库中不包含用户可见的数据表：数据库名 = {database}");
                    return 0;
                }
                else
                {
                    if (identifierConversion)
                    {
                        return CreateModelIdentifierConversion(tables, () =>
                        {
                            _progressCount++;
                            action(_progressCount / 2, tables.Length);
                        });
                    }
                    else
                    {
                        return CreateModel(tables, () =>
                        {
                            _progressCount++;
                            action(_progressCount / 2, tables.Length);
                        });
                    }
                }
            }
        }
        /// <summary>
        /// 创建模型
        /// </summary>
        /// <param name="tables">数据表</param>
        /// <param name="action">进度条委托</param>
        /// <returns>创建模型数量</returns>
        private int CreateModel(IEnumerable<Table> tables, Action action)
        {
            var pathList = new List<string>();//记录文件路径防止冲突
            if (string.IsNullOrEmpty(_space))
            {
                _space = "Default.Models";
            }
            if (!BaseTool.IsValidPath(_path))//替换非法目录
            {
                _path = AppDomain.CurrentDomain.BaseDirectory;
            }
            _path = Path.Combine(_path, "Models");
            var ret = 0;
            var classNameList = new List<string>();//记录类名防止冲突
            foreach (var table in tables)
            {
                var sb = new StringBuilder();
                var sb1 = new StringBuilder();
                var className = table.TableName;
                if (!string.IsNullOrEmpty(className))
                {
                    if (className.LastIndexOf('_') != -1)
                    {
                        className = className.Split('_').Where(str => !string.IsNullOrEmpty(str)).Aggregate(className, (current, str) => current + (str.Substring(0, 1).ToUpper() + str.Substring(1).ToLower()));
                    }
                    else
                    {
                        className = className.Substring(0, 1).ToUpper() + className.Substring(1).ToLower();
                    }
                    className = BaseTool.ReplaceIllegalCharacter(className);
                }
                else
                {
                    className = "_";
                }
                while (classNameList.Count(x => x.Equals(className)) > 0)
                {
                    className = $"_{className}";
                }
                classNameList.Add(className);
                sb.Append("using System;\r\nusing System.ComponentModel.DataAnnotations;\r\nusing System.ComponentModel.DataAnnotations.Schema;\r\n\r\nnamespace ");
                sb.Append(_space);
                sb.Append("\r\n{\r\n");
                if (!string.IsNullOrEmpty(table.TableComment))
                {
                    sb.Append("\t/// <summary>\r\n");
                    sb.Append("\t/// ").Append(Regex.Replace(table.TableComment, @"[\r\n]", "")).Append("\r\n");
                    sb.Append("\t/// </summary>\r\n");
                }
                sb.Append("\tpublic class ");
                sb.Append(className);
                sb.Append("\r\n\t{\r\n");
                if (table.TableColumns.Length > 0)
                {
                    sb.Append("\t\t#region Model\r\n");
                    var columnPropertieNameList = new List<string>();//记录属性名称防止冲突
                    foreach (var column in table.TableColumns)
                    {
                        var propertieName = column.ColumnName;
                        if (!string.IsNullOrEmpty(propertieName))
                        {
                            if (propertieName.LastIndexOf('_') != -1)
                            {
                                propertieName = propertieName.Split('_').Where(str => !string.IsNullOrEmpty(str)).Aggregate(propertieName, (current, str) => current + (str.Substring(0, 1).ToUpper() + str.Substring(1).ToLower()));
                            }
                            else
                            {
                                propertieName = propertieName.Substring(0, 1).ToUpper() + propertieName.Substring(1).ToLower();
                            }
                            propertieName = BaseTool.ReplaceIllegalCharacter(propertieName);
                        }
                        else
                        {
                            propertieName = "_";
                        }
                        while (columnPropertieNameList.Count(x => x.Equals(propertieName)) > 0 || propertieName == className)
                        {
                            propertieName = $"_{propertieName}";
                        }
                        columnPropertieNameList.Add(propertieName);
                        if (!string.IsNullOrEmpty(column.Comments))
                        {
                            sb.Append("\t\t/// <summary>\r\n");
                            sb.Append("\t\t/// ").Append(Regex.Replace(column.Comments, @"[\r\n]", "")).Append("\r\n");
                            sb.Append("\t\t/// </summary>\r\n");
                        }
                        if (string.IsNullOrEmpty(column.DataType))
                        {
                            sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                        }
                        else
                        {
                            switch (column.DataType.ToUpper())
                            {
                                case "VARCHAR":
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "CHAR":
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "BLOB":
                                    {
                                        sb.Append("\t\tpublic byte? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "TEXT":
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "INTEGER":
                                    {
                                        sb.Append("\t\tpublic long? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "TINYINT":
                                    {
                                        sb.Append("\t\tpublic byte? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "SMALLINT":
                                    {
                                        sb.Append("\t\tpublic short? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "MEDIUMINT":
                                    {
                                        sb.Append("\t\tpublic int? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "BIT":
                                    {
                                        sb.Append("\t\tpublic bool? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "BIGINT":
                                    {
                                        sb.Append("\t\tpublic long? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "FLOAT":
                                    {
                                        sb.Append("\t\tpublic float? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "DOUBLE":
                                    {
                                        sb.Append("\t\tpublic double? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "Decimal":
                                    {
                                        sb.Append("\t\tpublic decimal? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "BOOLEAN":
                                    {
                                        sb.Append("\t\tpublic bool? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "ID":
                                    {
                                        sb.Append("\t\tpublic long? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "DATE":
                                    {
                                        sb.Append("\t\tpublic DATE? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "TIME":
                                    {
                                        sb.Append("\t\tpublic DateTime? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "DATETIME":
                                    {
                                        sb.Append("\t\tpublic DateTime? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "TIMESTAMP":
                                    {
                                        sb.Append("\t\tpublic DateTime? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "YEAR":
                                    {
                                        sb.Append("\t\tpublic DateTime? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "MONEY":
                                    {
                                        sb.Append("\t\tpublic decimal? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "IMAGE":
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "NVARCHAR":
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "JSON":
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                default:
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                            }
                        }
                        sb.Append("\t\t\tset;\r\n");
                        sb.Append("\t\t\tget;\r\n");
                        sb.Append("\t\t}\r\n");
                        sb.Append("\r\n");
                        sb1.Append(propertieName);
                        sb1.Append("=\" + ");
                        sb1.Append(propertieName);
                        sb1.Append(" + \",");
                    }
                    if (sb1.Length >= 5)
                    {
                        sb1.Remove(sb1.Length - 5, 5);
                    }
                    sb.Append("\t\tpublic override string ToString()\r\n");
                    sb.Append("\t\t{\r\n");
                    sb.Append("\t\t\treturn \"");
                    sb.Append(sb1);
                    sb.Append(";");
                    sb.Append("\r\n");
                    sb.Append("\t\t}\r\n");
                    sb.Append("\t\t#endregion Model\r\n");
                }
                else
                {
                    sb.Append("\r\n\r\n");
                    Loger.Debug($"表中不包含用户可见的列：表名 = {table.TableName}");
                }
                sb.Append("\t}\r\n").Append("}");
                var filePath = BaseTool.ReservedWordsReplace(Path.Combine(_path, $"{className}.cs"));
                while (pathList.Count(x => filePath == x) > 0)
                {
                    filePath = $"_{filePath}";
                }
                pathList.Add(filePath);
                if (WriteFile(filePath, sb.ToString()))
                {
                    ret++;
                }
                action();
            }
            return ret;
        }
        /// <summary>
        /// 创建模型
        /// </summary>
        /// <param name="tables">数据表</param>
        /// <param name="action">进度条委托</param>
        /// <returns>创建模型数量</returns>
        private int CreateModelIdentifierConversion(IEnumerable<Table> tables, Action action)
        {
            var pathList = new List<string>();//记录文件路径防止冲突
            if (string.IsNullOrEmpty(_space))
            {
                _space = "Default.Models";
            }
            if (!BaseTool.IsValidPath(_path))//替换非法目录
            {
                _path = AppDomain.CurrentDomain.BaseDirectory;
            }
            _path = Path.Combine(_path, "Models");
            var ret = 0;
            var classNameList = new List<string>();//记录类名防止冲突
            foreach (var table in tables)
            {
                var sb = new StringBuilder();
                var sb1 = new StringBuilder();
                var className = !string.IsNullOrEmpty(table.TableName) ? BaseTool.ReplaceIllegalCharacter(table.TableName) : "_";
                while (classNameList.Count(x => x.Equals(className)) > 0)
                {
                    className = $"_{className}";
                }
                classNameList.Add(className);
                sb.Append("using System;\r\n\r\nnamespace ");
                sb.Append(_space);
                sb.Append("\r\n{\r\n");
                if (!string.IsNullOrEmpty(table.TableComment))
                {
                    sb.Append("\t/// <summary>\r\n");
                    sb.Append("\t/// ").Append(Regex.Replace(table.TableComment, @"[\r\n]", "")).Append("\r\n");
                    sb.Append("\t/// </summary>\r\n");
                }
                sb.Append("\t[Table(\"").Append(table.TableName).Append("\")]\r\n");  //数据标记
                sb.Append("\tpublic class ");
                sb.Append(className);
                sb.Append("\r\n\t{\r\n");
                if (table.TableColumns.Length > 0)
                {
                    sb.Append("\t\t#region Model\r\n");
                    var order = 0;//记录主键数量
                    var columnPropertieNameList = new List<string>();//记录属性名称防止冲突
                    foreach (var column in table.TableColumns)
                    {
                        var propertieName = !string.IsNullOrEmpty(column.ColumnName) ? BaseTool.ReplaceIllegalCharacter(column.ColumnName) : "_";
                        while (columnPropertieNameList.Count(x => x.Equals(propertieName)) > 0 || propertieName == className)
                        {
                            propertieName = $"_{propertieName}";
                        }
                        columnPropertieNameList.Add(propertieName);
                        if (!string.IsNullOrEmpty(column.Comments))
                        {
                            sb.Append("\t\t/// <summary>\r\n");
                            sb.Append("\t\t/// ").Append(Regex.Replace(column.Comments, @"[\r\n]", "")).Append("\r\n");
                            sb.Append("\t\t/// </summary>\r\n");
                        }
                        if (column.ConstraintType == "主键")
                        {
                            sb.Append("\t\t[Key, Column(\"").Append(column.ColumnName).Append("\", Order = ").Append(order).Append(")]\r\n");
                            order++;
                        }
                        else
                        {
                            sb.Append("\t\t[Column(\"").Append(column.ColumnName).Append("\")]\r\n");
                        }
                        if (string.IsNullOrEmpty(column.DataType))
                        {
                            sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                        }
                        else
                        {
                            switch (column.DataType.ToUpper())
                            {
                                case "VARCHAR":
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "CHAR":
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "BLOB":
                                    {
                                        sb.Append("\t\tpublic byte? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "TEXT":
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "INTEGER":
                                    {
                                        sb.Append("\t\tpublic long? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "TINYINT":
                                    {
                                        sb.Append("\t\tpublic byte? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "SMALLINT":
                                    {
                                        sb.Append("\t\tpublic short? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "MEDIUMINT":
                                    {
                                        sb.Append("\t\tpublic int? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "BIT":
                                    {
                                        sb.Append("\t\tpublic bool? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "BIGINT":
                                    {
                                        sb.Append("\t\tpublic long? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "FLOAT":
                                    {
                                        sb.Append("\t\tpublic float? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "DOUBLE":
                                    {
                                        sb.Append("\t\tpublic double? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "Decimal":
                                    {
                                        sb.Append("\t\tpublic decimal? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "BOOLEAN":
                                    {
                                        sb.Append("\t\tpublic bool? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "ID":
                                    {
                                        sb.Append("\t\tpublic long? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "DATE":
                                    {
                                        sb.Append("\t\tpublic DATE? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "TIME":
                                    {
                                        sb.Append("\t\tpublic DateTime? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "DATETIME":
                                    {
                                        sb.Append("\t\tpublic DateTime? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "TIMESTAMP":
                                    {
                                        sb.Append("\t\tpublic DateTime? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "YEAR":
                                    {
                                        sb.Append("\t\tpublic DateTime? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "MONEY":
                                    {
                                        sb.Append("\t\tpublic decimal? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "IMAGE":
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "NVARCHAR":
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "JSON":
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                default:
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                            }
                        }
                        sb.Append("\t\t\tset;\r\n");
                        sb.Append("\t\t\tget;\r\n");
                        sb.Append("\t\t}\r\n");
                        sb.Append("\r\n");
                        sb1.Append(propertieName);
                        sb1.Append("=\" + ");
                        sb1.Append(propertieName);
                        sb1.Append(" + \",");
                    }
                    if (sb1.Length >= 5)
                    {
                        sb1.Remove(sb1.Length - 5, 5);
                    }
                    sb.Append("\t\tpublic override string ToString()\r\n");
                    sb.Append("\t\t{\r\n");
                    sb.Append("\t\t\treturn \"");
                    sb.Append(sb1);
                    sb.Append(";");
                    sb.Append("\r\n");
                    sb.Append("\t\t}\r\n");
                    sb.Append("\t\t#endregion Model\r\n");
                }
                else
                {
                    sb.Append("\r\n\r\n");
                    Loger.Debug($"表中不包含用户可见的列：表名 = {table.TableName}");
                }
                sb.Append("\t}\r\n").Append("}");
                var filePath = BaseTool.ReservedWordsReplace(Path.Combine(_path, $"{className}.cs"));
                while (pathList.Count(x => filePath == x) > 0)
                {
                    filePath = $"_{filePath}";
                }
                pathList.Add(filePath);
                if (WriteFile(filePath, sb.ToString()))
                {
                    ret++;
                }
                action();
            }
            return ret;
        }
    }
}
