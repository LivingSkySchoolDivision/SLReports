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
        public string schoolName { get; set; }

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

        public Track(int id, string name, DateTime start, DateTime end, int schoolid, bool dailyAttendance, string code, int daysInCycle, int blocksPerDay, int dailyBlocksPerDay)
        {
            this.terms = new List<Term>();
            this.ID = id;
            this.name = name;
            this.startDate = start;
            this.endDate = end;
            this.schoolID = schoolid;
            this.daily = dailyAttendance;
            this.daysInCycle = daysInCycle;
            this.blocksPerDay = blocksPerDay;
            this.dailyBlocksPerDay = dailyBlocksPerDay;
            this.code = code;
        }

        public override string ToString()
        {
            return "Track: { Name: " + this.name + ", ID: "+this.ID+", Daily: "+ LSKYCommon.boolToYesOrNo(this.daily)+", SchoolID: "+this.schoolID+",  Starts: "+this.startDate.ToShortDateString()+", Ends: "+this.endDate.ToShortDateString()+"}";
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
                            bool.Parse(dataReader["lDaily"].ToString().Trim()),
                            dataReader["cCode"].ToString().Trim(),
                            int.Parse(dataReader["iDaysInCycle"].ToString().Trim()),
                            int.Parse(dataReader["iBlocksPerDay"].ToString().Trim()),
                            int.Parse(dataReader["iDailyBlocksPerDay"].ToString().Trim())
                            );
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }

        public static List<Track> loadAllTracks(SqlConnection connection)
        {
            List<School> allSchools = new List<School>();
            allSchools = School.loadAllSchools(connection);

            List<Track> returnMe = new List<Track>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM Track ORDER BY iSchoolID ASC, cName ASC;";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    Track newTrack = new Track(
                            int.Parse(dataReader["iTrackID"].ToString().Trim()),
                            dataReader["cName"].ToString().Trim(),
                            DateTime.Parse(dataReader["dStartDate"].ToString()),
                            DateTime.Parse(dataReader["dEndDate"].ToString()),
                            int.Parse(dataReader["iSchoolID"].ToString().Trim()),
                            bool.Parse(dataReader["lDaily"].ToString().Trim()),
                            dataReader["cCode"].ToString().Trim(),
                            int.Parse(dataReader["iDaysInCycle"].ToString().Trim()),
                            int.Parse(dataReader["iBlocksPerDay"].ToString().Trim()),
                            int.Parse(dataReader["iDailyBlocksPerDay"].ToString().Trim())
                            );

                    foreach (School school in allSchools)
                    {
                        if (school.getSchoolLogicID() == dataReader["iSchoolID"].ToString().Trim())
                        {
                            newTrack.school = school;
                        }
                    }
                    
                    returnMe.Add(newTrack);
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
                            bool.Parse(dataReader["lDaily"].ToString().Trim()),
                            dataReader["cCode"].ToString().Trim(),
                            int.Parse(dataReader["iDaysInCycle"].ToString().Trim()),
                            int.Parse(dataReader["iBlocksPerDay"].ToString().Trim()),
                            int.Parse(dataReader["iDailyBlocksPerDay"].ToString().Trim())
                            );
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }
    }
}