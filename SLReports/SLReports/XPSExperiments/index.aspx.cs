using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace SLReports.XPSExperiments
{
    public partial class index : System.Web.UI.Page
    {

        Student selectedStudent = null;
        ReportPeriod selectedReportPeriod = null;

        Font font_body = FontFactory.GetFont("Verdana", 12, BaseColor.BLACK);
        Font font_body_bold = FontFactory.GetFont("Verdana", 12, Font.BOLD, BaseColor.BLACK);
        Font font_body_italic = FontFactory.GetFont("Verdana", 12, Font.ITALIC, BaseColor.BLACK);
        Font font_heading = FontFactory.GetFont("Verdana", 14, Font.BOLD, BaseColor.BLACK);
        Font font_small = FontFactory.GetFont("Verdana", 10, BaseColor.BLACK);
        Font font_small_bold = FontFactory.GetFont("Verdana", 10, Font.BOLD, BaseColor.BLACK);

        protected void Page_Init(object sender, EventArgs e)
        {
        }



        protected iTextSharp.text.Image outcomeBar(PdfContentByte content, String value)
        {
            int width = 125;
            int height = 11;
            int maxvalue = 4;
            int rectancleCurveRadius = 4;
                        
            /* Colors */
            BaseColor fillColor = new BaseColor(70, 70, 70);
            BaseColor borderColor = new BaseColor(0, 0, 0);

            Single parsedValue = -1;

            PdfTemplate canvas = content.CreateTemplate(width, height);

            if (Single.TryParse(value, out parsedValue))
            {
                Single percentFill = parsedValue / maxvalue;

                /* Background */
                canvas.Rectangle(0, 0, width, height);
                canvas.SetRGBColorFill(255, 255, 255);
                canvas.Fill();
                
                /* Determine fill color  based on value */
                if (parsedValue <= 1)
                {
                    fillColor = new BaseColor(255, 51, 0);
                }
                else if (parsedValue <= 2.25)
                {
                    fillColor = new BaseColor(255, 165, 0);
                }
                else if (parsedValue <= 3.5)
                {
                    fillColor = new BaseColor(0, 128, 0);
                }
                else if (parsedValue <= 4)
                {
                    fillColor = new BaseColor(0, 128, 0);
                }
                

                /* Fill */
                if ((parsedValue > 0) && (parsedValue <= 4))
                {
                    canvas.RoundRectangle(0, 0, width * percentFill, height, rectancleCurveRadius);
                    if (parsedValue < 4)
                    {
                        canvas.Rectangle(rectancleCurveRadius, 0, (width * percentFill) - rectancleCurveRadius, height);
                    }
                    canvas.SetColorFill(fillColor);
                    canvas.Fill();
                }

                /* Overlay */
                canvas.SetColorStroke(borderColor);
                canvas.SetLineWidth(1);
                canvas.MoveTo((float)(width * 0.25) * 1, (float)0);
                canvas.LineTo((float)(width * 0.25) * 1, (float)height);
                canvas.MoveTo((float)(width * 0.25) * 2, (float)0);
                canvas.LineTo((float)(width * 0.25) * 2, (float)height);
                canvas.MoveTo((float)(width * 0.25) * 3, (float)0);
                canvas.LineTo((float)(width * 0.25) * 3, (float)height);
                canvas.Stroke();

                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                canvas.BeginText();
                canvas.SetFontAndSize(bf, 8);

                canvas.SetColorFill(borderColor);
                
                canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "1", (float)((width * 0.25) / 2), (float)(height / 2) - 2, 0);
                canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "2", (float)(((width * 0.25) / 2) + (width * 0.25)), (float)(height / 2) - 2, 0);
                canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "3", (float)(((width * 0.25) / 2) + (width * 0.50)), (float)(height / 2) - 2, 0);
                canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "4", (float)(((width * 0.25) / 2) + (width * 0.75)), (float)(height / 2) - 2, 0);
                
                canvas.EndText();


            }
            else
            {
                /* IE */
                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                canvas.SetRGBColorFill(0, 0, 0);
                canvas.BeginText();
                canvas.SetFontAndSize(bf, 8);

                if (value.ToLower() == "ie")
                {
                    canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "INSUFFICIENT EVIDENCE", width / 2, (height / 2) - 2, 0);
                }
                else
                {
                    canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, value, width / 2, (height / 2) - 2, 0);
                }
                canvas.EndText();
            }

            /* Border */

            canvas.SetRGBColorStroke(255, 255, 255);
            //canvas.Rectangle(0, 0, width, height);
            //canvas.Stroke();
            canvas.RoundRectangle(0, 0, width, height, rectancleCurveRadius);
            canvas.SetColorStroke(borderColor);
            canvas.Stroke();


            return iTextSharp.text.Image.GetInstance(canvas);;
        }

        protected PdfPTable namePlateTable(Student student)
        {
            int cellpadding = 3;

            if (student == null)
            {
                student = new Student("John", "Smith", "J", "00000", "00000", "School Name", "00000", "Grade 15", "Saskatchewan", "North Battleford", "Fake St", "123", "", "H0H0H0", "3065551234", "Male", "Instatus", "Instatuscode", "Homeroom here", DateTime.Now.AddDays(-1), DateTime.Now, "00000", false);
            }

            PdfPTable nameplateTable = new PdfPTable(3);
            nameplateTable.HorizontalAlignment = 1;
            nameplateTable.TotalWidth = 400f;
            nameplateTable.LockedWidth = true;

            float[] widths = new float[] { 1f, 1.25f, 3f};
            nameplateTable.SetWidths(widths);


            PdfPCell photoCell = new PdfPCell(new Phrase("(No Photo)", font_body_italic));

            

            if (student.hasPhoto())
            {
                try
                {
                    iTextSharp.text.Image photo = iTextSharp.text.Image.GetInstance(@"https://sldata.lskysd.ca/SLReports/photos/GetPhoto.aspx?studentnumber=" + student.getStudentID());
                    photo.Border = Rectangle.BOX;
                    photo.BorderWidth = 1;
                    photoCell.PaddingRight = 10f;

                    photoCell = new PdfPCell(photo);
                }
                catch (Exception ex) {
                    photoCell = new PdfPCell(new Phrase(ex.Message, font_body_italic));
                }; 
            }

            photoCell.Border = 0;
            photoCell.MinimumHeight = 300f;            
            photoCell.Rowspan = 10;
            photoCell.VerticalAlignment = 1;
            photoCell.HorizontalAlignment = 1;
            nameplateTable.AddCell(photoCell);


            PdfPCell StudentNameCell_Header = new PdfPCell(new Phrase("Student", font_small_bold));
            StudentNameCell_Header.VerticalAlignment = 0;
            StudentNameCell_Header.Padding = cellpadding;

            StudentNameCell_Header.Border = 0;
            nameplateTable.AddCell(StudentNameCell_Header);
            PdfPCell StudentNameCell = new PdfPCell(new Phrase(student.getDisplayName(), font_body_bold));
            StudentNameCell.Border = 0;
            StudentNameCell.Padding = cellpadding;
            nameplateTable.AddCell(StudentNameCell);

            PdfPCell StudentNumberCell_Header = new PdfPCell(new Phrase("Student Number", font_small_bold));
            StudentNumberCell_Header.Border = 0;
            StudentNumberCell_Header.Padding = cellpadding;
            nameplateTable.AddCell(StudentNumberCell_Header);
            PdfPCell StudentNumberCell = new PdfPCell(new Phrase(student.getStudentID(), font_small));
            StudentNumberCell.Border = 0;
            StudentNumberCell.Padding = cellpadding;
            nameplateTable.AddCell(StudentNumberCell);

            PdfPCell SchoolNameCell_Header = new PdfPCell(new Phrase("School", font_small_bold));
            SchoolNameCell_Header.Border = 0;
            SchoolNameCell_Header.Padding = cellpadding;
            nameplateTable.AddCell(SchoolNameCell_Header);
            PdfPCell SchoolNameCell = new PdfPCell(new Phrase(student.getSchoolName(), font_small));
            SchoolNameCell.Border = 0;
            SchoolNameCell.Padding = cellpadding;
            nameplateTable.AddCell(SchoolNameCell);

            PdfPCell HomeroomCell_Header = new PdfPCell(new Phrase("Home Room", font_small_bold));
            HomeroomCell_Header.Border = 0;
            HomeroomCell_Header.Padding = cellpadding;
            nameplateTable.AddCell(HomeroomCell_Header);
            PdfPCell HomeroomCell = new PdfPCell(new Phrase(student.getHomeRoom(), font_small));
            HomeroomCell.Border = 0;
            HomeroomCell.Padding = cellpadding;
            nameplateTable.AddCell(HomeroomCell);

            PdfPCell ReportPeriodDateCell_Header = new PdfPCell(new Phrase("Report Period", font_small_bold));
            ReportPeriodDateCell_Header.Border = 0;
            ReportPeriodDateCell_Header.Padding = cellpadding;
            nameplateTable.AddCell(ReportPeriodDateCell_Header);
            PdfPCell ReportPeriodDateCell = new PdfPCell(new Phrase("January 1, 1000 - December 31, 2090", font_small));
            ReportPeriodDateCell.Border = 0;
            ReportPeriodDateCell.Padding = cellpadding;
            nameplateTable.AddCell(ReportPeriodDateCell);

            nameplateTable.SpacingAfter = 25f;

            return nameplateTable;

        }

        protected PdfPTable outcomeLegend(PdfContentByte content)
        {            
            PdfPTable outcomeLegendTable = new PdfPTable(2);            

            outcomeLegendTable.HorizontalAlignment = 1;
            outcomeLegendTable.TotalWidth = 450f;
            outcomeLegendTable.LockedWidth = true;

            float[] widths = new float[] { 1f, 2f };
            outcomeLegendTable.SetWidths(widths);

            PdfPCell newCell = null;
            Paragraph description = null;

            newCell = new PdfPCell(outcomeBar(content, "4"));
            newCell.Border = 0;
            newCell.Padding = 2;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell();
            description = new Paragraph();
            description.SpacingBefore = 0;
            description.SpacingAfter = 6;
            description.Add(new Phrase("4: Master: ", font_small_bold));
            description.Add(new Phrase("Insightful understanding of the outcome", font_small));
            newCell.PaddingTop = 0;
            newCell.AddElement(description);
            newCell.Border = 0;
            newCell.VerticalAlignment = 1;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell(outcomeBar(content, "3"));
            newCell.Border = 0;
            newCell.Padding = 2;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell();
            description = new Paragraph();
            description.SpacingBefore = 0;
            description.SpacingAfter = 6;
            description.Add(new Phrase("3: Proficient: ", font_small_bold));
            description.Add(new Phrase("A well developed understanding of the outcome", font_small));
            newCell.PaddingTop = 0;
            newCell.AddElement(description);
            newCell.Border = 0;
            newCell.VerticalAlignment = 1;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell(outcomeBar(content, "2"));
            newCell.Border = 0;
            newCell.Padding = 2;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell();
            description = new Paragraph();
            description.SpacingBefore = 0;
            description.SpacingAfter = 6;
            description.Add(new Phrase("2: Approaching: ", font_small_bold));
            description.Add(new Phrase("A basic understanding", font_small));
            newCell.PaddingTop = 0;
            newCell.AddElement(description);
            newCell.Border = 0;
            newCell.VerticalAlignment = 1;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell(outcomeBar(content, "1"));
            newCell.Border = 0;
            newCell.Padding = 2;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell();
            description = new Paragraph();
            description.SpacingBefore = 0;
            description.SpacingAfter = 6;
            description.Add(new Phrase("1: Beginning: ", font_small_bold));
            description.Add(new Phrase("A partial understanding", font_small));
            newCell.PaddingTop = 0;
            newCell.AddElement(description);
            newCell.Border = 0;
            newCell.VerticalAlignment = 1;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell(outcomeBar(content, "IE"));
            newCell.Border = 0;
            newCell.Padding = 2;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell();
            description = new Paragraph();
            description.SpacingBefore = 0;
            description.SpacingAfter = 6;
            description.Add(new Phrase("IE:", font_small_bold));
            description.Add(new Phrase("Insufficient Evidence", font_small));
            newCell.PaddingTop = 0;
            newCell.AddElement(description);
            newCell.Border = 0;
            newCell.VerticalAlignment = 1;
            outcomeLegendTable.AddCell(newCell);

            return outcomeLegendTable;
        }

        
        protected void Page_Load(object sender, EventArgs e)
        {
            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;


            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "attachment; filename=test.pdf");
                 
            
            using (Document doc = new Document(PageSize.LETTER)){

                PdfWriter writer = PdfWriter.GetInstance(doc, Response.OutputStream);
                doc.Open();
                PdfContentByte content = writer.DirectContent;

                //Rectangle r = new Rectangle(0,0,PageSize.LETTER.Width, PageSize.LETTER.Height);
                //r.BackgroundColor = new CMYKColor(25, 90, 25, 0);
                //doc.Add(r);
                //doc.Add(new Paragraph("Derp", font_heading));                
                
                //doc.Add(new Paragraph("This should be small text", font_small));

                /* Alignment
                 *  0 : Left
                 *  1 : Center
                 *  2 : Right
                 *  */

                doc.Add(namePlateTable(null));
                doc.Add(outcomeLegend(content));

                doc.Add(new Paragraph("Vivamus ante magna, laoreet id hendrerit tincidunt, tincidunt vitae lacus. Fusce vulputate, libero vel commodo placerat, nisi nunc pretium eros, sit amet viverra purus turpis sit amet enim. Maecenas blandit felis id nulla ultrices, ut dictum mauris sagittis. Pellentesque viverra tristique pellentesque. Maecenas porttitor nisl erat, in tincidunt mauris fringilla a. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Sed iaculis condimentum augue id interdum. Aenean sollicitudin sem nisl, at lacinia libero posuere sit amet. Ut aliquet turpis eu lacus euismod, id feugiat odio dignissim. Duis egestas nisi neque. Suspendisse potenti. Pellentesque fermentum diam posuere mi cursus mollis. Phasellus consectetur enim nec sollicitudin blandit. Mauris imperdiet pellentesque massa, at feugiat ipsum iaculis ut. Vivamus urna ante, consequat sed magna adipiscing, scelerisque rutrum odio. Sed eget pretium urna. ", font_body));
                doc.Add(new Paragraph("Vivamus ante magna, laoreet id hendrerit tincidunt, tincidunt vitae lacus. Fusce vulputate, libero vel commodo placerat, nisi nunc pretium eros, sit amet viverra purus turpis sit amet enim. Maecenas blandit felis id nulla ultrices, ut dictum mauris sagittis. Pellentesque viverra tristique pellentesque. Maecenas porttitor nisl erat, in tincidunt mauris fringilla a. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Sed iaculis condimentum augue id interdum. Aenean sollicitudin sem nisl, at lacinia libero posuere sit amet. Ut aliquet turpis eu lacus euismod, id feugiat odio dignissim. Duis egestas nisi neque. Suspendisse potenti. Pellentesque fermentum diam posuere mi cursus mollis. Phasellus consectetur enim nec sollicitudin blandit. Mauris imperdiet pellentesque massa, at feugiat ipsum iaculis ut. Vivamus urna ante, consequat sed magna adipiscing, scelerisque rutrum odio. Sed eget pretium urna. ", font_body));
                doc.Add(new Paragraph("Vivamus ante magna, laoreet id hendrerit tincidunt, tincidunt vitae lacus. Fusce vulputate, libero vel commodo placerat, nisi nunc pretium eros, sit amet viverra purus turpis sit amet enim. Maecenas blandit felis id nulla ultrices, ut dictum mauris sagittis. Pellentesque viverra tristique pellentesque. Maecenas porttitor nisl erat, in tincidunt mauris fringilla a. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Sed iaculis condimentum augue id interdum. Aenean sollicitudin sem nisl, at lacinia libero posuere sit amet. Ut aliquet turpis eu lacus euismod, id feugiat odio dignissim. Duis egestas nisi neque. Suspendisse potenti. Pellentesque fermentum diam posuere mi cursus mollis. Phasellus consectetur enim nec sollicitudin blandit. Mauris imperdiet pellentesque massa, at feugiat ipsum iaculis ut. Vivamus urna ante, consequat sed magna adipiscing, scelerisque rutrum odio. Sed eget pretium urna. ", font_body));
                doc.Add(new Paragraph("Vivamus ante magna, laoreet id hendrerit tincidunt, tincidunt vitae lacus. Fusce vulputate, libero vel commodo placerat, nisi nunc pretium eros, sit amet viverra purus turpis sit amet enim. Maecenas blandit felis id nulla ultrices, ut dictum mauris sagittis. Pellentesque viverra tristique pellentesque. Maecenas porttitor nisl erat, in tincidunt mauris fringilla a. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Sed iaculis condimentum augue id interdum. Aenean sollicitudin sem nisl, at lacinia libero posuere sit amet. Ut aliquet turpis eu lacus euismod, id feugiat odio dignissim. Duis egestas nisi neque. Suspendisse potenti. Pellentesque fermentum diam posuere mi cursus mollis. Phasellus consectetur enim nec sollicitudin blandit. Mauris imperdiet pellentesque massa, at feugiat ipsum iaculis ut. Vivamus urna ante, consequat sed magna adipiscing, scelerisque rutrum odio. Sed eget pretium urna. ", font_body));
                doc.Add(new Paragraph("Vivamus ante magna, laoreet id hendrerit tincidunt, tincidunt vitae lacus. Fusce vulputate, libero vel commodo placerat, nisi nunc pretium eros, sit amet viverra purus turpis sit amet enim. Maecenas blandit felis id nulla ultrices, ut dictum mauris sagittis. Pellentesque viverra tristique pellentesque. Maecenas porttitor nisl erat, in tincidunt mauris fringilla a. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Sed iaculis condimentum augue id interdum. Aenean sollicitudin sem nisl, at lacinia libero posuere sit amet. Ut aliquet turpis eu lacus euismod, id feugiat odio dignissim. Duis egestas nisi neque. Suspendisse potenti. Pellentesque fermentum diam posuere mi cursus mollis. Phasellus consectetur enim nec sollicitudin blandit. Mauris imperdiet pellentesque massa, at feugiat ipsum iaculis ut. Vivamus urna ante, consequat sed magna adipiscing, scelerisque rutrum odio. Sed eget pretium urna. ", font_body));
                doc.Add(new Paragraph("Vivamus ante magna, laoreet id hendrerit tincidunt, tincidunt vitae lacus. Fusce vulputate, libero vel commodo placerat, nisi nunc pretium eros, sit amet viverra purus turpis sit amet enim. Maecenas blandit felis id nulla ultrices, ut dictum mauris sagittis. Pellentesque viverra tristique pellentesque. Maecenas porttitor nisl erat, in tincidunt mauris fringilla a. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Sed iaculis condimentum augue id interdum. Aenean sollicitudin sem nisl, at lacinia libero posuere sit amet. Ut aliquet turpis eu lacus euismod, id feugiat odio dignissim. Duis egestas nisi neque. Suspendisse potenti. Pellentesque fermentum diam posuere mi cursus mollis. Phasellus consectetur enim nec sollicitudin blandit. Mauris imperdiet pellentesque massa, at feugiat ipsum iaculis ut. Vivamus urna ante, consequat sed magna adipiscing, scelerisque rutrum odio. Sed eget pretium urna. ", font_body));                
                
                doc.Close();
                Response.End();                
            }
        }

    }
}