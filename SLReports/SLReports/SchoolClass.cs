﻿using System;
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
        public int blockNumber { get; set; }
        public int dayNumber { get; set; }
        public string mark { get; set; }
        public string schoolName { get; set; }
        public string lowestGrade { get; set; }
        public string highestGrade { get; set; }
        public string gradeLegend { get; set; }

        public Track track { get; set; }
        public Term term { get; set; }
        public List<Mark> Marks { get; set; }
        public List<Outcome> Outcomes { get; set; }
        public List<OutcomeMark> OutcomeMarks { get; set; }
        public List<Student> EnrolledStudents { get; set; }
        public List<Teacher> teachers { get; set; }

        public List<ReportPeriod> ReportPeriods { get; set; }

        public bool hasObjectives()
        {
            if (this.Outcomes.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool isHighSchoolLevel()
        {
            if ((this.getGradeLevel() <= 12) && (this.getGradeLevel() >= 10))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool hasGradeLegend()
        {
            if (string.IsNullOrEmpty(gradeLegend)) {
                return false;
            } else {
                return true;
            }
        }

        public bool isOutcomeBased()
        {
            if (this.hasGradeLegend())
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
                for (int x = 0; x < teachers.Count; x++ )
                {
                    returnMe.Append(teachers[x].displayName);
                    if (x < teachers.Count-1)
                    {
                        returnMe.Append(", ");
                    }
                }
                
                return returnMe.ToString();
            }

            set {}
        }

        public int getGradeLevel()
        {
            int lowGradeNumber;

            if (int.TryParse(this.lowestGrade, out lowGradeNumber))
            {
                return lowGradeNumber;
            }

            return 0;
        }

        public override string ToString()
        {
            return "Class: { Name: " + this.name + ", ClassID: " + this.classid + ", CourseID: " + this.courseid + ", Block: " + this.blockNumber + ", Day: " + this.dayNumber + ", Has Objectives: " + this.Outcomes.Count + ", IsHighSchool: " + LSKYCommon.boolToYesOrNo(this.isHighSchoolLevel()) + ", LowGrade: " + this.lowestGrade + ", HighGrade: " + this.highestGrade + ", Translated grade: " + this.getGradeLevel() + ", Grade Legend: " + this.gradeLegend + "}";
        }
        
        /* This constructor should be removed and code relying on it should be redone */
        public SchoolClass(string name, int classid, int courseid)
        {
            Outcomes = new List<Outcome>();
            Marks = new List<Mark>();
            ReportPeriods = new List<ReportPeriod>();
            OutcomeMarks = new List<OutcomeMark>();

            this.name = name;
            this.classid = classid;
            this.courseid = courseid;
        }
        
        public SchoolClass(string name, int classid, int courseid, string teacherFirst, string teacherLast, string teacherTitle, string schoolName, int blockNum, int dayNum, Track track, string lowestGrade, string highestGrade, string gradeLegendName)
        {
            Outcomes = new List<Outcome>();
            Marks = new List<Mark>();
            ReportPeriods = new List<ReportPeriod>();
            OutcomeMarks = new List<OutcomeMark>();

            this.name = name;
            this.classid = classid;
            this.courseid = courseid;
            this.schoolName = schoolName;
            this.dayNumber = dayNum;
            this.blockNumber = blockNum;
            this.lowestGrade = lowestGrade;
            this.highestGrade = highestGrade;
            this.track = track;
            this.gradeLegend = gradeLegendName;

        }

        public static List<SchoolClass> loadStudentEnrolledClasses(SqlConnection connection, Student student, Term term)
        {
            List<SchoolClass> returnMe = new List<SchoolClass>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_StudentclassEnrollment WHERE cStudentNumber=@STUDENTNUM AND iTermID=@TERMID ORDER BY SchoolName ASC, cName ASC, iClassID ASC;";
            sqlCommand.Parameters.AddWithValue("@STUDENTNUM", student.getStudentID());
            sqlCommand.Parameters.AddWithValue("@TERMID", term.ID);
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

                    int blockNum = -1;
                    if (!int.TryParse(dataReader["iblockNumber"].ToString().Trim(), out blockNum)) 
                    {
                        blockNum = -1;
                    }

                    int dayNum = -1;
                    if (!int.TryParse(dataReader["iDayNumber"].ToString().Trim(), out dayNum)) 
                    {
                        dayNum = -1;
                    }
                                        

                    SchoolClass newSchoolClass = new SchoolClass(
                            dataReader["cName"].ToString().Trim(),
                            int.Parse(dataReader["iClassID"].ToString().Trim()),
                            int.Parse(dataReader["iCourseID"].ToString().Trim()),
                            dataReader["TeacherFirstName"].ToString().Trim(),
                            dataReader["TeacherLastName"].ToString().Trim(),
                            dataReader["TeacherTitle"].ToString().Trim(),
                            dataReader["SchoolName"].ToString().Trim(),
                            blockNum,
                            dayNum,
                            newTrack,
                            dataReader["LowestGrade"].ToString().Trim(),
                            dataReader["HighestGrade"].ToString().Trim(),
                            dataReader["MarkLegendName"].ToString().Trim()
                        );
                    returnMe.Add(newSchoolClass);
                }
            }

            sqlCommand.Connection.Close();

            returnMe.Sort();

            foreach (SchoolClass sc in returnMe)
            {
                sc.teachers = Teacher.loadTeachersForThisClass(connection, sc.classid);
            }

            return returnMe;

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

            
                    SchoolClass newSchoolClass = new SchoolClass(
                            dataReader["cName"].ToString().Trim(),
                            int.Parse(dataReader["iClassID"].ToString().Trim()),
                            int.Parse(dataReader["iCourseID"].ToString().Trim()),
                            dataReader["TeacherFirstName"].ToString().Trim(),
                            dataReader["TeacherLastName"].ToString().Trim(),
                            dataReader["TeacherTitle"].ToString().Trim(),
                            dataReader["SchoolName"].ToString().Trim(),
                            0,
                            0,
                            newTrack,
                            dataReader["LowestGrade"].ToString().Trim(),
                            dataReader["HighestGrade"].ToString().Trim(),
                            dataReader["MarkLegendName"].ToString().Trim()
                        );                   


                    returnMe.Add(newSchoolClass);
                }
            }

            sqlCommand.Connection.Close();

            returnMe.Sort();

            foreach (SchoolClass sc in returnMe)
            {
                sc.teachers = Teacher.loadTeachersForThisClass(connection, sc.classid); 
            }

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
                if ((this.blockNumber != 0)  && (obj2.blockNumber != 0)) {
                    return this.blockNumber.CompareTo(obj2.blockNumber);
                } else {
                    return this.name.CompareTo(obj2.name);
                }
            }
            else
            {
                throw new ArgumentException("Object is not a SchoolClass");
            }
        }
    }
}