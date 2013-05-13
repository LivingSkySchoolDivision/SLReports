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
        public static List<Contact> AllContacts;

        private static School selectedSchool = null;

        DateTime startDate = DateTime.Now.AddMonths(-1);
        DateTime endDate = DateTime.Now;

        private string getMonthName(int monthNum)
        {
            string returnMe = "Unknown";
            switch(monthNum)
            {
                case 1:
                    returnMe = "January";
                    break;
                case 2:
                    returnMe = "February";
                    break;
                case 3:
                    returnMe = "March";
                    break;
                case 4:
                    returnMe = "April";
                    break;
                case 5:
                    returnMe = "May";
                    break;
                case 6:
                    returnMe = "June";
                    break;
                case 7:
                    returnMe = "July";
                    break;
                case 8:
                    returnMe = "August";
                    break;
                case 9:
                    returnMe = "September";
                    break;
                case 10:
                    returnMe = "October";
                    break;
                case 11:
                    returnMe = "November";
                    break;
                case 12:
                    returnMe = "December";
                    break;
            }
            return returnMe;
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            AllStudents = new List<Student>();
            Schools = new List<School>();
            AllAbsences = new List<Absence>();
            AllContacts = new List<Contact>();

            String dbUser = @"sql_readonly";
            String dbPassword = @"XTXVDUNHlrdbefjTBgY4";
            String dbHost = "dcsql.lskysd.ca";
            String dbDatabase = "SchoolLogicDB";
            //String dbDatabase = "SchoolLogicDB";
            String dbConnectionString = "data source=" + dbHost + ";initial catalog=" + dbDatabase + ";user id=" + dbUser + ";password=" + dbPassword + ";Trusted_Connection=false";

            if (!IsPostBack)
            {
                /* Set up date picker fields */
                
                for (int x = DateTime.Now.Year - 10; x <= DateTime.Now.Year + 10; x++)
                {
                    ListItem newLI_From = new ListItem(x.ToString(), x.ToString());
                    if (!IsPostBack)
                    {
                        if (x == DateTime.Now.Year)
                            newLI_From.Selected = true;
                    }

                    ListItem newLI_To = new ListItem(x.ToString(), x.ToString());
                    if (!IsPostBack)
                    {
                        if (x == DateTime.Now.Year)
                            newLI_To.Selected = true;
                    }
                    from_year.Items.Add(newLI_From);
                    to_year.Items.Add(newLI_To);
                }

                for (int x = 1; x <= 12; x++)
                {
                    ListItem newLI_From = new ListItem(getMonthName(x), x.ToString());
                    ListItem newLI_To = new ListItem(getMonthName(x), x.ToString());
                    if (!IsPostBack)
                    {
                        if (x == (DateTime.Now.Month - 1))
                            newLI_From.Selected = true;
                    }

                    if (!IsPostBack)
                    {
                        if (x == DateTime.Now.Month)
                            newLI_To.Selected = true;
                    }


                    from_month.Items.Add(newLI_From);
                    to_month.Items.Add(newLI_To);
                }

                for (int x = 1; x <= 31; x++)
                {
                    ListItem newLI_From = new ListItem(x.ToString(), x.ToString());
                    ListItem newLI_To = new ListItem(x.ToString(), x.ToString());
                    if (!IsPostBack)
                    {
                        if (x == (DateTime.Now.Day))
                            newLI_From.Selected = true;
                    }

                    if (!IsPostBack)
                    {
                        if (x == DateTime.Now.Day)
                            newLI_To.Selected = true;
                    }

                    from_day.Items.Add(newLI_From);
                    to_day.Items.Add(newLI_To);
                }
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

            if (!IsPostBack)
            {
                selectedSchool = null;

                foreach (School school in Schools)
                {
                    ListItem newLI = new ListItem();
                    newLI.Text = school.getName();
                    newLI.Value = school.getGovID();
                    lstSchoolList.Items.Add(newLI);
                }
            }
            else
            {
                foreach (School school in Schools)
                {
                    if (lstSchoolList.SelectedItem.Value == school.getGovID())
                    {
                        selectedSchool = school;
                    }
                }

                int startYear = int.Parse(from_year.SelectedValue);
                int startMonth = int.Parse(from_month.SelectedValue);
                int startDay = int.Parse(from_day.SelectedValue);
                if (startDay > DateTime.DaysInMonth(startYear, startMonth))
                    startDay = DateTime.DaysInMonth(startYear, startMonth);

                int endYear = int.Parse(to_year.SelectedValue);
                int endMonth = int.Parse(to_month.SelectedValue);
                int endDay = int.Parse(to_day.SelectedValue);
                if (endDay > DateTime.DaysInMonth(endYear, endMonth))
                    endDay = DateTime.DaysInMonth(endYear, endMonth);

                startDate = new DateTime(startYear,startMonth,startDay);
                endDate = new DateTime(endYear,endMonth,endDay);
            }

            /* Load all students */
            #region Load all students
            try
            {
                if (selectedSchool != null)
                {
                    SqlConnection dbConnection = new SqlConnection(dbConnectionString);
                    SqlCommand sqlCommand = new SqlCommand();

                    sqlCommand.Connection = dbConnection;
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.CommandText = "SELECT * FROM LSKY_Students WHERE ResideOnReserve=1 AND SchoolID=@SchoolID;";
                    sqlCommand.Parameters.AddWithValue("@SchoolID", selectedSchool.getGovID());

                    //sqlCommand.CommandText = "SELECT * FROM LSKY_Students;";
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
                                dbDataReader["HomeRoom"].ToString(),
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

            
            #region Load all contacts
            try
            {
                if (AllStudents.Count > 0)
                {
                    SqlConnection dbConnection = new SqlConnection(dbConnectionString);
                    SqlCommand sqlCommand = new SqlCommand();
                    sqlCommand.Connection = dbConnection;
                    sqlCommand.CommandType = CommandType.Text;
                    /* This looks a bit ugly, but it tests alright, and it speeds up the loading of absences considerably */
                    StringBuilder SQL = new StringBuilder();
                    SQL.Append("SELECT * FROM LSKY_Contacts WHERE (");
                    foreach (Student student in AllStudents)
                    {
                        SQL.Append("(StudentNumber = '" + student.getStudentID() + "') OR ");
                    }
                    SQL.Remove(SQL.Length - 4, 4);
                    SQL.Append(") ORDER BY StudentNumber ASC;");
                    sqlCommand.CommandText = SQL.ToString();
                    sqlCommand.Connection.Open();
                    SqlDataReader dataReader = sqlCommand.ExecuteReader();

                    if (dataReader.HasRows)
                    {
                        AllContacts.Clear();
                        while (dataReader.Read())
                        {
                            AllContacts.Add(new Contact(
                                dataReader["FirstName"].ToString().Trim(),
                                dataReader["LastName"].ToString().Trim(),
                                dataReader["Relation"].ToString().Trim(),
                                dataReader["StudentNumber"].ToString().Trim()
                                ));
                        }
                    }
                    sqlCommand.Connection.Close();

                }
            }
            catch {} 
            #endregion

            #region Load all Absences
            try
            {
                if (AllStudents.Count > 0)
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
                        SQL.Append("(StudentNumber = '" + student.getStudentID() + "') OR ");
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
                                DateTime.Parse(dataReader["tEndTime"].ToString()),
                                int.Parse(dataReader["Minutes"].ToString())
                                ));
                        }
                    }
                    sqlCommand.Connection.Close();
                }
            }
            catch { }            
            #endregion
            
            AllStudents.Sort();

            /* Load absences and contacts into the Student list */


            foreach (Student student in AllStudents)
            {
                foreach (Absence abs in AllAbsences)
                {
                    if (abs.getStudentID().ToLower().Equals(student.getStudentID().ToLower()))
                    {
                        student.addAbsence(abs);
                    }
                }

                foreach (Contact con in AllContacts)
                {
                    if (con.getStudentID().ToLower().Equals(student.getStudentID().ToLower()))
                    {
                        student.addContact(con);
                    }                    
                }
            }

            long LoadTime_End = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            

        }

        public void buildStudentTable(List<Student> students)
        {
            if (selectedSchool == null)
            {
                return;
            }
            int displayedStudentCount = 0;            
            /* Create the table header */


            Response.Write("<div class=\"small_infobox\">Found "+students.Count+" students for <b>" + selectedSchool.getName() + "</b><br/>Displaying attendance between <b>" + startDate.ToLongDateString() + "</b> and <b>" + endDate.ToLongDateString() + "</b></div><br/>");
            Response.Write("<table border=0 class=\"datatable\" cellpadding=3>");
            Response.Write("<tr valign=\"top\" class=\"datatable_header\">");
            Response.Write("<th valign=\"top\" width=\"50\"><b>ID #</b></th>");
            Response.Write("<th valign=\"top\" width=\"150\"><b>Given Name</b></th>");
            Response.Write("<th valign=\"top\" width=\"150\"><b>Surname</b></th>");
            Response.Write("<th valign=\"top\" width=\"150\"><b>Middle Name</b></th>");
            Response.Write("<th valign=\"top\" width=\"150\"><b>Gender</b></th>");
            Response.Write("<th valign=\"top\" width=\"150\"><b>Birthday</b></th>");
            Response.Write("<th valign=\"top\" width=\"150\"><b>Grade</b></th>");
            Response.Write("<th valign=\"top\" width=\"200\"><b>Band Affiliation</b></th>");
            Response.Write("<th valign=\"top\" width=\"150\"><b>Band No</b></th>");
            Response.Write("<th valign=\"top\" width=\"100\"><b>Status No</b></th>");
            Response.Write("<th valign=\"top\" width=\"100\"><b>Reserve of Residence</b></th>");
            Response.Write("<th valign=\"top\" width=\"100\"><b>House</b></th>");
            Response.Write("<th valign=\"top\" width=\"300\"><b>Parent / Guardian Name(s)<a href=\"#foot_1\"><sup>1</sup></a></b></th>");
            Response.Write("<th valign=\"top\" width=\"200\"><b>Instatus date</b></th>");
            Response.Write("<th valign=\"top\" width=\"200\"><b><u>Blocks</u> Absent (Unknown)</b></th>");
            Response.Write("<th valign=\"top\" width=\"200\"><b><u>Blocks</u> absent (Known)</b></th>");
            Response.Write("<th valign=\"top\" width=\"200\"><b><u>Blocks</u> Late</b></th>");
            Response.Write("<th valign=\"top\" width=\"200\"><b>Total minutes Late</b></th>");
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
            Response.Write("<div><a name=\"foot_1\"><sup>1</sup></a>: Parent / Guardian is calculated as any priority 1 contact that lives with the student.</div>");
        }
        public void buildStudentTable_Row(Student student)
        {
            int numAbs_Unexplained = 0;
            int numAbs_Explained = 0;
            int numLates = 0;
            int totalMinutesLate = 0;

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
                    totalMinutesLate += abs.getMinutes();
                }
            }

            Response.Write("<tr class=\"row\">");
            Response.Write("<td>" + student.getStudentID() + "</td>");
            Response.Write("<td>" + student.getGivenName() + "</td>");
            Response.Write("<td>" + student.getSN() + "</td>");
            Response.Write("<td>" + student.getMiddleName() + "</td>");
            Response.Write("<td>" + student.getGender() + "</td>");
            Response.Write("<td>" + student.getDateOfBirth().ToShortDateString() + "</td>");
            Response.Write("<td>" + student.getGrade() + "</td>");
            Response.Write("<td>" + student.getBandName() + "</td>");
            Response.Write("<td>" + student.getBandNo() + "</td>");
            Response.Write("<td>" + student.getStatusNo() + "</td>");
            Response.Write("<td>" + student.getReserveName() + "</td>");
            Response.Write("<td>" + student.getReserveHouse() + "</td>");
            Response.Write("<td>");
            StringBuilder contactDisplay = new StringBuilder();
            foreach (Contact con in student.getContacts())
            {
                contactDisplay.Append(con + ", ");
            }
            if (contactDisplay.Length > 2)
            {
                contactDisplay.Remove(contactDisplay.Length - 2, 2);
            }
            Response.Write(contactDisplay);
            Response.Write("</td>");            
            Response.Write("<td>" + student.getEnrollDate().ToShortDateString() + "</td>");
            Response.Write("<td>"+numAbs_Unexplained+"</td>");
            Response.Write("<td>"+numAbs_Explained+"</td>");
            Response.Write("<td>" + numLates + "</td>");
            Response.Write("<td>" + totalMinutesLate + " </td>");
            Response.Write("</td>");
            Response.Write("</tr>\n");
        }
        
    }
}