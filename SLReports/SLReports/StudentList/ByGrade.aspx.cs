using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.StudentList
{
    public partial class ByGrade : System.Web.UI.Page
    {
        private TableRow addHeaderRow(string title)
        {
            TableRow newRow = new TableRow();

            TableCell headingCell = new TableCell();
            headingCell.CssClass = "datatable_header";
            headingCell.ColumnSpan = 9;
            headingCell.Text = "<a name=\"anchor_" + title + "\"></a> Grade " + title;

            newRow.Cells.Add(headingCell);

            return newRow;
        }

        private TableRow addStudentRow(Student student)
        {
            TableRow newRow = new TableRow();
            newRow.CssClass = "row";

            TableCell cell_ID = new TableCell();
            cell_ID.Text = student.getStudentID();
            newRow.Cells.Add(cell_ID);

            TableCell cell_GivenName = new TableCell();
            cell_GivenName.Text = student.getGivenName();
            newRow.Cells.Add(cell_GivenName);

            TableCell cell_MiddleName = new TableCell();
            cell_MiddleName.Text = student.getMiddleName();
            newRow.Cells.Add(cell_MiddleName);

            TableCell cell_Surname = new TableCell();
            cell_Surname.Text = student.getSN();
            newRow.Cells.Add(cell_Surname);

            TableCell cell_GovID = new TableCell();
            cell_GovID.Text = student.getGovernmentID();
            newRow.Cells.Add(cell_GovID);

            TableCell cell_Gender = new TableCell();
            cell_Gender.Text = student.getGender();
            newRow.Cells.Add(cell_Gender);

            TableCell cell_HomeRoom = new TableCell();
            cell_HomeRoom.Text = student.getHomeRoom();
            newRow.Cells.Add(cell_HomeRoom);

            TableCell cell_UserName = new TableCell();
            cell_UserName.Text = student.LDAPUserName;
            newRow.Cells.Add(cell_UserName);

            TableCell cell_DOB = new TableCell();
            cell_DOB.Text = student.getDateOfBirth().ToShortDateString();
            newRow.Cells.Add(cell_DOB);

            return newRow;

        }

        private TableRow addStudentHeadingsRow()
        {
            TableRow newRow = new TableRow();

            TableCell cell_ID = new TableCell();
            cell_ID.CssClass = "datatable_header";
            cell_ID.Text = "Student ID";
            newRow.Cells.Add(cell_ID);

            TableCell cell_GivenName = new TableCell();
            cell_GivenName.CssClass = "datatable_header";
            cell_GivenName.Text = "Given Name";
            newRow.Cells.Add(cell_GivenName);

            TableCell cell_MiddleName = new TableCell();
            cell_MiddleName.CssClass = "datatable_header";
            cell_MiddleName.Text = "Middle Name";
            newRow.Cells.Add(cell_MiddleName);

            TableCell cell_Surname = new TableCell();
            cell_Surname.CssClass = "datatable_header";
            cell_Surname.Text = "Surname";
            newRow.Cells.Add(cell_Surname);

            TableCell cell_GovID = new TableCell();
            cell_GovID.CssClass = "datatable_header";
            cell_GovID.Text = "Government ID";
            newRow.Cells.Add(cell_GovID);

            TableCell cell_Gender = new TableCell();
            cell_Gender.CssClass = "datatable_header";
            cell_Gender.Text = "Gender";
            newRow.Cells.Add(cell_Gender);

            TableCell cell_HomeRoom = new TableCell();
            cell_HomeRoom.CssClass = "datatable_header";
            cell_HomeRoom.Text = "Home Room";
            newRow.Cells.Add(cell_HomeRoom);

            TableCell cell_UserName = new TableCell();
            cell_UserName.CssClass = "datatable_header";
            cell_UserName.Text = "LDAP Username";
            newRow.Cells.Add(cell_UserName);

            TableCell cell_DOB = new TableCell();
            cell_DOB.CssClass = "datatable_header";
            cell_DOB.Text = "Date Of Birth";
            newRow.Cells.Add(cell_DOB);

            return newRow;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<School> allSchools = new List<School>();
                using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogic))
                {
                    allSchools = School.loadAllSchools(connection);
                }

                drpSchools.Items.Clear();
                foreach (School school in allSchools)
                {
                    ListItem newItem = new ListItem();
                    newItem.Text = school.getName();
                    newItem.Value = school.getGovID().ToString();
                    drpSchools.Items.Add(newItem);
                }
            }

        }

        protected void btnChooseSchool_Click(object sender, EventArgs e)
        {
            // Parse selected school
            int schoolID = -1;
            if (int.TryParse(drpSchools.SelectedValue, out schoolID))
            {
                if (schoolID > 0)
                {
                    List<Student> allStudents = new List<Student>();
                    using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogic))
                    {
                        allStudents = Student.loadStudentsFromThisSchool(connection, schoolID);
                    }

                    allStudents.Sort();

                    // Sort by grade
                    List<String> allGrades = new List<String>();
                    foreach (Student student in allStudents)
                    {
                        if (!allGrades.Contains(student.getGrade()))
                        {
                            allGrades.Add(student.getGrade());
                        }
                    }
                    allGrades.Sort();

                    // Display anchors
                    StringBuilder anchors = new StringBuilder();
                    foreach (string grade in allGrades)
                    {
                        anchors.Append("<a href=\"#anchor_"+grade+"\">"+grade+"</a> ");
                    }
                    lblAnchors.Text = anchors.ToString();


                    // Display students
                    tblStudents.Rows.Clear();
                    foreach (string grade in allGrades)
                    {
                        tblStudents.Rows.Add(addHeaderRow(grade));
                        tblStudents.Rows.Add(addStudentHeadingsRow());

                        foreach (Student student in allStudents)
                        {
                            if (student.getGrade() == grade)
                            {
                                tblStudents.Rows.Add(addStudentRow(student));
                            }
                        }
                    }
                    tblStudents.Visible = true;
                }
            }            
        }
    }
}