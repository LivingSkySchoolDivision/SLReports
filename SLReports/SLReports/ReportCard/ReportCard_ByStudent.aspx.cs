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
        string sqlConnectionString = LSKYCommon.dbConnectionString_OldSchoolLogic;


        protected void Page_Load(object sender, EventArgs e)
        {
            // Load list of schools
            if (!IsPostBack)
            {
                List<School> allSchools = new List<School>();

                using (SqlConnection connection = new SqlConnection(sqlConnectionString))
                {
                    allSchools = School.loadAllSchools(connection);
                }

                foreach (School school in allSchools)
                {
                    if (school != null)
                    {
                        ListItem newItem = new ListItem();
                        newItem.Text = school.getName();
                        newItem.Value = school.getGovIDAsString();
                        drpSchools.Items.Add(newItem);
                    }
                }
            }
        }

        /// <summary>
        /// Selects a school and then loads students into the dropdown lists
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnStep1_Click(object sender, EventArgs e)
        {
            List<Student> schoolStudents = new List<Student>();
            schoolStudents.Clear();

            lstSelectedStudents.Items.Clear();

            drpStudentsByID.Items.Clear();
            drpStudentsByFirstName.Items.Clear();
            drpStudentsByLastName.Items.Clear();

            // Validate the input, just in case
            int schoolID = 0;
            if (int.TryParse(drpSchools.SelectedValue, out schoolID)) 
            {
                using (SqlConnection connection = new SqlConnection(sqlConnectionString))
                {
                    // Load students
                    School selectedSchool = School.loadThisSchool(connection, schoolID);                    
                    if (selectedSchool != null) 
                    {
                        lblSelectedSchool.Text = selectedSchool.getName();
                        lblSelectedSchool2.Text = selectedSchool.getName();

                        schoolStudents = Student.loadStudentsFromThisSchool(connection, selectedSchool.getGovID());
                        foreach (Student student in schoolStudents)
                        {
                            ListItem newStudentItemByFirstName = new ListItem();
                            newStudentItemByFirstName.Text = student.getFirstName() + " " + student.getLastName() + " (" + student.getStudentID() + ")";
                            newStudentItemByFirstName.Value = student.getStudentID();
                            drpStudentsByFirstName.Items.Add(newStudentItemByFirstName);

                            ListItem newStudentItemByLastName = new ListItem();
                            newStudentItemByLastName.Text = student.getLastName() + ", " + student.getFirstName() + " (" + student.getStudentID() + ")"; ;
                            newStudentItemByLastName.Value = student.getStudentID();
                            drpStudentsByLastName.Items.Add(newStudentItemByLastName);

                            ListItem newStudentItemByID = new ListItem();
                            newStudentItemByID.Text = student.getStudentID() + " " + student.getDisplayName();
                            newStudentItemByID.Value = student.getStudentID();
                            drpStudentsByID.Items.Add(newStudentItemByID);

                            tbl_Step1.Visible = false;
                            tbl_Step2.Visible = true;
                        
                        }
                    }               
                }                
            }
        }

        private Student findStudentByID(List<Student> haystack, string id)
        {
            foreach (Student student in haystack)
            {
                if (student.getStudentID() == id)
                {
                    return student;
                }
            }
            return null;
        }

        private void pickStudent(ListItem item) 
        {
            /*
            List<ListItem> currentItems = new List<ListItem>();
            foreach (ListItem i in lstSelectedStudents.Items)
            {
                currentItems.Add(i);
            }
            
            */

            bool alreadyExists = false;
            foreach (ListItem i in lstSelectedStudents.Items)
            {
                if (i.Value == item.Value)
                {
                    alreadyExists = true;
                }
            }

            if (!alreadyExists)
            {
                item.Selected = false;
                lstSelectedStudents.Items.Add(item);
            }            
        }

        protected void btnByFirstName_Click(object sender, EventArgs e)
        {
            pickStudent(drpStudentsByFirstName.SelectedItem);
            
        }
        protected void btnByLastName_Click(object sender, EventArgs e)
        {
            pickStudent(drpStudentsByLastName.SelectedItem);
            //lstSelectedStudents.Items.Add(drpStudentsByLastName.SelectedItem);
        }

        protected void btnByID_Click(object sender, EventArgs e)
        {
            pickStudent(drpStudentsByID.SelectedItem);
            //lstSelectedStudents.Items.Add(drpStudentsByID.SelectedItem);
        }

        protected void btnUnSelectStudents_Click(object sender, EventArgs e)
        {
            List<ListItem> newItems = new List<ListItem>();

            foreach (ListItem item in lstSelectedStudents.Items)
            {
                if (!item.Selected)
                {
                    newItems.Add(item);
                }
            }

            lstSelectedStudents.Items.Clear();

            foreach (ListItem item in newItems)
            {
                lstSelectedStudents.Items.Add(item);
            }
        }        

        protected void btn_BackToStep1_Click(object sender, EventArgs e)
        {
            lstSelectedStudents.Items.Clear();
            tbl_Step1.Visible = true;
            tbl_Step2.Visible = false;
        }

        protected void btn_Step2_Click(object sender, EventArgs e)
        {
            // Only proceed if there are students selected
            if (lstSelectedStudents.Items.Count > 0)
            {
                lblSelectedStudents.Text = "(" + lstSelectedStudents.Items.Count + ") students selected";

                // Get selected school
                int schoolID = 0;
                if (int.TryParse(drpSchools.SelectedValue, out schoolID)) 
                {
                    using (SqlConnection connection = new SqlConnection(sqlConnectionString))
                    {
                        School selectedSchool = School.loadThisSchool(connection, schoolID);
                        List<Track> schoolTracks = Track.loadAllTracksFromThisSchool(connection, selectedSchool);
                        foreach (Track track in schoolTracks)
                        {
                            track.terms = Term.loadTermsFromThisTrack(connection, track);
                            foreach (Term term in track.terms)
                            {
                                term.ReportPeriods = ReportPeriod.loadReportPeriodsFromThisTerm(connection, term);
                            }
                        }


                        drpReportPeriods.Items.Clear();
                        foreach (Track track in schoolTracks)
                        {
                            if (track != null)
                            {
                                foreach (Term term in track.terms)
                                {
                                    if (term != null)
                                    {
                                        foreach (ReportPeriod rp in term.ReportPeriods)
                                        {
                                            if (rp != null)
                                            {
                                                ListItem newItem = new ListItem();
                                                newItem.Value = rp.ID.ToString();
                                                newItem.Text = track.name + " - " + term.name + " - " + rp.name;
                                                drpReportPeriods.Items.Add(newItem);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        tbl_Step2.Visible = false;
                        tbl_Step3.Visible = true;
                    }
                }
            }
        }

        protected void btn_BackToStep2_Click(object sender, EventArgs e)
        {
            // Clear the list of report periods
            lstSelectedReportPeriods.Items.Clear();

            tbl_Step2.Visible = true;
            tbl_Step3.Visible = false;
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
            Response.End();
        }

        protected void btn_Step3_Click(object sender, EventArgs e)
        {
            // Load student data for selected students
            List<Student> final_students = new List<Student>();
            List<ReportPeriod> final_reportPeriods = new List<ReportPeriod>();

            using (SqlConnection connection = new SqlConnection(sqlConnectionString))
            {
                // We need to do a final validation on the selected items, because they come from
                //  web forms that could have been modified in transit.

                foreach (ListItem item in lstSelectedReportPeriods.Items)
                {
                    if (!string.IsNullOrEmpty(item.Value))
                    {
                        int rpID = -1;
                        if (int.TryParse(item.Value, out rpID))
                        {
                            ReportPeriod thisRP = ReportPeriod.loadThisReportPeriod(connection, rpID);
                            if (thisRP != null)
                            {
                                final_reportPeriods.Add(thisRP);
                            }
                        }
                    }
                }

                if (final_reportPeriods.Count > 0)
                {
                    foreach (ListItem item in lstSelectedStudents.Items)
                    {
                        if (!string.IsNullOrEmpty(item.Value))
                        {
                            int studentID = -1;
                            if (int.TryParse(item.Value, out studentID))
                            {
                                Student thisStudent = Student.loadThisStudent(connection, studentID.ToString());
                                if (thisStudent != null)
                                {
                                    //final_students.Add(LSKYCommon.loadStudentMarkData(connection, thisStudent, final_reportPeriods));
                                    final_students.Add(thisStudent);
                                }
                            }
                        }
                    }
                }                
            }

            
            // Generate the PDF (If we loaded the mark data above that is)
            /*
            String fileName = "ReportCards_" + DateTime.Today.Year + "_" + DateTime.Today.Month + "_" + DateTime.Today.Day + ".pdf";
            if ((final_reportPeriods.Count > 0) && (final_students.Count > 0))
            {
                sendPDF(PDFReportCardParts.GeneratePDF(final_students, anonymize), fileName);
            }            
            */
            
            // Generate a list of student IDs
            StringBuilder studentList = new StringBuilder();
            foreach (Student student in final_students) 
            {
                studentList.Append(student.getStudentID());
                studentList.Append(";");
            }

            StringBuilder reportPeriodList = new StringBuilder();
            foreach (ReportPeriod reportPeriod in final_reportPeriods)
            {
                reportPeriodList.Append(reportPeriod.ID);
                reportPeriodList.Append(";");
            }
            Response.Redirect("GetReportCardPDF.aspx?students=" + studentList.ToString() + "&reportperiods=" + reportPeriodList.ToString() + "&debug=true");
        }


        private void pickReportPeriod(ListItem item)
        {           
            bool alreadyExists = false;
            foreach (ListItem i in lstSelectedReportPeriods.Items)
            {
                if (i.Value == item.Value)
                {
                    alreadyExists = true;
                }
            }

            if (!alreadyExists)
            {
                item.Selected = false;
                lstSelectedReportPeriods.Items.Add(item);
            }
        }

        protected void btnAddReportPeriod_Click(object sender, EventArgs e)
        {
            pickReportPeriod(drpReportPeriods.SelectedItem);
            
        }

        protected void btnUnSelectReportPeriod_Click(object sender, EventArgs e)
        {
            List<ListItem> newItems = new List<ListItem>();

            foreach (ListItem item in lstSelectedReportPeriods.Items)
            {
                if (!item.Selected)
                {
                    newItems.Add(item);
                }
            }

            lstSelectedReportPeriods.Items.Clear();
            foreach (ListItem item in newItems)
            {
                lstSelectedReportPeriods.Items.Add(item);
            }
        }
    }
}