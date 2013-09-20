using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLReports.SaskSubmitted
{
    public class SaskSubmittedSchoolCount
    {
        public string schoolName { get; set; }
        public int totalCount { get; set; }
        public int failures { get; set; }
        public int successes { get; set; }


        public SaskSubmittedSchoolCount(string schoolName, int total, int failures, int successes)
        {
            this.schoolName = schoolName;
            this.totalCount = total;
            this.failures = failures;
            this.successes = successes;
        }
    }
}