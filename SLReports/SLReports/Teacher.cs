using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace SLReports
{
    public class Teacher
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string title { get; set; }

        public string displayName
        {
            get {
                StringBuilder returnMe = new StringBuilder();
                if (!string.IsNullOrEmpty(title))
                {
                    returnMe.Append(title + " ");
                }
                returnMe.Append(firstName.Substring(0,1));
                returnMe.Append(" ");
                returnMe.Append(lastName);

                return returnMe.ToString();                    
            }

            set {
                // Do nothing, because you should never set this
                throw new Exception("This value is not writable");
            }
        }

        public Teacher(string firstname, string lastname, string title)
        {
            this.firstName = firstname;
            this.lastName = lastname;
            this.title = title;
        }


        public static List<Teacher> loadTeachersForThisClass(SqlConnection connection, SchoolClass thisclass)
        {
            return loadTeachersForThisClass(connection, thisclass.classid);
        }

        public static List<Teacher> loadTeachersForThisClass(SqlConnection connection, int classID)
        {
            List<Teacher> returnMe = new List<Teacher>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_ClassTeachers WHERE iClassID=@CLASSID";
            sqlCommand.Parameters.AddWithValue("@CLASSID", classID);
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    Teacher newTeacher = new Teacher(
                            dataReader["cFirstName"].ToString().Trim(),
                            dataReader["cLastName"].ToString().Trim(),
                            dataReader["cTitle"].ToString().Trim()
                            );
                    returnMe.Add(newTeacher);
                }
            }
            sqlCommand.Connection.Close();            
            return returnMe;
        }


        public override string ToString()
        {
            return this.displayName;
        }



    }
}