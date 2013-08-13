using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.INAC
{
    public partial class INAC_CSV : System.Web.UI.Page
    {
        String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
        

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




        protected MemoryStream GenerateCSV(List<Student> students)
        {
            /* Figure out days absent */

            MemoryStream csvFile = new MemoryStream();
            StreamWriter writer = new StreamWriter(csvFile, Encoding.UTF8);

            /* Headings */
            StringBuilder headingLine = new StringBuilder();
            headingLine.Append("Grade, StudentName, DateOfBirth, BandAffiliation, StatusNo, ReserveOfResidence, HouseNo, ParentOrGuardian, DaysAbsent, BlocksAbsent, InStatusDate");
            writer.WriteLine(headingLine.ToString());

            /* Data */
            foreach (Student student in students)
            {
                StringBuilder studentLine = new StringBuilder();
                
                
                studentLine.Append(",");
                
                writer.WriteLine(studentLine.ToString());
            }
            writer.Flush();
            csvFile.Flush();
            return csvFile;
        }


        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}