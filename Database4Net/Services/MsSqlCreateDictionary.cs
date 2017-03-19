using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Dapper;
using Database4Net.Util;
using Database4Net.Models;

namespace Database4Net.Services
{
    /// <summary>
    /// MSSQL创建数据库字典
    /// </summary>
    public class MsSqlCreateDictionary : DatabaseCreateDictionary
    {
        /// <summary>
        /// 文件输出路径
        /// </summary>
        private string _path;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">文件输出路径</param>
        public MsSqlCreateDictionary(string path)
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
                var connectionString = ConfigurationManager.ConnectionStrings["MSSQLConnection"].ConnectionString;
                using (var db = new SqlConnection { ConnectionString = connectionString })
                {
                    var database = string.IsNullOrEmpty(db.Database) ? Regex.Match(connectionString, @"Catalog=([^;]+)").Groups[1].Value : db.Database;
                    var tables = db.Query<Table>("select a.name TableName,b.value TableComment from sys.tables a left join sys.extended_properties b on a.object_id = b.major_id and b.minor_id = 0").ToArray();
                    action(_progressCount / 2, tables.Length);
                    foreach (var table in tables)
                    {
                        var sql = $"select a.name columnname,b.name datatype,columnproperty(a.id, a.name, 'precision') datalength,isnull(columnproperty(a.id, a.name, 'scale'),'') datascale,case when a.isnullable = 1 then 'Y' else 'N' end nullable,isnull(e. text, 0) datadefault,isnull(g.[value], '') comments from syscolumns a left join systypes b on a.xusertype = b.xusertype inner join sysobjects d on a.id = d.id and d.xtype = 'u' left join syscomments e on a.cdefault = e.id left join sys.extended_properties g on a.id = g.major_id and a.colid = g.minor_id left join sys.extended_properties f on d.id = f.major_id and f.minor_id = 0 where d.name = '{table.TableName}' order by a.id,a.colorder";
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
                    #region 设置路径      
                    if (string.IsNullOrEmpty(_path) || !BaseTool.IsValidPath(_path))
                    {
                        _path = AppDomain.CurrentDomain.BaseDirectory;
                    }
                    _path = Path.Combine(_path, $"{database}_mssql.xlsx");
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
