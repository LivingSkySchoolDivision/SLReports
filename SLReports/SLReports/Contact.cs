using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class Contact
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string studentNumber { get; set; }
        public string relation { get; set; }
            

        public Contact(string fn, string ln, string rel, string snumber)
        {
            this.firstName = fn;
            this.lastName = ln;
            this.studentNumber = snumber;
            this.relation = rel;
        }

        public override string ToString()
        {
            return firstName + " " + lastName + " ("+relation+")";
        }

        public string getStudentID()
        {
            return studentNumber;
        }
    }
}