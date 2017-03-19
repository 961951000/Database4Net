using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Dapper;
using Database4Net.Util;
using Database4Net.Models;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Database4Net.Services
{
    /// <summary>
    /// MSSQL自动创建Model
    /// </summary>
    public class MsSqlCreateModel : DatabaseCreateModel
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
        public MsSqlCreateModel(string path, string space)
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
        /// <param name="action">进度条委托</param>
        /// <returns>创建模型数量</returns>
        public int Start(Action<int, int> action)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MSSQLConnection"].ConnectionString;
            using (var db = new SqlConnection { ConnectionString = connectionString })
            {
                var tables = db.Query<Table>("select a.name TableName,b.value TableComment from sys.tables a left join sys.extended_properties b on a.object_id = b.major_id and b.minor_id = 0").ToArray();
                action(_progressCount / 2, tables.Length);
                foreach (var table in tables)
                {
                    var sql = $"select a.name columnname,b.name datatype,columnproperty(a.id, a.name, 'precision') datalength,isnull(columnproperty(a.id, a.name, 'scale'),0) datascale,case when a.isnullable = 1 then 'Y' else 'N' end nullable,isnull(e. text, '') datadefault,isnull(g.[value], '') comments from syscolumns a left join systypes b on a.xusertype = b.xusertype inner join sysobjects d on a.id = d.id and d.xtype = 'u' left join syscomments e on a.cdefault = e.id left join sys.extended_properties g on a.id = g.major_id and a.colid = g.minor_id left join sys.extended_properties f on d.id = f.major_id and f.minor_id = 0 where d.name = '{table.TableName}' order by a.id,a.colorder";
                    table.TableColumns = db.Query<TableColumn>(sql).Distinct(new TableColumnNoComparer()).ToArray();
                    sql = $"SELECT col.name FROM sys.indexes idx JOIN sys.index_columns idxCol ON (idx.object_id = idxCol.object_id AND idx.index_id = idxCol.index_id AND idx.is_primary_key = 1)JOIN sys.tables tab ON (idx.object_id = tab.object_id) JOIN sys.columns col ON (idx.object_id = col.object_id AND idxCol.column_id = col.column_id) WHERE tab.name = '{table.TableName}'";
                    var primaryKeyArr = db.Query<string>(sql).ToArray();
                    sql = $"select b.rkey 主键列id,(select name from syscolumns where colid = b.rkey and id = b.rkeyid) 主键列名,b.fkeyid 外键表id,object_name(b.fkeyid) 外键表名称,b.fkey 外键列id,(select name from syscolumns where colid = b.fkey and id = b.fkeyid) 外键列名,objectproperty(a.id, 'cnstisupdatecascade') 级联更新,objectproperty(a.id, 'cnstisdeletecascade') 级联删除 from sysobjects a join sysforeignkeys b on a.id = b.constid join sysobjects c on a.parent_obj = c.id where a.xtype = 'f' and c.xtype = 'u' and object_name(b.rkeyid) = '{table.TableName}'";
                    var foreignKeyArr = db.Query<ForeignKey>(sql).ToArray();
                    sql = $"select maincol.name from sys.foreign_keys fk join sys.all_objects osub on (fk.parent_object_id = osub.object_id)join sys.all_objects omain on (fk.referenced_object_id = omain.object_id)join sys.foreign_key_columns fkcols on (fk.object_id = fkcols.constraint_object_id)join sys.columns subcol on (osub.object_id = subcol.object_id and fkcols.parent_column_id = subcol.column_id)join sys.columns maincol on (omain.object_id = maincol.object_id and fkcols.referenced_column_id = maincol.column_id) where osub.name = '{table.TableName}' or maincol.name = '{table.TableName}'";
                    var uniqueArr = db.Query<string>(sql).ToArray();
                    sql = $"select col.name from sys.check_constraints chk join sys.tables tab on (chk.parent_object_id = tab.object_id) join sys.columns col on (chk.parent_object_id = col.object_id and chk.parent_column_id = col.column_id) where tab.name = '{table.TableName}'";
                    var checkArr = db.Query<string>(sql).ToArray();
                    foreach (var tableColumn in table.TableColumns)
                    {
                        if (tableColumn.DataScale == "0")
                        {
                            tableColumn.DataScale = string.Empty;
                        }
                        var count = 0;
                        foreach (var item in primaryKeyArr)
                        {
                            if (tableColumn.ColumnName == item)
                            {
                                tableColumn.ConstraintType = "主键";
                                count++;
                            }
                        }
                        if (count > 0)
                        {
                            foreach (var item in foreignKeyArr)
                            {
                                if (tableColumn.ColumnName == item.外键列名)
                                {
                                    tableColumn.ConstraintType = $"外键（主键表名称：{item.主键表名称}；主键列名：{item.主键列名}）";
                                    count++;
                                }
                            }
                        }
                        if (count > 0)
                        {
                            foreach (var item in uniqueArr)
                            {
                                if (tableColumn.ColumnName == item)
                                {
                                    tableColumn.ConstraintType = "唯一键";
                                    count++;
                                }
                            }
                        }
                        if (count > 0)
                        {
                            foreach (var item in checkArr)
                            {
                                if (tableColumn.ColumnName == item)
                                {
                                    tableColumn.ConstraintType = "检查";
                                    count++;
                                }
                            }
                        }
                    }
                    _progressCount++;
                    action(_progressCount / 2, tables.Length);
                }
                db.Dispose();
                return CreateModel(tables, () =>
                {
                    _progressCount++;
                    action(_progressCount / 2, tables.Length);
                });
            }
        }
        /// <summary>
        /// 创建模型
        /// </summary>
        /// <param name="tables">数据表</param>
        /// <param name="action">进度条委托</param>
        /// <returns>创建模型数量</returns>
        private int CreateModel(Table[] tables, Action action)
        {
            if (string.IsNullOrEmpty(_space))
            {
                _space = "Default.Models";
            }
            if (string.IsNullOrEmpty(_path) || !BaseTool.IsValidPath(_path))
            {
                _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models");
            }
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            var count = 0;
            var tableList = new List<string>();
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
                while (tableList.Count(x => x.Equals(className)) > 0)
                {
                    className += "_";
                }
                tableList.Add(className);
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
                    var order = 0;
                    var columnList = new List<string>();
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
                            else
                            {
                                while (columnList.Count(x => x.Equals(propertieName)) > 0)
                                {
                                    propertieName += "_";
                                }
                                columnList.Add(propertieName);
                            }
                        }
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
                            sb.Append("\t\t[Column(\"").Append(column.ColumnName).Append("\")]\r\n");  //数据标记
                        }
                        if (string.IsNullOrEmpty(column.DataType))
                        {
                            sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                        }
                        else
                        {
                            switch (column.DataType.ToUpper())
                            {
                                case "BIGINT":
                                    {
                                        sb.Append("\t\tpublic long? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "BINARY":
                                    {
                                        sb.Append("\t\tpublic byte? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "BIT":
                                    {
                                        sb.Append("\t\tpublic bool? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "CHAR":
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "DATE":
                                    {
                                        sb.Append("\t\tpublic DateTime? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "DATETIME":
                                    {
                                        sb.Append("\t\tpublic DateTime? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "DATETIME2":
                                    {
                                        sb.Append("\t\tpublic DateTime? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "DATETIMEOFFSET":
                                    {
                                        sb.Append("\t\tpublic DateTime? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "DECIMAL":
                                    {
                                        sb.Append("\t\tpublic decimal? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "FILESTREAM":
                                    {
                                        sb.Append("\t\tpublic byte[] " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "FLOAT":
                                    {
                                        sb.Append("\t\tpublic double? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "IMAGE":
                                    {
                                        sb.Append("\t\tpublic byte[] " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "INT":
                                    {
                                        sb.Append("\t\tpublic int? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "MONEY":
                                    {
                                        sb.Append("\t\tpublic decimal? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "NCHAR":
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "NTEXT":
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "NUMERIC":
                                    {
                                        sb.Append("\t\tpublic decimal? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "NVARCHAR":
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "REAL":
                                    {
                                        sb.Append("\t\tpublic float? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "ROWVERSION":
                                    {
                                        sb.Append("\t\tpublic byte[] " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "SMALLDATETIME":
                                    {
                                        sb.Append("\t\tpublic DateTime? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "SMALLINT":
                                    {
                                        sb.Append("\t\tpublic short? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "SMALLMONEY":
                                    {
                                        sb.Append("\t\tpublic decimal? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "SQL_VARIANT":
                                    {
                                        sb.Append("\t\tpublic object " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "TEXT":
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "TIME":
                                    {
                                        sb.Append("\t\tpublic TimeSpan? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "TIMESTAMP":
                                    {
                                        sb.Append("\t\tpublic byte[] " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "TINYINT":
                                    {
                                        sb.Append("\t\tpublic byte[] " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "UNIQUEIDENTIFIER":
                                    {
                                        sb.Append("\t\tpublic Guid? " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "VARBINARY":
                                    {
                                        sb.Append("\t\tpublic byte[] " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "VARCHAR":
                                    {
                                        sb.Append("\t\tpublic string " + propertieName + "\r\n\t\t{\r\n");
                                    }
                                    break;
                                case "XML":
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
                var filePath = Path.Combine(_path, $"{className}.cs");
                if (WriteFile(filePath, sb.ToString()))
                {
                    count++;
                }
                action();
            }
            return count;
        }
    }
}
