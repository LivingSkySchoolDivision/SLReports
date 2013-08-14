using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports
{
    public partial class index : System.Web.UI.Page
    {

        String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;

        private TableRow addNavItem(NavMenuItem item)
        {
            TableRow newRow = new TableRow();            

            TableCell nameCell = new TableCell();
            nameCell.Text = "<a href=\"/SLReports"+item.url+"\">" + item.name + "</a>";
            nameCell.CssClass = "navigation_table_name";
            newRow.Cells.Add(nameCell);

            TableCell descriptionCell = new TableCell();
            descriptionCell.Text = item.description;
            descriptionCell.CssClass = "navigation_table_description";
            newRow.Cells.Add(descriptionCell);

            return newRow;            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            List<Student> allStudents = new List<Student>();
            List<School> allSchools = new List<School>();
            List<StaffMember> allStaff = new List<StaffMember>();

            List<NavMenuItem> MainMenu = Nav.getMainMenu();
            foreach (NavMenuItem item in MainMenu)
            {
                if ((!item.hidden) && (!item.admin_only))
                {
                    if (item.name != "-- Front Page --")
                    {
                        tblNavigation.Rows.Add(addNavItem(item));
                    }
                }
            }



            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                allStudents = Student.loadAllStudents(connection);
                allSchools = School.loadAllSchools(connection);
                allStaff = StaffMember.loadAllStaff(connection);
            }

            float numStudents = allStudents.Count;
            float numMales = 0;
            float numFemales = 0;
            float numBirthdays = 0;

            List<String> allCities = new List<String>();
            List<String> allRegions = new List<String>();

            foreach (Student student in allStudents)
            {
                if (student.getGender().ToLower() == "male")
                {
                    numMales++;
                }

                if (student.getGender().ToLower() == "female")
                {
                    numFemales++;
                }

                if (
                    (student.getDateOfBirth().Day == DateTime.Now.Day) &&
                    (student.getDateOfBirth().Month == DateTime.Now.Month)
                    )
                {
                    numBirthdays++;
                }

                if (!allCities.Contains(student.getCity()))
                {
                    allCities.Add(student.getCity());
                }

                if (!allRegions.Contains(student.getRegion()))
                {
                    allRegions.Add(student.getRegion());
                }

            }

            lblActiveStudentCount.Text = numStudents.ToString();
            lblSchoolCount.Text = allSchools.Count.ToString();
            lblStaffCount.Text = allStaff.Count.ToString();

            lblMaleCount.Text = numMales.ToString();
            lblFemaleCount.Text = numFemales.ToString();
            lblBirthdayCount.Text = numBirthdays.ToString();


            lblMalePercent.Text = Math.Round((float)((numMales / numStudents) * 100)) + "%";
            lblFemalePercent.Text = Math.Round((float)((numFemales / numStudents) * 100)) + "%";

            lblCities.Text = allCities.Count().ToString();
            lblRegions.Text = allRegions.Count().ToString();
            
        }


    }
}