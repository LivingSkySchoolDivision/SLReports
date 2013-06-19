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


        protected iTextSharp.text.Image outcomeBar_Normal(int value) {
            //PdfTemplate template = new PdfTemplate();

            

        }

        protected void coloredOutcomeBar_Normal(PdfContentByte content, float x, float y) {
            int width = 100;
            int height = 20;

            content.SaveState();
            PdfGState state = new PdfGState();

            /* Fill */

            /* Overlay */

            state.FillOpacity = 0.6f;
            content.SetRGBColorFill(120, 0, 240);
            content.SetLineWidth(1);
            content.Rectangle(x, y, width, height);
            content.FillStroke();
            content.RestoreState();            
        }


        protected PdfPTable namePlateTable(Student student)
        {
            int cellpadding = 3;

            if (student == null)
            {
                student = new Student("John", "Smith", "J", "00000", "00000", "School Name", "00000", "Grade 15", "Saskatchewan", "North Battleford", "Fake St", "123", "", "H0H0H0", "3065551234", "Male", "Instatus", "Instatuscode", "Homeroom here", DateTime.Now.AddDays(-1), DateTime.Now, "00000", true);                
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
                    iTextSharp.text.Image photo = iTextSharp.text.Image.GetInstance(@"https://sldata.lskysd.ca/SLReports/photos/GetPhoto.aspx?studentnumber=12511");
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



            return nameplateTable;

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

                //Rectangle r = new Rectangle(0,0,PageSize.LETTER.Width, PageSize.LETTER.Height);
                //r.BackgroundColor = new CMYKColor(25, 90, 25, 0);
                //doc.Add(r);
                //doc.Add(new Paragraph("Derp", font_heading));                
               // doc.Add(new Paragraph("Vivamus ante magna, laoreet id hendrerit tincidunt, tincidunt vitae lacus. Fusce vulputate, libero vel commodo placerat, nisi nunc pretium eros, sit amet viverra purus turpis sit amet enim. Maecenas blandit felis id nulla ultrices, ut dictum mauris sagittis. Pellentesque viverra tristique pellentesque. Maecenas porttitor nisl erat, in tincidunt mauris fringilla a. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Sed iaculis condimentum augue id interdum. Aenean sollicitudin sem nisl, at lacinia libero posuere sit amet. Ut aliquet turpis eu lacus euismod, id feugiat odio dignissim. Duis egestas nisi neque. Suspendisse potenti. Pellentesque fermentum diam posuere mi cursus mollis. Phasellus consectetur enim nec sollicitudin blandit. Mauris imperdiet pellentesque massa, at feugiat ipsum iaculis ut. Vivamus urna ante, consequat sed magna adipiscing, scelerisque rutrum odio. Sed eget pretium urna. ", font_body));
                //doc.Add(new Paragraph("This should be small text", font_small));

                /* Alignment
                 *  0 : Left
                 *  1 : Center
                 *  2 : Right
                 *  */

                doc.Add(namePlateTable(null));

                PdfContentByte content = writer.DirectContent;
                coloredOutcomeBar_Normal(content, 25, writer.PageSize.Height - 50);
                
                doc.Close();
                Response.End();                
            }
        }

    }
}