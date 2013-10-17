using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class Outcome
    {
        public int id { get; set; }
        public string subject { get; set; }
        public string notes { get; set; }
        public int courseid { get; set; }
        public string courseName { get; set; }
        public string courseCode { get; set; }
        public int order { get; set; }
        public string studentvalue { get; set; }
        public string category { get; set; }        
        public List<OutcomeMark> marks { get; set; }

        public Outcome(int id, int courseid, string subject, string notes, string category, string coursename, string courseCode)
        {
            this.marks = new List<OutcomeMark>();
            this.id = id;
            this.notes = notes;
            this.courseid = courseid;
            this.subject = subject;
            this.category = category;
            this.courseName = coursename;
            this.courseCode = courseCode;
        }

        public static List<Outcome> loadObjectivesForThisCourse(SqlConnection connection, SchoolClass course)
        {
            List<Outcome> returnMe = new List<Outcome>();

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
                    returnMe.Add(new Outcome(
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

        public static List<Outcome> loadObjectivesForThisCourseByCategoryName(SqlConnection connection, SchoolClass course, string categoryName)
        {
            List<Outcome> returnMe = new List<Outcome>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_CourseObjectives WHERE iCourseID=@COURSEID AND ObjectiveCategory=@CATNAME";
            sqlCommand.Parameters.AddWithValue("@COURSEID", course.courseid);
            sqlCommand.Parameters.AddWithValue("@CATNAME", categoryName);
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe.Add(new Outcome(
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

        public static List<Outcome> loadAllObjectives(SqlConnection connection)
        {
            List<Outcome> returnMe = new List<Outcome>();

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
                    returnMe.Add(new Outcome(
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
            return "Outcome: { ID: " + this.id + ", Subject: " + subject + ", Category: " + this.category + ", ContainsOutcomeMarks: " + this.marks.Count + "}";
        }
    }
}