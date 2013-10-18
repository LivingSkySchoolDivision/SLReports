using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class School : IComparable
    {
        private string name;
        private string schoolLogicID;
        private string govID;

        public string telephone { get; set; }
        public string email { get; set; }
        public string website { get; set; }
        public string address { get; set; }

        public string getName()
        {
            return this.name;
        }

        public string getSchoolLogicID()
        {
            return this.schoolLogicID;
        }

        public int getGovID()
        {
            int returnMe = -1;
            if (int.TryParse(this.govID, out returnMe))
            {
                return returnMe;
            }
            return -1;
        }

        public string getGovIDAsString()
        {
            return this.govID;
        }

        public override string ToString()
        {
            return this.name + "(" + govID + ")";
        }

        /*
        public School(string name, string slid, string govid)
        {
            this.name = name;
            this.schoolLogicID = slid;
            this.govID = govid;
        }
        */

        public School(string name, string slid, string govid, string address)
        {
            this.name = name;
            this.schoolLogicID = slid;
            this.govID = govid;
            this.address = address;
        }

        public School(string name, string slid, string govid, string phone, string email, string website)
        {
            this.name = name;
            this.schoolLogicID = slid;
            this.govID = govid;
        }


        public static School loadThisSchool(SqlConnection connection, int schoolGovID)
        {
            School returnMe = null;

            try
            {
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = connection;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = "SELECT * FROM LSKY_LSKYSchools WHERE govID=" + schoolGovID + ";";
                sqlCommand.Connection.Open();
                SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                if (dbDataReader.HasRows)
                {
                    while (dbDataReader.Read())
                    {
                        returnMe = new School(dbDataReader["name"].ToString(), dbDataReader["internalID"].ToString(), dbDataReader["govID"].ToString(), dbDataReader["address"].ToString());
                    }
                }

                sqlCommand.Connection.Close();
            }
            catch { }

            return returnMe;
        }

        public static School loadThisSchool(SqlConnection connection, string schoolGovID)
        {
            School returnMe = null;

            try
            {
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = connection;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = "SELECT * FROM LSKY_LSKYSchools WHERE govID=@GOVID;";
                sqlCommand.Parameters.AddWithValue("@GOVID", schoolGovID);
                sqlCommand.Connection.Open();
                SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                if (dbDataReader.HasRows)
                {
                    while (dbDataReader.Read())
                    {
                        returnMe = new School(dbDataReader["name"].ToString(), dbDataReader["internalID"].ToString(), dbDataReader["govID"].ToString(), dbDataReader["address"].ToString());
                    }
                }

                sqlCommand.Connection.Close();
            }
            catch { }

            return returnMe;
        }

        public static School loadThisSchoolByDatabaseID(SqlConnection connection, int schoolID)
        {
            School returnMe = null;

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_LSKYSchools WHERE internalID=" + schoolID + ";";
            sqlCommand.Connection.Open();
            SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

            if (dbDataReader.HasRows)
            {
                while (dbDataReader.Read())
                {
                    returnMe = new School(dbDataReader["name"].ToString(), dbDataReader["internalID"].ToString(), dbDataReader["govID"].ToString(), dbDataReader["address"].ToString());
                }
            }

            sqlCommand.Connection.Close();

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
                        returnMe.Add(new School(dbDataReader["name"].ToString(), dbDataReader["internalID"].ToString(), dbDataReader["govID"].ToString(), dbDataReader["address"].ToString()));
                    }
                }

                sqlCommand.Connection.Close();
            }
            catch { }
            returnMe.Sort();
            return returnMe;
        }

        /// <summary>
        /// Loads all of the settings for a school
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> loadSchoolSettings(SqlConnection connection, int SchoolID)
        {
            Dictionary<string, string> returnMe = new Dictionary<string, string>();
            try
            {
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = connection;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = "SELECT * FROM Settings WHERE iSchoolID=@SCHOOLID;";
                sqlCommand.Parameters.AddWithValue("@SCHOOLID", SchoolID);
                sqlCommand.Connection.Open();
                SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                if (dbDataReader.HasRows)
                {
                    while (dbDataReader.Read())
                    {
                        returnMe.Add(dbDataReader["cKey"].ToString(), dbDataReader["cValue"].ToString());                            
                    }
                }

                sqlCommand.Connection.Close();
            }
            catch { }            
            return returnMe;
        }


        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            School obj2 = obj as School;

            if (obj2 != null)
            {
                return this.name.CompareTo(obj2.name);
            }
            else
            {
                throw new ArgumentException("Object is not a School");
            }
        }
    }
}