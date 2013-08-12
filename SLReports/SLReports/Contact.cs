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

        public int priority { get; set; }
        public bool livesWithStudent { get; set; }
        public string houseNo { get; set; }
        public string street { get; set; }
        public string apartment { get; set; }
        public string postalCode { get; set; }
        public string city { get; set; }
        public string region { get; set; }

        public Contact(string fn, string ln, string rel, string snumber, string telephone, int priority, bool livesWith, 
            string housenumber, string street, string apt, string postalcode, string city, string region)
        {
            this.firstName = fn;
            this.lastName = ln;
            this.studentNumber = snumber;
            this.relation = rel;
            this.telephone = telephone;

            this.priority = priority;
            this.livesWithStudent = livesWith;
            this.houseNo = housenumber;
            this.street = street;
            this.apartment = apt;
            this.postalCode = postalcode;
            this.city = city;
            this.region = region;
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
                    bool livesWithStudent = false;
                    bool.TryParse(dataReader["lLivesWithStudent"].ToString().Trim(), out livesWithStudent);

                    Contact newContact = new Contact(
                        dataReader["FirstName"].ToString().Trim(),
                        dataReader["LastName"].ToString().Trim(),
                        dataReader["Relation"].ToString().Trim(),
                        dataReader["StudentNumber"].ToString().Trim(),
                        dataReader["Telephone"].ToString().Trim(),
                        int.Parse(dataReader["iContactPriority"].ToString().Trim()),
                        livesWithStudent,
                        dataReader["cHouseNo"].ToString().Trim(),
                        dataReader["cStreet"].ToString().Trim(),
                        dataReader["cApartment"].ToString().Trim(),
                        dataReader["cPostalCode"].ToString().Trim(),
                        dataReader["City"].ToString().Trim(),
                        dataReader["Region"].ToString().Trim()
                        );

                    returnMe.Add(newContact);
                }
            }
            sqlCommand.Connection.Close();
            return returnMe;
        }
    }
}