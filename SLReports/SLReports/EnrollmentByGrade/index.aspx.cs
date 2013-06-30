using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.DivisionStats
{
    public partial class index : System.Web.UI.Page
    {
        public static List<Student> AllStudents;
        public static List<School> Schools;
        public SortedDictionary<School, List<Student>> StudentsBySchool;
                
        public void buildGradeStatisticsTable(List<Student> students)
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

            Response.Write("<table>");
            Response.Write("<tr class=\"datatable_header\">");
            Response.Write("<th width=\"75\">Grade</th>");
            foreach (var grade in grades)
            {
                Response.Write("<th align=\"center\" width=\"25\">" + grade.Key + "</th>");
            }
            Response.Write("</tr><tr>");
            Response.Write("<th class=\"datatable_header\"><b>Count</b></th>");
            foreach (var grade in grades)
            {
                Response.Write("<td align=\"center\" style=\"border: 1px solid black;\">" + grade.Value + " </td>");
            }

            Response.Write("</tr></table>");



        }

        public void buildEnrollmentTable(List<School> schools, List<Student> students)
        {
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
            }
            allGrades.Sort();

            int totalMales = 0;
            int totalFemales = 0;            

            /* School by School */
            Response.Write("<table class=\"datatable\" cellpadding=5>");
            Response.Write("<tr class=\"datatable_header\"><th></th><th colspan=3>Students</th><th colspan=\""+allGrades.Count()+"\">Grade</tr>");
            Response.Write("<tr class=\"datatable_header\"><th>School</th><th>Total</th><th>Male</th><th>Female</th>");
            foreach (String grade in allGrades)
            {
                Response.Write("<th>"+grade+"</th>");
            }
            Response.Write("</tr>");

            Dictionary<String, int> totalGradeEnrollment = new Dictionary<string, int>();
            foreach (School school in schools)
            {
                Dictionary<String, int> gradeEnrollment = new Dictionary<string, int>();
                List<Student> thisSchoolsStudents = Student.GetStudentsFromSchool(students, school.getGovID());                
                int numMale = 0;
                int numFemale = 0;
                foreach (Student student in thisSchoolsStudents)
                {                    
                    if (student.getGender().ToLower().Equals("male"))
                    {
                        numMale++;
                        totalMales++;
                    }
                    else
                    {
                        numFemale++;
                        totalFemales++;
                    }

                    if (!gradeEnrollment.ContainsKey(student.getGrade()))
                    {
                        gradeEnrollment.Add(student.getGrade(), 1);
                    }
                    else
                    {
                        gradeEnrollment[student.getGrade()]++;
                    }

                    if (!totalGradeEnrollment.ContainsKey(student.getGrade()))
                    {
                        totalGradeEnrollment.Add(student.getGrade(), 1);
                    }
                    else
                    {
                        totalGradeEnrollment[student.getGrade()]++;
                    }
                }                

                Response.Write("<tr class=\"row\"><td>" + school.getName() + "</td><td class=\"td_total\">" + thisSchoolsStudents.Count() + "</td><td class=\"td_male\">" + numMale + "</td><td class=\"td_female\">" + numFemale + "</td>");
                foreach (String grade in allGrades)
                {
                    Response.Write("<td>");
                    if (gradeEnrollment.ContainsKey(grade))
                    {
                        Response.Write(gradeEnrollment[grade].ToString());
                    }
                    Response.Write("</td>");
                }
                Response.Write("</tr>");
            }

            Response.Write("<tr class=\"datatable_header\"><td><b>Total</b></td><td>" + students.Count() + "</td><td>" + totalMales + "</td><td>" + totalFemales + "</td>");
            foreach (String grade in allGrades)
            {
                Response.Write("<td>");
                if (totalGradeEnrollment.ContainsKey(grade))
                {
                    Response.Write(totalGradeEnrollment[grade].ToString());
                }
                Response.Write("</td>");
            }
            Response.Write("</tr>");
            Response.Write("</table>");
        }

        public void buildStatisticsTableBySchool(List<School> schools, List<Student> students)
        {
            
            
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


        protected void Page_Load(object sender, EventArgs e)
        {
            AllStudents = new List<Student>();
            Schools = new List<School>();

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;

            #region Load all students
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                AllStudents = Student.loadAllStudents(connection);
            }
            #endregion

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
                        Schools.Add(new School(dbDataReader["name"].ToString(), dbDataReader["internalID"].ToString(), dbDataReader["govID"].ToString(), dbDataReader["address"].ToString()));
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
        }
    }
}