using System;
using System.Collections.Generic;
using System.Configuration;
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

        //String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
        String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogic2013"].ConnectionString;

        public static List<Student> AllStudents;
        public static List<Absence> AllAbsences;
        public static List<School> AllSchools;
        public static List<Contact> AllContacts;

        private static School selectedSchool = null;

        DateTime startDate = DateTime.Now.AddMonths(-1);
        DateTime endDate = DateTime.Now;        

        private TableRow createStudentRow(Student student)
        {
            /* figure out days absent */
            float daysAbsent =-1;

            // Should we be calculating daily absenses or class based absenses
            if (student.track.daily == true)
            {
                daysAbsent = (float)((float)student.absences.Count / (float)student.track.dailyBlocksPerDay);
            }
            else
            {
                // Figure out how many tracks the given date range spans
                // For each track
                 // Figure out how many classes per day this student actually has
                 // Calculate and add to the total                
            }

            /* figure out guardian(s) */
            StringBuilder guardians = new StringBuilder();

            foreach (Contact contact in student.contacts)
            {
                if ((contact.priority == 1) && (contact.livesWithStudent == true))
                {
                    guardians.Append(contact.firstName + " " + contact.lastName + " <i>(" + contact.relation + ")</i><br>");
                }                
            }

            TableRow newRow = new TableRow();

            TableCell gradeCell = new TableCell();
            gradeCell.Text = student.getGradeFormatted();
            gradeCell.VerticalAlign = VerticalAlign.Top;
            newRow.Cells.Add(gradeCell);

            TableCell nameCell = new TableCell();
            nameCell.Text = student.getDisplayName();
            nameCell.VerticalAlign = VerticalAlign.Top;
            newRow.Cells.Add(nameCell);

            TableCell birthdayCell = new TableCell();
            birthdayCell.Text = student.getDateOfBirth().Month + "/" + student.getDateOfBirth().Day + "/" + student.getDateOfBirth().Year;
            birthdayCell.VerticalAlign = VerticalAlign.Top;
            newRow.Cells.Add(birthdayCell);

            TableCell bandCell = new TableCell();
            bandCell.Text = student.getBandName();
            bandCell.VerticalAlign = VerticalAlign.Top;
            newRow.Cells.Add(bandCell);

            TableCell statusNoCell = new TableCell();
            statusNoCell.Text = student.getStatusNo();
            statusNoCell.VerticalAlign = VerticalAlign.Top;
            newRow.Cells.Add(statusNoCell);

            TableCell reserveCell = new TableCell();
            reserveCell.Text = student.getReserveName();
            reserveCell.VerticalAlign = VerticalAlign.Top;
            newRow.Cells.Add(reserveCell);

            TableCell houseNoCell = new TableCell();
            houseNoCell.Text = student.getReserveHouse();
            houseNoCell.VerticalAlign = VerticalAlign.Top;
            newRow.Cells.Add(houseNoCell);

            TableCell guardianCell = new TableCell();
            guardianCell.Text = guardians.ToString();
            guardianCell.VerticalAlign = VerticalAlign.Top;
            newRow.Cells.Add(guardianCell);

            TableCell daysAbsentCell = new TableCell();
            if (daysAbsent == 0)
            {
                daysAbsentCell.Text = "<i style=\"color: #707070;\">No Absences</i>";
            } else if (daysAbsent >= 0)
            {
                daysAbsentCell.Text = daysAbsent.ToString() + " days (" + student.absences.Count + " blocks)";
            }
            daysAbsentCell.VerticalAlign = VerticalAlign.Top;
            newRow.Cells.Add(daysAbsentCell);

            TableCell dateRegisterCell = new TableCell();
            dateRegisterCell.Text = student.getEnrollDate().Month + "/" + student.getEnrollDate().Day + "/" + student.getEnrollDate().Year;
            dateRegisterCell.VerticalAlign = VerticalAlign.Top;
            newRow.Cells.Add(dateRegisterCell);
            
            return newRow;

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AllStudents = new List<Student>();
                AllSchools = new List<School>();
                AllAbsences = new List<Absence>();
                AllContacts = new List<Contact>();
                selectedSchool = null;
                
                #region set up date picker fields

                #region Year
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
                #endregion

                #region Month
                for (int x = 1; x <= 12; x++)
                {
                    ListItem newLI_From = new ListItem(LSKYCommon.getMonthName(x), x.ToString());
                    ListItem newLI_To = new ListItem(LSKYCommon.getMonthName(x), x.ToString());
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
                #endregion

                #region Day
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
                #endregion

                #endregion

                #region Set up list of all schools
                using (SqlConnection connection = new SqlConnection(dbConnectionString))
                {
                    AllSchools = School.loadAllSchools(connection);
                }
                foreach (School school in AllSchools)
                {
                    ListItem newLI = new ListItem();
                    newLI.Text = school.getName();
                    newLI.Value = school.getGovID();
                    lstSchoolList.Items.Add(newLI);
                }
                #endregion

            }
            else
            {
                foreach (School school in AllSchools)
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


            using (SqlConnection connection = new SqlConnection(dbConnectionString)) 
            {
                AllSchools = School.loadAllSchools(connection);

                if (selectedSchool != null)
                {
                    // Load students
                    AllStudents = Student.loadReserveStudentsFromThisSchol(connection, selectedSchool);

                    lblCount.Text = "Found " + AllStudents.Count + " students";

                    if (AllStudents.Count > 0)
                    {
                        tblResults.Visible = true;
                    }

                    // Load some extra data for students
                    foreach (Student student in AllStudents)
                    {
                        // Load track for students
                        student.track = Track.loadThisTrack(connection, student.getTrackID());

                        // Load contacts for students
                        student.contacts = Contact.loadContactsForStudent(connection, student);

                        // Load absenses for students
                        student.absences = Absence.loadAbsencesForThisStudentAndTimePeriod(connection, student, startDate, endDate);
                    }
                }
            }
            
            AllStudents.Sort();

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


            foreach (Student student in AllStudents)
            {
                tblResults.Rows.Add(createStudentRow(student));
            }

        }        
    }
}