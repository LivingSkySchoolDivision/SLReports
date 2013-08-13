using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.INAC
{
    public partial class INAC_CSV : System.Web.UI.Page
    {
        //String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
        String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogic2013"].ConnectionString;        

        protected void sendCSV(MemoryStream CSVData, String filename)
        {
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "text/plain";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + filename + ".csv");

            Response.OutputStream.Write(CSVData.GetBuffer(), 0, (int)CSVData.Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();
            Response.End();
        }

        protected MemoryStream GenerateCSV(List<Student> students)
        {
            MemoryStream csvFile = new MemoryStream();
            StreamWriter writer = new StreamWriter(csvFile, Encoding.UTF8);

            // CSV Headings
            StringBuilder headingLine = new StringBuilder();
            headingLine.Append("Grade, StudentName, DateOfBirth, BandAffiliation, StatusNo, ReserveOfResidence, HouseNo, ParentOrGuardian, DaysAbsent, BlocksAbsent, InStatusDate");
            writer.WriteLine(headingLine.ToString());

            // CSV Data
            foreach (Student student in students)
            {
                // Figure out days absent
                string calculationExplaination = string.Empty;
                float daysAbsent = LSKY_INAC.getDaysAbsent(student, out calculationExplaination);

                // Figure out guardians
                List<Contact> guardiansList = LSKY_INAC.getINACGuardians(student.contacts);
                StringBuilder guardians = new StringBuilder();

                for (int x = 0; x < guardiansList.Count; x++)
                {
                    guardians.Append(guardiansList[x].firstName + " " + guardiansList[x].lastName + " (" + guardiansList[x].relation + ")");
                    if (x < guardiansList.Count - 1)
                    {
                        guardians.Append("; ");
                    }
                }

                StringBuilder studentLine = new StringBuilder();
                studentLine.Append(student.getGradeFormatted());
                studentLine.Append(",");

                studentLine.Append(student.getDisplayName());
                studentLine.Append(",");

                studentLine.Append(student.getDateOfBirth().Month + "/" + student.getDateOfBirth().Day + "/" + student.getDateOfBirth().Year);
                studentLine.Append(",");

                studentLine.Append(student.getBandName());
                studentLine.Append(",");

                studentLine.Append(student.getStatusNo());
                studentLine.Append(",");

                studentLine.Append(student.getReserveName());
                studentLine.Append(",");

                studentLine.Append(student.getReserveHouse());
                studentLine.Append(",");

                studentLine.Append(guardians.ToString());
                studentLine.Append(",");

                studentLine.Append(Math.Round(daysAbsent,2).ToString());
                studentLine.Append(",");

                studentLine.Append(student.absences.Count.ToString());
                studentLine.Append(",");

                studentLine.Append(student.getEnrollDate().Month + "/" + student.getEnrollDate().Day + "/" + student.getEnrollDate().Year);
                
                writer.WriteLine(studentLine.ToString());
            }
            writer.Flush();
            csvFile.Flush();
            return csvFile;
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            DateTime startDate = DateTime.Today;
            DateTime endDate = DateTime.Today;

            /* Get data from the query string */
            if (!String.IsNullOrEmpty(Request.QueryString["schoolid"]))
            {
                if ((!String.IsNullOrEmpty(Request.QueryString["from_year"])) || (!String.IsNullOrEmpty(Request.QueryString["from_month"])) || (!String.IsNullOrEmpty(Request.QueryString["from_day"])))
                {
                    if ((!String.IsNullOrEmpty(Request.QueryString["to_year"])) || (!String.IsNullOrEmpty(Request.QueryString["to_month"])) || (!String.IsNullOrEmpty(Request.QueryString["to_day"])))
                    {                        
                        // Parse school
                        int selectedSchoolID = -1;
                        if (int.TryParse(Request.QueryString["schoolid"], out selectedSchoolID))
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

                                // Grab data
                                using (SqlConnection connection = new SqlConnection(dbConnectionString))
                                {
                                    School selectedSchool = School.loadThisSchool(connection, selectedSchoolID);

                                    if (selectedSchool != null)
                                    {
                                        List<Student> DisplayedStudents = LSKY_INAC.loadStudentData(connection, selectedSchool, startDate, endDate);

                                        // Output a CSV file
                                        sendCSV(GenerateCSV(DisplayedStudents), "INAC_" + LSKYCommon.removeSpaces(selectedSchool.getName()) + "_" + startDate.Year + "-" + startDate.Month + "-" + startDate.Day + "_" + endDate.Year + "-" + endDate.Month + "-" + endDate.Day);

                                    }
                                    else
                                    {
                                        Response.Write("Invalid school specified");
                                    }
                                }

                            }
                            else
                            {
                                Response.Write("Invalid date specified<br>");
                                Response.Write(" From Year: " + startYear + "<br>");
                                Response.Write(" From Month: " + startMonth + "<br>");
                                Response.Write(" From Day: " + startDay + "<br>");

                                Response.Write(" To Year: " + endYear + "<br>");
                                Response.Write(" To Month: " + endMonth + "<br>");
                                Response.Write(" To Day: " + endDay + "<br>");
                            }

                        }
                        else
                        {
                            Response.Write("Invalid school ID");
                        }
                        
                    }
                    else
                    {
                        Response.Write("\"To\" data not valid");
                    }
                }
                else
                {
                    Response.Write("\"From\" data not valid");
                }
            } else {
                Response.Write("School ID not specified");
            }

            Response.End();

        }
    }
}