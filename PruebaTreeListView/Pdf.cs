using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using iTextSharp.text;
using iTextSharp.text.pdf;


namespace PruebaTreeListView
{
    public static class Pdf
    {

        public static void PdfFromListView(ObservableCollection<Chequeo> chequeos)
        {
            PdfPTable pdfTable = new PdfPTable(2);
            pdfTable.DefaultCell.Padding = 3;
            pdfTable.WidthPercentage = 30;
            pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;
            pdfTable.DefaultCell.BorderWidth = 1;

            //Adding Header row
            pdfTable.AddCell(new PdfPCell(new Phrase("Chequeo")));
            pdfTable.AddCell(new PdfPCell(new Phrase("Resultado")));
            //pdfTable.AddCell(new PdfPCell(new Phrase("Nota")));
        

            //Adding DataRow
            foreach (Chequeo chequeo in chequeos)
            {
                int i = 0;
                pdfTable.AddCell(chequeo.Nombre);
                pdfTable.AddCell(chequeo.ResultadoTest.ToString());
            }

            //Exporting to PDF
            string folderPath = @"C:/Temp/";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            using (FileStream stream = new FileStream(folderPath + "DataGridViewExport.pdf", FileMode.Create))
            {
                Document pdfDoc = new Document(PageSize.A2, 10f, 10f, 10f, 0f);
                PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                pdfDoc.Add(pdfTable);
                pdfDoc.Close();
                stream.Close();
            }
        }
    }
        
}
