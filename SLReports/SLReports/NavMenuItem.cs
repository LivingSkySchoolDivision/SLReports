using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class NavMenuItem
    {
        public string url { get; set; }
        public string name { get; set; }        
        public string description { get; set; }
        public int id { get; set; }
        public int parent { get; set; }

        public NavMenuItem(int id, int parent, string url, string name, string description)
        {
            this.id = id;
            this.parent = parent;
            this.description = description;
            this.name = name;
            this.url = url;
        }

        public override string ToString()
        {
            return this.name;
        }
    }
}