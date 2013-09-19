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
        String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;

        public static List<School> AllSchools;

        private static School selectedSchool = null;             

        private TableRow createStudentRow(Student student, DateTime startDate, DateTime endDate)
        {
            // Split absences into explained / unexplained
            List<Absence> explainedAbsences = new List<Absence>();
            List<Absence> unExplainedAbsences = new List<Absence>();
            foreach (Absence abs in student.absences)
            {
                if (abs.excused)
                {
                    explainedAbsences.Add(abs);
                }
                else
                {
                    unExplainedAbsences.Add(abs);
                }
            }

            string calculationExplaination_Total = string.Empty;
            string calculationExplaination_Explained = string.Empty;
            string calculationExplaination_Unexplained = string.Empty;
            float daysAbsent_Total = LSKY_INAC.getDaysAbsent(student, out calculationExplaination_Total);
            float daysAbsent_Explained = LSKY_INAC.getDaysAbsent_Explained(student, out calculationExplaination_Explained);
            float daysAbsent_Unexplained = LSKY_INAC.getDaysAbsent_Unexplained(student, out calculationExplaination_Unexplained);
            
            /* figure out guardian(s) */
            List<Contact> guardiansList = LSKY_INAC.getINACGuardians(student.contacts);
            StringBuilder guardians = new StringBuilder();
            foreach (Contact contact in guardiansList)
            {
                guardians.Append(contact.firstName + " " + contact.lastName + " </i>(" + contact.relation + ")</i><br>");
            } 

            
            TableRow newRow = new TableRow();
            newRow.CssClass = "datatable_row";

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
            birthdayCell.HorizontalAlign = HorizontalAlign.Right;
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

            
            // Unexplained absences
            TableCell daysAbsent_UnexplainedCell = new TableCell();
            if (daysAbsent_Unexplained == 0)
            {
                daysAbsent_UnexplainedCell.Text = "<i style=\"color: #707070;\">No Absences</i>";
            }
            else if (daysAbsent_Unexplained >= 0)
            {
                daysAbsent_UnexplainedCell.Text = "<abbr title=\"" + calculationExplaination_Unexplained + "\">" + Math.Round(daysAbsent_Unexplained, 2) + " days</abbr>";
            }
            daysAbsent_UnexplainedCell.VerticalAlign = VerticalAlign.Top;
            daysAbsent_UnexplainedCell.BorderColor = System.Drawing.ColorTranslator.FromHtml("#E8ADAA");
            newRow.Cells.Add(daysAbsent_UnexplainedCell);

            TableCell blocksAbsent_UnexplainedCell = new TableCell();
            blocksAbsent_UnexplainedCell.Text = unExplainedAbsences.Count + " blocks";
            blocksAbsent_UnexplainedCell.VerticalAlign = VerticalAlign.Top;
            blocksAbsent_UnexplainedCell.BorderColor = System.Drawing.ColorTranslator.FromHtml("#E8ADAA");
            newRow.Cells.Add(blocksAbsent_UnexplainedCell);

            // Explained absences
            /*
            TableCell daysAbsent_ExplainedCell = new TableCell();
            if (daysAbsent_Explained == 0)
            {
                daysAbsent_ExplainedCell.Text = "<i style=\"color: #707070;\">No Absences</i>";
            }
            else if (daysAbsent_Explained >= 0)
            {
                daysAbsent_ExplainedCell.Text = "<abbr title=\"" + calculationExplaination_Explained + "\">" + Math.Round(daysAbsent_Explained, 2) + " days</abbr>";
            }
            daysAbsent_ExplainedCell.VerticalAlign = VerticalAlign.Top;
            daysAbsent_ExplainedCell.BorderColor = System.Drawing.ColorTranslator.FromHtml("#C3FDB8");
            newRow.Cells.Add(daysAbsent_ExplainedCell);

            TableCell blocksAbsent_ExplainedCell = new TableCell();
            blocksAbsent_ExplainedCell.Text = explainedAbsences.Count + " blocks";
            blocksAbsent_ExplainedCell.VerticalAlign = VerticalAlign.Top;
            blocksAbsent_ExplainedCell.BorderColor = System.Drawing.ColorTranslator.FromHtml("#C3FDB8");
            newRow.Cells.Add(blocksAbsent_ExplainedCell);
            */


            // total absences
            TableCell daysAbsent_TotalCell = new TableCell();
            if (daysAbsent_Total == 0)
            {
                daysAbsent_TotalCell.Text = "<i style=\"color: #707070;\">No Absences</i>";
            }
            else if (daysAbsent_Total >= 0)
            {
                daysAbsent_TotalCell.Text = "<abbr title=\"" + calculationExplaination_Total + "\">" + Math.Round(daysAbsent_Total, 2) + " days</abbr>";
            }
            daysAbsent_TotalCell.VerticalAlign = VerticalAlign.Top;
            newRow.Cells.Add(daysAbsent_TotalCell);

            TableCell blocksAbsent_TotalCell = new TableCell();
            blocksAbsent_TotalCell.Text = student.absences.Count + " blocks";
            blocksAbsent_TotalCell.VerticalAlign = VerticalAlign.Top;
            newRow.Cells.Add(blocksAbsent_TotalCell);

            TableCell dateRegisterCell = new TableCell();
            dateRegisterCell.Text = student.getEnrollDate().Month + "/" + student.getEnrollDate().Day + "/" + student.getEnrollDate().Year;
            dateRegisterCell.VerticalAlign = VerticalAlign.Top;
            newRow.Cells.Add(dateRegisterCell);

            TableCell absenceLinkCell = new TableCell();
            absenceLinkCell.Text = "<a href=\"../Attendance/index.aspx?from_year=" + startDate.Year + "&from_month=" + startDate.Month + "&from_day=" + startDate.Day + "&to_year=" + endDate.Year + "&to_month=" + endDate.Month + "&to_day=" + endDate.Day + "&studentid=" + student.getStudentID() + "\" TARGET=\"_blank\">View absences</a>";
            absenceLinkCell.VerticalAlign = VerticalAlign.Top;
            newRow.Cells.Add(absenceLinkCell);
            
            return newRow;

        }
               
        protected void Page_Load(object sender, EventArgs e)
        {
            DateTime startDate = DateTime.Today;
            DateTime endDate = DateTime.Today;

            if (!IsPostBack)
            {                
                AllSchools = new List<School>();
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
                        if (x == (DateTime.Now.Month))
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
                ListItem firstDay_From = new ListItem("First Day", "1");
                if (!IsPostBack)
                {
                    firstDay_From.Selected = true;
                }
                ListItem firstDay_To = new ListItem("First Day", "1");
                from_day.Items.Add(firstDay_From);
                to_day.Items.Add(firstDay_To);                

                for (int x = 1; x <= 31; x++)
                {
                    ListItem newLI_From = new ListItem(x.ToString(), x.ToString());
                    ListItem newLI_To = new ListItem(x.ToString(), x.ToString());
                    
                    //if (!IsPostBack)
                    //{
                    //    if (x == (DateTime.Now.Day))
                    //        newLI_From.Selected = true;
                    //}

                    //if (!IsPostBack)
                    //{
                    //    if (x == DateTime.Now.Day)
                    //        newLI_To.Selected = true;
                    //}

                    from_day.Items.Add(newLI_From);
                    to_day.Items.Add(newLI_To);
                }

                ListItem lastDay_From = new ListItem("Last Day", "31");
                ListItem lastDay_To = new ListItem("Last Day", "31");
                if (!IsPostBack)
                {
                    lastDay_To.Selected = true;
                }
                from_day.Items.Add(lastDay_From);
                to_day.Items.Add(lastDay_To);

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
                    newLI.Value = school.getGovIDAsString();
                    lstSchoolList.Items.Add(newLI);
                }
                #endregion

            }
            else
            {
                #region Parse the selected school
                foreach (School school in AllSchools)
                {
                    if (lstSchoolList.SelectedItem.Value == school.getGovIDAsString())
                    {
                        selectedSchool = school;
                    }
                }
                #endregion

                #region Parse the given date
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
                #endregion
            }



            using (SqlConnection connection = new SqlConnection(dbConnectionString)) 
            {
                AllSchools = School.loadAllSchools(connection);

                if (IsPostBack)
                {
                    if (selectedSchool != null)
                    {
                        List<Student> DisplayedStudents = LSKY_INAC.loadStudentData(connection, selectedSchool, startDate, endDate);

                        lblCount.Text = "Found " + DisplayedStudents.Count + " students.";
                        if (DisplayedStudents.Count > 0)
                        {
                            tblResults.Visible = true;

                            lnkCSVDownload.Visible = true;
                            lnkCSVDownload.NavigateUrl = "INAC_CSV.aspx?schoolid=" + selectedSchool.getGovIDAsString() + "&from_year=" + startDate.Year + "&from_month=" + startDate.Month + "&from_day=" + startDate.Day + "&to_year=" + endDate.Year + "&to_month=" + endDate.Month + "&to_day=" + endDate.Day;
                        }
                        
                        foreach (Student student in DisplayedStudents)
                        {
                            tblResults.Rows.Add(createStudentRow(student, startDate, endDate));
                        }
                    }
                }
            }            
        }        
    }
}