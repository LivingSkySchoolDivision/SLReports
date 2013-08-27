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
        String dbConnectionString_Local = ConfigurationManager.ConnectionStrings["DataExplorerDatabase"].ConnectionString;

        private TableRow addNavCategory(string category)
        {
            TableRow newRow = new TableRow();

            TableCell categoryCell = new TableCell();
            categoryCell.Text = "<br/><B>" + category + "</B>";            

            categoryCell.ColumnSpan = 2;

            newRow.Cells.Add(categoryCell);

            return newRow;
        }

        private TableRow addNavItem(NavMenuItem item)
        {
            TableRow newRow = new TableRow();            

            TableCell nameCell = new TableCell();

            string cssClass = "nav_link_normal";

            if (item.admin_only)
            {
                cssClass = "nav_link_admin";
            }

            nameCell.Text = "<a href=\"" + LSKYCommon.translateLocalURL(item.url) + "\" class=\"" + cssClass + "\">" + item.name + "</a>";
            nameCell.CssClass = "navigation_table_name";
            newRow.Cells.Add(nameCell);

            TableCell descriptionCell = new TableCell();
            descriptionCell.Text = item.description;
            descriptionCell.CssClass = "navigation_table_description";
            newRow.Cells.Add(descriptionCell);

            return newRow;            
        }

        private string getSessionIDFromCookies()
        {
            HttpCookie sessionCookie = Request.Cookies["lskyDataExplorer"];
            if (sessionCookie != null)
            {
                return sessionCookie.Value;
            }
            else
            {
                return null;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Load some local session information
            List<session> activeSessions = new List<session>();
            session activeSession = null;
            using (SqlConnection connection = new SqlConnection(dbConnectionString_Local))
            {
                activeSessions = session.loadActiveSessions(connection);
                activeSession = session.loadThisSession(connection, getSessionIDFromCookies(), Request.ServerVariables["REMOTE_ADDR"], Request.ServerVariables["HTTP_USER_AGENT"]); 
            }

            int adminCount = 0;
            foreach (session ses in activeSessions)
            {
                if (ses.is_admin)
                {
                    adminCount++;
                }
            }

            lblActiveSessions.Text = activeSessions.Count.ToString();
            lblAdminSessions.Text = adminCount.ToString();

            // Generate the menu
            List<NavMenuItem> AllMenuItems = Nav.getMainMenu();
            List<NavMenuItem> MainMenu = new List<NavMenuItem>();

            // Figure out which menu items to display
            foreach (NavMenuItem item in AllMenuItems)
            {
                if (!item.hidden)
                {

                    if (activeSession.is_admin)
                    {
                        if (item.name != "-- Front Page --")
                        {
                            MainMenu.Add(item);
                        }
                    }
                    else
                    {
                        if ((!item.admin_only) && (!item.hidden) && (item.name != "-- Front Page --"))
                        {
                            MainMenu.Add(item);
                        }
                    }
                }
            }


            // Get a list of all categories
            List<string> MenuCategories = new List<string>();
            foreach (NavMenuItem item in MainMenu)
            {
                if (!MenuCategories.Contains(item.category))
                {
                    MenuCategories.Add(item.category);
                }                            
            }
            MenuCategories.Sort();

            foreach (string category in MenuCategories)
            {
                tblNavigation.Rows.Add(addNavCategory(category));
                foreach (NavMenuItem item in MainMenu)
                {
                    if (item.category == category)
                    {
                        tblNavigation.Rows.Add(addNavItem(item));
                    }
                }
            }


            // Load data for the statistics box
            List<Student> allStudents = new List<Student>();
            List<School> allSchools = new List<School>();
            List<StaffMember> allStaff = new List<StaffMember>();
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