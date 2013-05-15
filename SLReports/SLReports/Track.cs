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
        int ID;
        string name;
        string code;
        DateTime startDate;
        DateTime endDate;
        bool daily;
        int schoolID;
        int blocksPerDay;
        int daysInCycle;
        int dailyBlocksPerDay;
        int effordLegendID;
        School school;

        public Track(int id, string name, DateTime start, DateTime end, int schoolid)
        {
            this.ID = id;
            this.name = name;
            this.startDate = start;
            this.endDate = end;
            this.schoolID = schoolid; 
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
                            int.Parse(dataReader["iSchoolID"].ToString().Trim())
                            );
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }
    }
}