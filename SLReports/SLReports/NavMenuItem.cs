using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class NavMenuItem : IComparable
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

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            NavMenuItem obj2 = obj as NavMenuItem;

            if (obj2 != null)
            {
                return this.name.CompareTo(obj2.name);
            }
            else
            {
                throw new ArgumentException("Object is not a NavMenuItem");
            }
        }
    }
}