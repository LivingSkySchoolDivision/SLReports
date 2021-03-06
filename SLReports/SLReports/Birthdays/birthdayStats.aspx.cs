﻿using System;
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

        private string getDayName(int dayNum)
        {
            string[] days = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

            if ((dayNum <= 6) && (dayNum >= 0))
            {
                return days[dayNum];
            }
            else
            {
                return "Invalid day";
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
            
            /* Birthdays by month */
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

            /* Birthdays by day of the week */
            SortedDictionary<int, int> birthdaysByDayOfWeek = new SortedDictionary<int, int>();
            foreach (Student student in allStudents)
            {
                if (!birthdaysByDayOfWeek.ContainsKey((int)student.getDateOfBirth().DayOfWeek))
                {
                    birthdaysByDayOfWeek.Add((int)student.getDateOfBirth().DayOfWeek, 0);
                }
                birthdaysByDayOfWeek[(int)student.getDateOfBirth().DayOfWeek]++;
            }


            for (int x = 0; x < 7; x++ )
            {
                TableRow newRow = new TableRow();

                TableCell monthCell = new TableCell();
                monthCell.Text = getDayName(x);
                newRow.Cells.Add(monthCell);

                TableCell countCell = new TableCell();
                countCell.Text = birthdaysByDayOfWeek[x].ToString();
                newRow.Cells.Add(countCell);

                TableCell percentCell = new TableCell();
                float count = (float)birthdaysByDayOfWeek[x];
                float percent = (count / totalStudents) * 100;

                percentCell.Text = (Math.Round(percent) + "%");
                newRow.Cells.Add(percentCell);

                tblDays.Rows.Add(newRow);
            }
                     


        }
    }
}