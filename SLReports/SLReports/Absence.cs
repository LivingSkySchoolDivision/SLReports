using System;
using System.Collections.Generic;
using System.Linq;
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
        private int block;
        private DateTime blockStarttime;
        private DateTime blockEndTime;

        public Absence(DateTime date, string studentid, string courseName, string courseID, string status, string reason, string comment, int block, DateTime bStarttime, DateTime bEndTime)
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

    }
}