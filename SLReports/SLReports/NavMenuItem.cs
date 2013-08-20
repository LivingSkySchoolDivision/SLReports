using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        public bool admin_only { get; set; }
        public bool hidden { get; set; }
        public string category { get; set; }

        public NavMenuItem(int id, int parent, string url, string name, string description, bool admin_only, bool hidden, string category)
        {
            this.id = id;
            this.parent = parent;
            this.description = description;
            this.name = name;
            this.url = url;
            this.admin_only = admin_only;
            this.hidden = hidden;
            this.category = category;

            if (string.IsNullOrEmpty(category))
            {
                this.category = "General";
            }
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

        public static List<NavMenuItem> loadMenuItems(SqlConnection connection)
        {
            List<NavMenuItem> returnMe = new List<NavMenuItem>();

            using (SqlCommand sqlCommand = new SqlCommand())
            {
                sqlCommand.Connection = connection;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = "SELECT * FROM menu_items_with_categories;";
                sqlCommand.Connection.Open();
                SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                if (dbDataReader.HasRows)
                {
                    while (dbDataReader.Read())
                    {
                        bool admin_only = true;
                        if (!bool.TryParse(dbDataReader["admin_only"].ToString(), out admin_only))
                        {
                            admin_only = true;
                        }
                        
                        bool hidden = true;
                        if (!bool.TryParse(dbDataReader["hidden"].ToString(), out hidden))
                        {
                            hidden = true;
                        }

                        NavMenuItem newItem = new NavMenuItem(
                            int.Parse(dbDataReader["id"].ToString()),
                            int.Parse(dbDataReader["parent"].ToString()),
                            dbDataReader["url"].ToString(),
                            dbDataReader["name"].ToString(),
                            dbDataReader["description"].ToString(),
                            admin_only,
                            hidden,
                            dbDataReader["category"].ToString()
                            );

                        returnMe.Add(newItem);
                    }
                }

            }

            return returnMe;
        }

    }
}