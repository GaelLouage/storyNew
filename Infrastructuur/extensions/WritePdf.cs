using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.IO;
//using iTextSharp.text;
//using iTextSharp.text.pdf;
//using Infrastructuur.Models;
//using Microsoft.AspNetCore.Mvc;

//namespace Infrastructuur.extensions
//{
//    public static class WritePdf
//    {
//        public static void WriteStoryToPdf(this StoryzonEntity story)
//        {

//            // Create a new PDF document
//            Document document = new Document();

//            // Set the file path and name for the PDF file
//            string filePath = @"C:\Temp\MyPdf.pdf";

//            // Create a new PDF file
//            PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));

//            // Open the PDF document for writing
//            document.Open();

//            // Add some text to the document
//            document.Add(new Paragraph("Hello, World!"));

//            // Close the PDF document
//            document.Close();
//        }
//    }
//}