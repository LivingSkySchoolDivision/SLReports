using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.EnrollmentByGrade
{
    public partial class getCSV : System.Web.UI.Page
    {
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

        protected string addCSVRow(string school, List<string> grades, List<Student> students)
        {
            // Figure out grade numbers
            // and while you're at it, figure out male and female counts
            Dictionary<string, int> gradesWithCounts = new Dictionary<string, int>();
            int males = 0;
            int females = 0;

            // Get a list of all grades
            // Find "PK" and "K" first, so they get sorted at the beginning
            if (grades.Contains("PK"))
            {
                gradesWithCounts.Add("PK", 0);
            }
            if (grades.Contains("0K"))
            {
                gradesWithCounts.Add("0K", 0);
            }
            foreach (string grade in grades)
            {
                if ((grade != "PK") && (grade != "0K"))
                {
                    if (!gradesWithCounts.ContainsKey(grade))
                    {
                        gradesWithCounts.Add(grade, 0);
                    }
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

            StringBuilder csvLine = new StringBuilder();
            csvLine.Append("\"" + school + "\"");
            csvLine.Append(",");
            csvLine.Append(students.Count);
            csvLine.Append(",");
            csvLine.Append(males);
            csvLine.Append(",");
            csvLine.Append(females);
            csvLine.Append(",");
            
            // Grades
            for (int x = 0; x < grades.Count; x++)
            {
                csvLine.Append(gradesWithCounts[grades[x]].ToString());                
                if (x < grades.Count - 1)
                {
                    csvLine.Append(",");
                }
            }

            return csvLine.ToString();            
        }

        protected MemoryStream GenerateCSV(List<Student> AllStudents)
        {
            SortedDictionary<string, List<Student>> StudentsBySchool = new SortedDictionary<string, List<Student>>();

            // Figure out how many grades exist
            List<string> gradesUnsorted = new List<string>();
            List<string> grades = new List<string>();
            foreach (Student student in AllStudents)
            {
                if (!gradesUnsorted.Contains(student.getGrade()))
                {
                    gradesUnsorted.Add(student.getGrade());
                }
            }
            gradesUnsorted.Sort();

            if (gradesUnsorted.Contains("PK"))
            {
                grades.Add("PK");
            }
            if (gradesUnsorted.Contains("0K"))
            {
                grades.Add("0K");
            }
            foreach (string grade in gradesUnsorted)
            {
                if ((grade != "PK") && (grade != "0K"))
                {
                    grades.Add(grade);
                }
            }

            // Split students into lists according to school, for easier processing
            foreach (Student student in AllStudents)
            {
                if (!StudentsBySchool.ContainsKey(student.getSchoolName()))
                {
                    StudentsBySchool.Add(student.getSchoolName(), new List<Student>());
                }

                StudentsBySchool[student.getSchoolName()].Add(student);
            }

            // Generate the CSV
            MemoryStream csvFile = new MemoryStream();
            StreamWriter writer = new StreamWriter(csvFile, Encoding.UTF8);

            // Headings
            StringBuilder headingLine = new StringBuilder();
            headingLine.Append("School, TotalStudents, TotalMale, TotalFemale,");

            for (int x = 0; x < grades.Count; x++)
            {
                string gradeString = grades[x];
                headingLine.Append("Grade_" + gradeString);
                if (x < grades.Count - 1)
                {
                    headingLine.Append(",");
                }
            }
            writer.WriteLine(headingLine.ToString());

            // Data
            foreach (KeyValuePair<string, List<Student>> kvp in StudentsBySchool)
            {                
                writer.WriteLine(addCSVRow(kvp.Key, grades, kvp.Value));
            }

            writer.Flush();
            csvFile.Flush();
            return csvFile;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            List<Student> AllStudents;           

            using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogic))
            {
                AllStudents = Student.loadAllStudents(connection);
            }

            sendCSV(GenerateCSV(AllStudents), "LSKY_EnrollmentCounts_" + LSKYCommon.getCurrentTimeStampForFilename());            
        }
    }
}