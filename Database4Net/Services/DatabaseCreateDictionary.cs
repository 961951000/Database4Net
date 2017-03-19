using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Database4Net.Models;
using Database4Net.Util;
using Microsoft.Office.Interop.Excel;
using System.IO;
using Action = System.Action;

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
            Application app = null;
            Workbook workBook = null;
            Worksheet sheet = null;
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
            app.Sheets.Add(Missing.Value, Missing.Value, tables.Length);
            var worksheet = (Worksheet)workBook.Sheets[1];
            worksheet.Name = "数据库字典汇总表";
            worksheet.Cells[1, 1] = "数据库字典汇总表";
            worksheet.Cells[2, 1] = "编号";
            worksheet.Cells[2, 2] = "表英文名称";
            worksheet.Cells[2, 3] = "表中文名称";
            worksheet.Cells[2, 4] = "数据说明";
            worksheet.Cells[2, 5] = "表结构描述(页号)";
            var type = typeof(TableColumn);
            var properties = type.GetProperties();
            Range range;
            for (var i = 0; i < tables.Length; i++)
            {
                var fields = tables[i].TableColumns;
                sheet = (Worksheet)workBook.Sheets[i + 2];//数据表
                sheet.Name = $"{(101d + i) / 100:F}";
                sheet.Cells[1, 1] = "数据库表结构设计明细";
                sheet.Cells[2, 1] = $"表名：{tables[i].TableName}";
                sheet.Cells[3, 1] = tables[i].TableComment;
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
            workBook?.Close();
            app?.Quit();
            GC.Collect();
            return tables.Length;
        }
    }
}
