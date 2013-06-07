using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SLReports
{
    public class Course
    {
        public string name { get; set; }
        public int courseid { get; set; }
        public int classid { get; set; }
        public string teacherFirstName { get; set; }
        public string teacherLastName { get; set; }
        public string teacherTitle { get; set; }
        public string mark { get; set; } 
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

        public Course(string name, int classid, int courseid, string teacherFirst, string teacherLast, string teacherTitle)
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
    }
}