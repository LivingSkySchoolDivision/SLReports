using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class School
    {
        private string name;
        private string schoolLogicID;
        private string govID;

        public string getName()
        {
            return this.name;
        }
        public string getSchoolLogicID()
        {
            return this.schoolLogicID;
        }
        public string getGovID()
        {
            return this.govID;
        }

        public School(string name, string slid, string govid)
        {
            this.name = name;
            this.schoolLogicID = slid;
            this.govID = govid;
        }

    }
}