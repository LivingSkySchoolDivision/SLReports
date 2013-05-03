using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.INAC
{
    public partial class index : System.Web.UI.Page
    {
        public static List<Student> AllStudents;
        public static List<Absence> AllAbsences;
        public static List<School> Schools;

        protected void Page_Load(object sender, EventArgs e)
        {
            AllStudents = new List<Student>();
            Schools = new List<School>();
            AllAbsences = new List<Absence>();
            
            DateTime startDate = new DateTime(1900, 1, 1);
            DateTime endDate = DateTime.Now;

            String dbUser = @"sql_readonly";
            String dbPassword = @"XTXVDUNHlrdbefjTBgY4";
            String dbHost = "dcsql.lskysd.ca";
            String dbDatabase = "SchoolLogicDB";
            //String dbDatabase = "SchoolLogicDB";
            String dbConnectionString = "data source=" + dbHost + ";initial catalog=" + dbDatabase + ";user id=" + dbUser + ";password=" + dbPassword + ";Trusted_Connection=false";
            
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

            /* Load all students */
            #region Load all students
            try
            {
                SqlConnection dbConnection = new SqlConnection(dbConnectionString);
                SqlCommand sqlCommand = new SqlCommand();

                sqlCommand.Connection = dbConnection;
                sqlCommand.CommandType = CommandType.Text;
                //sqlCommand.CommandText = "SELECT * FROM FIM_Students WHERE LEN(StatusNo) > 0 AND LEN(BandName) > 0 AND LEN(ReserveName) > 0 ;";
                sqlCommand.CommandText = "SELECT * FROM FIM_Students WHERE ResideOnReserve=1;";
                //sqlCommand.CommandText = "SELECT * FROM FIM_Students;";
                sqlCommand.Connection.Open();

                SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                if (dbDataReader.HasRows)
                {

                    AllStudents.Clear();
                    while (dbDataReader.Read())
                    {
                        Student newStudent = new Student(
                            dbDataReader["LegalFirstName"].ToString(),
                            dbDataReader["LegalLastName"].ToString(),
                            dbDataReader["LegalMiddleName"].ToString(),
                            dbDataReader["StudentNumber"].ToString(),
                            dbDataReader["GovernmentIDNumber"].ToString(),
                            dbDataReader["School"].ToString(),
                            dbDataReader["SchoolID"].ToString(),
                            dbDataReader["Grade"].ToString(),
                            dbDataReader["Region"].ToString(),
                            dbDataReader["City"].ToString(),
                            dbDataReader["Street"].ToString(),
                            dbDataReader["HouseNo"].ToString(),
                            dbDataReader["ApartmentNo"].ToString(),
                            dbDataReader["PostalCode"].ToString(),
                            dbDataReader["Phone"].ToString(),
                            dbDataReader["Gender"].ToString(),
                            dbDataReader["InStatus"].ToString(),
                            DateTime.Parse(dbDataReader["InDate"].ToString()),
                            DateTime.Parse(dbDataReader["DateOfBirth"].ToString()),
                            dbDataReader["BandNo"].ToString(),
                            dbDataReader["BandName"].ToString(),
                            dbDataReader["ReserveName"].ToString(),
                            dbDataReader["ReserveHouse"].ToString(),
                            dbDataReader["StatusNo"].ToString(),
                            bool.Parse(dbDataReader["ResideOnReserve"].ToString())
                            );
                        AllStudents.Add(newStudent);
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

            long LoadTime_Start = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            /* Load all absences */

            /*
            try {
                SqlConnection dbConnection = new SqlConnection(dbConnectionString);
                foreach (Student student in AllStudents)
                {
                    student.setAbsences(Student.loadAbsencesFromStudent(dbConnection,student.getStudentID(),startDate,endDate));
                }
            } 
            catch {}
            */
            #region Load all Absences
            
            try
            {
                SqlConnection dbConnection = new SqlConnection(dbConnectionString);
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = dbConnection;
                sqlCommand.CommandType = CommandType.Text;
                /* This looks a bit ugly, but it tests alright, and it speeds up the loading of absences considerably */
                StringBuilder SQL = new StringBuilder();
                SQL.Append("SELECT * FROM LSKY_Attendance WHERE (dDate BETWEEN '" + startDate.ToShortDateString() + "' AND '" + endDate.ToShortDateString() + "') AND (");
                foreach (Student student in AllStudents)
                {
                    SQL.Append("(StudentNumber = '"+student.getStudentID()+"') OR ");
                }
                SQL.Remove(SQL.Length - 4, 4);
                SQL.Append(") ORDER BY dDate ASC, tStartTime ASC;");
                sqlCommand.CommandText = SQL.ToString();
                sqlCommand.Connection.Open();
                SqlDataReader dataReader = sqlCommand.ExecuteReader();

                if (dataReader.HasRows)
                {
                    AllAbsences.Clear();
                    while (dataReader.Read())
                    {
                        AllAbsences.Add(new Absence(
                            DateTime.Parse(dataReader["dDate"].ToString()),
                            dataReader["StudentNumber"].ToString().Trim(),
                            dataReader["ClassName"].ToString().Trim(),
                            dataReader["ClassID"].ToString().Trim(),
                            dataReader["Status"].ToString().Trim(),
                            dataReader["Reason"].ToString().Trim(),
                            dataReader["Comment"].ToString().Trim(),
                            int.Parse(dataReader["Block"].ToString()),
                            DateTime.Parse(dataReader["tStartTime"].ToString()),
                            DateTime.Parse(dataReader["tEndTime"].ToString())
                            ));
                    }
                }
                sqlCommand.Connection.Close();
            }
            catch { }            
            #endregion
            
            AllStudents.Sort();

            /* Load absences into the Student list */

            foreach (Student student in AllStudents)
            {
                foreach (Absence abs in AllAbsences)
                {
                    if (abs.getStudentID().ToLower().Equals(student.getStudentID().ToLower()))
                    {
                        student.addAbsence(abs);
                    }
                }
            }

            long LoadTime_End = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            

        }

        public void buildStudentTable(List<Student> students)
        {
            int displayedStudentCount = 0;            
            /* Create the table header */
            Response.Write("<table border=0 class=\"datatable\" cellpadding=3>");
            Response.Write("<tr valign=\"top\" class=\"datatable_header\">");
            Response.Write("<th valign=\"top\" width=\"150\"><b>Student ID</b></th>");
            Response.Write("<th valign=\"top\" width=\"150\"><b>Given Name</b></th>");
            Response.Write("<th valign=\"top\" width=\"150\"><b>Surname</b></th>");
            Response.Write("<th valign=\"top\" width=\"150\"><b>Middle Name</b></th>");
            Response.Write("<th valign=\"top\" width=\"150\"><b>Gender</b></th>");
            Response.Write("<th valign=\"top\" width=\"150\"><b>Birthday</b></th>");
            Response.Write("<th valign=\"top\" width=\"150\"><b>School</b></th>");
            Response.Write("<th valign=\"top\" width=\"150\"><b>Band Affiliation</b></th>");
            Response.Write("<th valign=\"top\" width=\"150\"><b>Band No</b></th>");
            Response.Write("<th valign=\"top\" width=\"100\"><b>Status No</b></th>");
            Response.Write("<th valign=\"top\" width=\"100\"><b>Reserve of Residence</b></th>");
            Response.Write("<th valign=\"top\" width=\"300\"><b>House</b></th>");
            Response.Write("<th valign=\"top\" width=\"100\"><b>Parent / Guardian Name</b></th>");
            Response.Write("<th valign=\"top\" width=\"200\"><b>Date Registered</b></th>");
            Response.Write("<th valign=\"top\" width=\"200\"><b><span style=\"color: red\">Blocks</span> Absent (Unknown)</b></th>");
            Response.Write("<th valign=\"top\" width=\"200\"><b><span style=\"color: red\">Blocks</span> absent (Known)</b></th>");
            Response.Write("<th valign=\"top\" width=\"200\"><b><span style=\"color: red\">Blocks</span> Late</b></th>");
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
        public void buildStudentTable_Row(Student student)
        {
            int numAbs_Unexplained = 0;
            int numAbs_Explained = 0;
            int numLates = 0;

            foreach (Absence abs in student.getAbsences())
            {
                if (abs.getStatus().ToLower().Equals("absent"))
                {
                    if (string.IsNullOrEmpty(abs.getReason()))
                    {
                        numAbs_Unexplained++;
                    }
                    else
                    {
                        numAbs_Explained++;
                    }
                } else if (abs.getStatus().ToLower().Equals("late"))
                {
                    numLates++;
                }
            }

            Response.Write("<tr class=\"row\">");
            Response.Write("<td>" + student.getStudentID() + "</td>");
            Response.Write("<td>" + student.getGivenName() + "</td>");
            Response.Write("<td>" + student.getSN() + "</td>");
            Response.Write("<td>" + student.getMiddleName() + "</td>");
            Response.Write("<td>" + student.getGender() + "</td>");
            Response.Write("<td>" + student.getDateOfBirth().ToShortDateString() + "</td>");
            Response.Write("<td>" + student.getSchoolName() + "</td>");
            Response.Write("<td>" + student.getBandName() + "</td>");
            Response.Write("<td>" + student.getBandNo() + "</td>");
            Response.Write("<td>" + student.getStatusNo() + "</td>");
            Response.Write("<td>" + student.getReserveName() + "</td>");
            Response.Write("<td>" + student.getReserveHouse() + "</td>");
            Response.Write("<td></td>");
            Response.Write("<td>" + student.getEnrollDate().ToShortDateString() + "</td>");
            Response.Write("<td>"+numAbs_Unexplained+"</td>");
            Response.Write("<td>"+numAbs_Explained+"</td>");
            Response.Write("<td>"+numLates+"</td>");
            Response.Write("</tr>\n");
        }

    }
}