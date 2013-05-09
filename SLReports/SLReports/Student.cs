using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SLReports
{
    public class Student : IComparable
    {
        private string givenName;
        private string sn;
        private string studentID;
        private string govID;
        private string middleName;
        private string schoolName;
        private string schoolID;
        private string grade;
        private string region;
        private string city;
        private string street;
        private string houseno;
        private string apartmentno;
        private string postalcode;
        private string phone;
        private string gender;
        private string InStatus;
        private DateTime enrollmentDate;
        private DateTime trackStartDate;
        private DateTime trackEndDate;

        private List<Absence> absences;

        private DateTime dateOfBirth;

        private string bandNo;
        private string bandName;
        private string reserveName;
        private string reserveHouse;
        private string treatyStatusNo;
        private bool resideOnReserve;

        public bool getresidesOnReserve()
        {
            return this.resideOnReserve;
        }

        public List<Absence> getAbsences()
        {
            return this.absences;
        }

        public void setAbsences(List<Absence> theseAbsences)
        {
           this.absences = theseAbsences;
        }

        public void addAbsence(Absence thisAbs)
        {
            this.absences.Add(thisAbs);
        }

        public string getInStatus()
        {
            return this.InStatus;
        }

        public DateTime getEnrollDate()
        {
            return this.enrollmentDate;
        }

        public string getGivenName()
        {
            return this.givenName;
        }
        public string getSN()
        {
            return this.sn;
        }
        public string getStudentID()
        {
            return this.studentID;
        }
        public int getStudentIDAsInt()
        {
            int returnMe = 0;
            int.TryParse(this.studentID, out returnMe);
            return returnMe;
        }
        public string getGovernmentID()
        {
            return this.govID;
        }
        public int getGovernmentIDAsInt()
        {
            int returnMe = 0;
            int.TryParse(this.govID, out returnMe);
            return returnMe;
        }
        public string getMiddleName()
        {
            return this.middleName;
        }
        public string getSchoolName()
        {
            return this.schoolName;
        }
        public string getSchoolID()
        {
            return this.schoolID;
        }
        public int getSchoolIDAsInt()
        {
            int returnMe = 0;
            int.TryParse(this.schoolID, out returnMe);
            return returnMe;
        }

        public string getGrade()
        {
            return this.grade;
        }

        public string getGenderInitial()
        {
            return this.gender.Substring(0, 1);
        }

        public string getGender()
        {
            return this.gender;
        }

        public string getDisplayName()
        {
            return givenName + " " + sn;
        }

        public override string ToString()
        {
            return getDisplayName() + " " + studentID;
        }

        public Student(string givenName, string sn, string middleName, string id, string govID, string schoolName, string schoolID,
            string grade, string region, string city, string street, string houseno, string apartmentno, string postalcode,
            string phone, string gender, string instat, DateTime inDate, DateTime dateOfBirth)
        {
            absences = new List<Absence>();
            this.givenName = givenName;
            this.sn = sn;
            this.studentID = id;
            this.govID = govID;
            this.middleName = middleName;
            this.schoolName = schoolName;
            this.schoolID = schoolID;
            this.grade = grade;
            this.region = region;
            this.city = city;
            this.street = street;
            this.houseno = houseno;
            this.apartmentno = apartmentno;
            this.postalcode = postalcode;
            this.phone = phone;
            this.gender = gender;
            this.InStatus = instat;
            this.enrollmentDate = inDate;
            this.dateOfBirth = dateOfBirth;
        }
        
        public Student(string givenName, string sn, string middleName, string id, string govID, string schoolName, string schoolID,
            string grade, string region, string city, string street, string houseno, string apartmentno, string postalcode,
            string phone, string gender, string instat, DateTime inDate, DateTime dateOfBirth, string bandNo, string bandName, 
            string reserveName, string reserveHouse, string treatyStatus, bool resideonreserve)
        {
            absences = new List<Absence>();
            this.givenName = givenName;
            this.sn = sn;
            this.studentID = id;
            this.govID = govID;
            this.middleName = middleName;
            this.schoolName = schoolName;
            this.schoolID = schoolID;
            this.grade = grade;
            this.region = region;
            this.city = city;
            this.street = street;
            this.houseno = houseno;
            this.apartmentno = apartmentno;
            this.postalcode = postalcode;
            this.phone = phone;
            this.gender = gender;
            this.InStatus = instat;
            this.enrollmentDate = inDate;
            this.dateOfBirth = dateOfBirth;
            this.bandNo = bandNo;
            this.bandName = bandName;
            this.reserveName = reserveName;
            this.reserveHouse = reserveHouse;
            this.treatyStatusNo = treatyStatus;
            this.resideOnReserve = resideonreserve;
        }

        public string getBandNo()
        {
            return this.bandNo;
        }

        public string getBandName()
        {
            return this.bandName;
        }

        public string getReserveName()
        {
            return this.reserveName;
        }

        public string getReserveHouse()
        {
            return this.reserveHouse;
        }

        public string getStatusNo() 
        {
            return this.treatyStatusNo;
        }


        public DateTime getDateOfBirth()
        {
            return this.dateOfBirth;
        }
        
        public void setTrack(DateTime start, DateTime end)
        {
            this.trackStartDate = start;
            this.trackEndDate = end;
        }

        public DateTime getCurrentTrackStart()
        {
            return this.trackStartDate;
        }

        public DateTime getCurrentTrackEnd()
        {
            return this.trackEndDate;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            Student obj2 = obj as Student;

            if (obj2 != null)
            {
                return this.getStudentIDAsInt().CompareTo(obj2.getStudentIDAsInt());
            }
            else
            {
                throw new ArgumentException("Object is not a Student");
            }
        }

        public static List<Student> GetStudentsFromSchool(List<Student> haystack, string schoolID)
        {
            List<Student> returnMe = new List<Student>();
            
            foreach (Student x in haystack)
            {
                if (x.getSchoolID().Equals(schoolID))
                {
                    returnMe.Add(x);
                }
            }

            return returnMe;
        }

        public static Student loadThisStudent(SqlConnection connection, string studentID)
        {
            Student returnMe = null;

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM FIM_Students WHERE StudentNumber='" + studentID + "'";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    returnMe = new Student(
                            dataReader["LegalFirstName"].ToString(),
                            dataReader["LegalLastName"].ToString(),
                            dataReader["LegalMiddleName"].ToString(),
                            dataReader["StudentNumber"].ToString(),
                            dataReader["GovernmentIDNumber"].ToString(),
                            dataReader["School"].ToString(),
                            dataReader["SchoolID"].ToString(),
                            dataReader["Grade"].ToString(),
                            dataReader["Region"].ToString(),
                            dataReader["City"].ToString(),
                            dataReader["Street"].ToString(),
                            dataReader["HouseNo"].ToString(),
                            dataReader["ApartmentNo"].ToString(),
                            dataReader["PostalCode"].ToString(),
                            dataReader["Phone"].ToString(),
                            dataReader["Gender"].ToString(),
                            dataReader["InStatus"].ToString(),
                            DateTime.Parse(dataReader["InDate"].ToString()),
                            DateTime.Parse(dataReader["DateOfBirth"].ToString())
                            );

                    returnMe.setTrack(DateTime.Parse(dataReader["CurrentTrackStart"].ToString()), DateTime.Parse(dataReader["CurrentTrackEnd"].ToString()));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }

        public static List<Absence> loadAbsencesFromStudent(SqlConnection connection, string studentID, DateTime startDate, DateTime endDate)
        {
            List<Absence> returnMe = new List<Absence>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_Attendance WHERE StudentNumber='" + studentID + "' AND dDate BETWEEN '" + startDate.ToShortDateString() + "' AND '" + endDate.ToShortDateString() + "' ORDER BY dDate ASC, tStartTime ASC;";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                returnMe.Clear();
                while (dataReader.Read())
                {
                    returnMe.Add(new Absence(
                        DateTime.Parse(dataReader["dDate"].ToString()),
                        dataReader["StudentNumber"].ToString(),
                        dataReader["ClassName"].ToString(),
                        dataReader["ClassID"].ToString(),
                        dataReader["Status"].ToString(),
                        dataReader["Reason"].ToString(),
                        dataReader["Comment"].ToString(),
                        int.Parse(dataReader["Block"].ToString()),
                        DateTime.Parse(dataReader["tStartTime"].ToString()),
                        DateTime.Parse(dataReader["tEndTime"].ToString())
                        ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }

    }
}