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
        public string description { get; set; }
        public int courseid { get; set; }
        public int order { get; set; }
        public string studentvalue { get; set; }

        public Outcome(int id, int courseid, string subject, string description)
        {
            this.id = id;
            this.description = description;
            this.courseid = courseid;
            this.subject = subject;
        }

        public static List<Outcome> loadOutcomesForThisCourse(SqlConnection connection, Course course)
        {
            List<Outcome> returnMe = new List<Outcome>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM CourseObjective WHERE iCourseID=" + course.courseid;
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
                            dataReader["mNotes"].ToString().Trim()
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }
    }
}