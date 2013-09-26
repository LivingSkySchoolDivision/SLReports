using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.StudentList
{
    public partial class ByGradePDF : System.Web.UI.Page
    {
        private static iTextSharp.text.Image lskyLogo = iTextSharp.text.Image.GetInstance(@"https://sldata.lskysd.ca/SLReports/Logo_Circle_Notext_Trans.png");

        public static Font font_large = FontFactory.GetFont("Verdana", 15, BaseColor.BLACK);
        public static Font font_large_bold = FontFactory.GetFont("Verdana", 15, Font.BOLD, BaseColor.BLACK);
        public static Font font_large_italic = FontFactory.GetFont("Verdana", 15, Font.ITALIC, BaseColor.BLACK);

        public static Font font_body = FontFactory.GetFont("Verdana", 10, BaseColor.BLACK);
        public static Font font_body_bold = FontFactory.GetFont("Verdana", 10, Font.BOLD, BaseColor.BLACK);
        public static Font font_body_italic = FontFactory.GetFont("Verdana", 10, Font.ITALIC, BaseColor.BLACK);

        public static Font font_small = FontFactory.GetFont("Verdana", 8, BaseColor.BLACK);
        public static Font font_small_bold = FontFactory.GetFont("Verdana", 8, Font.BOLD, BaseColor.BLACK);
        public static Font font_small_bold_white = FontFactory.GetFont("Verdana", 8, Font.BOLD, BaseColor.WHITE);
        public static Font font_small_italic = FontFactory.GetFont("Verdana", 8, Font.ITALIC, BaseColor.BLACK);

        private void DisplayError(string error)
        {
            Response.Clear();
            Response.Write("<B>Error: </B>" + error);
            Response.End();
        }

        protected void sendPDF(System.IO.MemoryStream PDFData, string filename)
        {
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + filename + "");

            Response.OutputStream.Write(PDFData.GetBuffer(), 0, PDFData.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();
            Response.End();
        }

        private static PdfPTable header(School school)
        {
            int cellpadding = 3;
            int border = Rectangle.NO_BORDER;

            PdfPTable schoolNamePlateTable = new PdfPTable(2);
            schoolNamePlateTable.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            schoolNamePlateTable.TotalWidth = 500f;
            schoolNamePlateTable.LockedWidth = true;
            schoolNamePlateTable.SpacingAfter = 15;

            float[] widths = new float[] { 25f, 275f };
            schoolNamePlateTable.SetWidths(widths);   

            PdfPCell newCell = null;

            // Logo
            lskyLogo.ScaleAbsolute(28f, 28f);
            newCell = new PdfPCell(lskyLogo);
            newCell.Rowspan = 2;
            newCell.VerticalAlignment = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.Padding = cellpadding;
            newCell.Border = border;
            schoolNamePlateTable.AddCell(newCell);

            //float[] widths = new float[] { 100f, 125f, 225f };
            //schoolNamePlateTable.SetWidths(widths);
            
            newCell = new PdfPCell(new Phrase(school.getName(), font_large_bold));
            newCell.VerticalAlignment = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.Padding = cellpadding;
            newCell.Border = border;
            schoolNamePlateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Students by Grade", font_body));
            newCell.VerticalAlignment = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.Padding = cellpadding;
            newCell.Border = border;
            schoolNamePlateTable.AddCell(newCell);


            return schoolNamePlateTable;
        }

        private static PdfPTable studentList(List<Student> students, string grade)
        {
            int numColumns = 5;
            PdfPTable returnMe = new PdfPTable(numColumns);
            returnMe.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            returnMe.TotalWidth = 500f;
            returnMe.LockedWidth = true;
            returnMe.SpacingAfter = 10;

            // Headings
            PdfPCell cell_grade = new PdfPCell(new Phrase("Grade: " + grade, font_large_bold));
            cell_grade.Border = 0;
            cell_grade.Padding = 3;
            cell_grade.Colspan = numColumns;
            cell_grade.PaddingLeft = 0;
            returnMe.AddCell(cell_grade);
                        
            PdfPCell cell_count = new PdfPCell(new Phrase("Students: " + students.Count, font_small));
            cell_count.Border = 0;
            cell_count.Padding = 3;
            cell_count.PaddingBottom = 10;
            cell_count.Colspan = numColumns;
            returnMe.AddCell(cell_count);


            PdfPCell cell_id = new PdfPCell(new Phrase("Student ID", font_body_bold));
            cell_id.Border = 0;            
            returnMe.AddCell(cell_id);

            PdfPCell cell_firstname = new PdfPCell(new Phrase("First Name", font_body_bold));
            cell_firstname.Border = 0;
            returnMe.AddCell(cell_firstname);

            PdfPCell cell_lastname = new PdfPCell(new Phrase("Last Name", font_body_bold));
            cell_lastname.Border = 0;
            returnMe.AddCell(cell_lastname);


            PdfPCell cell_homeroom = new PdfPCell(new Phrase("Home Room", font_body_bold));
            cell_homeroom.Border = 0;
            returnMe.AddCell(cell_homeroom);
            
            PdfPCell cell_gender = new PdfPCell(new Phrase("Gender", font_body_bold));
            cell_gender.Border = 0;
            cell_gender.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            returnMe.AddCell(cell_gender);

            BaseColor borderColor = BaseColor.LIGHT_GRAY;
            int bottomPadding = 5;

            // Student data
            foreach (Student student in students)
            {
                PdfPCell cell_id_value = new PdfPCell(new Phrase(student.getStudentID(), font_body));
                cell_id_value.Border = Rectangle.TOP_BORDER;
                cell_id_value.BorderColor = borderColor;
                cell_id_value.PaddingBottom = bottomPadding;
                returnMe.AddCell(cell_id_value);

                string firstNameString = student.getFirstName();
                if (student.getFirstName() != student.getLegalFirstName())
                {
                    firstNameString += " (" + student.getLegalFirstName() + ")";
                }

                string lastNameString = student.getLastName();
                if (student.getLastName() != student.getLegalLastName())
                {
                    lastNameString += " (" + student.getLegalLastName() + ")";
                }


                PdfPCell cell_firstname_value = new PdfPCell(new Phrase(firstNameString, font_body));
                cell_firstname_value.Border = Rectangle.TOP_BORDER;
                cell_firstname_value.BorderColor = borderColor;
                cell_firstname_value.PaddingBottom = bottomPadding;
                returnMe.AddCell(cell_firstname_value);

                PdfPCell cell_lastname_value = new PdfPCell(new Phrase(lastNameString, font_body));
                cell_lastname_value.Border = Rectangle.TOP_BORDER;
                cell_lastname_value.BorderColor = borderColor;
                cell_lastname_value.PaddingBottom = bottomPadding;
                returnMe.AddCell(cell_lastname_value);

                PdfPCell cell_homeroom_value = new PdfPCell(new Phrase(student.getHomeRoom(), font_body));
                cell_homeroom_value.Border = Rectangle.TOP_BORDER;
                cell_homeroom_value.BorderColor = borderColor;
                cell_homeroom_value.PaddingBottom = bottomPadding;
                returnMe.AddCell(cell_homeroom_value);

                PdfPCell cell_gender_value = new PdfPCell(new Phrase(student.getGenderInitial(), font_body));
                cell_gender_value.Border = Rectangle.TOP_BORDER;
                cell_gender_value.BorderColor = borderColor;
                cell_gender_value.PaddingBottom = bottomPadding;
                cell_gender_value.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                returnMe.AddCell(cell_gender_value);
                
            }

            return returnMe;
        }

        public static MemoryStream GeneratePDF(School school, List<Student> students, bool doubleSidedMode = true)
        {
            MemoryStream memstream = new MemoryStream();
            Document PDFDocument = new Document(PageSize.LETTER);
            PdfWriter writer = PdfWriter.GetInstance(PDFDocument, memstream);

            PDFDocument.Open();
            PdfContentByte content = writer.DirectContent;

            PdfPageEventHandler PageEventHandler = new PdfPageEventHandler();
            writer.PageEvent = PageEventHandler;
            PageEventHandler.DoubleSidedMode = doubleSidedMode;
            PageEventHandler.ShowOnFirstPage = true;
            PageEventHandler.bottomCenter = "Printed " + DateTime.Now.ToLongDateString();
            PageEventHandler.bottomLeft = "Student list by grade";
            
            // Organize students into grades
            SortedDictionary<string, List<Student>> studentsByGrade = new SortedDictionary<string, List<Student>>();
            foreach (Student student in students)
            {
                if (!studentsByGrade.ContainsKey(student.getGradeFormatted()))
                {
                    studentsByGrade.Add(student.getGradeFormatted(), new List<Student>());
                }
                studentsByGrade[student.getGradeFormatted()].Add(student);                
            }

            foreach (KeyValuePair<string, List<Student>> grade in studentsByGrade)
            {
                // Show a page header
                PDFDocument.Add(header(school));
                
                // Show a list
                PDFDocument.Add(studentList(grade.Value, grade.Key));

                // Page break after each grade
                PDFDocument.NewPage();
            }

            PDFDocument.Close();
            return memstream;

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int SelectedSchoolID = -1;
            if (int.TryParse(Request.QueryString["schoolid"], out SelectedSchoolID))
            {
                using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogic))
                {
                    School selectedSchool = School.loadThisSchool(connection, SelectedSchoolID);

                    if (selectedSchool != null)
                    {
                        List<Student> schoolStudents = Student.loadStudentsFromThisSchool(connection, selectedSchool.getGovID());                        
                        schoolStudents.Sort(
                            delegate(Student first,
                            Student next)
                            {
                                return first.getLastName().CompareTo(next.getLastName());
                            }
                            );
                        sendPDF(GeneratePDF(selectedSchool, schoolStudents, false), LSKYCommon.removeSpaces(selectedSchool.getName()) + "_StudentsByGrade");
                    }
                    else
                    {
                        DisplayError("School not found");
                    }
                }
            }
            else {
                DisplayError("Invalid school");            
            }

                        


            

        }
    }
}