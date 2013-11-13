using System;
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
    public partial class ReportCard_ByStudent : System.Web.UI.Page
    {

        // So that the database can be quickly changed
        //string sqlConnectionString = LSKYCommon.dbConnectionString_SchoolLogic;
        string sqlConnectionString = PDFReportCardParts.ReportCardDatabase;
        
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
            drpStudents.Items.Clear();

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

                // Populate list of students

                // Get all grades               
                foreach (Student student in schoolStudents)
                {
                    ListItem newItem = new ListItem();
                    newItem.Text = student.getLastName() + ", " + student.getFirstName() + " (" + student.getStudentID() + ") (Grade " + student.getGradeFormatted() + ")" ;
                    newItem.Value = student.getStudentID();
                    drpStudents.Items.Add(newItem);
                }

                btnStudent.Visible = true;
                tblrow_Grade.Visible = true;
                tblrow_ReportPeriod.Visible = false;
                tblrow_Options.Visible = false;
                tblrow_Options2.Visible = false;
                tblrow_Options3.Visible = false;

            }
        }

        protected void btnStudent_Click(object sender, EventArgs e)
        {
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

                    btnStudent.Visible = false;
                    tblrow_ReportPeriod.Visible = true;
                    tblrow_Options.Visible = true;
                    tblrow_Options2.Visible = true;
                    tblrow_Options3.Visible = true;
                }

            }

        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {

            // Parse the selected school ID
            int schoolID = -1;
            if (int.TryParse(drpSchools.SelectedValue, out schoolID))
            {
                School selectedSchool = null;
                using (SqlConnection connection = new SqlConnection(sqlConnectionString))
                {
                    // Load the school
                    selectedSchool = School.loadThisSchool(connection, schoolID);

                    if (selectedSchool != null)
                    {
                        // Load the selected student
                        Student selectedStudent = Student.loadThisStudent(connection, drpStudents.SelectedValue);

                        if (selectedStudent != null)
                        {
                            List<Student> selectedStudents = new List<Student>();
                            selectedStudents.Add(selectedStudent);

                            // Load checked report periods
                            List<int> selectedReportPeriodIDs = new List<int>();
                            List<ReportPeriod> selectedReportPeriods = new List<ReportPeriod>();

                            foreach (ListItem item in chkReportPeriods.Items)
                            {
                                if (item.Selected)
                                {
                                    int parsedValue = -1;
                                    if (int.TryParse(item.Value, out parsedValue))
                                    {
                                        if (!selectedReportPeriodIDs.Contains(parsedValue))
                                        {
                                            selectedReportPeriodIDs.Add(parsedValue);
                                        }
                                    }
                                }
                            }

                            if (selectedReportPeriodIDs.Count > 0)
                            {

                                foreach (int reportPeriodID in selectedReportPeriodIDs)
                                {
                                    ReportPeriod loadedReportPeriod = ReportPeriod.loadThisReportPeriod(connection, reportPeriodID);
                                    if (loadedReportPeriod != null)
                                    {
                                        selectedReportPeriods.Add(loadedReportPeriod);
                                    }
                                }

                                // Load student mark data
                                List<Student> studentsWithMarks = new List<Student>();
                                foreach (Student student in selectedStudents)
                                {
                                    studentsWithMarks.Add(LSKYCommon.loadStudentMarkData(connection, student, selectedReportPeriods));
                                }

                                // Options
                                bool doubleSidedMode = false;
                                if (chkDoubleSidedMode.Checked)
                                    doubleSidedMode = true;

                                bool anonymize = false;

                                bool showPhotos = false;
                                if (chkShowPhotos.Checked)
                                    showPhotos = true;

                                bool showClassAttendance = false;
                                if (chkClassAttendance.Checked)
                                    showClassAttendance = true;

                                bool showLegends = true;

                                bool showAttendanceSummary = false;
                                if (chkShowAttendanceSummary.Checked)
                                    showAttendanceSummary = true;

                                string adminComment = txtAdminComment.Text;

                                // Send the report card
                                String fileName = "ReportCards_" + selectedStudent.getStudentID() + "_" + DateTime.Today.Year + "_" + DateTime.Today.Month + "_" + DateTime.Today.Day + ".pdf";
                                if ((selectedReportPeriods.Count > 0) && (selectedStudents.Count > 0))
                                {
                                    sendPDF(PDFReportCardParts.GeneratePDF(selectedStudents, selectedReportPeriods, anonymize, showPhotos, doubleSidedMode, showClassAttendance, showLegends, showAttendanceSummary, adminComment), fileName);
                                }
                            }
                            else
                            {
                                // No report periods were selected - should display some kind of error here                                
                            }
                        }
                    }
                }
            }

        }

        protected void sendPDF(System.IO.MemoryStream PDFData, string filename)
        {
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + filename + "");

            Response.OutputStream.Write(PDFData.GetBuffer(), 0, PDFData.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();
            //Response.End();
        }
    }
}