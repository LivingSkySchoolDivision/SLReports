using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class ReportPeriod
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int ID { get; set; }
        public int termID { get; set; }
        public string name { get; set; }
        public int schoolID { get; set; }
        public List<Mark> marks { get; set; }

        public ReportPeriod(int id, string name, DateTime start, DateTime end, int schoolid, int termid)
        {
            this.ID = id;
            this.name = name;
            this.startDate = start;
            this.endDate = end;
            this.schoolID = schoolid;
            this.termID = termid;
        }

        public override string ToString()
        {
            return this.name;
        }

        public static List<ReportPeriod> loadReportPeriodsFromThisTerm(SqlConnection connection, int termID)
        {
            List<ReportPeriod> returnMe = new List<ReportPeriod>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM ReportPeriod WHERE iTermID=" + termID + "";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe.Add(new ReportPeriod(
                            int.Parse(dataReader["iReportPeriodID"].ToString().Trim()),
                            dataReader["cName"].ToString().Trim(),
                            DateTime.Parse(dataReader["dStartDate"].ToString()),
                            DateTime.Parse(dataReader["dEndDate"].ToString()),                            
                            int.Parse(dataReader["iSchoolID"].ToString().Trim()),
                            int.Parse(dataReader["iTermID"].ToString().Trim())
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }
        
    }
}