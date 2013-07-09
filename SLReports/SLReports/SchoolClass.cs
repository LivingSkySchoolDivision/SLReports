using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace SLReports
{
    public class SchoolClass : IComparable
    {
        public string name { get; set; }
        public int courseid { get; set; }
        public int classid { get; set; }
        public int trackID { get; set; }
        public int enrollmentCount { get; set; }
        public string teacherFirstName { get; set; }
        public string teacherLastName { get; set; }
        public string teacherTitle { get; set; }
        public string mark { get; set; }
        public string schoolName { get; set; }
        public Track track { get; set; }
        public List<Mark> Marks { get; set; }
        public List<Objective> Objectives { get; set; }
        public List<ObjectiveMark> ObjectiveMarks { get; set; }
        public List<Student> EnrolledStudents { get; set; }

        public List<ReportPeriod> ReportPeriods { get; set; }

        public bool hasObjectives()
        {
            if (this.Objectives.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string teacherName
        {
            get
            {
                StringBuilder returnMe = new StringBuilder();
                if (!string.IsNullOrEmpty(teacherTitle))
                {
                    returnMe.Append(teacherTitle + " ");
                }
                else
                {
                    returnMe.Append(teacherFirstName + " ");
                }

                returnMe.Append(teacherLastName);
                return returnMe.ToString();
            }

            set {}
        }

        public SchoolClass(string name, int classid, int courseid, string teacherFirst, string teacherLast, string teacherTitle)
        {
            Objectives = new List<Objective>();
            Marks = new List<Mark>();
            ReportPeriods = new List<ReportPeriod>();
            ObjectiveMarks = new List<ObjectiveMark>();

            this.name = name;
            this.classid = classid;
            this.courseid = courseid;
            this.teacherFirstName = teacherFirst;
            this.teacherLastName = teacherLast;
            this.teacherTitle = teacherTitle;
        }

        public SchoolClass(string name, int classid, int courseid, string teacherFirst, string teacherLast, string teacherTitle, string schoolName, Track track)
        {
            Objectives = new List<Objective>();
            Marks = new List<Mark>();
            ReportPeriods = new List<ReportPeriod>();
            ObjectiveMarks = new List<ObjectiveMark>();

            this.name = name;
            this.classid = classid;
            this.courseid = courseid;
            this.teacherFirstName = teacherFirst;
            this.teacherLastName = teacherLast;
            this.teacherTitle = teacherTitle;
            this.schoolName = schoolName;

            this.track = track;
        }


        public static List<SchoolClass> loadAllClasses(SqlConnection connection)
        {
            List<SchoolClass> returnMe = new List<SchoolClass>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_Classes ORDER BY SchoolName ASC, cName ASC, iClassID ASC;";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {

                    bool daily = false;
                    if (!String.IsNullOrEmpty(dataReader["TrackDaily"].ToString().Trim()))
                    {
                        daily = bool.Parse(dataReader["TrackDaily"].ToString().Trim());
                    }

                    Track newTrack = new Track(
                            int.Parse(dataReader["TrackID"].ToString().Trim()),
                            dataReader["TrackName"].ToString().Trim(),
                            DateTime.Parse(dataReader["TrackStart"].ToString().Trim()),
                            DateTime.Parse(dataReader["TrackEnd"].ToString().Trim()),
                            int.Parse(dataReader["SchoolID"].ToString().Trim()),
                            daily);

                    returnMe.Add(new SchoolClass(
                            dataReader["cName"].ToString().Trim(),
                            int.Parse(dataReader["iClassID"].ToString().Trim()),
                            int.Parse(dataReader["iCourseID"].ToString().Trim()),
                            dataReader["TeacherFirstName"].ToString().Trim(),
                            dataReader["TeacherLastName"].ToString().Trim(),
                            dataReader["TeacherTitle"].ToString().Trim(),
                            dataReader["SchoolName"].ToString().Trim(),
                            newTrack
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

            SchoolClass obj2 = obj as SchoolClass;

            if (obj2 != null)
            {
                return this.name.CompareTo(obj2.name);
            }
            else
            {
                throw new ArgumentException("Object is not a SchoolClass");
            }
        }
    }
}