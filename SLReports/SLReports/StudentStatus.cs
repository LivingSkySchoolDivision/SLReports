using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace SLReports
{
    public class StudentStatus
    {
        public int id { get; set; }
        public string studentNumber { get; set; }
        public DateTime inDate { get; set; }
        public DateTime outDate { get; set; }
        public string schoolName { get; set; }
        public int schoolNumber { get; set; }
        public string inStatus { get; set; }
        public string outStatus { get; set; }
        public bool hasInStatus { get; set; }
        public bool hasOutStatus { get; set; }


        public StudentStatus(int statusID, string studentNumber, string schoolName, int schoolNumber, bool hasInStatus, DateTime inDate, string inStatus, bool hasOutStatus, DateTime outDate, string outStatus)
        {
            this.id = statusID;
            this.studentNumber = studentNumber;
            this.schoolName = schoolName;
            this.schoolNumber = schoolNumber;
            this.inDate = inDate;
            this.inStatus = inStatus;
            this.outDate = outDate;
            this.outStatus = outStatus;
        }

        public override string ToString()
        {
            return "StudentStatus: {ID: " + this.id + ", Student: " + this.studentNumber + ", School: " + this.schoolName + ", inDate: " + this.inDate + ", inStatus: " + this.inStatus + ", outDate: " + this.outDate + ", outStatus: " + this.outStatus + "}";
        }


        public static List<StudentStatus> loadAllStudentStatuses(SqlConnection connection)
        {
            List<StudentStatus> returnMe = new List<StudentStatus>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;

            StringBuilder SQL = new StringBuilder();

            /* Load all attendance blocks, so we can reference them */
            List<AttendanceBlock> blocks = AttendanceBlock.loadAllAttendanceBlocks(connection);

            SQL.Append("SELECT * FROM LSKY_StudentStatuses;");

            sqlCommand.CommandText = SQL.ToString();
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    // Parse the instatus
                    bool hasInDate = false;
                    DateTime inDate = DateTime.MinValue;
                    if (dataReader["dInDate"].ToString() != "1900-01-01 00:00:00")
                    {
                        if (DateTime.TryParse(dataReader["dInDate"].ToString(), out inDate))
                        {
                            if (inDate != DateTime.MinValue)
                            {
                                hasInDate = true;
                            }
                        }
                    }

                    // Parse the outstatus
                    bool hasOutDate = false;
                    DateTime OutDate = DateTime.MinValue;
                    if (dataReader["dOutDate"].ToString() != "1900-01-01 00:00:00")
                    {
                        if (DateTime.TryParse(dataReader["dOutDate"].ToString(), out OutDate))
                        {
                            if (OutDate != DateTime.MinValue)
                            {
                                hasOutDate = true;
                            }
                        }
                    }

                    StudentStatus newStatus = new StudentStatus(
                        int.Parse(dataReader["iStudentStatusID"].ToString()),
                        dataReader["cStudentNumber"].ToString(),
                        dataReader["SchoolName"].ToString(),
                        int.Parse(dataReader["SchoolNumber"].ToString()),
                        hasInDate,
                        inDate,
                        dataReader["InStatus"].ToString(),
                        hasOutDate,
                        OutDate,
                        dataReader["OutStatus"].ToString()
                        );

                    returnMe.Add(newStatus);
                }
            }
            sqlCommand.Connection.Close();
            return returnMe;
        }

        public static List<StudentStatus> loadStatusesForThisStudent(SqlConnection connection, Student student)
        {
            List<StudentStatus> returnMe = new List<StudentStatus>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;

            StringBuilder SQL = new StringBuilder();

            /* Load all attendance blocks, so we can reference them */
            List<AttendanceBlock> blocks = AttendanceBlock.loadAllAttendanceBlocks(connection);

            SQL.Append("SELECT * FROM LSKY_StudentStatuses WHERE cStudentNumber = '" + student.getStudentID() + "';");

            sqlCommand.CommandText = SQL.ToString();
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    // Parse the instatus
                    bool hasInDate = false;
                    DateTime inDate;
                    if (DateTime.TryParse(dataReader["dInDate"].ToString(), out inDate)) 
                    {
                        hasInDate = true;                        
                    }

                    // Parse the outstatus
                    bool hasOutDate = false;
                    DateTime OutDate;
                    if (DateTime.TryParse(dataReader["dOutDate"].ToString(), out OutDate))
                    {
                        hasOutDate = true;
                    }

                    StudentStatus newStatus = new StudentStatus(
                        int.Parse(dataReader["iStudentStatusID"].ToString()),
                        dataReader["cStudentNumber"].ToString(),
                        dataReader["SchoolName"].ToString(),
                        int.Parse(dataReader["SchoolNumber"].ToString()),
                        hasInDate,
                        inDate,
                        dataReader["InStatus"].ToString(),
                        hasOutDate,
                        OutDate,
                        dataReader["OutStatus"].ToString()
                        );

                    returnMe.Add(newStatus);
                }
            }
            sqlCommand.Connection.Close();
            return returnMe;
        }

        public static List<StudentStatus> loadStatusesWithNoOutstatusForThisStudent(SqlConnection connection, Student student)
        {
            List<StudentStatus> returnMe = new List<StudentStatus>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;

            StringBuilder SQL = new StringBuilder();

            /* Load all attendance blocks, so we can reference them */
            List<AttendanceBlock> blocks = AttendanceBlock.loadAllAttendanceBlocks(connection);

            SQL.Append("SELECT * FROM LSKY_StudentStatuses WHERE cStudentNumber = '" + student.getStudentID() + "' AND OutStatus is null;");

            sqlCommand.CommandText = SQL.ToString();
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    // Parse the instatus
                    bool hasInDate = false;
                    DateTime inDate;
                    if (DateTime.TryParse(dataReader["dInDate"].ToString(), out inDate))
                    {
                        hasInDate = true;
                    }

                    // Parse the outstatus
                    bool hasOutDate = false;
                    DateTime OutDate;
                    if (DateTime.TryParse(dataReader["dOutDate"].ToString(), out OutDate))
                    {
                        hasOutDate = true;
                    }

                    StudentStatus newStatus = new StudentStatus(
                        int.Parse(dataReader["iStudentStatusID"].ToString()),
                        dataReader["cStudentNumber"].ToString(),
                        dataReader["SchoolName"].ToString(),
                        int.Parse(dataReader["SchoolNumber"].ToString()),
                        hasInDate,
                        inDate,
                        dataReader["InStatus"].ToString(),
                        hasOutDate,
                        OutDate,
                        dataReader["OutStatus"].ToString()
                        );

                    returnMe.Add(newStatus);
                }
            }
            sqlCommand.Connection.Close();
            return returnMe;
        }
        
    }
}