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

namespace Infrastructuur.extensions
{
    public static  class ExcelSystem 
    {

        public static bool WriteStoryDataToExcell(this List<StoryzonEntity> stories )
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
                var cell = row.CreateCell(0);
                cell.SetCellValue("Title");
                cell.CellStyle = style;
                cell = row.CreateCell(1);
                cell.SetCellValue("Genre");
                cell.CellStyle = style;
                cell = row.CreateCell(2);
                cell.SetCellValue("Rating");
                cell.CellStyle = style;
                cell = row.CreateCell(3);
                cell.SetCellValue("AddedDate");
                cell.CellStyle = style;
              
                for (int i = 0; i < stories.Count; i++)
                {
                    row = sheet.CreateRow(i+1);
                    row.CreateCell(0).SetCellValue(@stories[i].Title);
                    row.CreateCell(1).SetCellValue(@stories[i].Genre);
                    row.CreateCell(2).SetCellValue(@stories[i].Rating.ToString() ?? "no rating");
                    row.CreateCell(3).SetCellValue(@stories[i].AddedDate);
                }
                var fileName = $"StoryData.xls";

                // Save the workbook to a file
                using (var fileStream = File.Create(fileName))
                {
                    workbook.Write(fileStream);

                }
                return true;
            }
            catch
            {

                return false;
            }
       
        }
    }
}
