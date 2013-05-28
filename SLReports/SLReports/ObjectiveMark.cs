using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class ObjectiveMark
    {
        public int objectiveMarkID { get; set; }
        public int studentID { get; set; }
        public int objectiveID { get; set; }
        public int reportPeriodID { get; set; }
        public int courseID { get; set; }
        public string mark { get; set; }
        public Objective objective { get; set; }

        public ObjectiveMark(int objectiveMarkID, int studentID, int objectiveID, int reportPeriodID, int courseID, string mark)
        {
            this.objectiveMarkID = objectiveMarkID;
            this.studentID = studentID;
            this.objectiveID = objectiveID;
            this.reportPeriodID = reportPeriodID;
            this.courseID = courseID;
            this.mark = mark;
        }

        public override string ToString()
        {
            return this.objectiveMarkID + " " + this.mark;
        }

        public static List<ObjectiveMark> loadObjectiveMarksForThisCourse(SqlConnection connection, Term term, Student student, Course course)
        {
            List<ObjectiveMark> returnMe = new List<ObjectiveMark>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_ObjectiveMarks WHERE cStudentNumber=@StudentNum AND iCourseID=@CourseID AND iTermID=@TermID";
            sqlCommand.Parameters.AddWithValue("@TermID", term.ID);
            sqlCommand.Parameters.AddWithValue("@StudentNum", student.getStudentID());
            sqlCommand.Parameters.AddWithValue("@CourseID", course.courseid);
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe.Add(new ObjectiveMark(
                            int.Parse(dataReader["iStudentCourseObjectiveID"].ToString().Trim()),
                            int.Parse(dataReader["cStudentNumber"].ToString().Trim()),
                            int.Parse(dataReader["iCourseObjectiveID"].ToString().Trim()),
                            int.Parse(dataReader["iReportPeriodID"].ToString().Trim()),
                            int.Parse(dataReader["iCourseID"].ToString().Trim()),
                            dataReader["cMark"].ToString().Trim()
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }

        public static List<ObjectiveMark> loadObjectiveMarksForThisStudent(SqlConnection connection, Term term, Student student)
        {
            List<ObjectiveMark> returnMe = new List<ObjectiveMark>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_ObjectiveMarks WHERE cStudentNumber=@StudentNum AND iTermID=@TermID;";
            sqlCommand.Parameters.AddWithValue("@StudentNum", student.getStudentID());
            sqlCommand.Parameters.AddWithValue("@TermID", term.ID);
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe.Add(new ObjectiveMark(
                            int.Parse(dataReader["iStudentCourseObjectiveID"].ToString().Trim()),
                            int.Parse(dataReader["cStudentNumber"].ToString().Trim()),
                            int.Parse(dataReader["iCourseObjectiveID"].ToString().Trim()),
                            int.Parse(dataReader["iReportPeriodID"].ToString().Trim()),
                            int.Parse(dataReader["iCourseID"].ToString().Trim()),
                            dataReader["cMark"].ToString().Trim()
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }
                
    }
}