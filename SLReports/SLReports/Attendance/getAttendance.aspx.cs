using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.Attendance
{
    public partial class getAttendance : System.Web.UI.Page
    {
        private TableRow addAbsenceRow(Absence abs)
        {
            TableRow newRow = new TableRow();

            TableCell dateCell = new TableCell();
            dateCell.Text = abs.getDate().ToLongDateString();
            newRow.Cells.Add(dateCell);

            TableCell blockCell = new TableCell();
            blockCell.Text = abs.getBlock().ToString();
            newRow.Cells.Add(blockCell);

            TableCell statusCell = new TableCell();

            string status = abs.getStatus();
            if (abs.getStatus().ToLower() == "late")
            {
                status = status + " (" + abs.getMinutes() + " min)";
            }

            statusCell.Text = status;
            newRow.Cells.Add(statusCell);

            TableCell reasonCell = new TableCell();
            reasonCell.Text = abs.getReason();
            newRow.Cells.Add(reasonCell);

            TableCell excusedCell = new TableCell();
            excusedCell.Text = LSKYCommon.boolToYesOrNoHTML(abs.excused);
            newRow.Cells.Add(excusedCell);

            TableCell commentCell = new TableCell();
            commentCell.Text = abs.getComment();
            newRow.Cells.Add(commentCell);

            return newRow;
        }

        private void displayError(string thisError)
        {
            lblError.Visible = true;
            lblError.Text = thisError;
            lblError.ForeColor = System.Drawing.Color.Red;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Parse the dates and student number

            DateTime startDate = DateTime.Today;
            DateTime endDate = DateTime.Today;

            /* Get data from the query string */
            if ((!String.IsNullOrEmpty(Request.QueryString["from_year"])) || (!String.IsNullOrEmpty(Request.QueryString["from_month"])) || (!String.IsNullOrEmpty(Request.QueryString["from_day"])))
            {
                if ((!String.IsNullOrEmpty(Request.QueryString["to_year"])) || (!String.IsNullOrEmpty(Request.QueryString["to_month"])) || (!String.IsNullOrEmpty(Request.QueryString["to_day"])))
                {
                    // Parse from date
                    int startYear = -1;
                    int startMonth = -1;
                    int startDay = -1;

                    int.TryParse(Request.QueryString["from_year"], out startYear);
                    int.TryParse(Request.QueryString["from_month"], out startMonth);
                    int.TryParse(Request.QueryString["from_day"], out startDay);

                    // Parse to date
                    int endYear = -1;
                    int endMonth = -1;
                    int endDay = -1;

                    int.TryParse(Request.QueryString["to_year"], out endYear);
                    int.TryParse(Request.QueryString["to_month"], out endMonth);
                    int.TryParse(Request.QueryString["to_day"], out endDay);

                    if (
                        !(
                        (startYear == -1) ||
                        (startMonth == -1) ||
                        (startDay == -1) ||
                        (endYear == -1) ||
                        (endMonth == -1) ||
                        (endDay == -1)
                        )
                        )
                    {
                        if (startDay > DateTime.DaysInMonth(startYear, startMonth))
                            startDay = DateTime.DaysInMonth(startYear, startMonth);

                        if (endDay > DateTime.DaysInMonth(endYear, endMonth))
                            endDay = DateTime.DaysInMonth(endYear, endMonth);

                        startDate = new DateTime(startYear, startMonth, startDay);
                        endDate = new DateTime(endYear, endMonth, endDay);

                        // Parse student id
                        int studentID = -1;
                        if (!String.IsNullOrEmpty(Request.QueryString["studentid"]))
                        {
                            if (int.TryParse(Request.QueryString["studentid"], out studentID))
                            {
                                using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogic))
                                {
                                    Student selectedStudent = Student.loadThisStudent(connection, studentID.ToString());
                                    lblStudentName.Text = selectedStudent.getDisplayName();
                                    lblDates.Text = startDate.ToString() + " to " + endDate.ToString();
                                    if (selectedStudent != null)
                                    {
                                        List<Absence> absences = Absence.loadAbsencesForThisStudentAndTimePeriod(connection, selectedStudent, startDate, endDate);
                                        lblError.Visible = false;
                                        //tblAbsences.Rows.Clear();
                                        tblAbsences.Visible = true;
                                        foreach (Absence abs in absences)
                                        {
                                            tblAbsences.Rows.Add(addAbsenceRow(abs));
                                        }
                                    }
                                    else
                                    {
                                        displayError("Student not found");
                                    }
                                }
                            }
                            else
                            {
                                displayError("Invalid StudentID");
                            }
                        }
                        else
                        {
                            displayError("Invalid StudentID");
                        }

                    }
                    else
                    {
                        displayError("Unable to parse date");
                    }
                }
                else
                {
                    displayError("Unable to parse TO date");
                }


            }
            else
            {
                displayError("Unable to parse FROM date");
            }
        } 






    }
}