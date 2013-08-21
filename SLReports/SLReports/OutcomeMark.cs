using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class OutcomeMark
    {
        public int objectiveMarkID { get; set; }
        public int studentID { get; set; }
        public int objectiveID { get; set; }
        public int reportPeriodID { get; set; }
        public int courseID { get; set; }
        public string cMark { get; set; }
        public float nMark { get; set; }
        public string mark {
            get
            {
                if (string.IsNullOrEmpty(cMark))
                {
                    return nMark.ToString();
                }
                else
                {
                    return cMark;
                }
            }

            set
            {
                throw new Exception("This value cannot be modified");
            }
        }
        
        public Outcome objective { get; set; }
        public ReportPeriod reportPeriod { get; set; }

        public string description 
        {
            get
            {
                if (this.objective != null)
                {
                    return this.objective.notes;
                } else {
                    return "Objective ID " + this.objectiveID;
                }
            }

            set
            {
                // Do nothing beacuse this should never be set directly
            }
        }


        public OutcomeMark(int objectiveMarkID, int studentID, int objectiveID, int reportPeriodID, int courseID, string cmark, float nmark)
        {
            this.objectiveMarkID = objectiveMarkID;
            this.studentID = studentID;
            this.objectiveID = objectiveID;
            this.reportPeriodID = reportPeriodID;
            this.courseID = courseID;
            this.cMark = cmark;
            this.nMark = nmark;
        }

        public override string ToString()
        {
            bool hasObjectiveAlso = false;

            if (this.objective != null)
            {
                hasObjectiveAlso = true;
            }

            bool hasReportPeriod = false;
            if (this.reportPeriod != null)
            {
                hasReportPeriod = true;
            }

            return "ObjectiveMark: { ID: " + this.objectiveMarkID + ", Objective ID: " + this.objectiveID + ", nMark: " + this.nMark + ", cMark: " + this.cMark + ", Translated Mark: "+this.mark+", Report Period: " + this.reportPeriodID + ", HasObjectiveInfo: " + LSKYCommon.boolToYesOrNo(hasObjectiveAlso) + " , HasReportPeriod: " + LSKYCommon.boolToYesOrNo(hasReportPeriod) + " }";
        }

        public static List<OutcomeMark> loadObjectiveMarksForThisCourse(SqlConnection connection, Term term, Student student, SchoolClass course)
        {
            List<OutcomeMark> returnMe = new List<OutcomeMark>();

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

                    float nMark = -1;
                    float.TryParse(dataReader["nMark"].ToString().Trim(), out nMark);

                    returnMe.Add(new OutcomeMark(
                            int.Parse(dataReader["iStudentCourseObjectiveID"].ToString().Trim()),
                            int.Parse(dataReader["cStudentNumber"].ToString().Trim()),
                            int.Parse(dataReader["iCourseObjectiveID"].ToString().Trim()),
                            int.Parse(dataReader["iReportPeriodID"].ToString().Trim()),
                            int.Parse(dataReader["iCourseID"].ToString().Trim()),
                            dataReader["cMark"].ToString().Trim(),
                            nMark
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }

        public static List<OutcomeMark> loadObjectiveMarksForThisStudent(SqlConnection connection, Term term, Student student)
        {
            List<OutcomeMark> returnMe = new List<OutcomeMark>();

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
                    float nMark = -1;
                    float.TryParse(dataReader["nMark"].ToString().Trim(), out nMark);

                    returnMe.Add(new OutcomeMark(
                            int.Parse(dataReader["iStudentCourseObjectiveID"].ToString().Trim()),
                            int.Parse(dataReader["cStudentNumber"].ToString().Trim()),
                            int.Parse(dataReader["iCourseObjectiveID"].ToString().Trim()),
                            int.Parse(dataReader["iReportPeriodID"].ToString().Trim()),
                            int.Parse(dataReader["iCourseID"].ToString().Trim()),
                            dataReader["cMark"].ToString().Trim(),
                            nMark
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }
                
    }
}