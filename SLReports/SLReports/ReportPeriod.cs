﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class ReportPeriod :IComparable
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int ID { get; set; }
        public int termID { get; set; }
        public string name { get; set; }
        public int schoolID { get; set; }
        public List<Mark> marks { get; set; }
        public List<SchoolClass> courses { get; set; }
        public List<Outcome> objectives { get; set; }
        public int daysOpenBeforeEnd { get; set; }
        public int daysOpenAfterEnd { get; set; }
        public DateTime DateOpens
        {
            get
            {
                return endDate.AddDays(this.daysOpenBeforeEnd * -1);
            }
            set
            {
                // Do nothing because this value should never be set
            }
        }
        public DateTime DateCloses
        {
            get
            {
                return endDate.AddDays(this.daysOpenAfterEnd);
            }
            set
            {
                // Do nothing because this value should never be set
            }
        }

        private void loadReportPeriodSettings(SqlConnection connection)
        {
            // Get the days before and after
            int daysBefore = 0;
            int daysAfter = 0;

            Dictionary<string, string> schoolSettings = School.loadSchoolSettings(connection, this.schoolID);

            foreach (KeyValuePair<string, string> kvp in schoolSettings)
            {
                if (kvp.Key.ToLower().Trim() == "Grades/PreReportDays".ToLower().Trim())
                {
                    int.TryParse(kvp.Value, out daysBefore);
                }

                if (kvp.Key.ToLower().Trim() == "Grades/PostReportDays".ToLower().Trim())
                {
                    int.TryParse(kvp.Value, out daysAfter);
                }
            }

            this.daysOpenAfterEnd = daysAfter;
            this.daysOpenBeforeEnd = daysBefore;

        }

        public ReportPeriod(int id, string name, DateTime start, DateTime end, int schoolid, int termid)
        {
            this.ID = id;
            this.name = name;
            this.startDate = start;
            this.endDate = end;
            this.schoolID = schoolid;
            this.termID = termid;
            this.marks = new List<Mark>();
            this.courses = new List<SchoolClass>();
            this.objectives = new List<Outcome>();
        }

        public override string ToString()
        {
            return "ReportPeriod: { ID: "+this.ID+", Name: " + this.name + ", SchoolID: "+this.schoolID+" Starts: "+this.startDate.ToLongDateString()+", Ends: "+this.endDate.ToLongDateString()+"}";
        }

        public static ReportPeriod loadThisReportPeriod(SqlConnection connection, int reportPeriodID)
        {
            ReportPeriod returnMe = null;

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM ReportPeriod WHERE iReportPeriodID=" + reportPeriodID + "";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe = new ReportPeriod(
                            int.Parse(dataReader["iReportPeriodID"].ToString().Trim()),
                            dataReader["cName"].ToString().Trim(),
                            DateTime.Parse(dataReader["dStartDate"].ToString()),
                            DateTime.Parse(dataReader["dEndDate"].ToString()),
                            int.Parse(dataReader["iSchoolID"].ToString().Trim()),
                            int.Parse(dataReader["iTermID"].ToString().Trim())
                            );
                }
            }

            sqlCommand.Connection.Close();
            returnMe.loadReportPeriodSettings(connection);
            return returnMe;

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
            foreach (ReportPeriod rp in returnMe)
            {
                rp.loadReportPeriodSettings(connection);
            }
            return returnMe;
        }

        public static List<ReportPeriod> loadReportPeriodsFromThisSchool(SqlConnection connection, School school)
        {
            List<ReportPeriod> returnMe = new List<ReportPeriod>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM ReportPeriod WHERE iSchoolID=" + school.getSchoolLogicID() + "";
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
            foreach (ReportPeriod rp in returnMe)
            {
                rp.loadReportPeriodSettings(connection);
            }
            return returnMe;
        }

        public static List<ReportPeriod> loadAllReportPeriods(SqlConnection connection)
        {
            List<ReportPeriod> returnMe = new List<ReportPeriod>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM ReportPeriod ORDER BY iSchoolID ASC, cName ASC;";
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
            foreach (ReportPeriod rp in returnMe)
            {
                rp.loadReportPeriodSettings(connection);
            }
            return returnMe;
        }

        public static List<ReportPeriod> loadReportPeriodsFromThisTerm(SqlConnection connection, Term term)
        {
            List<ReportPeriod> returnMe = new List<ReportPeriod>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM ReportPeriod WHERE iTermID=" + term.ID + "";
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
            foreach (ReportPeriod rp in returnMe)
            {
                rp.loadReportPeriodSettings(connection);
            }
            return returnMe;
        }


        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            ReportPeriod obj2 = obj as ReportPeriod;

            if (obj2 != null)
            {
                return this.startDate.CompareTo(obj2.startDate);
            }
            else
            {
                throw new ArgumentException("Object is not a ReportPeriod");
            }
        }
    }
}