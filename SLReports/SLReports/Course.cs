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
        public List<Mark> Marks { get; set; }
        public List<Outcome> Outcomes { get; set; }

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
            Outcomes = new List<Outcome>();
            Marks = new List<Mark>();
            this.name = name;
            this.classid = classid;
            this.courseid = courseid;
            this.teacherFirstName = teacherFirst;
            this.teacherLastName = teacherLast;
            this.teacherTitle = teacherTitle;
        }
    }
}