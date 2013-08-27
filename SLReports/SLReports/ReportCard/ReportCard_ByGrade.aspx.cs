using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.ReportCard
{
    public partial class ReportCard_ByGrade : System.Web.UI.Page
    {
        // So that the database can be quickly changed
        //string sqlConnectionString = LSKYCommon.dbConnectionString_SchoolLogic;
        string sqlConnectionString = LSKYCommon.dbConnectionString_OldSchoolLogic;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Load schools
                List<School> allSchools = new List<School>();
                using (SqlConnection connection = new SqlConnection(sqlConnectionString))
                {
                    allSchools = School.loadAllSchools(connection);
                }

                foreach (School school in allSchools)
                {
                    ListItem newItem = new ListItem();
                    newItem.Text = school.getName();
                    newItem.Value = school.getGovID().ToString();
                    drpSchools.Items.Add(newItem);
                }
            }   
        }

        protected void btnSchool_Click(object sender, EventArgs e)
        {
            drpGrades.Items.Clear();
            
            // Parse the selected school ID
            int schoolID = -1;
            if (int.TryParse(drpSchools.SelectedValue, out schoolID))
            {
                // Load all students
                List<Student> schoolStudents = new List<Student>();
                using (SqlConnection connection = new SqlConnection(sqlConnectionString))
                {
                    schoolStudents = Student.loadStudentsFromThisSchool(connection, schoolID);
                }

                // Get all grades
                List<string> allGrades = new List<string>();
                foreach (Student student in schoolStudents)
                {
                    if (!allGrades.Contains(student.getGrade()))
                    {
                        allGrades.Add(student.getGrade());
                    }
                }
                allGrades.Sort();

                foreach (string grade in allGrades)
                {
                    ListItem newItem = new ListItem();
                    newItem.Text = grade;
                    newItem.Value = grade;
                    drpGrades.Items.Add(newItem);
                }
                
                tblrow_Grade.Visible = true;
                tblrow_ReportPeriod.Visible = false;
            }
        }

        protected void btnGrade_Click(object sender, EventArgs e)
        {
            string selectedGrade = drpGrades.SelectedValue;

            chkReportPeriods.Items.Clear();
            // Parse the selected school ID
            int schoolID = -1;
            if (int.TryParse(drpSchools.SelectedValue, out schoolID))
            {
                School selectedSchool = null;
                using (SqlConnection connection = new SqlConnection(sqlConnectionString))
                {
                    // Load the school
                    selectedSchool = School.loadThisSchool(connection, schoolID);
                }

                if (selectedSchool != null)
                {
                    // Load all students
                    List<Student> schoolStudents = new List<Student>();
                    using (SqlConnection connection = new SqlConnection(sqlConnectionString))
                    {
                        schoolStudents = Student.loadStudentsFromThisSchool(connection, schoolID);
                    }

                    // Filter out the ones for that grade
                    List<Student> thisGradeStudents = new List<Student>();
                    foreach (Student student in schoolStudents)
                    {
                        if (student.getGrade() == selectedGrade)
                        {
                            thisGradeStudents.Add(student);
                        }
                    }
                                        
                    // Get some report periods to display
                    List<Track> schoolTracks = new List<Track>();                    
                    List<ReportPeriod> schoolReportPeriod = new List<ReportPeriod>();
                    using (SqlConnection connection = new SqlConnection(sqlConnectionString))
                    {
                        schoolTracks = Track.loadAllTracksFromThisSchool(connection, selectedSchool);
                        foreach (Track track in schoolTracks)
                        {
                            track.terms = Term.loadTermsFromThisTrack(connection, track);
                            foreach (Term term in track.terms)
                            {
                                term.ReportPeriods = ReportPeriod.loadReportPeriodsFromThisTerm(connection, term);
                            }
                        }

                        schoolReportPeriod = ReportPeriod.loadReportPeriodsFromThisSchool(connection, selectedSchool);
                    }

                    foreach (Track track in schoolTracks)
                    {
                        foreach (Term term in track.terms)
                        {
                            foreach (ReportPeriod rp in term.ReportPeriods)
                            {
                                ListItem newItem = new ListItem();
                                newItem.Text = track.name + " - " + term.name + " - " + rp.name;
                                newItem.Value = rp.ID.ToString();
                                chkReportPeriods.Items.Add(newItem);
                            }
                        }
                    }

                    tblrow_ReportPeriod.Visible = true;
                }
            
            }

        }

        protected void btnReportPeriod_Click(object sender, EventArgs e)
        {
            // Load data and generate a report card



        }
    }
}