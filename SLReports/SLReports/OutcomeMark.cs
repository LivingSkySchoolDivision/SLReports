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
        private int _courseID;
        public int courseID {
            get
            {
                if (this._courseID == 0)
                {
                    return this.outcome.courseid;
                }
                else
                {
                    return this._courseID;
                }
            }

            set {
                this._courseID = value;
            } 
        }

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
            this._courseID = courseID;
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

            return "OutcomeMark: { ID: " + this.objectiveMarkID + ", Objective ID: " + this.objectiveID + ", Course ID: " + this.courseID + ", nMark: " + this.nMark + ", cMark: " + this.cMark + ", Report Period: " + this.reportPeriodID + ", HasOutcomeInfo: " + LSKYCommon.boolToYesOrNo(hasObjectiveAlso) + " , HasReportPeriod: " + LSKYCommon.boolToYesOrNo(hasReportPeriod) + " }";
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
                    string cMark = dataReader["cMark"].ToString().Trim();

                    // Only load the outcome mark if it is nonzero/nonnull, since SchoolLogic neve deletes "blanked out" marks
                    if ((nMark > 0) || (!string.IsNullOrEmpty(cMark)))
                    {
                        returnMe.Add(new OutcomeMark(
                                int.Parse(dataReader["iStudentCourseObjectiveID"].ToString().Trim()),
                                int.Parse(dataReader["cStudentNumber"].ToString().Trim()),
                                int.Parse(dataReader["iCourseObjectiveID"].ToString().Trim()),
                                int.Parse(dataReader["iReportPeriodID"].ToString().Trim()),
                                int.Parse(dataReader["MarkCourseID"].ToString().Trim()),
                                cMark,
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
            }

            dataReader.Close();
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
                    string cMark = dataReader["cMark"].ToString().Trim();

                    // Only load the outcome mark if it is nonzero/nonnull, since SchoolLogic neve deletes "blanked out" marks
                    if ((nMark > 0) || (!string.IsNullOrEmpty(cMark)))
                    {
                        returnMe.Add(new OutcomeMark(
                                int.Parse(dataReader["iStudentCourseObjectiveID"].ToString().Trim()),
                                int.Parse(dataReader["cStudentNumber"].ToString().Trim()),
                                int.Parse(dataReader["iCourseObjectiveID"].ToString().Trim()),
                                int.Parse(dataReader["iReportPeriodID"].ToString().Trim()),
                                int.Parse(dataReader["MarkCourseID"].ToString().Trim()),
                                cMark,
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
            }
            dataReader.Close();
            sqlCommand.Connection.Close();
            return returnMe;
        }
        
        public static List<OutcomeMark> loadOutcomeMarksForThisStudent(SqlConnection connection, ReportPeriod reportPeriod, Student student)
        {
            List<OutcomeMark> returnMe = new List<OutcomeMark>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_ObjectiveMarks WHERE cStudentNumber=@StudentNum AND iReportPeriodID=@RepPeriodID;";
            sqlCommand.Parameters.AddWithValue("@StudentNum", student.getStudentID());
            sqlCommand.Parameters.AddWithValue("@RepPeriodID", reportPeriod.ID);
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    float nMark = -1;
                    float.TryParse(dataReader["nMark"].ToString().Trim(), out nMark);
                    string cMark = dataReader["cMark"].ToString().Trim();

                    // Only load the outcome mark if it is nonzero/nonnull, since SchoolLogic neve deletes "blanked out" marks
                    if ((nMark > 0) || (!string.IsNullOrEmpty(cMark)))
                    {
                        returnMe.Add(new OutcomeMark(
                                int.Parse(dataReader["iStudentCourseObjectiveID"].ToString().Trim()),
                                int.Parse(dataReader["cStudentNumber"].ToString().Trim()),
                                int.Parse(dataReader["iCourseObjectiveID"].ToString().Trim()),
                                int.Parse(dataReader["iReportPeriodID"].ToString().Trim()),
                                int.Parse(dataReader["MarkCourseID"].ToString().Trim()),
                                cMark,
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
            }

            dataReader.Close();
            sqlCommand.Connection.Close();
            return returnMe;
        }

        public static List<OutcomeMark> loadOutcomeMarksForThisStudent(SqlConnection connection, List<ReportPeriod> reportPeriods, Student student)
        {
            List<OutcomeMark> returnMe = new List<OutcomeMark>();

            if (reportPeriods.Count > 0)
            {
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = connection;
                sqlCommand.Connection.Open();

                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = "SELECT * FROM LSKY_ObjectiveMarks WHERE cStudentNumber=@StudentNum AND (";
                sqlCommand.Parameters.AddWithValue("@StudentNum", student.getStudentID());

                for (int x = 0; x < reportPeriods.Count; x++)
                {
                    ReportPeriod reportPeriod = reportPeriods[x];
                    sqlCommand.CommandText += "iReportPeriodID=@RepPeriodID" + x;
                    if (x < reportPeriods.Count - 1)
                    {
                        sqlCommand.CommandText += " OR ";
                    }
                    sqlCommand.Parameters.AddWithValue("@RepPeriodID" + x, reportPeriod.ID);
                }

                sqlCommand.CommandText += ")";

                SqlDataReader dataReader = sqlCommand.ExecuteReader();

                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        float nMark = -1;
                        float.TryParse(dataReader["nMark"].ToString().Trim(), out nMark);
                        string cMark = dataReader["cMark"].ToString().Trim();

                        // Only load the outcome mark if it is nonzero/nonnull, since SchoolLogic neve deletes "blanked out" marks
                        if ((nMark > 0) || (!string.IsNullOrEmpty(cMark)))
                        {
                            returnMe.Add(new OutcomeMark(
                                int.Parse(dataReader["iStudentCourseObjectiveID"].ToString().Trim()),
                                int.Parse(dataReader["cStudentNumber"].ToString().Trim()),
                                int.Parse(dataReader["iCourseObjectiveID"].ToString().Trim()),
                                int.Parse(dataReader["iReportPeriodID"].ToString().Trim()),
                                int.Parse(dataReader["MarkCourseID"].ToString().Trim()),
                                cMark,
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
                }
                dataReader.Close();
                sqlCommand.Connection.Close();
            }
            
            return returnMe;
        }

        public static List<OutcomeMark> loadOutcomeMarksForThisStudent(SqlConnection connection, List<Term> terms, Student student)
        {
            List<OutcomeMark> returnMe = new List<OutcomeMark>();

            if (terms.Count > 0)
            {
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = connection;
                sqlCommand.Connection.Open();

                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = "SELECT * FROM LSKY_ObjectiveMarks WHERE cStudentNumber=@StudentNum AND (";
                sqlCommand.Parameters.AddWithValue("@StudentNum", student.getStudentID());

                for (int x = 0; x < terms.Count; x++)
                {
                    Term term = terms[x];
                    sqlCommand.CommandText += "iTermID=@TERMID" + x;
                    if (x < terms.Count - 1)
                    {
                        sqlCommand.CommandText += " OR ";
                    }
                    sqlCommand.Parameters.AddWithValue("@TERMID" + x, term.ID);
                }

                sqlCommand.CommandText += ")";

                SqlDataReader dataReader = sqlCommand.ExecuteReader();

                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        float nMark = -1;
                        float.TryParse(dataReader["nMark"].ToString().Trim(), out nMark);
                        string cMark = dataReader["cMark"].ToString().Trim();

                        // Only load the outcome mark if it is nonzero/nonnull, since SchoolLogic neve deletes "blanked out" marks
                        if ((nMark > 0) || (!string.IsNullOrEmpty(cMark)))
                        {
                            returnMe.Add(new OutcomeMark(
                                int.Parse(dataReader["iStudentCourseObjectiveID"].ToString().Trim()),
                                int.Parse(dataReader["cStudentNumber"].ToString().Trim()),
                                int.Parse(dataReader["iCourseObjectiveID"].ToString().Trim()),
                                int.Parse(dataReader["iReportPeriodID"].ToString().Trim()),
                                int.Parse(dataReader["MarkCourseID"].ToString().Trim()),
                                cMark,
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
                }
                dataReader.Close();
                sqlCommand.Connection.Close();
            }

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
                    string cMark = dataReader["cMark"].ToString().Trim();

                    // Only load the outcome mark if it is nonzero/nonnull, since SchoolLogic neve deletes "blanked out" marks
                    if ((nMark > 0) || (!string.IsNullOrEmpty(cMark)))
                    {
                        returnMe.Add(new OutcomeMark(
                                int.Parse(dataReader["iStudentCourseObjectiveID"].ToString().Trim()),
                                int.Parse(dataReader["cStudentNumber"].ToString().Trim()),
                                int.Parse(dataReader["iCourseObjectiveID"].ToString().Trim()),
                                int.Parse(dataReader["iReportPeriodID"].ToString().Trim()),
                                int.Parse(dataReader["MarkCourseID"].ToString().Trim()),
                                cMark,
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
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }
        
    }
}