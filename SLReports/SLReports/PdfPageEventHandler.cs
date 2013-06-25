using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SLReports
{
    public class PdfPageEventHandler : PdfPageEventHelper
    {
        PdfContentByte content;
        PdfTemplate template;
        BaseFont font = null;
        DateTime PrintTime = DateTime.Now;

        public string topLeft { get; set; }
        public string topRight { get; set; }
        public string topCenter { get; set; }
        public string bottomLeft { get; set; }
        public string bottomRight { get; set; }
        public string bottomCenter { get; set; }

        /* If true, this will insert a blank page if the total pages is odd, so that if you are printing double sided, 
         * it won't start a document on the back of another one */
        public bool DoubleSidedMode = true;
        public bool ShowPageNumbers = true;
        public bool ShowOnFirstPage = true;

        public void ResetPageNumbers(iTextSharp.text.Document document)
        {
            /* Check for double sided mode here */
            if ((pageNumber % 2) == 0)
            {
                if (DoubleSidedMode)
                {
                    document.NewPage();
                    document.Add(new Phrase(""));
                }
            }

            pageNumber = 0;

        }

        static int pageNumber = 0;
        
        public override void OnStartPage(PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnStartPage(writer, document);            
        }

        public override void OnEndPage(PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnEndPage(writer, document);

            pageNumber++;

            if (pageNumber == 1)
            {
                if (!ShowOnFirstPage)
                {
                    return;
                }
            }

            float width = 200;
            float height = document.PageSize.Height;

            PdfContentByte overlay = writer.DirectContent;
            overlay.SaveState();
            overlay.BeginText();
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            overlay.SetFontAndSize(bf, 8);
            overlay.SetRGBColorFill(160, 160, 160);

            StringBuilder bottomRightFooterText = new StringBuilder();

            if (!string.IsNullOrEmpty(bottomRight))
            {
                bottomRightFooterText.Append(bottomRight + ", ");
            }

            if (ShowPageNumbers)
            {
                bottomRightFooterText.Append("Page " + pageNumber);
            }

            int Adjustment = 15;


            /* Bottom Right */
            overlay.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, bottomRightFooterText.ToString(), document.PageSize.Width - document.RightMargin, document.BottomMargin - Adjustment, 0);


            /* Bottom Left */
            if (!string.IsNullOrEmpty(bottomLeft))
            {
                overlay.ShowTextAligned(PdfContentByte.ALIGN_LEFT, bottomLeft, document.LeftMargin, document.BottomMargin - Adjustment, 0);
            }

            /* Bottom middle */
            if (!string.IsNullOrEmpty(bottomCenter))
            {
                overlay.ShowTextAligned(PdfContentByte.ALIGN_CENTER, bottomCenter, document.PageSize.Width / 2, document.BottomMargin - Adjustment, 0);
            }


            /* Top left */
            if (!string.IsNullOrEmpty(topLeft))
            {
                overlay.ShowTextAligned(PdfContentByte.ALIGN_LEFT, topLeft, document.LeftMargin, document.PageSize.Height - document.TopMargin + Adjustment, 0);
            }

            /* Top right */
            if (!string.IsNullOrEmpty(topRight))
            {
                overlay.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, topRight, document.PageSize.Width - document.RightMargin, document.PageSize.Height - document.TopMargin + Adjustment, 0);
            }

            /* Top middle */
            if (!string.IsNullOrEmpty(topCenter))
            {
                overlay.ShowTextAligned(PdfContentByte.ALIGN_CENTER, topCenter, document.PageSize.Width / 2, document.PageSize.Height - document.TopMargin + Adjustment, 0);
            }

            overlay.EndText();
            overlay.RestoreState();

        }

        public override void OnCloseDocument(PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnCloseDocument(writer, document);

            /* Check page numbers to see if the number is even - You always want the pages to be even, so if it isn't, add a new blank page */
            if ((pageNumber % 2) == 0)
            {
                if (DoubleSidedMode)
                {
                    document.NewPage();
                    document.Add(new Phrase(""));
                }
            }

            pageNumber = 0;

            


        }

    }
}