using Infrastructuur.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructuur.extensions
{
    public static  class ExcelSystem  
    {
        public static MemoryStream WriteDataToExcel<T>(this List<T> data, string fileName, Dictionary<string, string> columnNames)
        {
            try
            {
                // Create a new workbook
                var workbook = new HSSFWorkbook();
                // Create a new worksheet
                // Create a new font and set its color to orange
                var font = workbook.CreateFont();

                // Create a new cell style and set the font
                var style = workbook.CreateCellStyle();
                style.FillForegroundColor = HSSFColor.Red.Index;
                style.FillPattern = FillPattern.SolidForeground;
                style.SetFont(font);
                var sheet = workbook.CreateSheet("Sheet1");

                // Write the header row
                var row = sheet.CreateRow(0);
                int cellIndex = 0;
                foreach (var column in columnNames)
                {
                    var cell = row.CreateCell(cellIndex);
                    cell.SetCellValue(column.Value);
                    cell.CellStyle = style;
                    cellIndex++;
                }

                for (int i = 0; i < data.Count; i++)
                {
                    row = sheet.CreateRow(i + 1);
                    cellIndex = 0;
                    foreach (var column in columnNames)
                    {
                        var value = typeof(T).GetProperty(column.Key)?.GetValue(data[i]);
                        row.CreateCell(cellIndex).SetCellValue(value?.ToString() ?? "no value");
                        cellIndex++;
                    }
                }
                MemoryStream stream = new MemoryStream();
                // Write the workbook to the memory stream
                workbook.Write(stream);
                stream.Position = 0;
            
                return stream;
            }
            catch
            {
                return null;
            }
        }
     
    }
}
