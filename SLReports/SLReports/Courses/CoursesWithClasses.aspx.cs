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
    public partial class CoursesWithClasses : System.Web.UI.Page
    {
        
        private TableRow addCourseTableRow(Course course)
        {
            TableRow newRow = new TableRow();
            newRow.CssClass = "datatable_header";

            TableCell newCell = null;

            newCell = new TableCell();
            newCell.Text = "<B>" + course.name + "</b> (Code: " + course.courseCode + ", ID: " + course.id + ")";
            newCell.ColumnSpan = 7;
            newRow.Cells.Add(newCell);

            return newRow;
        }

        private TableRow addClassTableHeaders()
        {
            TableRow newRow = new TableRow();
            newRow.CssClass = "datatable_header";

            TableCell newCell = null;

            // An empty cell for formatting
            newCell = new TableCell();
            newCell.Text = string.Empty;
            newCell.Width = 100;
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = "Class Name";
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = "Class Database ID";
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = "School";
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = "Teacher";
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = "Enrolled";
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = "Track";
            newRow.Cells.Add(newCell);

            return newRow;
        }

        private TableRow addEmptyRow()
        {
            TableRow newRow = new TableRow();

            TableCell newCell = new TableCell();
            newCell.Text = "&nbsp;";
            newCell.ColumnSpan = 7;
            newRow.Cells.Add(newCell);
            return newRow;

        }

        private TableRow addNoClassesRow()
        {
            TableRow newRow = new TableRow();

            TableCell newCell = new TableCell();
            newCell.Text = "<i>No classes</i>";
            newCell.ColumnSpan = 7;
            newRow.Cells.Add(newCell);
            return newRow;

        }

        private TableRow addClassTableRow(SchoolClass thisClass)
        {
            TableRow newRow = new TableRow();
            newRow.CssClass = "datatable_row";

            TableCell newCell = null;

            // An empty cell for formatting
            newCell = new TableCell();
            newCell.Text = string.Empty;
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = thisClass.name;
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = thisClass.classid.ToString();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = thisClass.schoolName;
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = thisClass.teacherName;
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = thisClass.enrollmentCount.ToString();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = thisClass.track.ToString();
            newRow.Cells.Add(newCell);

            return newRow;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            /* Load all courses */
            List<Course> allCourses = new List<Course>();
            List<SchoolClass> allclasses = new List<SchoolClass>();
            List<StudentEnrollmentEntry> allEnrollment = new List<StudentEnrollmentEntry>();

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                allCourses = Course.loadAllCourses(connection);
                allclasses = SchoolClass.loadAllClasses(connection);
                allEnrollment = StudentEnrollmentEntry.loadAllStudentEnrollment(connection);
            }

            /* Populate table */
            tblCourses.Rows.Clear();

            foreach (Course course in allCourses)
            {
                tblCourses.Rows.Add(addCourseTableRow(course));

                tblCourses.Rows.Add(addClassTableHeaders());
                int classCount = 0;
                foreach (SchoolClass thisClass in allclasses)
                {
                    if (thisClass.courseid == course.id)
                    {
                        /* Figure out student enrollment */
                        foreach (StudentEnrollmentEntry studentEE in allEnrollment)
                        {
                            if (studentEE.classID == thisClass.classid)
                            {
                                thisClass.enrollmentCount++;
                            }
                        }

                        tblCourses.Rows.Add(addClassTableRow(thisClass));
                        classCount++;
                    }    
                }
                if (classCount == 0)
                {
                    tblCourses.Rows.Add(addNoClassesRow());
                }
                tblCourses.Rows.Add(addEmptyRow());
            }

            lblClassCount.Text = allclasses.Count.ToString();
            lblCourseCount.Text = allCourses.Count.ToString();

        }
    }
}