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
    public partial class ReportCard_ByGrade : System.Web.UI.Page
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
                tblrow_Options.Visible = false;
                tblrow_Options2.Visible = false;
                tblrow_Options3.Visible = false;

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
                    tblrow_Options.Visible = true;
                    tblrow_Options2.Visible = true;
                    tblrow_Options3.Visible = true;
                }

            }

        }

        protected void btnReportPeriod_Click(object sender, EventArgs e)
        {
            string selectedGrade = drpGrades.SelectedValue;

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
                        // Load all students
                        List<Student> schoolStudents = new List<Student>();
                        schoolStudents = Student.loadStudentsFromThisSchool(connection, schoolID);

                        // Filter out the ones for that grade
                        List<Student> selectedStudents = new List<Student>();
                        foreach (Student student in schoolStudents)
                        {
                            if (student != null)
                            {
                                if (student.getGrade() == selectedGrade)
                                {
                                    selectedStudents.Add(student);
                                }
                            }
                        }

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
                        if (chkAnonymize.Checked)
                            anonymize = true;

                        bool showPhotos = false;
                        if (chkShowPhotos.Checked)
                            showPhotos = true;

                        bool showClassAttendance = false;
                        if (chkClassAttendance.Checked)
                            showClassAttendance = true;

                        bool showLegends = false;
                        if (chkShowLegend.Checked)
                            showLegends = true;

                        bool showAttendanceSummary = false;
                        if (chkShowAttendanceSummary.Checked)
                            showAttendanceSummary = true;

                        string adminComment = txtAdminComment.Text;

                        // Send the report card
                        String fileName = "ReportCards_" + LSKYCommon.removeSpaces(selectedSchool.getName()) + "_Grade" + selectedGrade + "_" + DateTime.Today.Year + "_" + DateTime.Today.Month + "_" + DateTime.Today.Day + ".pdf";
                        if ((selectedReportPeriods.Count > 0) && (selectedStudents.Count > 0))
                        {
                            sendPDF(PDFReportCardParts.GeneratePDF(selectedStudents, selectedReportPeriods, anonymize, showPhotos, doubleSidedMode, showClassAttendance, showLegends, showAttendanceSummary, adminComment), fileName);
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