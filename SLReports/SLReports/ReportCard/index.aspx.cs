using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.ReportCard
{
    public partial class index : System.Web.UI.Page
    {
        String dbUser = @"sql_readonly";
        String dbPassword = @"XTXVDUNHlrdbefjTBgY4";
        String dbHost = "dcsql.lskysd.ca";
        String dbDatabase = "SchoolLogicDB";
        
        List<School> AllSchools;
        List<Student> DisplayedStudents;
        List<Term> DisplayedTerms;
        List<ReportPeriod> DisplayedReportPeriods;
        List<Mark> DisplayedMarks;

        Student SelectedStudent;
        School SelectedSchool;
        int SelectedSchoolID;
        Term SelectedTerm;
        Track SelectedTrack;
        ReportPeriod SelectedReportPeriod;

        protected void Page_Init(object sender, EventArgs e)
        {
            String dbConnectionString = "data source=" + dbHost + ";initial catalog=" + dbDatabase + ";user id=" + dbUser + ";password=" + dbPassword + ";Trusted_Connection=false";
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                /* Load all schools */
                AllSchools = School.loadAllSchools(connection);
            }

            foreach (School school in AllSchools)
            {
                ListItem newItem = new ListItem();
                newItem.Text = school.getName();
                newItem.Value = school.getGovID();
                drpSchoolList.Items.Add(newItem);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            /* Load a specific student, for testing */
            String dbConnectionString = "data source=" + dbHost + ";initial catalog=" + dbDatabase + ";user id=" + dbUser + ";password=" + dbPassword + ";Trusted_Connection=false";

            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {

                SelectedStudent = Student.loadThisStudent(connection, "12511");

                if (!String.IsNullOrEmpty(SelectedStudent.getTrackID()))
                {
                    Track selectedTrack = Track.loadThisTrack(connection, int.Parse(SelectedStudent.getTrackID()));
                    List<Term> validTerms = Term.loadTermsFromThisTrack(connection, selectedTrack.getID());

                    if (selectedTrack != null)
                    {                        
                        foreach (Term term in validTerms)
                        {                            
                            List<ReportPeriod> reportPeriods = ReportPeriod.loadReportPeriodsFromThisTerm(connection,term.ID);
                            foreach (ReportPeriod period in reportPeriods)
                            {                                
                                List<Mark> marks = Mark.loadMarksFromThisReportPeriod(connection, period, SelectedStudent);                                
                                foreach (Mark mark in marks)
                                {                                    
                                }
                            }
                        }
                    }
                    else
                    {
                        Response.Write("No Track");
                    }
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            SelectedSchoolID = int.Parse(drpSchoolList.SelectedValue);
            String dbConnectionString = "data source=" + dbHost + ";initial catalog=" + dbDatabase + ";user id=" + dbUser + ";password=" + dbPassword + ";Trusted_Connection=false";
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                DisplayedStudents = Student.loadStudentsFromThisSchool(connection, SelectedSchoolID);
                drpStudentList.Items.Clear();

                foreach (Student student in DisplayedStudents)
                {
                    ListItem newItem = new ListItem();
                    newItem.Value = student.getStudentID();
                    newItem.Text = student.getStudentID() + " " + student.getDisplayName();
                    drpStudentList.Items.Add(newItem);
                }
                TableRow_Student.Visible = true;
                TableRow_Term.Visible = false;
                
            }            
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            SelectedSchoolID = int.Parse(drpSchoolList.SelectedValue);

            String dbConnectionString = "data source=" + dbHost + ";initial catalog=" + dbDatabase + ";user id=" + dbUser + ";password=" + dbPassword + ";Trusted_Connection=false";
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                SelectedStudent = Student.loadThisStudent(connection, drpStudentList.SelectedValue);
                DisplayedStudents = Student.loadStudentsFromThisSchool(connection, SelectedSchoolID);
                DisplayedTerms = Term.loadTermsFromThisTrack(connection, int.Parse(SelectedStudent.getTrackID()));
                drpTermList.Items.Clear();
                foreach (Term term in DisplayedTerms)
                {
                    ListItem newItem = new ListItem();
                    newItem.Value = term.ID.ToString();
                    newItem.Text = term.name;
                    drpTermList.Items.Add(newItem);
                }
                TableRow_Term.Visible = true;

            }            
            
        }

        protected string generateMarkTable(List<ReportPeriod> ReportPeriodsWithMarks)
        {

            Dictionary<Mark, List<ReportPeriod>> courses = new Dictionary<Mark, List<ReportPeriod>>();


            StringBuilder returnMe = new StringBuilder();

            foreach (ReportPeriod rp in ReportPeriodsWithMarks)
            {                

                foreach (Mark mark in rp.marks)
                {

                }
            }

            return returnMe.ToString();
        }

        protected string generateStudentNameplate(Student student)
        {
            StringBuilder returnMe = new StringBuilder();

            returnMe.Append("<hr>Nameplate"+student.getDisplayName()+"<hr>");

            return returnMe.ToString();
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            SelectedSchoolID = int.Parse(drpSchoolList.SelectedValue);

            String dbConnectionString = "data source=" + dbHost + ";initial catalog=" + dbDatabase + ";user id=" + dbUser + ";password=" + dbPassword + ";Trusted_Connection=false";
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                SelectedStudent = Student.loadThisStudent(connection, drpStudentList.SelectedValue);
                DisplayedStudents = Student.loadStudentsFromThisSchool(connection, SelectedSchoolID);
                DisplayedTerms = Term.loadTermsFromThisTrack(connection, int.Parse(SelectedStudent.getTrackID()));
                SelectedTerm = Term.loadThisTerm(connection, int.Parse(drpTermList.SelectedValue));

                DisplayedReportPeriods = ReportPeriod.loadReportPeriodsFromThisTerm(connection, SelectedTerm.ID);

                DisplayedMarks = Mark.loadMarksFromTheseReportPeriods(connection, DisplayedReportPeriods, SelectedStudent);

                foreach (Mark m in DisplayedMarks)
                {
                    Response.Write("<BR>" + m);
                }

                /*
                foreach (ReportPeriod rp in DisplayedReportPeriods)
                {
                    rp.marks = Mark.loadMarksFromThisReportPeriod(connection, rp, SelectedStudent);                    
                }
                 * */

                litNamePlate.Text = generateStudentNameplate(SelectedStudent);

                
            }     

        }
    }
}