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
        public ObjectiveMark mark { get; set; }

        public Objective(int id, int courseid, string subject, string description)
        {
            this.mark = null;
            this.id = id;
            this.description = description;
            this.courseid = courseid;
            this.subject = subject;
        }

        public static List<Objective> loadObjectivesForThisCourse(SqlConnection connection, SchoolClass course)
        {
            List<Objective> returnMe = new List<Objective>();

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
                    returnMe.Add(new Objective(
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