using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Database4Net.Models
{
    /// <summary>
    /// 字典表
    /// </summary>
    public class Table
    {
        /// <summary>
        /// 表名
        /// </summary>
        [Key]
        public string TableName { get; set; }
        /// <summary>
        /// 表备注
        /// </summary>
        public string TableComment { get; set; }
        /// <summary>
        /// 表中的列
        /// </summary>
        public TableColumn[] TableColumns { get; set; }
    }
    /// <summary>
    /// 表中的列
    /// </summary>
    public class TableColumn
    {
        /// <summary>
        /// 字段名
        /// </summary>
        [Description("字段名")]
        public string ColumnName { get; set; }
        /// <summary>
        /// 标识
        /// </summary>
        [Description("标识")]
        public string ConstraintType { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        [Description("数据类型")]
        public string DataType { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        [Description("长度")]
        public string DataLength { get; set; }
        /// <summary>
        /// 整数位
        /// </summary>
        [Description("整数位")]
        public string DataPrecision { get; set; }
        /// <summary>
        /// 小数位
        /// </summary>
        [Description("小数位")]
        public string DataScale { get; set; }
        /// <summary>
        /// 允许空值
        /// </summary>
        [Description("允许空值")]
        public string Nullable { get; set; }
        /// <summary>
        /// 缺省值
        /// </summary>
        [Description("缺省值")]
        public string DataDefault { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        public string Comments { get; set; }
    }

    /// <summary>
    /// 去"重复"时候的比较器(ColumnName，即认为是相同记录)
    /// </summary>
    public class TableColumnNoComparer : IEqualityComparer<TableColumn>
    {
        public bool Equals(TableColumn x, TableColumn y)
        {
            return x.ColumnName == y.ColumnName;
        }

        public int GetHashCode(TableColumn obj)
        {
            return obj.ColumnName.GetHashCode();
        }
    }
}
