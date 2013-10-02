using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.DivisionStats
{
    public partial class index : System.Web.UI.Page
    {            
        TableRow addTableHeader_1(List<string> grades)
        {
            TableHeaderRow newRow = new TableHeaderRow();

            TableHeaderCell titleCell_School = new TableHeaderCell();
            titleCell_School.RowSpan = 2;
            titleCell_School.CssClass = "datatable_header";
            titleCell_School.Text = "School";
            newRow.Cells.Add(titleCell_School);

            TableHeaderCell titleCell_Students = new TableHeaderCell();
            titleCell_Students.ColumnSpan = 3;
            titleCell_Students.CssClass = "datatable_header";
            titleCell_Students.Text = "Students";
            newRow.Cells.Add(titleCell_Students);

            TableHeaderCell titleCell_Grades = new TableHeaderCell();
            titleCell_Grades.ColumnSpan = grades.Count;
            titleCell_Grades.CssClass = "datatable_header";
            titleCell_Grades.Text = "Grades";
            newRow.Cells.Add(titleCell_Grades);

            return newRow;
        }

        TableRow addTableHeader_2(List<string> grades)
        {
            TableHeaderRow newRow = new TableHeaderRow();

            TableHeaderCell titleCell_Total = new TableHeaderCell();
            titleCell_Total.CssClass = "datatable_header";
            titleCell_Total.Text = "Total";
            newRow.Cells.Add(titleCell_Total);

            TableHeaderCell titleCell_Male = new TableHeaderCell();
            titleCell_Male.CssClass = "datatable_header";
            titleCell_Male.Text = "Male";
            newRow.Cells.Add(titleCell_Male);

            TableHeaderCell titleCell_Female = new TableHeaderCell();
            titleCell_Female.CssClass = "datatable_header";
            titleCell_Female.Text = "Female";
            newRow.Cells.Add(titleCell_Female);

            if (grades.Contains("PK"))
            {
                TableHeaderCell titleCell_Grades = new TableHeaderCell();
                titleCell_Grades.CssClass = "datatable_header";
                titleCell_Grades.Text = "PK";
                newRow.Cells.Add(titleCell_Grades);
            }

            if (grades.Contains("0K"))
            {
                TableHeaderCell titleCell_Grades = new TableHeaderCell();
                titleCell_Grades.CssClass = "datatable_header";
                titleCell_Grades.Text = "K";
                newRow.Cells.Add(titleCell_Grades);
            }
            
            foreach (string grade in grades)
            {
                if ((grade.ToLower() != "0k") && (grade.ToLower() != "k") && (grade.ToLower() != "pk"))
                {                    
                    TableHeaderCell titleCell_Grades = new TableHeaderCell();
                    titleCell_Grades.CssClass = "datatable_header";
                    
                    int gradeNum = 0;
                    if (int.TryParse(grade, out gradeNum))
                    {
                        titleCell_Grades.Text = gradeNum.ToString();
                    }
                    else
                    {
                        titleCell_Grades.Text = grade;
                    }
                    
                    
                    newRow.Cells.Add(titleCell_Grades);
                }
            }

            return newRow;
        }

        TableRow addSchoolRow(string school, List<string> grades, List<Student> students)
        {
            // Figure out grade numbers
            // and while you're at it, figure out male and female counts
            Dictionary<string, int> gradesWithCounts = new Dictionary<string, int>();
            int males = 0;
            int females = 0;

            foreach (string grade in grades)
            {
                if (!gradesWithCounts.ContainsKey(grade))
                {
                    gradesWithCounts.Add(grade, 0);
                }
            }

            foreach (Student student in students)
            {
                if (student.getGender().ToLower() == "male")
                {
                    males++;
                }
                else
                {
                    females++;
                }

                if (!gradesWithCounts.ContainsKey(student.getGrade()))
                {
                    gradesWithCounts.Add(student.getGrade(), 0);
                }

                gradesWithCounts[student.getGrade()]++;
            }
            
            TableRow newRow = new TableRow();
            newRow.CssClass = "datatable_row";

            TableCell cell_schoolName = new TableCell();
            cell_schoolName.CssClass = "datatable_row";
            cell_schoolName.Text = school;
            newRow.Cells.Add(cell_schoolName);

            TableCell cell_Total = new TableCell();
            cell_Total.CssClass = "datatable_row";
            cell_Total.Text = "<b>" + students.Count.ToString() + "</b>";
            newRow.Cells.Add(cell_Total);

            TableCell cell_Male = new TableCell();
            cell_Male.CssClass = "datatable_row";
            cell_Male.BackColor = System.Drawing.ColorTranslator.FromHtml("#F0F0F0");
            cell_Male.Text = males.ToString();
            newRow.Cells.Add(cell_Male);

            TableCell cell_Female = new TableCell();
            cell_Female.CssClass = "datatable_row";
            cell_Female.BackColor = System.Drawing.ColorTranslator.FromHtml("#F0F0F0");
            cell_Female.Text = females.ToString();
            newRow.Cells.Add(cell_Female);


            // If grade "PK" exists, display it before the numerical grades
            if (grades.Contains("PK"))
            {
                TableCell titleCell_Grades = new TableCell();
                titleCell_Grades.CssClass = "datatable_row";
                if (gradesWithCounts["PK"] > 0) 
                {
                    titleCell_Grades.Text = gradesWithCounts["PK"].ToString();
                }
                newRow.Cells.Add(titleCell_Grades);
            }

            // If grade "K" exists, display it before the numerical grades
            if (grades.Contains("0K"))
            {
                TableCell titleCell_Grades = new TableCell();
                titleCell_Grades.CssClass = "datatable_row";
                if (gradesWithCounts["0K"] > 0)
                {
                    titleCell_Grades.Text = gradesWithCounts["0K"].ToString();
                }
                newRow.Cells.Add(titleCell_Grades);
            }
            // Display numerical grades (but not PK or K)
            foreach (string grade in grades)
            {
                if ((grade.ToLower() != "0k") && (grade.ToLower() != "k") && (grade.ToLower() != "pk"))
                {
                    TableCell cell_Grades = new TableCell();
                    cell_Grades.CssClass = "datatable_row";
                    if (gradesWithCounts[grade] > 0)
                    {
                        cell_Grades.Text = gradesWithCounts[grade].ToString();
                    }
                    newRow.Cells.Add(cell_Grades);
                }
            }

            return newRow;
        }

        TableRow addTotalRow(List<string> grades, List<Student> students)
        {
            // Figure out grade numbers
            // and while you're at it, figure out male and female counts
            Dictionary<string, int> gradesWithCounts = new Dictionary<string, int>();
            int males = 0;
            int females = 0;

            foreach (string grade in grades)
            {
                if (!gradesWithCounts.ContainsKey(grade))
                {
                    gradesWithCounts.Add(grade, 0);
                }
            }

            foreach (Student student in students)
            {
                if (student.getGender().ToLower() == "male")
                {
                    males++;
                }
                else
                {
                    females++;
                }

                if (!gradesWithCounts.ContainsKey(student.getGrade()))
                {
                    gradesWithCounts.Add(student.getGrade(), 0);
                }

                gradesWithCounts[student.getGrade()]++;
            }

            TableRow newRow = new TableRow();

            TableCell cell_schoolName = new TableCell();
            cell_schoolName.CssClass = "datatable_row";
            cell_schoolName.Text = "<b>Total</b>";
            newRow.Cells.Add(cell_schoolName);

            TableCell cell_Total = new TableCell();
            cell_Total.CssClass = "datatable_row";
            cell_Total.Text = "<B>" + students.Count.ToString() + "</B>";
            newRow.Cells.Add(cell_Total);

            TableCell cell_Male = new TableCell();
            cell_Male.CssClass = "datatable_row";
            cell_Male.BackColor = System.Drawing.ColorTranslator.FromHtml("#F0F0F0");
            cell_Male.Text = "<B>" + males.ToString() + "</B>";
            newRow.Cells.Add(cell_Male);

            TableCell cell_Female = new TableCell();
            cell_Female.CssClass = "datatable_row";
            cell_Female.BackColor = System.Drawing.ColorTranslator.FromHtml("#F0F0F0");
            cell_Female.Text = "<B>" + females.ToString() + "</B>";
            newRow.Cells.Add(cell_Female);


            // If grade "PK" exists, display it before the numerical grades
            if (grades.Contains("PK"))
            {
                TableCell titleCell_Grades = new TableCell();
                titleCell_Grades.CssClass = "datatable_row";
                if (gradesWithCounts["PK"] > 0)
                {
                    titleCell_Grades.Text = "<B>" + gradesWithCounts["PK"].ToString() + "</B>";
                }
                newRow.Cells.Add(titleCell_Grades);
            }

            // If grade "K" exists, display it before the numerical grades
            if (grades.Contains("0K"))
            {
                TableCell titleCell_Grades = new TableCell();
                titleCell_Grades.CssClass = "datatable_row";
                if (gradesWithCounts["0K"] > 0)
                {
                    titleCell_Grades.Text = "<B>" + gradesWithCounts["0K"].ToString() + "</B>";
                }
                newRow.Cells.Add(titleCell_Grades);
            }
            // Display numerical grades (but not PK or K)
            foreach (string grade in grades)
            {
                if ((grade.ToLower() != "0k") && (grade.ToLower() != "k") && (grade.ToLower() != "pk"))
                {
                    TableCell cell_Grades = new TableCell();
                    cell_Grades.CssClass = "datatable_row";
                    if (gradesWithCounts[grade] > 0)
                    {
                        cell_Grades.Text = "<B>" + gradesWithCounts[grade].ToString() + "</B>";
                    }
                    newRow.Cells.Add(cell_Grades);
                }
            }

            return newRow;
        }


        protected void Page_Load(object sender, EventArgs e)
        {            
            List<Student> AllStudents;
            SortedDictionary<string, List<Student>> StudentsBySchool = new SortedDictionary<string,List<Student>>();

            using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogic))
            {
                AllStudents = Student.loadAllStudents(connection);
            }
            
            // Figure out how many grades exist
            List<string> grades = new List<string>();
            foreach (Student student in AllStudents)
            {
                if (!grades.Contains(student.getGrade()))
                {
                    grades.Add(student.getGrade());
                }
            }
            grades.Sort();

            // Split students into lists according to school, for easier processing
            foreach (Student student in AllStudents)
            {
                if (!StudentsBySchool.ContainsKey(student.getSchoolName()))
                {
                    StudentsBySchool.Add(student.getSchoolName(), new List<Student>());
                }

                StudentsBySchool[student.getSchoolName()].Add(student);
            }

            // Display table headings
            tblSchoolStats.Rows.Add(addTableHeader_1(grades));
            tblSchoolStats.Rows.Add(addTableHeader_2(grades));

            // Display the school rows
            foreach (KeyValuePair<string, List<Student>> kvp in StudentsBySchool)
            {
                tblSchoolStats.Rows.Add(addSchoolRow(kvp.Key, grades, kvp.Value));
            }
            
            // Display the total row
            tblSchoolStats.Rows.Add(addTotalRow(grades, AllStudents));

        }
    }
}