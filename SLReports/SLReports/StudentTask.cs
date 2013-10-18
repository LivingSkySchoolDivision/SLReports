using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class StudentTask
    {
        public int id { get; set; }
        public string name { get; set; }
        public GradeType gradeType { get; set; }
        public DateTime taskDate { get; set; }
        public int classID { get; set; }
        public string className { get; set; }
        public string schoolID { get; set; }
        public string schoolName { get; set; }
        
        public enum GradeType
        {
            Alpha,
            Numeric
        }

        public StudentTask(int id, string name, GradeType gradetype, DateTime taskdate, int classid, string classname, string schoolid, string schoolname)
        {
            this.id = id;
            this.name = name;
            this.gradeType = gradetype;
            this.taskDate = taskdate;
            this.classID = classid;
            this.className = classname;
            this.schoolID = schoolid;
            this.schoolName = schoolname;
        }
    }
}