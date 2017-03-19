using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Dapper;
using Database4Net.Models;
using Database4Net.Util;
using Oracle.ManagedDataAccess.Client;

namespace Database4Net.Services
{
    /// <summary>
    /// Oracle自动创建Model
    /// </summary>
    public class OracleCreateModel : DatabaseCreateModel
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
        /// 进度计数
        /// </summary>
        private int _progressCount = 0;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">文件输出路径</param>
        /// <param name="space">模型命名空间</param>
        public OracleCreateModel(string path, string space)
        {
            _path = path;
            _space = space;
        }
        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="action">进度条委托</param>
        /// <returns>创建模型数量</returns>
        public int Start(Action<int, int> action)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["OracleConnection"].ConnectionString;
            using (var db = new OracleConnection { ConnectionString = connectionString })
            {
                var database = string.IsNullOrEmpty(db.Database) ? Regex.Match(connectionString, @"User Id=([^;]+)").Groups[1].Value : db.Database;
                var sql = "select table_name TableName,comments TableComment from user_tab_comments order by table_name";
                var tables = db.Query<Table>(sql).ToArray();
                action(_progressCount / 2, tables.Length);
                foreach (var table in tables)
                {
                    sql = $"select a.column_name ColumnName,a.data_type DataType,a.data_length DataLength,a.data_precision DataPrecision,a.data_scale DataScale,a.nullable Nullable,a.data_default DataDefault,b.comments Comments,d.constraint_type ConstraintType from user_tab_columns a left join user_col_comments b on a.table_name = b.table_name and a.column_name = b.column_name left join user_cons_columns c on a.table_name = c.table_name and a.column_name = c.column_name left join user_constraints d on c.constraint_name = d.constraint_name where a.table_name = '{table.TableName}' order by column_id";
                    table.TableColumns = db.Query<TableColumn>(sql).Distinct(new TableColumnNoComparer()).ToArray();
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
                    return CreateModel(tables, () =>
                    {
                        _progressCount++;
                        action(_progressCount / 2, tables.Length);
                    });
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
            _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models");
            var ret = 0;
            var classNameList = new List<string>();//记录类名防止冲突
            foreach (var table in tables)
            {
                var sb = new StringBuilder();
                var sb1 = new StringBuilder();
                var className = BaseTool.ReplaceIllegalCharacter(table.TableName);
                if (!string.IsNullOrEmpty(className))
                {
                    if (className.LastIndexOf('_') != -1)
                    {
                        foreach (var str in className.Split('_'))
                        {
                            if (!string.IsNullOrEmpty(str))
                            {
                                className += str.Substring(0, 1).ToUpper() + str.Substring(1).ToLower();
                            }
                        }
                    }
                    else
                    {
                        className = className.Substring(0, 1).ToUpper() + className.Substring(1).ToLower();
                    }
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
                        var propertieName = BaseTool.ReplaceIllegalCharacter(column.ColumnName);
                        if (!string.IsNullOrEmpty(propertieName))
                        {
                            if (propertieName.LastIndexOf('_') != -1)
                            {
                                foreach (var str in propertieName.Split('_'))
                                {
                                    if (!string.IsNullOrEmpty(str))
                                    {
                                        propertieName += str.Substring(0, 1).ToUpper() + str.Substring(1).ToLower();
                                    }
                                }
                            }
                            else
                            {
                                propertieName = propertieName.Substring(0, 1).ToUpper() + propertieName.Substring(1).ToLower();
                            }
                            if (propertieName == className)
                            {
                                propertieName = $"_{propertieName}";
                            }
                            while (columnPropertieNameList.Count(x => x.Equals(propertieName)) > 0)
                            {
                                propertieName = $"_{propertieName}";
                            }
                            columnPropertieNameList.Add(propertieName);
                        }
                        if (!string.IsNullOrEmpty(column.Comments))
                        {
                            sb.Append("\t\t/// <summary>\r\n");
                            sb.Append("\t\t/// ").Append(Regex.Replace(column.Comments, @"[\r\n]", "")).Append("\r\n");
                            sb.Append("\t\t/// </summary>\r\n");
                        }
                        if (ModelConvertTool.ConvertOracleConstraintType(column.ConstraintType) == "主键")
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
                                case "BFILE":
                                    {
                                        sb.Append("\t\tpublic byte[] " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "BLOB":
                                    {
                                        sb.Append("\t\tpublic byte[] " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "CHAR":
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "CLOB":
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "DATE":
                                    {
                                        sb.Append("\t\tpublic DateTime? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "FLOAT":
                                    {
                                        sb.Append("\t\tpublic Decimal? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "INTEGER":
                                    {
                                        sb.Append("\t\tpublic Decimal? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "INTERVAL YEAR TO MONTH":
                                    {
                                        sb.Append("\t\tpublic int? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "INTERVAL DAY TO SECOND":
                                    {
                                        sb.Append("\t\tpublic TimeSpan? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "LONG":
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "LONG RAW":
                                    {
                                        sb.Append("\t\tpublic byte[] " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "NCHAR":
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "NCLOB":
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "NUMBER":
                                    {
                                        sb.Append("\t\tpublic decimal? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "NVARCHAR2":
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "RAW":
                                    {
                                        sb.Append("\t\tpublic byte[] " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "ROWID":
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "TIMESTAMP":
                                    {
                                        sb.Append("\t\tpublic DateTime? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "timestamp":
                                    {
                                        sb.Append("\t\tpublic DateTime? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "VARCHAR2":
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
