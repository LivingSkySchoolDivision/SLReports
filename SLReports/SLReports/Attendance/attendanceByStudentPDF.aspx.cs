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

namespace SLReports.Attendance
{
    public partial class attendanceByStudentPDF : System.Web.UI.Page
    {
        protected void sendPDF(System.IO.MemoryStream PDFData, String filename)
        {
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + filename + ".pdf");

            Response.OutputStream.Write(PDFData.GetBuffer(), 0, PDFData.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();

            Response.End();

        }

        protected MemoryStream GeneratePDF(DateTime from, DateTime to, Student student)
        {
            MemoryStream memstream = new MemoryStream();
            Document Report = new Document(PageSize.LETTER);
            Report.SetMargins(36, 36, 36, 60);

            PdfWriter writer = PdfWriter.GetInstance(Report, memstream);
            Report.Open();
            PdfContentByte content = writer.DirectContent;

            /* Header and footer stuff - work to be done on this before it can be implemented */
            PdfPageEventHandler PageEventHandler = new PdfPageEventHandler();
            writer.PageEvent = PageEventHandler;
            
            PageEventHandler.bottomCenter = "Printed " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
            PageEventHandler.bottomLeft = student.getDisplayName();            

            Report.Add(SLReports.Attendance.AttendancePDFParts.livingSkyHeading());
            Report.Add(SLReports.Attendance.AttendancePDFParts.pageTitle(content, from, to));
            Report.Add(SLReports.Attendance.AttendancePDFParts.studentNamePlate(student));
            Report.Add(SLReports.Attendance.AttendancePDFParts.attendanceTable(student));
            Report.Add(SLReports.Attendance.AttendancePDFParts.attendanceSummary(student));            

            Report.Close();
            return memstream;
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Request.QueryString["studentid"]))
            {
                int studentID = 0;
                if (int.TryParse(Request.QueryString["studentid"], out studentID))
                {
                    // Parse the date
                    DateTime Date_From = DateTime.Parse(Request.QueryString["from_date"]);
                    DateTime Date_To = DateTime.Parse(Request.QueryString["to_date"]);

                    // Load student
                    Student selectedStudent = null;
                    using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogic))
                    {
                        selectedStudent = Student.loadThisStudent(connection, studentID.ToString());
                        if (selectedStudent != null)
                        {
                            // Load attendnace for the student
                            selectedStudent.absences = Absence.loadAbsencesForThisStudentAndTimePeriod(connection, selectedStudent, Date_From, Date_To);

                            // Load contacts for the student
                            selectedStudent.contacts = Contact.loadContactsForStudent(connection, selectedStudent);                                                        

                            sendPDF(GeneratePDF(Date_From, Date_To, selectedStudent), "Attendance_" + LSKYCommon.removeSpaces(selectedStudent.getDisplayName()) + "_" + Date_From.Year + "_" + Date_From.Month + "_" + Date_From.Day + "_to_" + Date_To.Year + "_" + Date_To.Month + "_" + Date_To.Day);
                        }
                        else
                        {
                            Response.Write("Invalid student");
                        }
                    }                    
                    
                }
                else
                {
                    Response.Write("Invalid student ID");
                }
            }
            else
            {
                Response.Write("Missing student ID");
            }

        }
    }
}