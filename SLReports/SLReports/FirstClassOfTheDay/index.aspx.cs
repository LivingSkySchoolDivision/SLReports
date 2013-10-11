using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.FirstClassOfTheDay
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Load school list
                List<School> allSchools = new List<School>();

                using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogic))
                {
                    allSchools = School.loadAllSchools(connection);
                }

                drpSchoolList.Items.Clear();
                foreach (School school in allSchools)
                {
                    ListItem newItem = new ListItem();

                    newItem.Text = school.getName();
                    newItem.Value = school.getGovID().ToString();

                    drpSchoolList.Items.Add(newItem);

                }
            }
        }

        protected void btnSchool_Click(object sender, EventArgs e)
        {
            // Load tracks from the school
            List<Track> schoolTracks = new List<Track>();
            School selectedSchool = null;
            using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogic))
            {
                // Determine which school was selected
                int selectedSchoolID = -1;
                if (int.TryParse(drpSchoolList.SelectedValue, out selectedSchoolID)) 
                {
                    selectedSchool = School.loadThisSchool(connection, selectedSchoolID);
                    if (selectedSchool != null)
                    {
                        // Load tracks from that school
                        schoolTracks = Track.loadAllTracksFromThisSchool(connection, selectedSchool);
                    }
                }                
            }

            drpTrack.Items.Clear();
            foreach (Track track in schoolTracks)
            {
                ListItem newItem = new ListItem();

                newItem.Text = track.name + " (" + track.startDate.ToShortDateString() + " - " + track.endDate.ToShortDateString() + ")";
                newItem.Value = track.ID.ToString();

                drpTrack.Items.Add(newItem);
            }

            if (schoolTracks.Count > 0)
            {
                tblrow_Track.Visible = true;
            }

            tblrow_Term.Visible = false;
            tblCSVLink.Visible = false;
        }
        
        protected void btnTrack_Click(object sender, EventArgs e)
        {
            Track selectedTrack = null;
            List<Term> schoolTerms = new List<Term>();

            using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogic))
            {
                int selectedTrackID = -1;
                if (int.TryParse(drpTrack.SelectedValue, out selectedTrackID))
                {
                    selectedTrack = Track.loadThisTrack(connection, selectedTrackID);
                    if (selectedTrack != null)
                    {
                        schoolTerms = Term.loadTermsFromThisTrack(connection, selectedTrack);                        
                    }
                }
            }

            if (schoolTerms.Count > 0)
            {
                tblrow_Term.Visible = true;
                drpTerm.Items.Clear();
                foreach (Term term in schoolTerms)
                {
                    ListItem newItem = new ListItem();

                    newItem.Text = term.name + " (" + term.startDate.ToShortDateString() + " - " + term.endDate.ToShortDateString() + ")";
                    newItem.Value = term.ID.ToString();

                    drpTerm.Items.Add(newItem);
                }
            }

            tblCSVLink.Visible = false;

        }

        TableRow generateStudentTableHeader(List<SchoolDay> days)
        {
            TableRow newRow = new TableRow();
            newRow.CssClass = "datatable_header";

            TableCell cell_ID = new TableCell();
            cell_ID.CssClass = "datatable_header";
            cell_ID.Text = "ID #";
            newRow.Cells.Add(cell_ID);

            TableCell cell_FirstName = new TableCell();
            cell_FirstName.CssClass = "datatable_header";
            cell_FirstName.Text = "First Name";
            newRow.Cells.Add(cell_FirstName);

            TableCell cell_LastName = new TableCell();
            cell_LastName.CssClass = "datatable_header";
            cell_LastName.Text = "Last Name";
            newRow.Cells.Add(cell_LastName);

            foreach (SchoolDay day in days)
            {
                TableCell cell_Day = new TableCell();
                cell_Day.CssClass = "datatable_header";
                cell_Day.Text = day.name;
                newRow.Cells.Add(cell_Day);
            }

            return newRow;
        }

        TableRow generateStudentRow(Student student, List<SchoolDay> days)
        {
            TableRow newRow = new TableRow();

            TableCell cell_ID = new TableCell();
            cell_ID.CssClass = "datatable_row";
            cell_ID.Text = student.getStudentID();
            newRow.Cells.Add(cell_ID);

            TableCell cell_FirstName = new TableCell();
            cell_FirstName.CssClass = "datatable_row";
            cell_FirstName.Text = student.getFirstName();
            newRow.Cells.Add(cell_FirstName);

            TableCell cell_LastName = new TableCell();
            cell_LastName.CssClass = "datatable_row";
            cell_LastName.Text = student.getLastName();
            newRow.Cells.Add(cell_LastName);
            
            foreach (SchoolDay day in days)
            {
                StringBuilder timeTableInfo = new StringBuilder();
                List<TimeTableEntry> thisDayClasses = new List<TimeTableEntry>();
                                
                foreach (TimeTableEntry tte in student.TimeTable)
                {
                    if (tte.dayNum == day.dayNumber)
                    {
                        thisDayClasses.Add(tte);
                    }
                }
                thisDayClasses.Sort();


                if (thisDayClasses.Count > 0)
                {
                    TimeTableEntry firstClassOfTheDay = TimeTableEntry.getEarliest(thisDayClasses);
                    //timeTableInfo.Append(firstClassOfTheDay.schoolClass.name + " (Teacher: " + firstClassOfTheDay.schoolClass.teacherName + ", Room: " + firstClassOfTheDay.roomName + ", Period: " + firstClassOfTheDay.blockNum + ")");
                    timeTableInfo.Append(firstClassOfTheDay.ToStringFormatted());
                }
                else
                {
                    timeTableInfo.Append("No classes today");
                }

                TableCell cell_Day = new TableCell();
                cell_Day.VerticalAlign = VerticalAlign.Top;
                cell_Day.CssClass = "datatable_row";
                cell_Day.Text = timeTableInfo.ToString();
                newRow.Cells.Add(cell_Day);
            }

            return newRow;
        }

        protected void btnTerm_Click(object sender, EventArgs e)
        {            
            List<Student> schoolStudents = new List<Student>();
            Term selectedTerm = null;
            List<SchoolDay> schoolDays = new List<SchoolDay>();

            using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogic))
            {    
                int selectedTermID = -1;
                if (int.TryParse(drpTerm.SelectedValue, out selectedTermID))
                {
                    selectedTerm = Term.loadThisTerm(connection, selectedTermID);
                    if (selectedTerm != null)
                    {
                        Track selectedTrack = Track.loadThisTrack(connection, selectedTerm.trackID);
                        
                        // Load school days for this school / track
                        schoolDays = SchoolDay.loadDaysFromThisTrack(connection, selectedTrack);

                        // Load students from the track
                        schoolStudents = Student.loadStudentsFromThisTrack(connection, selectedTrack.ID);
                        
                        // Load timetables for the students that were loaded
                        foreach (Student student in schoolStudents)
                        {
                            student.TimeTable = TimeTableEntry.loadStudentTimeTable(connection, student, selectedTerm);
                        }

                    }
                }
            }
            
            tblCSVLink.Visible = true;
            lnkCSVLink.NavigateUrl = "getCSV.aspx?schoolid=" + selectedTerm.schoolID + "&trackid=" + selectedTerm.trackID + "&termid=" + selectedTerm.ID;
            
            tblStudents.Rows.Clear();
            tblStudents.Visible = true;
            tblStudents.Rows.Add(generateStudentTableHeader(schoolDays));

            foreach (Student student in schoolStudents)
            {
                tblStudents.Rows.Add(generateStudentRow(student, schoolDays));
            }

            // Load the days from the specified track

            // Load all students

            // For each day, determine what their classes are

        }
    
    }
}