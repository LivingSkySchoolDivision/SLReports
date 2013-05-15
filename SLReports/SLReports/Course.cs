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
        public string id { get; set; }
        public string teacherFirstName { get; set; }
        public string teacherLastName { get; set; }
        public string teacherTitle { get; set; }

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

        public  List<Mark> Marks { get; set; }

        public Course(string name, string id, string teacherFirst, string teacherLast, string teacherTitle)
        {
            Marks = new List<Mark>();
            this.name = name;
            this.id = id;
            this.teacherFirstName = teacherFirst;
            this.teacherLastName = teacherLast;
            this.teacherTitle = teacherTitle;
        }
    }
}