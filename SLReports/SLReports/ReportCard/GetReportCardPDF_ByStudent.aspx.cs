using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    public partial class GetReportCardPDF_ByStudent : System.Web.UI.Page
    {
        string dbConnectionString = PDFReportCardParts.ReportCardDatabase;    

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

            bool anonymize = false;
            if (!string.IsNullOrEmpty(Request.QueryString["anon"]))
            {
                anonymize = true;
            }
            

            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                // Parse student IDs
                if (!string.IsNullOrEmpty(Request.QueryString["students"]))
                {

                    foreach (string student in Request.QueryString["students"].Split(';'))
                    {
                        if (!string.IsNullOrEmpty(student))
                        {
                            int student_id = -1;
                            if (int.TryParse(student, out student_id))
                            {
                                students.Add(Student.loadThisStudent(connection, student_id.ToString()));
                            }
                        }
                    }

                    foreach (string rp in Request.QueryString["reportperiods"].Split(';'))
                    {
                        if (!string.IsNullOrEmpty(rp))
                        {
                            int rp_id = -1;
                            if (int.TryParse(rp, out rp_id))
                            {
                                selectedReportPeriods.Add(ReportPeriod.loadThisReportPeriod(connection, rp_id));
                            }                            
                        }
                    }
                }
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

            String fileName = "ReportCards_" + DateTime.Today.Year + "_" + DateTime.Today.Month + "_" + DateTime.Today.Day + ".pdf";

            if ((selectedReportPeriods.Count > 0) && (displayedStudents.Count > 0))
            {
                sendPDF(PDFReportCardParts.GeneratePDF(displayedStudents, anonymize), fileName);
            }
        

        }
    }
}