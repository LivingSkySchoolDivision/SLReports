using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class School
    {
        private string name;
        private string schoolLogicID;
        private string govID;

        public string getName()
        {
            return this.name;
        }
        public string getSchoolLogicID()
        {
            return this.schoolLogicID;
        }
        public string getGovID()
        {
            return this.govID;
        }

        public School(string name, string slid, string govid)
        {
            this.name = name;
            this.schoolLogicID = slid;
            this.govID = govid;
        }


        public static School loadThisSchool(SqlConnection connection, int schoolID)
        {
            School returnMe = null;

            try
            {

                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = connection;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = "SELECT * FROM LSKY_LSKYSchools WHERE govID="+schoolID+";";
                sqlCommand.Connection.Open();
                SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                if (dbDataReader.HasRows)
                {
                    while (dbDataReader.Read())
                    {
                        returnMe = new School(dbDataReader["name"].ToString(), dbDataReader["internalID"].ToString(), dbDataReader["govID"].ToString());
                    }
                }

                sqlCommand.Connection.Close();
            }
            catch { }

            return returnMe;
        }

        public static List<School> loadAllSchools(SqlConnection connection)
        {
            List<School> returnMe = new List<School>();

            try
            {

                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = connection;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = "SELECT * FROM LSKY_LSKYSchools;";
                sqlCommand.Connection.Open();
                SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                if (dbDataReader.HasRows)
                {
                    while (dbDataReader.Read())
                    {
                        returnMe.Add(new School(dbDataReader["name"].ToString(), dbDataReader["internalID"].ToString(), dbDataReader["govID"].ToString()));
                    }
                }

                sqlCommand.Connection.Close();
            }
            catch { }

            return returnMe;
        }

    }
}