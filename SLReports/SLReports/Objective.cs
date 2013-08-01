using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class Objective
    {
        public int id { get; set; }
        public string subject { get; set; }
        public string description { get; set; }
        public int courseid { get; set; }
        public int order { get; set; }
        public string studentvalue { get; set; }
        public string category { get; set; }        
        public List<ObjectiveMark> marks { get; set; }

        public Objective(int id, int courseid, string subject, string description, string category)
        {
            this.marks = new List<ObjectiveMark>();
            this.id = id;
            this.description = description;
            this.courseid = courseid;
            this.subject = subject;
            this.category = category;
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
                            dataReader["ObjectiveCategory"].ToString().Trim()
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
            return "Objective: { ID: " + this.id + ", ContainsObjectiveMarks: " + this.marks.Count + "}";
        }
    }
}