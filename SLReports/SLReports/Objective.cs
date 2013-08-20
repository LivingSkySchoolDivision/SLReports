using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class Objective // rename me to outcome
    {
        public int id { get; set; }
        public string subject { get; set; }
        public string description { get; set; } // Rename me to notes
        public int courseid { get; set; }
        public string courseName { get; set; }
        public string courseCode { get; set; }
        public int order { get; set; }
        public string studentvalue { get; set; }
        public string category { get; set; }        
        public List<ObjectiveMark> marks { get; set; }

        public Objective(int id, int courseid, string subject, string description, string category, string coursename, string courseCode)
        {
            this.marks = new List<ObjectiveMark>();
            this.id = id;
            this.description = description;
            this.courseid = courseid;
            this.subject = subject;
            this.category = category;
            this.courseName = coursename;
            this.courseCode = courseCode;
        }

        public static List<Objective> loadObjectivesForThisCourse(SqlConnection connection, SchoolClass course)
        {
            List<Objective> returnMe = new List<Objective>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_CourseObjectives WHERE iCourseID=" + course.courseid;
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe.Add(new Objective(
                            int.Parse(dataReader["iCourseObjectiveID"].ToString().Trim()),
                            int.Parse(dataReader["iCourseID"].ToString().Trim()),
                            dataReader["cSubject"].ToString().Trim(),
                            dataReader["mNotes"].ToString().Trim(),
                            dataReader["ObjectiveCategory"].ToString().Trim(),
                            dataReader["CourseName"].ToString().Trim(),
                            dataReader["cCourseCode"].ToString().Trim()
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }

        public static List<Objective> loadAllObjectives(SqlConnection connection)
        {
            List<Objective> returnMe = new List<Objective>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_CourseObjectives";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe.Add(new Objective(
                            int.Parse(dataReader["iCourseObjectiveID"].ToString().Trim()),
                            int.Parse(dataReader["iCourseID"].ToString().Trim()),
                            dataReader["cSubject"].ToString().Trim(),
                            dataReader["mNotes"].ToString().Trim(),
                            dataReader["ObjectiveCategory"].ToString().Trim(),
                            dataReader["CourseName"].ToString().Trim(),
                            dataReader["cCourseCode"].ToString().Trim()
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }


        public bool hasMarks()
        {
            if (marks.Count > 0)
            {
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            string subject = this.subject;
            if (this.subject.Length > 25)
            {
                subject = this.subject.Substring(0, 25);
            }
            return "Objective: { ID: " + this.id + ", Subject: "+subject+", Category: " + this.category + ", ContainsObjectiveMarks: " + this.marks.Count + "}";
        }
    }
}