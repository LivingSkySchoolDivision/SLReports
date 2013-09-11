using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports
{
    public partial class Testing : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Stopwatch linkInCode = new Stopwatch();
            linkInCode.Start();
            List<Student> allStudents;
            List<StudentStatus> allStatuses;
            using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogic))
            {
                allStudents = Student.loadAllStudents(connection);
                allStatuses = StudentStatus.loadAllStudentStatuses(connection);
            }

            foreach (StudentStatus status in allStatuses)
            {
                if (string.IsNullOrEmpty(status.outStatus))
                {
                    foreach (Student student in allStudents)
                    {
                        if (status.studentNumber == student.getStudentID())
                        {
                            student.statuses.Add(status);
                        }
                    }
                }
            }
            linkInCode.Stop();
            Response.Write("Loaded data and linked in code in: " + linkInCode.Elapsed);
            Response.Write("<BR>");

            foreach (Student student in allStudents)
            {
                Response.Write("<BR><b>" + student + "</b>");
                foreach(StudentStatus status in student.statuses)
                {
                    Response.Write("<BR>&nbsp;&nbsp;&nbsp;" + status);
                }
            }



        }
    }
}