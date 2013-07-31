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
        public string period { get; set; }
        public int track { get; set; }
        public AttendanceBlock attendanceBlock { get; set; }
        public DateTime blockStarttime { get; set; }
        public DateTime blockEndTime { get; set; }
        public bool excused { get; set; }

        
        /* TODO: Turn these all into properties */
        private DateTime date;
        private string studentID;
        private string courseName;
        private string courseID;
        private string status;
        private string reason;
        private string comment;
        private int minutes;
        private int block;
        

        public Absence(DateTime date, int track, string studentid, string courseName, string courseID, string status, string reason, string comment, int block, int minutes, bool? excused)
        {
            this.track = track;
            this.date = date;
            this.studentID = studentid;
            this.courseName = courseName;
            this.courseID = courseID;
            this.status = status;
            this.reason = reason;
            this.comment = comment;
            this.block = block;
            this.minutes = minutes;
            this.period = period;

            if (excused == true)
            {
                this.excused = true;
            }
            else
            {
                this.excused = false;
            }
        }
        /*
        public DateTime getStartTime()
        {
            return this.blockStarttime;
        }

        public DateTime getEndTime()
        {
            return this.blockEndTime;
        }
        */
        public DateTime getDate()
        {
            return this.date;
        }

        public string getPeriod()
        {
            return this.period;
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

        private static bool parseExcused(string thisThing) {

            if (String.IsNullOrEmpty(thisThing))
            {
                return false;
            }
            else
            {
                return bool.Parse(thisThing);
            }            
        }

        public static List<Absence> loadAbsencesForThisStudentAndTimePeriod(SqlConnection connection, Student student, DateTime start, DateTime end)
        {

            List<Absence> returnMe = new List<Absence>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;

            StringBuilder SQL = new StringBuilder();

            /* Load all attendance blocks, so we can reference them */
            List<AttendanceBlock> blocks = AttendanceBlock.loadAllAttendanceBlocks(connection);

            SQL.Append("SELECT * FROM LSKY_Attendance WHERE StudentNumber = '" + student.getStudentID() + "' AND dDate > '" + start + "' AND dDate < '" + end + "' ORDER BY dDate ASC, block ASC;");

            sqlCommand.CommandText = SQL.ToString();
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    Absence newAbsence = new Absence(
                        DateTime.Parse(dataReader["dDate"].ToString()),
                        int.Parse(dataReader["iTrackID"].ToString().Trim()),
                        dataReader["StudentNumber"].ToString().Trim(),
                        dataReader["ClassName"].ToString().Trim(),
                        dataReader["ClassID"].ToString().Trim(),
                        dataReader["Status"].ToString().Trim(),
                        dataReader["Reason"].ToString().Trim(),
                        dataReader["Comment"].ToString().Trim(),
                        int.Parse(dataReader["Block"].ToString()),
                        int.Parse(dataReader["Minutes"].ToString()),
                        parseExcused(dataReader["lExcusable"].ToString())
                        );

                    newAbsence.period = newAbsence.getBlock().ToString();

                    foreach (AttendanceBlock atBlock in blocks)
                    {
                        if (atBlock.block == newAbsence.block)
                        {
                            if (atBlock.track == newAbsence.track)
                            {
                                newAbsence.period = atBlock.name;
                                newAbsence.attendanceBlock = atBlock;
                            }
                        }
                    }

                    returnMe.Add(newAbsence);
                }
            }
            sqlCommand.Connection.Close();

            foreach (Absence abs in returnMe)
            {

            }

            return returnMe;
        }

        public static List<Absence> loadAbsencesForThisStudent(SqlConnection connection, Student student)
        {

            List<Absence> returnMe = new List<Absence>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;

            StringBuilder SQL = new StringBuilder();

            /* Load all attendance blocks, so we can reference them */
            List<AttendanceBlock> blocks = AttendanceBlock.loadAllAttendanceBlocks(connection);



            SQL.Append("SELECT * FROM LSKY_Attendance WHERE StudentNumber = '" + student.getStudentID() + "' ORDER BY dDate ASC, block ASC;");

            sqlCommand.CommandText = SQL.ToString();
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    Absence newAbsence = new Absence(
                        DateTime.Parse(dataReader["dDate"].ToString()),
                        int.Parse(dataReader["iTrackID"].ToString().Trim()),
                        dataReader["StudentNumber"].ToString().Trim(),
                        dataReader["ClassName"].ToString().Trim(),
                        dataReader["ClassID"].ToString().Trim(),
                        dataReader["Status"].ToString().Trim(),
                        dataReader["Reason"].ToString().Trim(),
                        dataReader["Comment"].ToString().Trim(),
                        int.Parse(dataReader["Block"].ToString()),
                        int.Parse(dataReader["Minutes"].ToString()),
                        parseExcused(dataReader["lExcusable"].ToString())
                        );

                    newAbsence.period = newAbsence.getBlock().ToString();

                    foreach (AttendanceBlock atBlock in blocks)
                    {
                        if (atBlock.block == newAbsence.block)
                        {
                            if (atBlock.track == newAbsence.track)
                            {
                                newAbsence.period = atBlock.name;
                                newAbsence.attendanceBlock = atBlock;
                            }
                        }
                    }

                    returnMe.Add(newAbsence);
                }
            }
            sqlCommand.Connection.Close();

            foreach (Absence abs in returnMe)
            {

            }

            return returnMe;
        }

        public static List<Absence> loadAbsencesForThisDate(SqlConnection connection, DateTime date)
        {

            List<Absence> returnMe = new List<Absence>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;

            StringBuilder SQL = new StringBuilder();

            /* Load all attendance blocks, so we can reference them */
            List<AttendanceBlock> blocks = AttendanceBlock.loadAllAttendanceBlocks(connection);



            SQL.Append("SELECT * FROM LSKY_Attendance WHERE dDate='" + date.Year + "-" + date.Month + "-" + date.Day + " 00:00:00' ORDER BY dDate ASC, block ASC;");

            sqlCommand.CommandText = SQL.ToString();
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    Absence newAbsence = new Absence(
                        DateTime.Parse(dataReader["dDate"].ToString()),
                        int.Parse(dataReader["iTrackID"].ToString().Trim()),
                        dataReader["StudentNumber"].ToString().Trim(),
                        dataReader["ClassName"].ToString().Trim(),
                        dataReader["ClassID"].ToString().Trim(),
                        dataReader["Status"].ToString().Trim(),
                        dataReader["Reason"].ToString().Trim(),
                        dataReader["Comment"].ToString().Trim(),
                        int.Parse(dataReader["Block"].ToString()),
                        int.Parse(dataReader["Minutes"].ToString()),
                        parseExcused(dataReader["lExcusable"].ToString())
                        );

                    newAbsence.period = newAbsence.getBlock().ToString();

                    foreach (AttendanceBlock atBlock in blocks)
                    {
                        if (atBlock.block == newAbsence.block)
                        {
                            if (atBlock.track == newAbsence.track)
                            {
                                newAbsence.period = atBlock.name;
                                newAbsence.attendanceBlock = atBlock;
                            }
                        }
                    }

                    returnMe.Add(newAbsence);
                }
            }
            sqlCommand.Connection.Close();

            foreach (Absence abs in returnMe)
            {

            }

            return returnMe;
        }

    }
}