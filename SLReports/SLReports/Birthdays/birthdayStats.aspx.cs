using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.Birthdays
{
    public partial class birthdayStats : System.Web.UI.Page
    {
        private string getMonthName(int monthNum)
        {
            string[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

            if ((monthNum <= 12) && (monthNum >= 1)) {
                return months[monthNum-1];
            } else {
                return "Invalid month";
            }
                
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            List<Student> allStudents = new List<Student>();
            
            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                allStudents = Student.loadAllStudents(connection);
            }

            float totalStudents = (float)allStudents.Count;
            

            Dictionary<int, int> birthdaysByMonth = new Dictionary<int, int>();

            for (int x = 1; x <= 12; x++)
            {
                birthdaysByMonth.Add(x, 0);
            }

            foreach (Student student in allStudents)
            {
                birthdaysByMonth[student.getDateOfBirth().Month]++;
            }

            for (int x = 1; x <= 12; x++)
            {
                TableRow newRow = new TableRow();

                TableCell monthCell = new TableCell();
                monthCell.Text = getMonthName(x);
                newRow.Cells.Add(monthCell);

                TableCell countCell = new TableCell();
                countCell.Text = birthdaysByMonth[x].ToString();
                newRow.Cells.Add(countCell);

                TableCell percentCell = new TableCell();
                float count = (float)birthdaysByMonth[x];
                float percent = (count / totalStudents) * 100;

                percentCell.Text = (Math.Round(percent) + "%");
                newRow.Cells.Add(percentCell);
                
                tblMonths.Rows.Add(newRow);
            }

        }
    }
}