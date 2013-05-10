using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class LoginAttempt
    {
        public DateTime eventTime { get; set; }
        public string enteredUserName { get; set; }
        public string ipAddress { get; set; }
        public string userAgent { get; set; }
        public string status { get; set; }
        public string info { get; set; }

        public LoginAttempt(DateTime time, string username, string ip, string uagent, string stat, string nfo)
        {
            this.eventTime = time;
            this.enteredUserName = username;
            this.ipAddress = ip;
            this.userAgent = uagent;
            this.info = nfo;
            this.status = stat;

        }

        public override string ToString()
        {
            return "LoginAttempt ("+eventTime.ToShortDateString()+" "+eventTime.ToShortTimeString()+","+enteredUserName+","+ipAddress+","+ipAddress+")";
        }


    }
}