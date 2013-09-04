using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.JSON
{
    public partial class getEnrollmentStats : System.Web.UI.Page
    {
        String dbConnectionString = LSKYCommon.dbConnectionString_SchoolLogic;
        String dbConnectionString_Local = LSKYCommon.dbConnectionString_DataExplorer;

        protected void Page_Load(object sender, EventArgs e)
        {
            List<Student> allStudents = new List<Student>();

            using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogic))
            {
                allStudents = Student.loadAllStudents(connection);
            }

            


            // Output JSON file

            Response.Clear();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write("{\n\"Students\": {");

            Response.Write("\"TotalEnrolled\": \"" + allStudents.Count + "\"\n");

            Response.Write("}\n");
            Response.Write("}\n");
            Response.End();


        }
    }
}