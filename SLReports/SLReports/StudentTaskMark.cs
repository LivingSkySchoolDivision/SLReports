using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class StudentTaskMark
    {
        public int id { get; set; }
        public string studentID { get; set; }
        public decimal mark { get; set; }
        public string note { get; set; }
        public StudentTask task { get; set; }

        public StudentTaskMark(int id, string studentID, decimal mark, string note, StudentTask task)
        {
            this.task = task;
            this.id = id;
            this.studentID = studentID;
            this.mark = mark;
            this.note = note;
        }

        public static List<StudentTaskMark> loadTaskMarksFromThisSchool(SqlConnection connection, School school)
        {
            List<StudentTaskMark> returnMe = new List<StudentTaskMark>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_TaskMarks WHERE SchoolGovID=@SCHOOLGOVID ORDER BY ClassName ASC, cLastName ASC, cFirstName ASC";
            sqlCommand.Parameters.AddWithValue("@SCHOOLGOVID", school.getGovIDAsString());
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    SLReports.StudentTask.GradeType gradetype = StudentTask.GradeType.Numeric;
                    if (dataReader["GradeType"].ToString().Trim().ToLower() == "alpha") 
                    {
                        gradetype = StudentTask.GradeType.Alpha;
                    } else if (dataReader["GradeType"].ToString().Trim() == "numeric") 
                    {
                        gradetype = StudentTask.GradeType.Numeric;
                    }

                    returnMe.Add(new StudentTaskMark(
                            int.Parse(dataReader["TaskMarkID"].ToString().Trim()),
                            dataReader["cStudentNumber"].ToString().Trim(),
                            decimal.Parse(dataReader["fGrade"].ToString().Trim()),
                            dataReader["mNote"].ToString().Trim(),
                            new StudentTask(
                                int.Parse(dataReader["iTaskID"].ToString().Trim()),
                                dataReader["cTaskName"].ToString().Trim(),
                                gradetype, 
                                DateTime.Parse(dataReader["dTaskDate"].ToString().Trim()),
                                int.Parse(dataReader["iClassID"].ToString().Trim()),
                                dataReader["ClassName"].ToString().Trim(),
                                dataReader["SchoolGovID"].ToString().Trim(),
                                dataReader["SchoolName"].ToString().Trim()
                                )
                            ));
                }
            }

            dataReader.Close();
            sqlCommand.Connection.Close();
            return returnMe;
        }
        

    }

    
}