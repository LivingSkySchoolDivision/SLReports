using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class SchoolDay
    {
        public int id { get; set; }
        public int trackID { get; set; }
        public int dayNumber { get; set; }
        public int schoolID { get; set; }
        public string name { get; set; }

        public override string ToString()
        {
            return "SchoolDay {ID: " + this.id + ", TrackID: " + this.trackID + ", dayNumber: " + this.dayNumber + ", schoolID: " + this.schoolID + ", name: " + this.name + "}";
        }

        public SchoolDay(int id, int trackid, int daynumber, int schoolid, string name)
        {
            this.id = id;
            this.trackID = trackid;
            this.dayNumber = daynumber;
            this.schoolID = schoolid;
            this.name = name;
        }

        public static List<SchoolDay> loadDaysFromThisSchool(SqlConnection connection, School school)
        {
            List<SchoolDay> returnMe = new List<SchoolDay>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM Days WHERE iSchoolID=" + school.getSchoolLogicID() + "";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe.Add(new SchoolDay(
                            int.Parse(dataReader["iDaysID"].ToString().Trim()),
                            int.Parse(dataReader["iTrackID"].ToString().Trim()),
                            int.Parse(dataReader["iDayNumber"].ToString().Trim()),
                            int.Parse(dataReader["iSchoolID"].ToString().Trim()),
                            dataReader["cName"].ToString().Trim()
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }

        public static List<SchoolDay> loadDaysFromThisTrack(SqlConnection connection, Track track)
        {
            List<SchoolDay> returnMe = new List<SchoolDay>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM Days WHERE iTrackID=" + track.ID + " ORDER BY iDayNumber ASC";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe.Add(new SchoolDay(
                            int.Parse(dataReader["iDaysID"].ToString().Trim()),
                            int.Parse(dataReader["iTrackID"].ToString().Trim()),
                            int.Parse(dataReader["iDayNumber"].ToString().Trim()),
                            int.Parse(dataReader["iSchoolID"].ToString().Trim()),
                            dataReader["cName"].ToString().Trim()
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }

    }
}