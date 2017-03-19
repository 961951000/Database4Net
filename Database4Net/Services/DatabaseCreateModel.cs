using System;
using System.IO;

namespace Database4Net.Services
{
    public class DatabaseCreateModel
    {
        /// <summary>
        /// 文件写入
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="text">文本内容</param>
        protected bool WriteFile(string path, string text)
        {
            var folderPath = path.Substring(0, path.LastIndexOf("\\", StringComparison.Ordinal));
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            FileStream fs = null;
            StreamWriter sw = null;
            if (!File.Exists(path))
            {
                // 创建写入文件
                fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                sw = new StreamWriter(fs);
                sw.WriteLine(text);

            }
            else
            {
                // 删除文件在创建
                File.Delete(path);
                fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                sw = new StreamWriter(fs);
                sw.WriteLine(text);
            }
            sw.Flush();
            sw.Close();
            fs.Close();
            return true;
        }
    }
}
