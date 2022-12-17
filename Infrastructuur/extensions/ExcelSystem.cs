using Infrastructuur.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Infrastructuur.extensions
{
    public  class ExcelSystem
    {
        public  void WriteStoryToExcell( List<StoryzonEntity> stories)
        {
            // Create an instance of the Excel application
            var excelApp = new Application();
            // Create a new workbook
            var workbook = excelApp.Workbooks.Add();

            // Get the first worksheet
            var worksheet = workbook.Sheets[1];

            // Write data to the worksheet
            worksheet.Cells[1, 1].Value = "Title";
            worksheet.Cells[1, 2].Value = "Genre";
            worksheet.Cells[1, 3].Value = "Rating";
            worksheet.Cells[1, 4].Value = "AddedDate";

            for (int i = 0; i < stories.Count; i++)
            {
                worksheet.Cells[i, 1].Value = stories[i].Title;
                worksheet.Cells[i, 2].Value = stories[i].Genre;
                worksheet.Cells[i, 3].Value = stories[i].rating;
                worksheet.Cells[i, 4].Value = stories[i].AddedDate;
            }
          



            // Save the workbook
            workbook.SaveAs("C:\\StoryzonData.xlsx");

            // Clean up
            workbook.Close();
            excelApp.Quit();
        }
    }
}
