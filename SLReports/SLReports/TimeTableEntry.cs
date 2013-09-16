using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class TimeTableEntry : IComparable
    {
        public int schoolID { get; set; }
        public int termID { get; set; }
        public int classID { get; set; }
        public int dayNum { get; set; }
        public int blockNum { get; set; }
        public string className { get; set; }
        public string roomName { get; set; }
        public SchoolClass schoolClass { get; set; }        

        public TimeTableEntry(int dayNumber, int blockNumber, int schoolID, int TermID, string room, SchoolClass schoolClass)
        {
            this.dayNum = dayNumber;
            this.blockNum = blockNumber;
            this.schoolClass = schoolClass;
            this.termID = termID;
            this.schoolID = schoolID;
            this.roomName = room;
        }

        public string ToStringFormatted()
        {
            return this.schoolClass.name + "; Room " + roomName + "; Period " + blockNum + "; Teacher " + schoolClass.teacherName;
        }

        public override string ToString()
        {
            return "TimeTableEntry {Day: " + this.dayNum + ", Block: " + this.blockNum + ", Class: " + this.schoolClass.name + "}";
        }

        public static List<TimeTableEntry> loadStudentTimeTable(SqlConnection connection, Student student, Term term)
        {
            List<TimeTableEntry> returnMe = new List<TimeTableEntry>();

            // get student enrolled classes for this term

            List<SchoolClass> studentEnrolledCoursesThisTerm = SchoolClass.loadStudentEnrolledClassesForThisTerm(connection, student, term);

            // Get schedule for this school, term and class
            foreach (SchoolClass thisclass in studentEnrolledCoursesThisTerm)
            {
                returnMe.AddRange(loadTimeTableEntries(connection, term.ID, thisclass));
            }

            return returnMe;
        }



        public static List<TimeTableEntry> loadTimeTableEntries(SqlConnection connection, int termID, SchoolClass schoolClass)
        {
            List<TimeTableEntry> returnMe = new List<TimeTableEntry>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_ClassSchedule WHERE iTermID=@TERMID AND iClassID=@CLASSID ORDER BY iBlockNumber ASC, cName ASC;";
            sqlCommand.Parameters.AddWithValue("@TERMID", termID);
            sqlCommand.Parameters.AddWithValue("@CLASSID", schoolClass.classid);
            sqlCommand.Connection.Open();

            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {

                    TimeTableEntry newTimeTableEntry = new TimeTableEntry(
                            int.Parse(dataReader["iDayNumber"].ToString().Trim()),
                            int.Parse(dataReader["iBlockNumber"].ToString().Trim()),
                            int.Parse(dataReader["iSchoolID"].ToString().Trim()),
                            int.Parse(dataReader["itermID"].ToString().Trim()),
                            dataReader["Room"].ToString().Trim(),
                            schoolClass
                        );
                    returnMe.Add(newTimeTableEntry);
                }
            }

            sqlCommand.Connection.Close();


            return returnMe;

        }

        /*
        public static List<TimeTableEntry> loadAllTimeTableEntries(SqlConnection connection)
        {
            List<TimeTableEntry> returnMe = new List<TimeTableEntry>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_ClassSchedule ORDER BY iBlockNumber ASC, cName ASC;";
            sqlCommand.Connection.Open();

            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    TimeTableEntry newTimeTableEntry = new TimeTableEntry(
                            int.Parse(dataReader["iDayNumber"].ToString().Trim()),
                            int.Parse(dataReader["iBlockNumber"].ToString().Trim()),
                            int.Parse(dataReader["iSchoolID"].ToString().Trim()),
                            int.Parse(dataReader["itermID"].ToString().Trim()),
                            dataReader["Room"].ToString().Trim(),
                            null
                        );
                    returnMe.Add(newTimeTableEntry);
                }
            }

            sqlCommand.Connection.Close();


            return returnMe;

        }
        */

        public static List<TimeTableEntry> loadTimeTableEntries(SqlConnection connection, Term term, SchoolClass sclass)
        {
            return loadTimeTableEntries(connection, term.ID, sclass);
        }


        public static TimeTableEntry getEarliest(List<TimeTableEntry> timetableentries)
        {
            TimeTableEntry returnMe = null;

            foreach (TimeTableEntry tte in timetableentries)
            {
                if (returnMe == null)
                {
                    returnMe = tte;
                }

                if (returnMe.blockNum > tte.blockNum)
                {
                    returnMe = tte;
                }
            }

            return returnMe;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            TimeTableEntry obj2 = obj as TimeTableEntry;

            if (obj2 != null)
            {
                return this.blockNum.CompareTo(obj2.blockNum);
            }
            else
            {
                throw new ArgumentException("Object is not a TimeTableEntry");
            }
        }

        
    }
}