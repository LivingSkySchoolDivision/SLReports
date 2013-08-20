using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class Term
    {
        public int ID { get; set; }
        public int trackID { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string name { get; set; }
        public int schoolID { get; set; }
        public List<ReportPeriod> ReportPeriods { get; set;}
        public List<SchoolClass> Courses { get; set; }
        public ReportPeriod FinalReportPeriod { get; set; }

        public Term(int id, int trackid, DateTime start, DateTime end, string name, int schoolid)
        {
            this.ReportPeriods = new List<ReportPeriod>();
            this.Courses = new List<SchoolClass>();
            this.FinalReportPeriod = null;

            this.ID = id;
            this.trackID = trackid;
            this.startDate = start;
            this.endDate = end;
            this.name = name;
            this.schoolID = schoolid;
        }

        public string getName()
        {
            return this.name;
        }

        public override string ToString()
        {
            return "Term: { ID: "+this.ID+", TrackID: "+this.trackID+", Name: " + this.name + ", SchoolID: "+this.schoolID+", Starts: "+this.startDate.ToShortDateString()+", Ends: "+this.endDate.ToShortDateString()+", Final report period: "+this.FinalReportPeriod+"}";
        }

        public static Term loadThisTerm(SqlConnection connection, int termID)
        {
            Term returnMe = null;

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM Term WHERE iTermID=" + termID + "";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe = new Term(
                            int.Parse(dataReader["iTermID"].ToString().Trim()),
                            int.Parse(dataReader["iTrackID"].ToString().Trim()),
                            DateTime.Parse(dataReader["dStartDate"].ToString()),
                            DateTime.Parse(dataReader["dEndDate"].ToString()),
                            dataReader["cName"].ToString().Trim(),
                            int.Parse(dataReader["iSchoolID"].ToString().Trim())
                            );
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }

        public static List<Term> loadTermsFromThisTrack(SqlConnection connection, int trackID)
        {
            List<Term> returnMe = new List<Term>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM Term WHERE iTrackID=" + trackID + "";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe.Add(new Term(
                            int.Parse(dataReader["iTermID"].ToString().Trim()),
                            int.Parse(dataReader["iTrackID"].ToString().Trim()),
                            DateTime.Parse(dataReader["dStartDate"].ToString()),
                            DateTime.Parse(dataReader["dEndDate"].ToString()),
                            dataReader["cName"].ToString().Trim(),
                            int.Parse(dataReader["iSchoolID"].ToString().Trim())
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }

        public static List<Term> loadTermsFromThisTrack(SqlConnection connection, Track track)
        {
            List<Term> returnMe = new List<Term>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM Term WHERE iTrackID=" + track.ID + "";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe.Add(new Term(
                            int.Parse(dataReader["iTermID"].ToString().Trim()),
                            int.Parse(dataReader["iTrackID"].ToString().Trim()),
                            DateTime.Parse(dataReader["dStartDate"].ToString()),
                            DateTime.Parse(dataReader["dEndDate"].ToString()),
                            dataReader["cName"].ToString().Trim(),
                            int.Parse(dataReader["iSchoolID"].ToString().Trim())
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }

        public static List<Term> loadTermsBetweenTheseDates(SqlConnection connection, Track track, DateTime startDate, DateTime endDate)
        {
            List<Term> returnMe = new List<Term>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM Term WHERE iTrackID=@TRACKID AND dEndDate>@STARTDATE AND dStartDate<@ENDDATE";
            sqlCommand.Parameters.AddWithValue("@TRACKID", track.ID);
            sqlCommand.Parameters.AddWithValue("@STARTDATE", startDate);
            sqlCommand.Parameters.AddWithValue("@ENDDATE", endDate);
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe.Add(new Term(
                            int.Parse(dataReader["iTermID"].ToString().Trim()),
                            int.Parse(dataReader["iTrackID"].ToString().Trim()),
                            DateTime.Parse(dataReader["dStartDate"].ToString()),
                            DateTime.Parse(dataReader["dEndDate"].ToString()),
                            dataReader["cName"].ToString().Trim(),
                            int.Parse(dataReader["iSchoolID"].ToString().Trim())
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }
    }
}