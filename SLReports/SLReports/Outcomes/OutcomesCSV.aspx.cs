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

namespace SLReports.Outcomes
{
    public partial class OutcomesCSV : System.Web.UI.Page
    {
        protected void sendCSV(MemoryStream CSVData, String filename)
        {
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "text/plain";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + filename + ".csv");

            Response.OutputStream.Write(CSVData.GetBuffer(), 0, (int)CSVData.Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();
            Response.End();
        }

        protected MemoryStream GenerateCSV(List<Objective> outcomes)
        {
            MemoryStream csvFile = new MemoryStream();
            StreamWriter writer = new StreamWriter(csvFile, Encoding.UTF8);

            /* Headings */
            StringBuilder headingLine = new StringBuilder();
            headingLine.Append("ObjectiveID, Subject, Notes, Category, CourseName, CourseCode");
            writer.WriteLine(headingLine.ToString());

            /* Data */
            foreach (Objective outcome in outcomes)
            {
                StringBuilder studentLine = new StringBuilder();
                studentLine.Append(outcome.id);
                studentLine.Append(",");
                studentLine.Append("\"" + outcome.subject + "\"");
                studentLine.Append(",");
                studentLine.Append("\"" + outcome.description + "\"");
                studentLine.Append(",");
                studentLine.Append("\"" + outcome.category + "\"");
                studentLine.Append(",");
                studentLine.Append("\"" + outcome.courseName + "\"");
                studentLine.Append(",");
                studentLine.Append(outcome.courseCode);
                writer.WriteLine(studentLine.ToString());
            }
            writer.Flush();
            csvFile.Flush();
            return csvFile;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            List<Objective> outcomes = new List<Objective>();

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                outcomes = Objective.loadAllObjectives(connection);
            }

            sendCSV(GenerateCSV(outcomes), "LSKY_OUTCOMES_" + LSKYCommon.getCurrentTimeStampForFilename());

            
        }
    }
}