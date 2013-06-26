using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
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
        List<School> AllSchools;
        List<Student> DisplayedStudents;
        List<Term> DisplayedTerms;
        List<ReportPeriod> DisplayedPeriods;
        Student SelectedStudent;
        int SelectedSchoolID;

        protected void Page_Init(object sender, EventArgs e)
        {
            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                /* Load all schools */
                AllSchools = School.loadAllSchools(connection);
                AllSchools.Sort();
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
        }
              
        protected void Button1_Click(object sender, EventArgs e)
        {
            SelectedSchoolID = int.Parse(drpSchoolList.SelectedValue);
            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
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
                TableRow_ReportPeriod.Visible = false;

            }
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            SelectedSchoolID = int.Parse(drpSchoolList.SelectedValue);

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                SelectedStudent = Student.loadThisStudent(connection, drpStudentList.SelectedValue);
                DisplayedStudents = Student.loadStudentsFromThisSchool(connection, SelectedSchoolID);
                

                /* Load some terms to fill the dropdown box */
                DisplayedTerms = Term.loadTermsFromThisTrack(connection, SelectedStudent.getTrackID());
                drpTermList.Items.Clear();

                foreach (Term term in DisplayedTerms)
                {
                    ListItem newItem = new ListItem();
                    newItem.Text = term.name;
                    newItem.Value = term.ID.ToString();
                    drpTermList.Items.Add(newItem);
                }

                /* Load some report periods to fill the dropdown box */

                DisplayedPeriods = new List<ReportPeriod>();
                
                foreach (Term term in DisplayedTerms) 
                {
                    List<ReportPeriod> tempList = new List<ReportPeriod>();
                    tempList.Clear();
                    tempList = ReportPeriod.loadReportPeriodsFromThisTerm(connection, term);
                    foreach (ReportPeriod rp in tempList)
                    {
                        DisplayedPeriods.Add(rp);
                    }
                }

                drpReportPeriodList.Items.Clear();
                foreach (ReportPeriod rp in DisplayedPeriods)
                {
                    ListItem newItem = new ListItem();
                    newItem.Text = rp.name;
                    newItem.Value = rp.ID.ToString();
                    drpReportPeriodList.Items.Add(newItem);
                }

                TableRow_Term.Visible = true;
                TableRow_ReportPeriod.Visible = true;
            }

        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            /* This would redirect to the multi term report card when finished */

        }

        protected void btnRPGenPDF_Click(object sender, EventArgs e)
        {
            Response.Redirect("/SLReports/ReportCard/SingleReportPeriodPDF.aspx?studentid=" + drpStudentList.SelectedValue + "&reportperiod=" + drpReportPeriodList.SelectedValue);
        }

        protected void btnTermGenPDF_Click(object sender, EventArgs e)
        {

        }

        protected void btnTermGenHTML_Click(object sender, EventArgs e)
        {

        }

        protected void btnRPGenHTML_Click(object sender, EventArgs e)
        {
            Response.Redirect("/SLReports/ReportCard/SingleReportPeriodHTML.aspx?studentid=" + drpStudentList.SelectedValue + "&reportperiod=" + drpReportPeriodList.SelectedValue);
        }
    }
}