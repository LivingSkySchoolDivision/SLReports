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
        private string InStatusCode;
        private DateTime enrollmentDate;
        private DateTime trackStartDate;
        private DateTime trackEndDate;
        private List<Absence> absences;
        private List<Contact> contacts;
        private DateTime dateOfBirth;        
        private string HomeRoom;
        private string bandNo;
        private string bandName;
        private string reserveName;
        private string reserveHouse;
        private string treatyStatusNo;
        private bool resideOnReserve;
        private string trackID;
        private bool bHasPhoto;
        private object photo;

        public Track track { get; set; }
        public School school { get; set; }
        

        public object getPhoto()
        {
            return this.photo;
        }

        public void setPhoto(object photoData)
        {
            this.photo = photoData;
        }

        public bool hasPhoto()
        {
            return bHasPhoto;
        }

        public string getTrackID()
        {
            return this.trackID;
        }

        public string getGradeFormatted()
        {
            string returnMe = this.grade;

            try
            {
                int intVal = int.Parse(this.grade);
                returnMe = intVal.ToString();
            }
            catch {
                if (this.grade.ToLower() == "0k")
                {
                    returnMe = "K";
                }

                if (this.grade.ToLower() == "k")
                {
                    returnMe = "k";
                }

                if (this.grade.ToLower() == "pk")
                {
                    returnMe = "pk";
                }
            }
            
            return returnMe;
        }

        public string getCity()
        {
            return this.city;
        }

        public string getRegion()
        {
            return this.region;
        }
        
        public string getStreet()
        {
            return this.street;
        }

        public string getHouseNo()
        {
            return this.houseno;
        }

        public string getApartmentNo()
        {
            return this.apartmentno;
        }

        public string getPostalCode()
        {
            return this.postalcode;
        }

        public string getTelephone()
        {
            return this.phone;
        }

        public string getTelephoneFormatted()
        {
            string areaCode = this.phone.Substring(0,3);
            string exchange = this.phone.Substring(3,3);
            string number = this.phone.Substring(6,4);

            return "("+areaCode+") "+exchange+"-" + number;
        }

        public List<Contact> getContacts()
        {
            return contacts;
        }

        public void addContact(Contact thisContact)
        {
            this.contacts.Add(thisContact);
        }

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

        public string getInStatusCode()
        {
            return this.InStatusCode;
        }

        public string getInStatusWithCode()
        {
            return this.InStatusCode + ":" + this.InStatus;
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

        public string getHomeRoom()
        {
            return this.HomeRoom;
        }

        public Student(string givenName, string sn, string middleName, string id, string govID, string schoolName, string schoolID,
            string grade, string region, string city, string street, string houseno, string apartmentno, string postalcode,
            string phone, string gender, string instat, string instatcode, string homeRm, DateTime inDate, DateTime dateOfBirth, 
            string trackid, bool hasPhoto)
        {
            absences = new List<Absence>();
            contacts = new List<Contact>();
            
            this.track = null;

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
            this.HomeRoom = homeRm;
            this.trackID = trackid;
            this.InStatusCode = instatcode;
            this.bHasPhoto = hasPhoto;

        }
        
        public Student(string givenName, string sn, string middleName, string id, string govID, string schoolName, string schoolID,
            string grade, string region, string city, string street, string houseno, string apartmentno, string postalcode,
            string phone, string gender, string instat, string instatcode, string homeRm, DateTime inDate, DateTime dateOfBirth, 
            string bandNo, string bandName, string reserveName, string reserveHouse, string treatyStatus, bool resideonreserve,
            string trackid, bool hasPhoto)
        {
            absences = new List<Absence>();
            contacts = new List<Contact>();

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
            this.HomeRoom = homeRm;
            this.trackID = trackid;
            this.InStatusCode = instatcode;
            this.bHasPhoto = hasPhoto;
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
            sqlCommand.CommandText = "SELECT * FROM LSKY_ActiveStudents WHERE StudentNumber='" + studentID + "'";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    string HomeRoom = dataReader["HomeRoom"].ToString().Trim();
                    string HRT_Title = dataReader["HomeRoomTeacherTitle"].ToString().Trim();
                    string HRT_First = dataReader["HomeRoomTeacherFirstName"].ToString().Trim();
                    string HRT_Last = dataReader["HomeRoomTeacherLastName"].ToString().Trim();
                    if ((!string.IsNullOrEmpty(HRT_First)) && (!string.IsNullOrEmpty(HRT_Last)))
                    {
                        HomeRoom = HomeRoom + " (";
                        if ((!string.IsNullOrEmpty(HRT_Title)))
                        {
                            HomeRoom = HomeRoom + HRT_Title;
                        }
                        else
                        {
                            HomeRoom = HomeRoom + HRT_First;
                        }

                        HomeRoom = HomeRoom + " " + HRT_Last + ")";
                    }

                    bool hasPhoto = false;
                    if (!string.IsNullOrEmpty(dataReader["PhotoType"].ToString()))
                    {
                        hasPhoto = true;
                    }

                    returnMe = new Student(
                            dataReader["LegalFirstName"].ToString().Trim(),
                            dataReader["LegalLastName"].ToString().Trim(),
                            dataReader["LegalMiddleName"].ToString().Trim(),
                            dataReader["StudentNumber"].ToString().Trim(),
                            dataReader["GovernmentIDNumber"].ToString().Trim(),
                            dataReader["School"].ToString().Trim(),
                            dataReader["SchoolID"].ToString().Trim(),
                            dataReader["Grade"].ToString().Trim(),
                            dataReader["Region"].ToString().Trim(),
                            dataReader["City"].ToString().Trim(),
                            dataReader["Street"].ToString().Trim(),
                            dataReader["HouseNo"].ToString().Trim(),
                            dataReader["ApartmentNo"].ToString().Trim(),
                            dataReader["PostalCode"].ToString().Trim(),
                            dataReader["Phone"].ToString().Trim(),
                            dataReader["Gender"].ToString().Trim(),
                            dataReader["InStatus"].ToString(),
                            dataReader["InStatusCode"].ToString(),
                            HomeRoom,
                            DateTime.Parse(dataReader["InDate"].ToString()),
                            DateTime.Parse(dataReader["DateOfBirth"].ToString()),
                            dataReader["TrackID"].ToString(),
                            hasPhoto
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
                        DateTime.Parse(dataReader["tEndTime"].ToString()),
                        int.Parse(dataReader["Minutes"].ToString())
                        ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }

        public static List<Student> loadStudentsFromThisSchool(SqlConnection connection, int schoolID)
        {
            List<Student> returnMe = new List<Student>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_ActiveStudents WHERE SchoolID='" + schoolID + "' ORDER BY Grade ASC;";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    string HomeRoom = dataReader["HomeRoom"].ToString().Trim();
                    string HRT_Title = dataReader["HomeRoomTeacherTitle"].ToString().Trim();
                    string HRT_First = dataReader["HomeRoomTeacherFirstName"].ToString().Trim();
                    string HRT_Last = dataReader["HomeRoomTeacherLastName"].ToString().Trim();
                    if ((!string.IsNullOrEmpty(HRT_First)) && (!string.IsNullOrEmpty(HRT_Last)))
                    {
                        HomeRoom = HomeRoom + " (";
                        if ((!string.IsNullOrEmpty(HRT_Title)))
                        {
                            HomeRoom = HomeRoom + HRT_Title;
                        }
                        else
                        {
                            HomeRoom = HomeRoom + HRT_First;
                        }

                        HomeRoom = HomeRoom + " " + HRT_Last + ")";
                    }

                    bool hasPhoto = false;
                    if (!string.IsNullOrEmpty(dataReader["PhotoType"].ToString()))
                    {
                        hasPhoto = true;
                    }

                    returnMe.Add(new Student(
                            dataReader["LegalFirstName"].ToString().Trim(),
                            dataReader["LegalLastName"].ToString().Trim(),
                            dataReader["LegalMiddleName"].ToString().Trim(),
                            dataReader["StudentNumber"].ToString().Trim(),
                            dataReader["GovernmentIDNumber"].ToString().Trim(),
                            dataReader["School"].ToString().Trim(),
                            dataReader["SchoolID"].ToString().Trim(),
                            dataReader["Grade"].ToString().Trim(),
                            dataReader["Region"].ToString().Trim(),
                            dataReader["City"].ToString().Trim(),
                            dataReader["Street"].ToString().Trim(),
                            dataReader["HouseNo"].ToString().Trim(),
                            dataReader["ApartmentNo"].ToString().Trim(),
                            dataReader["PostalCode"].ToString().Trim(),
                            dataReader["Phone"].ToString().Trim(),
                            dataReader["Gender"].ToString().Trim(),
                            dataReader["InStatus"].ToString(),
                            dataReader["InStatusCode"].ToString(),
                            HomeRoom,
                            DateTime.Parse(dataReader["InDate"].ToString()),
                            DateTime.Parse(dataReader["DateOfBirth"].ToString()),
                            dataReader["TrackID"].ToString(),
                            hasPhoto
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }

        public static List<Student> loadAllStudents(SqlConnection connection)
        {
            List<Student> returnMe = new List<Student>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_ActiveStudents ORDER BY SchoolID ASC, Grade ASC;";
            sqlCommand.Connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    string HomeRoom = dataReader["HomeRoom"].ToString().Trim();
                    string HRT_Title = dataReader["HomeRoomTeacherTitle"].ToString().Trim();
                    string HRT_First = dataReader["HomeRoomTeacherFirstName"].ToString().Trim();
                    string HRT_Last = dataReader["HomeRoomTeacherLastName"].ToString().Trim();
                    if ((!string.IsNullOrEmpty(HRT_First)) && (!string.IsNullOrEmpty(HRT_Last)))
                    {
                        HomeRoom = HomeRoom + " (";
                        if ((!string.IsNullOrEmpty(HRT_Title)))
                        {
                            HomeRoom = HomeRoom + HRT_Title;
                        }
                        else
                        {
                            HomeRoom = HomeRoom + HRT_First;
                        }

                        HomeRoom = HomeRoom + " " + HRT_Last + ")";
                    }
                    bool hasPhoto = false;
                    if (!string.IsNullOrEmpty(dataReader["PhotoType"].ToString()))
                    {
                        hasPhoto = true;
                    }
                    returnMe.Add(new Student(
                            dataReader["LegalFirstName"].ToString().Trim(),
                            dataReader["LegalLastName"].ToString().Trim(),
                            dataReader["LegalMiddleName"].ToString().Trim(),
                            dataReader["StudentNumber"].ToString().Trim(),
                            dataReader["GovernmentIDNumber"].ToString().Trim(),
                            dataReader["School"].ToString().Trim(),
                            dataReader["SchoolID"].ToString().Trim(),
                            dataReader["Grade"].ToString().Trim(),
                            dataReader["Region"].ToString().Trim(),
                            dataReader["City"].ToString().Trim(),
                            dataReader["Street"].ToString().Trim(),
                            dataReader["HouseNo"].ToString().Trim(),
                            dataReader["ApartmentNo"].ToString().Trim(),
                            dataReader["PostalCode"].ToString().Trim(),
                            dataReader["Phone"].ToString().Trim(),
                            dataReader["Gender"].ToString().Trim(),
                            dataReader["InStatus"].ToString(),
                            dataReader["InStatusCode"].ToString(),
                            HomeRoom,
                            DateTime.Parse(dataReader["InDate"].ToString()),
                            DateTime.Parse(dataReader["DateOfBirth"].ToString()),
                            dataReader["TrackID"].ToString(),
                            hasPhoto
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }

    }
}