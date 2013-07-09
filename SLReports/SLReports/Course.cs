using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class Course : IComparable
    {
        public int id { get;set; }
        public int governmentCourseID { get; set; }

        public string name { get; set; }
        public string courseCode { get; set; }
        public string school { get; set; }
        public string governmentCode { get; set; }
        
        public bool offeredInSchool { get; set; }
        public bool schoolExam { get; set; }

        public Course(int id, int govID, string name, string courseCode, string school, string govCode, bool offeredInSchool, bool schoolExam)
        {
            this.id = id;
            this.governmentCode = govCode;
            this.governmentCourseID = govID;
            this.name = name;
            this.courseCode = courseCode;
            this.school = school;
            this.offeredInSchool = offeredInSchool;
            this.schoolExam = schoolExam;
        }

        public static List<Course> loadAllCourses(SqlConnection connection)
        {
            List<Course> returnMe = new List<Course>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_Courses;";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    bool offered = false;
                    if (!string.IsNullOrEmpty(dataReader["lOfferedInSchool"].ToString().Trim())) {
                        offered = bool.Parse(dataReader["lOfferedInSchool"].ToString().Trim());
                    }

                    bool exam = false;
                    if (!string.IsNullOrEmpty(dataReader["lSchoolExam"].ToString().Trim())) {
                        exam = bool.Parse(dataReader["lSchoolExam"].ToString().Trim());
                    }

                    returnMe.Add(new Course(
                            int.Parse(dataReader["iCourseID"].ToString().Trim()),
                            int.Parse(dataReader["iGovCourseID"].ToString().Trim()),
                            dataReader["cName"].ToString().Trim(),
                            dataReader["cCourseCode"].ToString().Trim(),
                            dataReader["SchoolName"].ToString().Trim(),
                            dataReader["cGovernmentCode"].ToString().Trim(),
                            offered,
                            exam
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

            Course obj2 = obj as Course;

            if (obj2 != null)
            {
                return this.name.CompareTo(obj2.name);
            }
            else
            {
                throw new ArgumentException("Object is not a Course");
            }
        }
    }
}