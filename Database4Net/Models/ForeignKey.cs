namespace Database4Net.Models
{
    /// <summary>
    /// 外键
    /// </summary>
    public class ForeignKey
    {
        public string 主键表名称 { get; set; }
        public string 主键列名 { get; set; }
        public string 外键表名称 { get; set; }
        public string 外键列名 { get; set; }
        public string 约束名 { get; set; }
        public string 级联更新 { get; set; }
        public string 级联删除 { get; set; }
    }
}
