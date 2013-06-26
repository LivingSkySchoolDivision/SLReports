using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class Track
    {
        public int ID { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public bool daily { get; set; }
        public int schoolID { get; set; }
        public int blocksPerDay { get; set; }
        public int daysInCycle { get; set; }
        public int dailyBlocksPerDay { get; set; }
        public int effordLegendID { get; set; }
        public School school { get; set; }
        public List<Term> terms { get; set; }
        
        public Track(int id, string name, DateTime start, DateTime end, int schoolid, bool dailyAttendance)
        {
            this.terms = new List<Term>();
            this.ID = id;
            this.name = name;
            this.startDate = start;
            this.endDate = end;
            this.schoolID = schoolid;
            this.daily = dailyAttendance;
        }

        public override string ToString()
        {
            return this.name;
        }

        public int getID()
        {
            return this.ID;
        }

        public static Track loadThisTrack(SqlConnection connection, int trackID)
        {
            Track returnMe = null;

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM Track WHERE iTrackID=" + trackID + "";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe = new Track(
                            int.Parse(dataReader["iTrackID"].ToString().Trim()),
                            dataReader["cName"].ToString().Trim(),
                            DateTime.Parse(dataReader["dStartDate"].ToString()),
                            DateTime.Parse(dataReader["dEndDate"].ToString()),
                            int.Parse(dataReader["iSchoolID"].ToString().Trim()),
                            bool.Parse(dataReader["lDaily"].ToString().Trim())
                            );
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }

        public static Track loadThisTrack(SqlConnection connection, Track track)
        {
            Track returnMe = null;

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM Track WHERE iTrackID=" + track.ID + "";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe = new Track(
                            int.Parse(dataReader["iTrackID"].ToString().Trim()),
                            dataReader["cName"].ToString().Trim(),
                            DateTime.Parse(dataReader["dStartDate"].ToString()),
                            DateTime.Parse(dataReader["dEndDate"].ToString()),
                            int.Parse(dataReader["iSchoolID"].ToString().Trim()),
                            bool.Parse(dataReader["lDaily"].ToString().Trim())
                            );
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }
    }
}