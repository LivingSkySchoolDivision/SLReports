using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.Courses
{
    public partial class index : System.Web.UI.Page
    {

        private TableRow addCourseTableRow(Course course)
        {
            TableRow newRow = new TableRow();
            newRow.CssClass = "datatable_row";

            TableCell newCell = null;

            newCell = new TableCell();
            newCell.Text = course.name;
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = course.courseCode;
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = course.governmentCode;
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = course.governmentCourseID.ToString();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = course.id.ToString();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = course.school;
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = LSKYCommon.boolToYesOrNo(course.offeredInSchool);
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = LSKYCommon.boolToYesOrNo(course.schoolExam);
            newRow.Cells.Add(newCell);

            return newRow;

        }

        private TableRow addCourseTableHeaders()
        {
            TableRow newRow = new TableRow();
            newRow.CssClass = "datatable_header";

            TableCell newCell = null;

            newCell = new TableCell();
            newCell.Text = "Course Name";
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = "Course Code";
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = "Government Course Code";
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = "Government Course ID";
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = "Database ID";
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = "School (if applicable)";
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = "Offered in school";
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = "School exam";
            newRow.Cells.Add(newCell);

            return newRow;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            /* Load all courses */
            List<Course> allCourses = new List<Course>();

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                allCourses = Course.loadAllCourses(connection);
            }

            /* Populate table */
            tblCourses.Rows.Clear();
            tblCourses.Rows.Add(addCourseTableHeaders());

            foreach (Course course in allCourses)
            {
                tblCourses.Rows.Add(addCourseTableRow(course));
            }

            lblCourseCount.Text = " ("+allCourses.Count+")";

        }
    }
}