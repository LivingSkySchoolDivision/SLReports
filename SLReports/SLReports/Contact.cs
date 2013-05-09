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

        public Contact(string firstName, string LastName, string StudentNumber)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.studentNumber = studentNumber;
        }
    }
}