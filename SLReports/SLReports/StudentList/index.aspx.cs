using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.StudentList
{
    public partial class index : System.Web.UI.Page
    {

        private static List<Student> AllStudents;
        public static List<Student> DisplayedStudents;
        public static List<School> Schools;
        public static string filterSchoolID = null;

        public void buildSchoolDropdown(List<School> schools)
        {
            Response.Write("<FORM NAME=\"SchoolPicker\" METHOD=\"POST\">");
            Response.Write("<SELECT NAME=\"schoolid\">");

            if (schools != null)
            {
                if (schools.Count > 0)
                {
                    //Response.Write("<OPTION VALUE=\"\">All Schools</OPTION>");
                    foreach (School school in schools)
                    {
                        Response.Write("<OPTION VALUE=\"" + school.getGovID() + "\"");
                        if (filterSchoolID == school.getGovID())
                        {
                            Response.Write(" SELECTED=\"SELECTED\"");
                        }
                        Response.Write(">" + school.getName() + "</OPTION>");
                    }
                }
            }

            Response.Write("</SELECT>");
            Response.Write("<INPUT TYPE=\"SUBMIT\" VALUE=\">>\">");
            Response.Write("</FORM>\n");

        }

        public void buildStudentTable_Row(Student student)
        {
            Response.Write("<tr class=\"row\">");
            Response.Write("<td>" + student.getStudentID() + "</td>");
            Response.Write("<td>" + student.getGivenName() + "</td>");
            Response.Write("<td>" + student.getSN() + "</td>");
            Response.Write("<td>" + student.getGovernmentID() + "</td>");
            Response.Write("<td>" + student.getGrade() + "</td>");
            Response.Write("<td>" + student.getGender() + "</td>");
            Response.Write("<td>" + student.getDateOfBirth().ToShortDateString() + "</td>");
            Response.Write("<td>" + student.getHomeRoom()+ "</td>");
            Response.Write("<td>" + student.getInStatusWithCode() + "</td>");
            Response.Write("<td>" + student.getEnrollDate().ToShortDateString() + "</td>");            
            Response.Write("<td><a href=\"/SLReports/Attendance/?studentid=" + student.getStudentID() + "\">Attendance</a></td>");
            Response.Write("</tr>\n");
        }

        public void buildStudentTable(List<Student> students)
        {
            if (students.Count > 0)
            {
                int displayedStudentCount = 0;
                Table table = new Table();
                /* Create the table header */

                Response.Write("<table border=0 class=\"datatable\" cellpadding=3>");
                Response.Write("<tr class=\"datatable_header\">");
                Response.Write("<th width=\"150\"><b>Student ID</b></th>");
                Response.Write("<th width=\"150\"><b>Given Name</b></th>");
                Response.Write("<th width=\"150\"><b>Surname</b></th>");
                Response.Write("<th width=\"150\"><b>Government ID</b></th>");
                Response.Write("<th width=\"100\"><b>Grade</b></th>");
                Response.Write("<th width=\"100\"><b>Gender</b></th>");
                Response.Write("<th width=\"150\"><b>Date of Birth</b></th>");
                Response.Write("<th width=\"300\"><b>Home Room</b></th>");
                Response.Write("<th width=\"200\"><b>InStatus</b></th>");
                Response.Write("<th width=\"100\"><b>InDate</b></th>");
                Response.Write("<th width=\"200\"><b>Reports</b></th>");
                Response.Write("</tr>\n");

                displayedStudentCount = 0;
                if (students != null)
                {
                    if (students.Count > 0)
                    {
                        foreach (Student student in students)
                        {
                            buildStudentTable_Row(student);
                            displayedStudentCount++;
                        }
                    }
                }
                Response.Write("</table>");
            }
        }

        public void buildStatisticsTable(List<Student> students)
        {
            if (students.Count > 0)
            {
                /* Gather statistics */
                int totalStudents = 0;
                int totalMales = 0;
                int totalFemales = 0;
                List<String> allGrades = new List<String>();
                SortedDictionary<string, int> grades = new SortedDictionary<string, int>();

                /* Get list of possible grades */
                foreach (Student student in students)
                {
                    /* Compile a list of all possible grades */
                    if (!allGrades.Contains(student.getGrade()))
                    {
                        allGrades.Add(student.getGrade());
                    }

                    /* Get some basic headcounts */
                    totalStudents++;
                    if (student.getGender().ToLower().Equals("male"))
                    {
                        totalMales++;
                    }
                    else
                    {
                        totalFemales++;
                    }
                }

                allGrades.Sort();
                int thisGradeCount;
                grades.Clear();

                foreach (String grade in allGrades)
                {
                    thisGradeCount = 0;
                    foreach (Student student in students)
                    {
                        if (student.getGrade().Equals(grade))
                        {
                            thisGradeCount++;
                        }
                    }
                    grades.Add(grade, thisGradeCount);
                }

                /* Display in a table */
                Response.Write("<table border=0 cellspacing=0 cellpadding=0>");
                Response.Write("<tr><td width=\"150\"><b>Total Students</b></td><td>" + totalStudents + " (" + totalMales + " male, " + totalFemales + " female)</td></tr>");
                Response.Write("</table>");

                Response.Write("<br><table border=0 cellspacing=0 cellpadding=3>");
                Response.Write("<tr>");
                Response.Write("<td width=\"75\"><b>Grade</b></td>");
                foreach (var grade in grades)
                {
                    Response.Write("<td align=\"center\" style=\"border: 1px solid black;background-color: #555555; color: white;\" width=\"25\"><b>" + grade.Key + "</b></td>");
                }
                Response.Write("</tr><tr>");
                Response.Write("<td><b>Count</b></td>");
                foreach (var grade in grades)
                {
                    Response.Write("<td align=\"center\" style=\"border: 1px solid black;\">" + grade.Value + " </td>");
                }

                Response.Write("</tr></table>");


            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AllStudents = new List<Student>();
            DisplayedStudents = new List<Student>();
            Schools = new List<School>();

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;

            //DisplayedStudents = AllStudents;

            NameValueCollection post = Request.Form;
            if (!string.IsNullOrEmpty(post["schoolid"]))
            {
                filterSchoolID = post["schoolid"];
            }
            else
            {
                filterSchoolID = null;
            }

            /* Load Schools */
            
            #region Load all schools
            try
            {
                SqlConnection dbConnection = new SqlConnection(dbConnectionString);
                SqlCommand sqlCommand = new SqlCommand();

                sqlCommand.Connection = dbConnection;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = "SELECT * FROM LSKY_LSKYSchools;";
                sqlCommand.Connection.Open();

                SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                if (dbDataReader.HasRows)
                {
                    Schools.Clear();
                    while (dbDataReader.Read())
                    {
                        //dbDataReader["LegalFirstName"].ToString() + " " + dbDataReader["LegalLastName"].ToString()
                        Schools.Add(new School(dbDataReader["name"].ToString(), dbDataReader["internalID"].ToString(), dbDataReader["govID"].ToString()));
                    }
                }

                sqlCommand.Connection.Close();
            }
            catch (Exception ex)
            {
                Response.Write("Exception: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Response.Write("Exception: " + ex.InnerException.Message);
                }
            }
           
            #endregion

            

            /* Load students */
            if (filterSchoolID != null)
            {
                #region Load all students
                try
                {

                    using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
                    {
                        AllStudents = Student.loadStudentsFromThisSchool(dbConnection, int.Parse(filterSchoolID));
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("Exception: " + ex.Message);
                    if (ex.InnerException != null)
                    {
                        Response.Write("Exception: " + ex.InnerException.Message);
                    }
                }
                #endregion

            }
            //AllStudents.Sort();
            if (!string.IsNullOrEmpty(filterSchoolID))
            {
                DisplayedStudents = Student.GetStudentsFromSchool(AllStudents, filterSchoolID);
            }
            else
            {
                //DisplayedStudents = AllStudents;
            }

        }
    }
}