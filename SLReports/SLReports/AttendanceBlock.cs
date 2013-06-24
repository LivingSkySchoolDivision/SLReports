using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace SLReports
{
    public class AttendanceBlock
    {
        public int id { get; set; }
        public int track { get; set; }
        public int block { get; set; }
        public int instructionalMinutes { get; set; }
        public string name { get; set; }
        public bool reported { get; set; }
        public DateTime blockStartTime { get; set; }
        public DateTime blockEndTime { get; set; }

        public AttendanceBlock(int id, string name, int track, int block, int minutes, bool reported, DateTime startTime, DateTime endTime)
        {
            this.id = id;
            this.track = track;
            this.block = block;
            this.instructionalMinutes = minutes;
            this.name = name;
            this.reported = reported;
            this.blockStartTime = startTime;
            this.blockEndTime = endTime;
        }

        public static List<AttendanceBlock> loadAllAttendanceBlocks(SqlConnection connection)
        {
            List<AttendanceBlock> returnMe = new List<AttendanceBlock>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;

            StringBuilder SQL = new StringBuilder();
            
            SQL.Clear();
            SQL.Append("SELECT * FROM AttendanceBlocks;");

            sqlCommand.CommandText = SQL.ToString();
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe.Add(new AttendanceBlock(
                        int.Parse(dataReader["iAttendanceBlocksID"].ToString()),
                        dataReader["cName"].ToString().Trim(),
                        int.Parse(dataReader["iTrackID"].ToString().Trim()),
                        int.Parse(dataReader["iBlockNumber"].ToString().Trim()),
                        int.Parse(dataReader["iInstructionalMinutes"].ToString().Trim()),
                        bool.Parse(dataReader["lNotReported"].ToString().Trim()),
                        DateTime.Parse(dataReader["tStartTime"].ToString().Trim()),
                        DateTime.Parse(dataReader["tEndTime"].ToString().Trim())
                        ));
                }
            }
            sqlCommand.Connection.Close();

            return returnMe;

        }
    }
}