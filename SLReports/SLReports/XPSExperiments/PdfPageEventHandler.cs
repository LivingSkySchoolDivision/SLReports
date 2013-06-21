using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SLReports.XPSExperiments
{
    public class PdfPageEventHandler : PdfPageEventHelper
    {
        PdfContentByte content;
        PdfTemplate template;
        BaseFont font = null;
        DateTime PrintTime = DateTime.Now;

        public Student student { get; set; }
        public ReportPeriod reportperiod { get; set; }

        static int pageNumber = 1;
        
        public override void OnStartPage(PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnStartPage(writer, document);
            pageNumber++;
            float width = document.PageSize.Width;
            float height = document.PageSize.Height;            
            
            PdfContentByte overlay = writer.DirectContent;
            overlay.SaveState();
            overlay.BeginText();
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            overlay.SetFontAndSize(bf, 8);


            StringBuilder footerText = new StringBuilder();

            if (student != null)
            {
                footerText.Append(student.getDisplayName() + ", ");
            }
            if (student != null)
            {
                footerText.Append(reportperiod.name + ", ");
            }
            footerText.Append("Page " + pageNumber);
            overlay.SetRGBColorFill(160, 160, 160);

            overlay.ShowTextAligned(PdfContentByte.ALIGN_RIGHT,footerText.ToString(), width - document.LeftMargin, document.BottomMargin, 0);
            overlay.EndText();
            overlay.RestoreState();

            
        }

        public override void OnEndPage(PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnEndPage(writer, document);


            
        }

        public override void OnCloseDocument(PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnCloseDocument(writer, document);

            /* Check page numbers to see if the number is even - You always want the pages to be even, so if it isn't, add a new blank page */
            if ((pageNumber % 2) == 0)
            {
                document.NewPage();
                document.Add(new Phrase("New page!"));
            }

            pageNumber = 1;

            


        }

    }
}