using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.ReportCard
{
    public partial class ReportCard_Demo : System.Web.UI.Page
    {
        string dbConnectionString = LSKYCommon.dbConnectionString_OldSchoolLogic;


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

        

        private void displayError(string error)
        {
            Response.Write(error);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            List<Student> students = new List<Student>();
            List<Student> displayedStudents = new List<Student>();

            List<ReportPeriod> selectedReportPeriods = new List<ReportPeriod>();

            //String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                /* Debugging info */
                //selectedTerm = Term.loadThisTerm(connection, 20);

                /* McKitrick report periods for testing */
                //selectedReportPeriods.Add(ReportPeriod.loadThisReportPeriod(connection, 266));
                //selectedReportPeriods.Add(ReportPeriod.loadThisReportPeriod(connection, 267));
                //selectedReportPeriods.Add(ReportPeriod.loadThisReportPeriod(connection, 268));

                /* McKitrick students for testing */
                //students.Add(Student.loadThisStudent(connection, "80451"));
                //students.Add(Student.loadThisStudent(connection, "80891"));

                /* NBCHS report periods for testing */

                selectedReportPeriods.Add(ReportPeriod.loadThisReportPeriod(connection, 258));
                selectedReportPeriods.Add(ReportPeriod.loadThisReportPeriod(connection, 257));
                //selectedReportPeriods.Add(ReportPeriod.loadThisReportPeriod(connection, 256));
                //selectedReportPeriods.Add(ReportPeriod.loadThisReportPeriod(connection, 255));
                //selectedReportPeriods.Add(ReportPeriod.loadThisReportPeriod(connection, 254));
                //selectedReportPeriods.Add(ReportPeriod.loadThisReportPeriod(connection, 253));


                /* NBCHS students for testing */
                students.Add(Student.loadThisStudent(connection, "12511"));
                //students.Add(Student.loadThisStudent(connection, "11871"));
                students.Add(Student.loadThisStudent(connection, "11804"));
            }
            
            selectedReportPeriods.Sort();

            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                foreach (Student student in students)
                {
                    if (student != null)
                    {
                        Stopwatch studentStopWatch = new Stopwatch();
                        studentStopWatch.Start();
                        displayedStudents.Add(LSKYCommon.loadStudentMarkData(connection, student, selectedReportPeriods));
                        studentStopWatch.Stop();                        
                    }
                }
                students.Clear();
            }
            
            String selectedGrade = "DEMO";
            String fileName = "ReportCards_" + selectedGrade + "_" + DateTime.Today.Year + "_" + DateTime.Today.Month + "_" + DateTime.Today.Day + ".pdf";
            sendPDF(PDFReportCardParts.GeneratePDF(displayedStudents, true), fileName);
            


        }
    }
}