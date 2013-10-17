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
        
        public Outcome outcome { get; set; }
        public ReportPeriod reportPeriod { get; set; }


        public OutcomeMark(int objectiveMarkID, int studentID, int objectiveID, int reportPeriodID, int courseID, string cmark, float nmark, Outcome outcome)
        {
            this.objectiveMarkID = objectiveMarkID;
            this.studentID = studentID;
            this.objectiveID = objectiveID;
            this.reportPeriodID = reportPeriodID;
            this.courseID = courseID;
            this.cMark = cmark;
            this.nMark = nmark;
            this.outcome = outcome;

            // Add this mark to the child outcome's list of marks so it is easy to manipulate this data later            
            if (!this.outcome.marks.Contains(this))
            {
                this.outcome.marks.Add(this);
            }

        }

        public override string ToString()
        {
            bool hasObjectiveAlso = false;
            if (this.outcome != null)
            {
                hasObjectiveAlso = true;
            }

            bool hasReportPeriod = false;
            if (this.reportPeriod != null)
            {
                hasReportPeriod = true;
            }

            return "OutcomeMark: { ID: " + this.objectiveMarkID + ", Objective ID: " + this.objectiveID + ", nMark: " + this.nMark + ", cMark: " + this.cMark + ", Report Period: " + this.reportPeriodID + ", HasOutcomeInfo: " + LSKYCommon.boolToYesOrNo(hasObjectiveAlso) + " , HasReportPeriod: " + LSKYCommon.boolToYesOrNo(hasReportPeriod) + " }";
        }
        
        public static List<OutcomeMark> loadOutcomeMarksForThisCourse(SqlConnection connection, Term term, Student student, SchoolClass course)
        {
            List<OutcomeMark> returnMe = new List<OutcomeMark>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_ObjectiveMarks WHERE cStudentNumber=@StudentNum AND iTermID=@TermID AND (ObjectiveCourseID=@CourseID OR MarkCourseID=@CourseID)";
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
                            int.Parse(dataReader["MarkCourseID"].ToString().Trim()),
                            dataReader["cMark"].ToString().Trim(),
                            (float)Math.Round(nMark, 1),
                            new Outcome(
                                int.Parse(dataReader["iCourseObjectiveID"].ToString().Trim()),
                                int.Parse(dataReader["ObjectiveCourseID"].ToString().Trim()),
                                dataReader["cSubject"].ToString().Trim(),
                                dataReader["mNotes"].ToString().Trim(),
                                dataReader["OutcomeCategory"].ToString().Trim(),
                                dataReader["CourseName"].ToString().Trim(),
                                dataReader["cCourseCode"].ToString().Trim()
                                )
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }

        public static List<OutcomeMark> loadOutcomeMarksForThisStudent(SqlConnection connection, Term term, Student student)
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
                            (float)Math.Round(nMark, 1),
                            new Outcome(
                                int.Parse(dataReader["iCourseObjectiveID"].ToString()),
                                int.Parse(dataReader["ObjectiveCourseID"].ToString()),
                                dataReader["cSubject"].ToString(),
                                dataReader["cNotes"].ToString(),
                                dataReader["OutcomeCategory"].ToString(),
                                dataReader["CourseName"].ToString(),
                                dataReader["cCourseCode"].ToString()
                                )
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }
        
        public static List<OutcomeMark> loadAllOutcomeMarks(SqlConnection connection)
        {
            List<OutcomeMark> returnMe = new List<OutcomeMark>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_ObjectiveMarks;";
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
                            (float)Math.Round(nMark, 1),
                            new Outcome(
                                int.Parse(dataReader["iCourseObjectiveID"].ToString()),
                                int.Parse(dataReader["ObjectiveCourseID"].ToString()),
                                dataReader["cSubject"].ToString(),
                                dataReader["cNotes"].ToString(),
                                dataReader["OutcomeCategory"].ToString(),
                                dataReader["CourseName"].ToString(),
                                dataReader["cCourseCode"].ToString()
                                )
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }
        
    }
}