using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.GradeBook
{
    public partial class StudentIETasks : System.Web.UI.Page
    {
        private TableRow addTableHeaders()
        {
            TableRow returnMe = new TableRow();

            /*
             * - Class
             * - Student Name
             * - Student ID
             * - Task name
             */

            TableCell classCell = new TableCell();
            classCell.Text = "Class";
            classCell.CssClass = "datatable_header";
            returnMe.Cells.Add(classCell);

            TableCell studentNameCell = new TableCell();
            studentNameCell.Text = "Student Name";
            studentNameCell.CssClass = "datatable_header";
            returnMe.Cells.Add(studentNameCell);

            TableCell studentIDCell = new TableCell();
            studentIDCell.Text = "Student ID";
            studentIDCell.CssClass = "datatable_header";
            returnMe.Cells.Add(studentIDCell);

            TableCell taskNameCell = new TableCell();
            taskNameCell.Text = "Task Name";
            taskNameCell.CssClass = "datatable_header";
            returnMe.Cells.Add(taskNameCell);
            
            return returnMe;
        }

        private TableRow addTaskMarkRow(StudentTaskMark mark, List<Student> students)
        {
            // firstly, find the student that belongs to this ID number
            Student selectedStudent = null;
            foreach (Student student in students)
            {
                if (student.getStudentID() == mark.studentID)
                {
                    selectedStudent = student;
                    break;
                }
            }


            TableRow returnMe = new TableRow();
            
            TableCell classCell = new TableCell();
            classCell.Text = mark.task.className;
            returnMe.Cells.Add(classCell);

            string studentName = "<i>Unknown Student</i>";
            if (selectedStudent != null) 
            {
                studentName = selectedStudent.getDisplayName();
            }

            TableCell studentNameCell = new TableCell();
            studentNameCell.Text = studentName;
            returnMe.Cells.Add(studentNameCell);
            
            TableCell studentIDCell = new TableCell();
            studentIDCell.Text = mark.studentID;
            returnMe.Cells.Add(studentIDCell);

            TableCell taskNameCell = new TableCell();
            taskNameCell.Text = mark.task.name;
            returnMe.Cells.Add(taskNameCell);
            
            return returnMe;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<School> allSchools = new List<School>();
                using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogicTest))
                {
                    allSchools = School.loadAllSchools(connection);
                }

                foreach (School school in allSchools)
                {
                    ListItem newListItem = new ListItem();
                    newListItem.Text = school.getName();
                    newListItem.Value = school.getGovIDAsString();
                    drpSchools.Items.Add(newListItem);
                }
            }
        }

        protected void btnSchool_Click(object sender, EventArgs e)
        {            
            List<StudentTaskMark> allTaskMarks = new List<StudentTaskMark>();
            List<Student> allStudents = new List<Student>();
            using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogicTest))
            {
                School selectedSchool = School.loadThisSchool(connection, drpSchools.SelectedValue);

                if (selectedSchool != null)
                {
                    allTaskMarks = StudentTaskMark.loadTaskMarksFromThisSchool(connection, selectedSchool);
                    allStudents = Student.loadStudentsFromThisSchool(connection, selectedSchool.getGovID());
                }
            }

            // Get the marks that are IE
            List<StudentTaskMark> IEMarks = new List<StudentTaskMark>();
            foreach (StudentTaskMark mark in allTaskMarks)
            {
                if ((mark.mark == 0) && mark.task.gradeType == StudentTask.GradeType.Alpha)
                {
                    IEMarks.Add(mark);
                }
            }

            // Sort the marks by class? or by student?
            /*
            IEMarks.Sort(
                delegate(StudentTaskMark first,
                StudentTaskMark next)
                {
                    return first.task.className.CompareTo(next.task.className);
                }
                );
            */
            tblStudents.Rows.Clear();
            tblStudents.Rows.Add(addTableHeaders());
            foreach (StudentTaskMark mark in IEMarks)
            {
                tblStudents.Rows.Add(addTaskMarkRow(mark, allStudents));
            }
            
        }
    }
}