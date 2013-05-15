using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class Mark
    {
        public int ID { get; set; }
        public int reportPeriodID { get; set; }
        public string numberMark { get; set; }
        public string outcomeMark { get; set; }
        public string className { get; set; }
        public string classID { get; set; }
        public string teacherFirst { get; set; }
        public string teacherLast { get; set; }
        public string teacherTitle { get; set; }
        public string comment { get; set; }

        public ReportPeriod reportPeriod { get; set; }

        public Mark(int id, int reportPeriodID, string numberMark, string outcomeMark, string classname, string classid, string comment, string teacherFirst, string teacherLast, string teacherTitle, ReportPeriod rperiod)
        {
            this.ID = id;
            this.reportPeriodID = reportPeriodID;
            this.numberMark = numberMark;
            this.outcomeMark = outcomeMark;
            this.className = classname;
            this.comment = comment;
            this.reportPeriod = rperiod;
            this.classID = classid;
            this.teacherFirst = teacherFirst;
            this.teacherLast = teacherLast;
            this.teacherTitle = teacherTitle;
        }

        public string getMark()
        {
            if (((int)Double.Parse(this.numberMark) == 0) && (true))
            {
                return "(Not Available)";
            }
            else
            {
                if (string.IsNullOrEmpty(outcomeMark))
                {
                    return this.numberMark + "%";
                }
                else
                {
                    return this.outcomeMark;
                }
            }
        }

        public override string ToString()
        {
            return this.classID + ":" + this.className + " Mark: " + this.getMark() + ", Period: " + this.reportPeriod;
        }

        public static List<Mark> loadMarksFromThisReportPeriod(SqlConnection connection, ReportPeriod reportPeriod, Student student)
        {
            List<Mark> returnMe = new List<Mark>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_Marks WHERE iReportPeriodID=" + reportPeriod.ID + " AND cStudentNumber='" + student.getStudentID() + "'";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {                    
                    returnMe.Add(new Mark(
                            int.Parse(dataReader["iMarksID"].ToString().Trim()),
                            int.Parse(dataReader["iReportPeriodID"].ToString().Trim()),
                            dataReader["MarkPercent"].ToString().Trim(),
                            dataReader["MarkOutcome"].ToString().Trim(),
                            dataReader["Class"].ToString().Trim(),
                            dataReader["iClassID"].ToString().Trim(),
                            dataReader["mComment"].ToString().Trim(),
                            dataReader["TeacherFirstName"].ToString().Trim(),
                            dataReader["TeacherLastName"].ToString().Trim(),
                            dataReader["TeacherTitle"].ToString().Trim(),
                            reportPeriod
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }

        
        public static List<Mark> loadMarksFromTheseReportPeriods(SqlConnection connection, List<ReportPeriod> periods, Student student)
        {
            List<Mark> returnMe = new List<Mark>();

            foreach (ReportPeriod rp in periods)
            {

                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = connection;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = "SELECT * FROM LSKY_Marks WHERE iReportPeriodID=" + rp.ID + " AND cStudentNumber='" + student.getStudentID() + "'";
                sqlCommand.Connection.Open();
                SqlDataReader dataReader = sqlCommand.ExecuteReader();

                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        Mark newMark = new Mark(
                                int.Parse(dataReader["iMarksID"].ToString().Trim()),
                                int.Parse(dataReader["iReportPeriodID"].ToString().Trim()),
                                dataReader["MarkPercent"].ToString().Trim(),
                                dataReader["MarkOutcome"].ToString().Trim(),
                                dataReader["Class"].ToString().Trim(),
                                dataReader["iClassID"].ToString().Trim(),
                                dataReader["mComment"].ToString().Trim(),
                                dataReader["TeacherFirstName"].ToString().Trim(),
                                dataReader["TeacherLastName"].ToString().Trim(),
                                dataReader["TeacherTitle"].ToString().Trim(),
                                rp
                                );

                        newMark.reportPeriod = rp;
                        returnMe.Add(newMark);

                    }
                }
            sqlCommand.Connection.Close();
            }
            return returnMe;
        }
    }
}