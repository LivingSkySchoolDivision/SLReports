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
        public string nMark { get; set; }
        public string cMark { get; set; }
        public string className { get; set; }
        public int classID { get; set; }
        public int courseID { get; set; }
        public string comment { get; set; }
        public SchoolClass schoolclass { get; set; }
        public List<Teacher> teachers { get; set; }

        public ReportPeriod reportPeriod { get; set; }

        public Mark(int id, int reportPeriodID, string numberMark, string outcomeMark, string classname, int classid, int courseid, string comment, ReportPeriod rperiod)
        {            
            this.ID = id;
            this.reportPeriodID = reportPeriodID;
            this.nMark = numberMark;
            this.cMark = outcomeMark;
            this.className = classname;
            this.comment = comment;
            this.reportPeriod = rperiod;
            this.classID = classid;
            this.courseID = courseid;
            this.schoolclass = new SchoolClass(this.className, this.classID, this.courseID);            
        }

       
        public string getMark()
        {   
            if (((int)Double.Parse(this.nMark) == 0) && (string.IsNullOrEmpty(cMark)))
            {
                return string.Empty;
            }
            else
            {
                if (string.IsNullOrEmpty(cMark))
                {
                    int markVal = (int)double.Parse(this.nMark);

                    return markVal + "%";
                }
                else
                {
                    return this.cMark;
                }
            }
        }

        public override string ToString()
        {
            return "Mark: {Mark ID: "+this.ID+",CourseID: "+this.courseID+", ClassID: " + this.classID + ", ClassName: " + this.className + ", nMark: " + this.nMark + ", cMark: "+this.cMark+" , ReportPeriodID: " + this.reportPeriod.ID + "}";
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
                            int.Parse(dataReader["iClassID"].ToString().Trim()),
                            int.Parse(dataReader["iCourseID"].ToString().Trim()),
                            dataReader["mComment"].ToString().Trim(),
                            reportPeriod
                            ));

                    /* TODO: get teachers */


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
                                int.Parse(dataReader["iClassID"].ToString().Trim()),
                                int.Parse(dataReader["iCourseID"].ToString().Trim()),
                                dataReader["mComment"].ToString().Trim(),
                                rp
                                );


                        newMark.reportPeriod = rp;

                        
                        returnMe.Add(newMark);

                    }
                }
            sqlCommand.Connection.Close();
            }

            foreach (Mark newMark in returnMe)
            {
                newMark.teachers = Teacher.loadTeachersForThisClass(connection, newMark.classID);
            }

            return returnMe;
        }
    }
}