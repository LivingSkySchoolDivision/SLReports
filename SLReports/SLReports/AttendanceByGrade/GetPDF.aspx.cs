﻿using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.AttendanceByGrade
{
    public partial class GetPDF : System.Web.UI.Page
    {
        Font font_large = FontFactory.GetFont("Verdana", 15, BaseColor.BLACK);
        Font font_large_bold = FontFactory.GetFont("Verdana", 15, Font.BOLD, BaseColor.BLACK);
        Font font_large_italic = FontFactory.GetFont("Verdana", 15, Font.ITALIC, BaseColor.BLACK);

        Font font_body = FontFactory.GetFont("Verdana", 8, BaseColor.BLACK);
        Font font_body_bold = FontFactory.GetFont("Verdana", 8, Font.BOLD, BaseColor.BLACK);
        Font font_body_italic = FontFactory.GetFont("Verdana", 8, Font.ITALIC, BaseColor.BLACK);

        Font font_small = FontFactory.GetFont("Verdana", 8, BaseColor.BLACK);
        Font font_small_bold = FontFactory.GetFont("Verdana", 8, Font.BOLD, BaseColor.BLACK);
        Font font_small_italic = FontFactory.GetFont("Verdana", 8, Font.ITALIC, BaseColor.BLACK);

        protected void Page_Load(object sender, EventArgs e)
        {
            List<Student> SelectedStudents = new List<Student>();
            School SelectedSchool = null;
            String SelectedGrade = String.Empty;
            DateTime Date_From = DateTime.Now;
            DateTime Date_To = DateTime.Now;

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;


            /* Translate school */
            if (!String.IsNullOrEmpty(Request.QueryString["schoolid"]))
            {
                int SelectedSchoolID = -1;
                if (int.TryParse(Request.QueryString["schoolid"], out SelectedSchoolID))
                {
                    using (SqlConnection connection = new SqlConnection(dbConnectionString))
                    {
                        SelectedSchool = School.loadThisSchool(connection, SelectedSchoolID);
                    }
                    if (SelectedSchool != null)
                    {

                        /* Translate grade */
                        if (!String.IsNullOrEmpty(Request.QueryString["grade"]))
                        {
                            SelectedGrade = Request.QueryString["grade"];
                            /* Translate date */

                            if ((!String.IsNullOrEmpty(Request.QueryString["to_date"])) && (!String.IsNullOrEmpty(Request.QueryString["from_date"])))
                            {

                                Date_From = DateTime.Parse(Request.QueryString["from_date"]);
                                Date_To = DateTime.Parse(Request.QueryString["to_date"]);


                                List<Student> AllStudentsAtThisSchool = new List<Student>();

                                using (SqlConnection connection = new SqlConnection(dbConnectionString))
                                {
                                    /* Get list of all students at school */
                                    AllStudentsAtThisSchool = Student.loadStudentsFromThisSchool(connection, int.Parse(SelectedSchool.getGovID()));
                                }

                                /* Filter out only the specified grade */
                                SelectedStudents.Clear();
                                foreach (Student student in AllStudentsAtThisSchool)
                                {
                                    if (student.getGrade() == SelectedGrade)
                                    {
                                        SelectedStudents.Add(student);
                                    }
                                }

                                /* Get attendance for all students */
                                foreach (Student student in SelectedStudents)
                                {
                                    List<Absence> studentAbsences = new List<Absence>();
                                    studentAbsences.Clear();
                                    using (SqlConnection connection = new SqlConnection(dbConnectionString))
                                    {
                                        studentAbsences = Absence.loadAbsencesForThisStudent(connection, student);
                                    }

                                    foreach (Absence abs in studentAbsences)
                                    {
                                        if ((abs.getDate() < Date_To) && (abs.getDate() > Date_From))
                                        {
                                            student.addAbsence(abs);
                                        }
                                    }

                                    
                                    using (SqlConnection connection = new SqlConnection(dbConnectionString))
                                    {
                                        student.contacts = Contact.loadContactsForStudent(connection, student);
                                    }

                                }
                                sendPDF(GeneratePDF(Date_From, Date_To, SelectedStudents), "Attendance_" + (SelectedSchool.getName()).ToLower().Substring(0, 4) + "_Gr" + SelectedGrade + "_" + Date_From.Year + "_" + Date_From.Month + "_" + Date_From.Day + "_to_" + Date_To.Year + "_" + Date_To.Month + "_" + Date_From.Day);
                            }
                            else
                            {
                                Response.Write("A from and to date is required");
                            }
                        }
                        else
                        {
                            Response.Write("You must specify a grade");
                        }
                    }
                    else
                    {
                        Response.Write("Invalid school specified");
                    }
                }
                else
                {
                    Response.Write("Invalid school ID");
                }
            }
            else
            {
                Response.Write("You must specify a school ID");
            }
        }
        
        protected void sendPDF(System.IO.MemoryStream PDFData, String filename)
        {
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "attachment; filename="+filename+".pdf");

            Response.OutputStream.Write(PDFData.GetBuffer(), 0, PDFData.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();

            Response.End();

        }

        protected PdfPTable pageTitle(PdfContentByte content, DateTime from, DateTime to)
        {
            PdfPTable titleTable = new PdfPTable(1);
            titleTable.SpacingAfter = 10f;
            titleTable.HorizontalAlignment = 1;
            titleTable.TotalWidth = 450f;
            titleTable.LockedWidth = true;


            PdfPCell newCell = null;
            newCell = new PdfPCell(new Phrase("Detailed Student Attendance", font_large_bold));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            titleTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase(from.ToLongDateString() + " to " + to.ToLongDateString(), font_body));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            titleTable.AddCell(newCell);

            return titleTable;
        }

        protected PdfPTable studentNamePlate(Student student)
        {

            PdfPTable nameplateTable = new PdfPTable(6);
            nameplateTable.SpacingAfter = 10f;
            nameplateTable.HorizontalAlignment = 1;
            nameplateTable.TotalWidth = 500f;
            nameplateTable.LockedWidth = true;

            float[] widths = new float[] { 125, 175, 125, 175, 125, 175 };
            nameplateTable.SetWidths(widths);

            PdfPCell newCell = null;

            newCell = new PdfPCell(new Phrase("Student", font_body_bold));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase(student.getDisplayName(), font_body));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("School", font_body_bold));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase(student.getSchoolName(), font_body));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Student Number", font_body_bold));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase(student.getStudentID(), font_body));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Government ID", font_body_bold));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase(student.getGovernmentID(), font_body));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Grade", font_body_bold));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase(student.getGrade(), font_body));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Homeroom", font_body_bold));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase(student.getHomeRoom(), font_body));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Contact", font_body_bold));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);

            StringBuilder contactList = new StringBuilder();
            foreach (Contact contact in student.getContacts())
            {
                contactList.Append(contact.firstName + " " + contact.lastName + " " + contact.telephone + " (" + contact.relation + ")\n");
            }

            newCell = new PdfPCell(new Phrase(contactList.ToString(), font_body));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.Colspan = 5;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);           
            

            return nameplateTable;

        }

        protected PdfPTable attendanceSummary(Student student)
        {
            PdfPTable summaryTable = new PdfPTable(2);
            summaryTable.SpacingAfter = 25f;
            summaryTable.HorizontalAlignment = 1;
            summaryTable.TotalWidth = 500f;
            summaryTable.LockedWidth = true;

            PdfPCell newCell = null;

            /* This can be done 2 ways:
             *  - A dynamic table, similar to the original attendance report
             *  - A table of simple absences vs lates
             *  
             * 
             *  Bonus: Figure out the total classroom minutes the student SHOULD have and calculate percentages of time missed / late
             *  Bonus: total up late time minutes
             
             */


            /* Figure out some statistics to display */

            /* Headings */
            newCell = new PdfPCell(new Phrase("Class / Period", font_body_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.BOTTOM_BORDER;
            newCell.PaddingBottom = 5;
            newCell.BorderWidth = 2;
            summaryTable.AddCell(newCell);

            
            /* Values */




            return summaryTable;
        }

        protected PdfPTable attendanceTable(Student student)
        {
            if (student.absences.Count > 0)
            {

                PdfPTable attendanceTable = new PdfPTable(5);
                attendanceTable.SpacingAfter = 25f;
                attendanceTable.HorizontalAlignment = 1;
                attendanceTable.TotalWidth = 500f;
                attendanceTable.LockedWidth = true;

                float[] widths = new float[] { 50, 100, 100, 100, 150 };
                attendanceTable.SetWidths(widths);

                /* 
                 Date
                 Course name
                 Status
                 Reason
                 Comment
                 Minutes
                 Block  
                 * 
                 */

                PdfPCell newCell = null;

                // copy and paste these
                //newCell.Padding = 2;
                //newCell.Border = 0;

                /* Headings */
                newCell = new PdfPCell(new Phrase("Date", font_body_bold));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.BOTTOM_BORDER;
                newCell.PaddingBottom = 5;
                newCell.BorderWidth = 2;
                attendanceTable.AddCell(newCell);

                newCell = new PdfPCell(new Phrase("Period", font_body_bold));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.BOTTOM_BORDER;
                newCell.PaddingBottom = 5;
                newCell.BorderWidth = 2;
                attendanceTable.AddCell(newCell);

                newCell = new PdfPCell(new Phrase("Status", font_body_bold));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.BOTTOM_BORDER;
                newCell.PaddingBottom = 5;
                newCell.BorderWidth = 2;
                attendanceTable.AddCell(newCell);

                newCell = new PdfPCell(new Phrase("Reason", font_body_bold));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.BOTTOM_BORDER;
                newCell.PaddingBottom = 5;
                newCell.BorderWidth = 2;
                attendanceTable.AddCell(newCell);

                newCell = new PdfPCell(new Phrase("Comment", font_body_bold));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.BOTTOM_BORDER;
                newCell.PaddingBottom = 5;
                newCell.BorderWidth = 2;
                attendanceTable.AddCell(newCell);


                DateTime lastDay = DateTime.Now;
                BaseColor[] backgroundColors = { new BaseColor(255, 255, 255), new BaseColor(235, 235, 235) };
                int colorindex = 0;

                foreach (Absence abs in student.absences)
                {                    
                    newCell = new PdfPCell(new Phrase(abs.getDate().ToShortDateString(), font_body));
                    newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    newCell.Border = Rectangle.BOTTOM_BORDER;
                    
                    if ((abs.getDate().Day == lastDay.Day) && (abs.getDate().Month == lastDay.Month) && (abs.getDate().Year == lastDay.Year))
                    {
                        newCell.AddElement(new Phrase(""));
                    }
                    else
                    {
                        colorindex++;
                        if (colorindex >= backgroundColors.Length)
                        {
                            colorindex = 0;
                        }
                    }

                    lastDay = abs.getDate();

                    newCell.BackgroundColor = backgroundColors[colorindex];

                    attendanceTable.AddCell(newCell);


                    /* Block */
                    /* Check if course has a name */

                    StringBuilder blockName = new StringBuilder();

                    blockName.Append(abs.getPeriod());
                    
                    if (!string.IsNullOrEmpty(abs.getCourseName()))
                    {
                        blockName.Append(" (" + abs.getCourseName() + ")");
                    }

                    newCell = new PdfPCell(new Phrase(blockName.ToString(), font_body));
                    newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    newCell.Border = Rectangle.BOTTOM_BORDER;
                    newCell.BackgroundColor = backgroundColors[colorindex];
                    attendanceTable.AddCell(newCell);


                    StringBuilder status = new StringBuilder();
                    status.Append(abs.getStatus());
                    if (abs.getMinutes() > 0)
                    {
                        status.Append(" (" + abs.getMinutes() + " min)");
                    }

                    newCell = new PdfPCell(new Phrase(status.ToString(), font_body));
                    newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    newCell.Border = Rectangle.BOTTOM_BORDER;
                    newCell.BackgroundColor = backgroundColors[colorindex];
                    attendanceTable.AddCell(newCell);

                    newCell = new PdfPCell(new Phrase(abs.getReason(), font_body));
                    newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    newCell.Border = Rectangle.BOTTOM_BORDER;
                    newCell.BackgroundColor = backgroundColors[colorindex];
                    attendanceTable.AddCell(newCell);

                    newCell = new PdfPCell(new Phrase(abs.getComment(), font_body));
                    newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    newCell.Border = Rectangle.BOTTOM_BORDER;
                    newCell.BackgroundColor = backgroundColors[colorindex];
                    attendanceTable.AddCell(newCell);
                }



                return attendanceTable;
            }
            else
            {
                /* No absences */
                PdfPTable attendanceTable = new PdfPTable(1);
                attendanceTable.SpacingAfter = 25f;
                attendanceTable.HorizontalAlignment = 1;
                attendanceTable.TotalWidth = 500f;
                attendanceTable.LockedWidth = true;


                PdfPCell newCell = null;

                newCell = new PdfPCell(new Phrase("- No absences within the specified time period - ", font_body));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.NO_BORDER;
                attendanceTable.AddCell(newCell);

                return attendanceTable;
            }

        }


        protected MemoryStream GeneratePDF(DateTime from, DateTime to, List<Student> students)
        {
            MemoryStream memstream = new MemoryStream();
            Document Report = new Document(PageSize.LETTER);
            PdfWriter writer = PdfWriter.GetInstance(Report, memstream);
            Report.Open();
            PdfContentByte content = writer.DirectContent;
            /* Header and footer stuff - work to be done on this before it can be implemented
            PdfPageEventHandler PageEventHandler = new PdfPageEventHandler();
            writer.PageEvent = PageEventHandler;
            PageEventHandler.student = student;
            PageEventHandler.reportperiod = period;*/

            foreach (Student student in students)
            {
                Report.Add(pageTitle(content, from, to));
                Report.Add(studentNamePlate(student));
                Report.Add(attendanceTable(student));
                Report.Add(attendanceSummary(student));
                Report.NewPage();
                Report.Add(new Phrase(String.Empty));
            }

            

            Report.Close();
            return memstream;
        }

    }
}
