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

        public string getGovID()
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
                        returnMe = new School(dbDataReader["name"].ToString(), dbDataReader["internalID"].ToString(), dbDataReader["govID"].ToString(), dbDataReader["address"].ToString());
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
                        returnMe.Add(new School(dbDataReader["name"].ToString(), dbDataReader["internalID"].ToString(), dbDataReader["govID"].ToString(), dbDataReader["address"].ToString()));
                    }
                }

                sqlCommand.Connection.Close();
            }
            catch { }
            returnMe.Sort();
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