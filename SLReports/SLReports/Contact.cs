using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace SLReports
{
    public class Contact
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string studentNumber { get; set; }
        public string relation { get; set; }
        public string telephone { get; set; }


        public Contact(string fn, string ln, string rel, string snumber)
        {
            this.firstName = fn;
            this.lastName = ln;
            this.studentNumber = snumber;
            this.relation = rel;
        }

        public Contact(string fn, string ln, string rel, string snumber, string telephone)
        {
            this.firstName = fn;
            this.lastName = ln;
            this.studentNumber = snumber;
            this.relation = rel;
            this.telephone = telephone;
        }

        public override string ToString()
        {
            return firstName + " " + lastName + " ("+relation+")";
        }

        public string getStudentID()
        {
            return studentNumber;
        }

        public static List<Contact> loadContactsForStudent(SqlConnection connection, Student student)
        {
            List<Contact> returnMe = new List<Contact>();
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            
            
            StringBuilder SQL = new StringBuilder();
            SQL.Append("SELECT * FROM LSKY_Contacts WHERE StudentNumber = '" + student.getStudentID() + "';");
            sqlCommand.CommandText = SQL.ToString();
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                returnMe.Clear();
                while (dataReader.Read())
                {
                    returnMe.Add(new Contact(
                        dataReader["FirstName"].ToString().Trim(),
                        dataReader["LastName"].ToString().Trim(),
                        dataReader["Relation"].ToString().Trim(),
                        dataReader["StudentNumber"].ToString().Trim(),
                        dataReader["Telephone"].ToString().Trim()
                        
                        ));
                }
            }
            sqlCommand.Connection.Close();
            return returnMe;
        }
    }
}