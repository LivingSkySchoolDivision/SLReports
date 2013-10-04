using iTextSharp.text;
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
    public partial class attendanceByGradePDF : System.Web.UI.Page
    {      
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
                                    AllStudentsAtThisSchool = Student.loadStudentsFromThisSchool(connection, int.Parse(SelectedSchool.getGovIDAsString()));
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
                                        if ((abs.getDate() <= Date_To) && (abs.getDate() >= Date_From))
                                        {
                                            student.addAbsence(abs);
                                        }
                                    }

                                    
                                    using (SqlConnection connection = new SqlConnection(dbConnectionString))
                                    {
                                        student.contacts = Contact.loadContactsForStudent(connection, student);
                                    }

                                }
                                sendPDF(GeneratePDF(Date_From, Date_To, SelectedStudents), "Attendance_" + (SelectedSchool.getName()).ToLower().Substring(0, 4) + "_Gr" + SelectedGrade + "_" + Date_From.Year + "_" + Date_From.Month + "_" + Date_From.Day + "_to_" + Date_To.Year + "_" + Date_To.Month + "_" + Date_To.Day);
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



        protected MemoryStream GeneratePDF(DateTime from, DateTime to, List<Student> students)
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

            PageEventHandler.DoubleSidedMode = false; // Doesn't work so well with large multi-student documents...

            foreach (Student student in students)
            {
                PageEventHandler.bottomCenter = "Printed " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
                PageEventHandler.bottomLeft = student.getDisplayName();
                PageEventHandler.ResetPageNumbers(Report);

                Report.Add(SLReports.Attendance.AttendancePDFParts.livingSkyHeading());
                Report.Add(SLReports.Attendance.AttendancePDFParts.pageTitle(content, from, to));
                Report.Add(SLReports.Attendance.AttendancePDFParts.studentNamePlate(student));
                Report.Add(SLReports.Attendance.AttendancePDFParts.attendanceTable(student));
                Report.Add(SLReports.Attendance.AttendancePDFParts.attendanceSummary(student));
                Report.NewPage();
                Report.Add(new Phrase(String.Empty));
            }

            Report.Close();
            return memstream;
        }

    }
}
