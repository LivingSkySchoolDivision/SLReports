using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SLReports.SaskSubmitted
{
    public class SkSubmittedEntry
    {
        public int id { get; set; }
        public string submittedID { get; set; }
        public int schoolID { get; set; }
        public string govID { get; set; }
        public bool failed { get; set; }
        public DateTime SubmittedDate { get; set; } //dsubdate
        public DateTime DateOfBirth { get; set; }
        public string StudentNumber { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string gender { get; set; }
        
        public SkSubmittedEntry(int id, bool failed, DateTime submitted, string submittedid, int schoolid, string govid, string studentnumber, string firstname, string lastname, string gender, DateTime dateOfBirth)
        {
            this.failed = failed;
            this.id = id;
            this.submittedID = submittedid;
            this.schoolID = schoolid;
            this.govID = govid;
            this.StudentNumber = studentnumber;
            this.firstName = firstname;
            this.lastName = lastname;
            this.gender = gender;

            this.SubmittedDate = submitted;
            this.DateOfBirth = dateOfBirth;
        }


        public static List<SkSubmittedEntry> loadAllSaskSubmittedEntries(SqlConnection connection)
        {
            List<SkSubmittedEntry> returnMe = new List<SkSubmittedEntry>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM SK_Submitted ORDER BY ISCHOOLID ASC, DSUBDATE ASC;";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    bool failed = false;
                    if (!bool.TryParse(dataReader["lFailed"].ToString().Trim(), out failed))
                    {
                        failed = false;
                    }

                    returnMe.Add(new SkSubmittedEntry(
                            int.Parse(dataReader["iSK_SubmittedID"].ToString()),
                            failed,
                            DateTime.Parse(dataReader["DSUBDATE"].ToString().Trim()), 
                            dataReader["CSUBMITTEDID"].ToString().Trim(),
                            int.Parse(dataReader["ISCHOOLID"].ToString()),
                            dataReader["CGOVID"].ToString().Trim(),
                            dataReader["CSTUDENTNUMBER"].ToString().Trim(),
                            dataReader["CLEGALFIRSTNM"].ToString().Trim(),
                            dataReader["CLEGALLASTNM"].ToString().Trim(),
                            dataReader["CGENDER"].ToString().Trim(),
                            DateTime.Parse(dataReader["DDOB"].ToString().Trim())
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }

        public static List<SkSubmittedEntry> loadEntriesFromThisSchool(SqlConnection connection, int SchoolID)
        {
            List<SkSubmittedEntry> returnMe = new List<SkSubmittedEntry>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM SK_Submitted WHERE iSchoolID=@SCHOOLID ORDER BY ISCHOOLID ASC, DSUBDATE ASC;";
            sqlCommand.Parameters.AddWithValue("@SCHOOLID", SchoolID);
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    bool failed = false;
                    if (!bool.TryParse(dataReader["lFailed"].ToString().Trim(), out failed))
                    {
                        failed = false;
                    }

                    returnMe.Add(new SkSubmittedEntry(
                            int.Parse(dataReader["iSK_SubmittedID"].ToString()),
                            failed,
                            DateTime.Parse(dataReader["DSUBDATE"].ToString().Trim()),
                            dataReader["CSUBMITTEDID"].ToString().Trim(),
                            int.Parse(dataReader["ISCHOOLID"].ToString()),
                            dataReader["CGOVID"].ToString().Trim(),
                            dataReader["CSTUDENTNUMBER"].ToString().Trim(),
                            dataReader["CLEGALFIRSTNM"].ToString().Trim(),
                            dataReader["CLEGALLASTNM"].ToString().Trim(),
                            dataReader["CGENDER"].ToString().Trim(),
                            DateTime.Parse(dataReader["DDOB"].ToString().Trim())
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }


    }
}