using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class session
    {
        private string username;
        private string ip;
        private string hash;
        private string useragent;
        private DateTime starts;
        private DateTime ends;

        public session(string username, string ip, string hash, string useragent, DateTime starts, DateTime ends)
        {
            this.username = username;
            this.ip = ip;
            this.hash = hash;
            this.useragent = useragent;
            this.starts = starts;
            this.ends = ends;
        }

        public string getUsername() 
        {
            return username;
        }

        public string getIP()
        {
            return ip;
        }

        public string getHash()
        {
            return hash;
        }

        public string getUserAgent()
        {
            return useragent;
        }

        public DateTime getStart()
        {
            return starts;
        }

        public DateTime getEnd()
        {
            return ends;
        }
    }
}