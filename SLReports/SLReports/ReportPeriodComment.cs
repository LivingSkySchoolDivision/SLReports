using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class ReportPeriodComment : IComparable
    {
        public int id { get; set; }
        public string studentNumber { get; set; }
        public int reportPeriodID { get; set; }
        public string comment { get; set; }
        public string reportPeriodName { get; set; }

        public ReportPeriodComment(int commentid, string studNumber, int reportPeriod, string reportPeriodName, string commentValue)
        {
            this.id = commentid;
            this.studentNumber = studNumber;
            this.reportPeriodID = reportPeriod;
            this.comment = commentValue;
            this.reportPeriodName = reportPeriodName;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            ReportPeriodComment obj2 = obj as ReportPeriodComment;

            if (obj2 != null)
            {
                return this.id.CompareTo(obj2.id);
            }
            else
            {
                throw new ArgumentException("Object is not a ReportPeriodComment");
            }
        }

        /// <summary>
        /// Load all report period comments in the system
        /// </summary>
        /// <param name="connection">A valid SQL connection</param>
        /// <returns></returns>
        public static List<ReportPeriodComment> loadAllRPComments(SqlConnection connection)
        {
            List<ReportPeriodComment> returnMe = new List<ReportPeriodComment>();
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_RPComments;";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe.Add(new ReportPeriodComment(
                            int.Parse(dataReader["iRPCommentsID"].ToString().Trim()),
                            dataReader["cStudentNumber"].ToString().Trim(),
                            int.Parse(dataReader["iReportPeriodID"].ToString().Trim()),
                            dataReader["cName"].ToString().Trim(),
                            dataReader["mComment"].ToString().Trim()
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }

        /// <summary>
        /// Load all report period comments for a given student 
        /// </summary>
        /// <param name="connection">A valid SQL connection</param>
        /// <param name="studentNumber">The student number of the given student</param>
        /// <returns></returns>
        public static List<ReportPeriodComment> loadRPCommentsForStudent(SqlConnection connection, string studentNumber)
        {
            List<ReportPeriodComment> returnMe = new List<ReportPeriodComment>();
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_RPComments WHERE cStudentNumber=@STUDNUM;";
            sqlCommand.Parameters.AddWithValue("@STUDNUM", studentNumber);
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe.Add(new ReportPeriodComment(
                            int.Parse(dataReader["iRPCommentsID"].ToString().Trim()),
                            dataReader["cStudentNumber"].ToString().Trim(),
                            int.Parse(dataReader["iReportPeriodID"].ToString().Trim()),
                            dataReader["cName"].ToString().Trim(),
                            dataReader["mComment"].ToString().Trim()
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }

        /// <summary>
        /// Load all report period comments for a given student and report period
        /// </summary>
        /// <param name="connection">A valid SQL connection</param>
        /// <param name="studentNumber">The student number of the given student</param>
        /// <param name="reportPeriodID">The report period ID</param>
        /// <returns></returns>
        public static List<ReportPeriodComment> loadRPCommentsForStudentandReportPeriod(SqlConnection connection, string studentNumber, int reportPeriodID)
        {
            List<ReportPeriodComment> returnMe = new List<ReportPeriodComment>();
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_RPComments WHERE cStudentNumber=@STUDNUM AND iReportPeriodID=@RPID;";
            sqlCommand.Parameters.AddWithValue("@STUDNUM", studentNumber);
            sqlCommand.Parameters.AddWithValue("@RPID", reportPeriodID);
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe.Add(new ReportPeriodComment(
                            int.Parse(dataReader["iRPCommentsID"].ToString().Trim()),
                            dataReader["cStudentNumber"].ToString().Trim(),
                            int.Parse(dataReader["iReportPeriodID"].ToString().Trim()),
                            dataReader["cName"].ToString().Trim(),
                            dataReader["mComment"].ToString().Trim()
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }
        
        /// <summary>
        /// Filter a list of report period comments to only include the specified report periods (by report period ID)
        /// </summary>
        /// <param name="comments">A list of comments</param>
        /// <param name="reportPeriodIDs">A list of report period ids to filter by</param>
        /// <returns></returns>
        public static List<ReportPeriodComment> getCommentsForTheseReportPeriods(List<ReportPeriodComment> comments, List<int> reportPeriodIDs)
        {
            List<ReportPeriodComment> returnMe = new List<ReportPeriodComment>();

            foreach (ReportPeriodComment comment in comments)
            {
                if (reportPeriodIDs.Contains(comment.reportPeriodID))
                {
                    returnMe.Add(comment);
                }
            }

            return returnMe;
        }

        /// <summary>
        /// Filter a list of report period comments to only include the specified report periods (by report period objects)
        /// </summary>
        /// <param name="comments">A list of comments</param>
        /// <param name="reportPeriods">A list of report periods to filter by</param>
        /// <returns></returns>
        public static List<ReportPeriodComment> getCommentsForTheseReportPeriods(List<ReportPeriodComment> comments, List<ReportPeriod> reportPeriods)
        {
            List<ReportPeriodComment> returnMe = new List<ReportPeriodComment>();

            // Get a list of all report period IDs
            List<int> reportPeriodIDs = new List<int>();
            foreach (ReportPeriod rp in reportPeriods)
            {
                if (!reportPeriodIDs.Contains(rp.ID))
                {
                    reportPeriodIDs.Add(rp.ID);
                }
            }

            return getCommentsForTheseReportPeriods(comments, reportPeriodIDs);
        }

        /// <summary>
        /// Filter a list of report period comments to only include the specified report period
        /// </summary>
        /// <param name="comments">A list of comments</param>
        /// <param name="reportPeriodID">A report period ID to filter by</param>
        /// <returns></returns>
        public static List<ReportPeriodComment> getCommentsForTheseReportPeriods(List<ReportPeriodComment> comments, int reportPeriodID)
        {
            List<int> reportPeriodIDs = new List<int>();
            reportPeriodIDs.Add(reportPeriodID);
            return getCommentsForTheseReportPeriods(comments, reportPeriodIDs);
        }

        /// <summary>
        /// Filter a list of report period comments to only include the specified report period
        /// </summary>
        /// <param name="comments">A list of comments</param>
        /// <param name="reportPeriod">A report period to filter by</param>
        /// <returns></returns>
        public static List<ReportPeriodComment> getCommentsForTheseReportPeriods(List<ReportPeriodComment> comments, ReportPeriod reportPeriod)
        {
            List<int> reportPeriodIDs = new List<int>();
            reportPeriodIDs.Add(reportPeriod.ID);
            return getCommentsForTheseReportPeriods(comments, reportPeriodIDs);
        }


    }
}