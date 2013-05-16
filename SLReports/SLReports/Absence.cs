using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace SLReports
{
    public class Absence : IComparable
    {
        private DateTime date;
        private string studentID;
        private string courseName;
        private string courseID;
        private string status;
        private string reason;
        private string comment;
        private int minutes;
        private int block;
        private DateTime blockStarttime;
        private DateTime blockEndTime;

        public Absence(DateTime date, string studentid, string courseName, string courseID, string status, string reason, string comment, int block, DateTime bStarttime, DateTime bEndTime, int minutes)
        {
            this.date = date;
            this.studentID = studentid;
            this.courseName = courseName;
            this.courseID = courseID;
            this.status = status;
            this.reason = reason;
            this.comment = comment;
            this.block = block;
            this.blockStarttime = bStarttime;
            this.blockEndTime = bEndTime;
            this.minutes = minutes;
        }

        public DateTime getStartTime()
        {
            return this.blockStarttime;
        }

        public DateTime getEndTime()
        {
            return this.blockEndTime;
        }

        public DateTime getDate()
        {
            return this.date;
        }

        public string getStudentID()
        {
            return this.studentID;
        }

        public int getMinutes() 
        {
            return this.minutes;
        }

        public int getStudentIDAsInt()
        {
            int returnMe = 0;
            int.TryParse(this.studentID, out returnMe);
            return returnMe;
        }

        public string getCourseName()
        {
            return this.courseName;
        }

        public string getCourseID()
        {
            return this.courseID;
        }

        public int getCourseIDAsInt()
        {
            int returnMe = 0;
            int.TryParse(this.courseID, out returnMe);
            return returnMe;
        }

        public string getStatus()
        {
            return this.status;
        }

        public string getReason()
        {
            return this.reason;
        }

        public int getBlock()
        {
            return this.block;
        }

        public string getComment()
        {
            return this.comment;
        }

        public override string ToString()
        {
            return this.date.ToShortDateString() + "(Block: " + this.block + ") (Course: " + this.courseName + ") (Status :" + this.status + ") (Reason :" + this.reason + ")";
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            Absence obj2 = obj as Absence;

            if (obj2 != null)
            {
                return this.getDate().CompareTo(obj2.getDate());
            }
            else
            {
                throw new ArgumentException("Object is not a Student");
            }
        }

        public List<Absence> loadAbsencesForThisStudent(SqlConnection connection, Student student)
        {

            List<Absence> returnMe = new List<Absence>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            
            StringBuilder SQL = new StringBuilder();
            /*
            SQL.Append("SELECT * FROM LSKY_Attendance WHERE (dDate BETWEEN '" + startDate.ToShortDateString() + "' AND '" + endDate.ToShortDateString() + "') AND (");
            foreach (Student student in AllStudents)
            {
                SQL.Append("(StudentNumber = '" + student.getStudentID() + "') OR ");
            }
             * */
            SQL.Remove(SQL.Length - 4, 4);
            SQL.Append(") ORDER BY dDate ASC, tStartTime ASC;");
            sqlCommand.CommandText = SQL.ToString();
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe.Add(new Absence(
                        DateTime.Parse(dataReader["dDate"].ToString()),
                        dataReader["StudentNumber"].ToString().Trim(),
                        dataReader["ClassName"].ToString().Trim(),
                        dataReader["ClassID"].ToString().Trim(),
                        dataReader["Status"].ToString().Trim(),
                        dataReader["Reason"].ToString().Trim(),
                        dataReader["Comment"].ToString().Trim(),
                        int.Parse(dataReader["Block"].ToString()),
                        DateTime.Parse(dataReader["tStartTime"].ToString()),
                        DateTime.Parse(dataReader["tEndTime"].ToString()),
                        int.Parse(dataReader["Minutes"].ToString())
                        ));
                }
            }
            sqlCommand.Connection.Close();
            return returnMe;
        }

    }
}