using System;
using System.IO;
using Database4Net.Util;

namespace Database4Net.Services
{
    public class DatabaseCreateModel
    {
        /// <summary>
        /// 文件写入
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="text">文本内容</param>
        protected bool WriteFile(string filePath, string text)
        {
            var flag = false;
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                if (!File.Exists(filePath))
                {
                    // 创建写入文件
                    fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                    sw = new StreamWriter(fs);
                    sw.WriteLine(text);

                }
                else
                {
                    // 删除文件在创建
                    File.Delete(filePath);
                    fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                    sw = new StreamWriter(fs);
                    sw.WriteLine(text);
                }
                flag = true;
            }
            catch (Exception e)
            {
                Loger.Error(e);
                Loger.Debug($"文件操作有误：文件路径 = {filePath}");
            }
            finally
            {
                sw?.Flush();
                sw?.Close();
                fs?.Close();
            }
            return flag;
        }
    }
}
