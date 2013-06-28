using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class StaffMember
    {
        public String givenName { get; set; }
        public String sn { get; set; }
        public String schoolName { get; set; }
        public int schoolID { get; set; }
        public String LDAPName { get; set; }
        public String notes { get; set; }
        public string teacherCertNumber { get; set; }
        public bool isLocked { get; set; }
        public bool isInactive { get; set; }
        public bool isActive
        {
            get
            {
                return !isInactive;
            }

            set {
                this.isInactive = !value;
            }
        }
        public String position { get; set; }

        public StaffMember(String givenName, String sn, String schoolName, int schoolID, String LDAP, String notes, String teacherCertNumber, String position, bool isInactive, bool isLocked)
        {
            this.givenName = givenName;
            this.sn = sn;
            this.schoolName = schoolName;
            this.schoolID = schoolID;
            this.LDAPName = LDAP;
            this.notes = notes;
            this.teacherCertNumber = teacherCertNumber;
            this.position = position;
            this.isInactive = isInactive;
            this.isLocked = isLocked;
        }

        public static List<StaffMember> loadAllStaff(SqlConnection connection)
        {
            List<StaffMember> returnMe = new List<StaffMember>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_Staff ORDER BY LastName ASC, FirstName ASC;";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    int SchoolID = 0;
                    if (!string.IsNullOrEmpty(dataReader["SchoolID"].ToString().Trim()))
                    {
                        SchoolID = int.Parse(dataReader["SchoolID"].ToString().Trim());
                    }

                    bool isLocked = false;
                    if (!string.IsNullOrEmpty(dataReader["lAccountLocked"].ToString().Trim()))
                    {
                        isLocked = bool.Parse(dataReader["lAccountLocked"].ToString().Trim());
                    }

                    bool isInactive = false;
                    if (!string.IsNullOrEmpty(dataReader["lInactive"].ToString().Trim()))
                    {
                        isInactive = bool.Parse(dataReader["lInactive"].ToString().Trim());
                    }

                    returnMe.Add(new StaffMember(
                        dataReader["FirstName"].ToString().Trim(),
                        dataReader["LastName"].ToString().Trim(),
                        dataReader["SchoolName"].ToString().Trim(),
                        SchoolID,
                        dataReader["cLDAPName"].ToString().Trim(),
                        dataReader["mNotes"].ToString().Trim(),
                        dataReader["TeacherCertNum"].ToString().Trim(),
                        dataReader["position"].ToString().Trim(),
                        isLocked,
                        isInactive
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }

    }
}