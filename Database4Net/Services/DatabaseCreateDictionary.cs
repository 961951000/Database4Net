using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Database4Net.Models;
using System.IO;
using System.Runtime.InteropServices;
using Database4Net.Util;
using Action = System.Action;
using Microsoft.Office.Interop.Excel;

namespace Database4Net.Services
{
    public class DatabaseCreateDictionary
    {
        /// <summary>
        /// 字典表
        /// </summary>
        /// <param name="path">文件输出路径</param>
        /// <param name="tables">字典表</param>
        /// <param name="action">进度条委托</param>
        /// <returns>创建数据字典表数量</returns>
        protected int CreateDictionary(string path, Table[] tables, Action action)
        {
            var folderPath = path.Substring(0, path.LastIndexOf("\\", StringComparison.Ordinal));
            if (!Directory.Exists(folderPath))//目录不存在就创建
            {
                Directory.CreateDirectory(folderPath);
            }
            Application app = null;
            Workbook workBook = null;
            Worksheet sheet = null;
            try
            {
                app = new Application()
                {
                    Visible = false,
                    DisplayAlerts = false
                };
                app.Workbooks.Add(true);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                workBook = app.Workbooks.Add(Missing.Value);
                var worksheet = (Worksheet)workBook.Sheets[1];
                worksheet.Name = "数据库字典汇总表";
                worksheet.Cells[1, 1] = "数据库字典汇总表";
                worksheet.Cells[2, 1] = "编号";
                worksheet.Cells[2, 2] = "表名";
                worksheet.Cells[2, 3] = "备注";
                worksheet.Cells[2, 4] = "数据说明";
                worksheet.Cells[2, 5] = "表结构描述(页号)";
                var type = typeof(TableColumn);
                var properties = type.GetProperties();
                Range range;
                var time = DateTime.Now;
                //app.Sheets.Add(Missing.Value, workBook.Sheets[workBook.Sheets.Count], tables.Length);
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["Batch"]))
                {
                    for (var i = 0; i < tables.Length; i++)
                    {
                        app.Sheets.Add(Missing.Value, workBook.Sheets[workBook.Sheets.Count]);
                        sheet = (Worksheet)workBook.Sheets[i + 2];//数据表
                        sheet.Name = $"{(100d + i + 1) / 100:F}";
                        sheet.Cells[1, 1] = "数据库表结构设计明细";
                        sheet.Cells[2, 1] = $"表名：{tables[i].TableName}";
                        sheet.Cells[3, 1] = tables[i].TableComment;
                        var fields = tables[i].TableColumns;
                        for (var j = 0; j < properties.Length; j++)
                        {
                            foreach (var colum in properties[j].GetCustomAttributes().OfType<DescriptionAttribute>())
                            {
                                sheet.Cells[4, j + 1] = string.IsNullOrEmpty(colum.Description) ? properties[j].Name : colum.Description;
                            }
                            for (var k = 0; k < fields.Length; k++)
                            {
                                sheet.Cells[k + 5, j + 1] = type.GetProperty(properties[j].Name).GetValue(fields[k], null);
                            }
                        }
                        worksheet.Cells[i + 3, 1] = i + 1;
                        worksheet.Cells[i + 3, 2] = tables[i].TableName;
                        worksheet.Cells[i + 3, 3] = tables[i].TableComment;
                        worksheet.Cells[i + 3, 4] = string.Empty;
                        worksheet.Cells[i + 3, 5] = $"表{sheet.Name}";
                        #region  数据表样式
                        range = sheet.Range[sheet.Cells[1, 1], sheet.Cells[fields.Length + 4, properties.Length]];//选取单元格
                        range.VerticalAlignment = XlVAlign.xlVAlignCenter;//垂直居中设置 
                        range.EntireColumn.AutoFit();//自动调整列宽
                        range.Borders.LineStyle = XlLineStyle.xlContinuous;//所有框线 
                        range.Borders.Weight = XlBorderWeight.xlMedium;//边框常规粗细
                        range.Font.Name = "宋体";//设置字体 
                        range.Font.Size = 14;//字体大小  
                        range.NumberFormatLocal = "@";
                        range = sheet.Range[sheet.Cells[4, 1], sheet.Cells[fields.Length + 4, properties.Length]];//选取单元格
                        range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//水平居中设置                   
                        range = sheet.Range[sheet.Cells[1, 1], sheet.Cells[1, properties.Length]];//选取单元格
                        range.Merge(Missing.Value);
                        range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//水平居中设置 
                        range.Font.Bold = true;//字体加粗
                        range.Font.Size = 24;//字体大小                           
                        range = sheet.Range[sheet.Cells[2, 1], sheet.Cells[2, properties.Length]];//选取单元格
                        range.Merge(Missing.Value);
                        range = sheet.Range[sheet.Cells[3, 1], sheet.Cells[3, properties.Length]];//选取单元格
                        range.Merge(Missing.Value);
                        #endregion
                        action();
                    }
                }
                else
                {
                    foreach (var table in tables)
                    {
                        app.Sheets.Add(Missing.Value, workBook.Sheets[workBook.Sheets.Count]);
                        sheet = (Worksheet)workBook.Sheets[workBook.Sheets.Count];//数据表
                        sheet.Name = $"{(100d + workBook.Sheets.Count - 1) / 100:F}";
                        sheet.Cells[1, 1] = "数据库表结构设计明细";
                        sheet.Cells[2, 1] = $"表名：{table.TableName}";
                        sheet.Cells[3, 1] = table.TableComment;
                        var fields = table.TableColumns;
                        for (var j = 0; j < properties.Length; j++)
                        {
                            foreach (var colum in properties[j].GetCustomAttributes().OfType<DescriptionAttribute>())
                            {
                                sheet.Cells[4, j + 1] = string.IsNullOrEmpty(colum.Description) ? properties[j].Name : colum.Description;
                            }
                            for (var k = 0; k < fields.Length; k++)
                            {
                                sheet.Cells[k + 5, j + 1] = type.GetProperty(properties[j].Name).GetValue(fields[k], null);
                            }
                        }
                        worksheet.Cells[workBook.Sheets.Count + 1, 1] = workBook.Sheets.Count;
                        worksheet.Cells[workBook.Sheets.Count + 1, 2] = tables[workBook.Sheets.Count - 2].TableName;
                        worksheet.Cells[workBook.Sheets.Count + 1, 3] = tables[workBook.Sheets.Count - 2].TableComment;
                        worksheet.Cells[workBook.Sheets.Count + 1, 4] = string.Empty;
                        worksheet.Cells[workBook.Sheets.Count + 1, 5] = $"表{sheet.Name}";
                        #region  数据表样式
                        range = sheet.Range[sheet.Cells[1, 1], sheet.Cells[fields.Length + 4, properties.Length]];//选取单元格
                        range.VerticalAlignment = XlVAlign.xlVAlignCenter;//垂直居中设置 
                        range.EntireColumn.AutoFit();//自动调整列宽
                        range.Borders.LineStyle = XlLineStyle.xlContinuous;//所有框线 
                        range.Borders.Weight = XlBorderWeight.xlMedium;//边框常规粗细
                        range.Font.Name = "宋体";//设置字体 
                        range.Font.Size = 14;//字体大小  
                        range.NumberFormatLocal = "@";
                        range = sheet.Range[sheet.Cells[4, 1], sheet.Cells[fields.Length + 4, properties.Length]];//选取单元格
                        range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//水平居中设置                   
                        range = sheet.Range[sheet.Cells[1, 1], sheet.Cells[1, properties.Length]];//选取单元格
                        range.Merge(Missing.Value);
                        range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//水平居中设置 
                        range.Font.Bold = true;//字体加粗
                        range.Font.Size = 24;//字体大小                           
                        range = sheet.Range[sheet.Cells[2, 1], sheet.Cells[2, properties.Length]];//选取单元格
                        range.Merge(Missing.Value);
                        range = sheet.Range[sheet.Cells[3, 1], sheet.Cells[3, properties.Length]];//选取单元格
                        range.Merge(Missing.Value);
                        #endregion
                        action();
                    }
                }
                Loger.Debug($"操作耗时{(DateTime.Now - time).TotalSeconds}秒");
                #region  汇总表样式             
                range = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[tables.Length + 2, 5]];//选取单元格
                range.HorizontalAlignment = XlHAlign.xlHAlignCenter;//水平居中设置 
                range.VerticalAlignment = XlVAlign.xlVAlignCenter;//垂直居中设置 
                range.ColumnWidth = 30;//设置列宽
                range.Borders.LineStyle = XlLineStyle.xlContinuous;//所有框线
                range.Borders.Weight = XlBorderWeight.xlMedium;//边框常规粗细  
                range.Font.Name = "宋体";//设置字体 
                range.Font.Size = 14;//字体大小 
                range.NumberFormatLocal = "@";
                range = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, 5]];//选取单元格
                range.Merge(Missing.Value);
                range.Font.Bold = true;//字体加粗
                range.Font.Size = 24;//字体大小 
                #endregion
                sheet?.SaveAs(path);
                worksheet.SaveAs(path);
                return tables.Length;
            }
            finally
            {
                workBook?.Close();
                app?.Quit();
                KillExcel(app);
                GC.Collect();
            }
        }

        /// <summary>
        /// 获得进程、线程ID
        /// </summary>
        /// <param name="hwnd">指定窗口句柄</param>
        /// <param name="id">进程ID</param>
        /// <returns></returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int id);
        /// <summary>
        /// 关闭Excel进程
        /// </summary>
        /// <param name="excel">Excel进程</param>
        private static void KillExcel(_Application excel)
        {
            if (excel == null) { return; }
            var hwnd = new IntPtr(excel.Hwnd); //得到这个句柄，具体作用是得到这块内存入口 
            GetWindowThreadProcessId(hwnd, out int id); //得到本进程唯一标志k
            var p = Process.GetProcessById(id); //得到对进程id的引用
            p.Kill(); //关闭进程k
        }
    }
}
