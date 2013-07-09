using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class StudentEnrollmentEntry : IComparable
    {
        public int studentID { get; set; }
        public int classID { get; set; }

        public StudentEnrollmentEntry(int studentID, int classID)
        {
            this.studentID = studentID;
            this.classID = classID;
        }

        public static List<StudentEnrollmentEntry> loadAllStudentEnrollment(SqlConnection connection)
        {
            List<StudentEnrollmentEntry> returnMe = new List<StudentEnrollmentEntry>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM Enrollment;";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe.Add(new StudentEnrollmentEntry(
                            int.Parse(dataReader["iStudentID"].ToString().Trim()),
                            int.Parse(dataReader["iClassID"].ToString().Trim())
                        ));
                }
            }

            sqlCommand.Connection.Close();

            returnMe.Sort();
            return returnMe;
        }


        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            StudentEnrollmentEntry obj2 = obj as StudentEnrollmentEntry;

            if (obj2 != null)
            {
                return this.studentID.CompareTo(obj2.studentID);
            }
            else
            {
                throw new ArgumentException("Object is not a StudentEnrollmentEntry");
            }
        }
    }
}