namespace Database4Net.Util
{
    public class ModelConvertTool
    {
        /// <summary>
        /// 转换Oracle约束类型
        /// </summary>
        /// <param name="constraintType">约束类型</param>
        /// <returns>约束说明</returns>
        public static string ConvertOracleConstraintType(string constraintType)
        {
            var ret = string.Empty;
            if (string.IsNullOrEmpty(constraintType))
            {
                return ret;
            }
            switch (constraintType.ToUpper())
            {
                case "C":
                    {
                        ret = "检查一个表";
                    }
                    break;
                case "O":
                    {
                        ret = "只读视图";
                    }
                    break;
                case "P":
                    {
                        ret = "主键";
                    }
                    break;
                case "R":
                    {
                        ret = "外键";
                    }
                    break;
                case "U":
                    {
                        ret = "唯一键";
                    }
                    break;
                case "V":
                    {
                        ret = "检查选项在一个视图";
                    }
                    break;
                default:
                    {
                        Loger.Debug("ConvertOracleConstraintType:default");
                    }
                    break;
            }
            return ret;
        }
        /// <summary>
        /// 转换MySQL约束类型
        /// </summary>
        /// <param name="constraintType">约束类型</param>
        /// <returns>约束说明</returns>
        public static string ConvertMySqlConstraintType(string constraintType)
        {
            var ret = string.Empty;
            if (string.IsNullOrEmpty(constraintType))
            {
                return ret;
            }
            switch (constraintType.ToUpper())
            {
                case "PRI":
                    {
                        ret = "主键";
                    }
                    break;
                case "UNI":
                    {
                        ret = "唯一键";
                    }
                    break;
                case "MUL":
                    {
                        ret = "重复";
                    }
                    break;
                case "R":
                    {
                        ret = "外键";
                    }
                    break;
                case "U":
                    {
                        ret = "唯一键";
                    }
                    break;
                case "V":
                    {
                        ret = "检查选项在一个视图";
                    }
                    break;
                default:
                    {
                        Loger.Debug("ConvertOracleConstraintType:default");
                    }
                    break;
            }
            return ret;
        }
    }
}
