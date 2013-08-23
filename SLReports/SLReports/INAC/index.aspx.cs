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

        private TableRow createStudentRow(Student student)
        {
            string calculationExplaination = string.Empty;
            float daysAbsent = LSKY_INAC.getDaysAbsent(student, out calculationExplaination);
            
            /* figure out guardian(s) */
            List<Contact> guardiansList = LSKY_INAC.getINACGuardians(student.contacts);
            StringBuilder guardians = new StringBuilder();
            foreach (Contact contact in guardiansList)
            {
                guardians.Append(contact.firstName + " " + contact.lastName + " </i>(" + contact.relation + ")</i><br>");
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

            TableCell daysAbsentCell = new TableCell();
            if (daysAbsent == 0)
            {
                daysAbsentCell.Text = "<i style=\"color: #707070;\">No Absences</i>";
            } else if (daysAbsent >= 0)
            {
                daysAbsentCell.Text = "<abbr title=\"" + calculationExplaination + "\">" + Math.Round(daysAbsent,2) + " days</abbr>";
            }
            daysAbsentCell.VerticalAlign = VerticalAlign.Top;
            newRow.Cells.Add(daysAbsentCell);

            TableCell blocksAbsentcell = new TableCell();
            blocksAbsentcell.Text = student.absences.Count + " blocks";
            blocksAbsentcell.VerticalAlign = VerticalAlign.Top;
            newRow.Cells.Add(blocksAbsentcell);

            TableCell dateRegisterCell = new TableCell();
            dateRegisterCell.Text = student.getEnrollDate().Month + "/" + student.getEnrollDate().Day + "/" + student.getEnrollDate().Year;
            dateRegisterCell.VerticalAlign = VerticalAlign.Top;
            newRow.Cells.Add(dateRegisterCell);
            
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
                            tblResults.Rows.Add(createStudentRow(student));
                        }
                    }
                }
            }            
        }        
    }
}