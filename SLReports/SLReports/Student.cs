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
        public string LDAPUserName { get; set; }
        public string displayFirstName { get; set; }
        public string displayLastName { get; set; }
        public string legalMiddleName { get; set; }
        public string legalFirstName { get; set; }
        public string legalLastName {get;set;}
        private string studentID;
        private string govID;        
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
        public List<Absence> absences { get; set; }
        public List<Contact> contacts { get; set; }
        private DateTime dateOfBirth;        
        private string HomeRoom;
        private string bandNo;
        private string bandName;
        private string reserveName;
        private string reserveHouse;
        private string treatyStatusNo;

        public string aborigStatus { get; set; }
        public string language { get; set; }

        private bool resideOnReserve;
        private int trackID;
        private bool bHasPhoto;
        private object photo;
        public int creditsEarned { get; set; }
        public List<StudentStatus> statuses { get; set; }
        public List<TimeTableEntry> TimeTable { get; set; }
        public Track track { get; set; }
        public School school { get; set; }
        public int activeStatusID { get; set; }
        public List<ReportPeriodComment> ReportPeriodComments { get; set; }

        public string getCountry()
        {
            return "Canada";
        }

        public string getFirstName()
        {
            return this.displayFirstName;
        }

        public string getLastName()
        {
            return this.displayLastName;
        }

        public string getLegalMiddleName()
        {
            return this.legalMiddleName;
        }

        public string getLegalFirstName()
        {
            return this.legalFirstName;
        }

        public string getLegalLastName()
        {
            return this.legalLastName;
        }

        public int getAge()
        {
            int age = DateTime.Today.Year - this.dateOfBirth.Year;
            if (this.dateOfBirth > DateTime.Today.AddYears(-age)) age--;
            return age;
        }

        public int getAgeAsOf(DateTime thisDate)
        {
            // TODO: Finish this
            throw new Exception("Not yet implemented");
        }

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

        public int getTrackID()
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
            return displayFirstName + " " + displayLastName;
        }

        public override string ToString()
        {
            return "Student: { DisplayName: " + getDisplayName() + ", ID: " + studentID + ", Grade: " + this.grade + ", Credits: " + this.creditsEarned + "}";
        }

        public string getHomeRoom()
        {
            return this.HomeRoom;
        }

        
        public Student(string displayFirstName, string displayLastName, string legalfirstname, string legallastname, string legalmiddleName, string id, string govID, string schoolName, string schoolID,
            string grade, string region, string city, string street, string houseno, string apartmentno, string postalcode,
            string phone, string gender, string instat, string instatcode, string homeRm, DateTime inDate, DateTime dateOfBirth, 
            string bandNo, string bandName, string reserveName, string reserveHouse, string treatyStatus, bool resideonreserve,
            int trackid, bool hasPhoto, string ldapusername, int credits, int activeStatusID, string aboriginalStatus, string homeLanguage)
        {
            absences = new List<Absence>();
            contacts = new List<Contact>();
            TimeTable = new List<TimeTableEntry>();
            statuses = new List<StudentStatus>();
            ReportPeriodComments = new List<ReportPeriodComment>();

            this.activeStatusID = activeStatusID;

            this.displayFirstName = displayFirstName;
            this.displayLastName = displayLastName;

            this.legalMiddleName = legalmiddleName;
            this.legalFirstName = legalfirstname;
            this.legalLastName = legallastname;

            this.studentID = id;
            this.govID = govID;
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
            this.LDAPUserName = ldapusername;
            this.creditsEarned = credits;

            this.language = homeLanguage;
            this.aborigStatus = aboriginalStatus;
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
        
        public void setTrackDates(DateTime start, DateTime end)
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

                    double credits = 0;
                    double.TryParse(dataReader["Credits"].ToString(), out credits);

                    returnMe = new Student(
                            dataReader["FirstName"].ToString().Trim(),
                            dataReader["LastName"].ToString().Trim(),
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
                            dataReader["InStatus"].ToString().Trim(),
                            dataReader["InStatusCode"].ToString().Trim(),
                            HomeRoom,
                            DateTime.Parse(dataReader["InDate"].ToString()),
                            DateTime.Parse(dataReader["DateOfBirth"].ToString()),
                            dataReader["BandNo"].ToString().Trim(),
                            dataReader["BandName"].ToString().Trim(),
                            dataReader["ReserveName"].ToString().Trim(),
                            dataReader["ReserveHouse"].ToString().Trim(),
                            dataReader["StatusNo"].ToString().Trim(),
                            bool.Parse(dataReader["ResideOnReserve"].ToString()),
                            int.Parse(dataReader["TrackID"].ToString()),
                            hasPhoto,
                            dataReader["cUserName"].ToString().Trim(),
                            (int)credits,
                            int.Parse(dataReader["iActive_StudentStatusID"].ToString()),
                            dataReader["AboriginalStatus"].ToString().Trim(),
                            dataReader["LanguageHome"].ToString().Trim()
                            );
                    //returnMe.setTrack(DateTime.Parse(dataReader["CurrentTrackStart"].ToString()), DateTime.Parse(dataReader["CurrentTrackEnd"].ToString()));
                }
            }

            sqlCommand.Connection.Close();
            if (returnMe != null)
            {
                returnMe.track = Track.loadThisTrack(connection, returnMe.getTrackID());
            }

            return returnMe;
        }

        public static List<Student> loadStudentsFromThisSchool(SqlConnection connection, int schoolID)
        {
            List<Student> returnMe = new List<Student>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            //sqlCommand.CommandText = "SELECT * FROM LSKY_ActiveStudents WHERE SchoolID='" + schoolID + "' ORDER BY Grade ASC;";
            sqlCommand.CommandText = "SELECT * FROM LSKY_ActiveStudents WHERE SchoolID='" + schoolID + "' ORDER BY LegalLastName ASC, LegalFirstName ASC;";

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

                    int credits = 0;
                    int.TryParse(dataReader["Credits"].ToString(), out credits);

                    bool resideOnReserve = false;
                    if (!string.IsNullOrEmpty(dataReader["ResideOnReserve"].ToString()))
                    {
                        bool.TryParse(dataReader["ResideOnReserve"].ToString(), out resideOnReserve);
                    }

                    returnMe.Add(new Student(
                            dataReader["FirstName"].ToString().Trim(),
                            dataReader["LastName"].ToString().Trim(),
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
                            dataReader["InStatus"].ToString().Trim(),
                            dataReader["InStatusCode"].ToString().Trim(),
                            HomeRoom,
                            DateTime.Parse(dataReader["InDate"].ToString()),
                            DateTime.Parse(dataReader["DateOfBirth"].ToString()),
                            dataReader["BandNo"].ToString().Trim(),
                            dataReader["BandName"].ToString().Trim(),
                            dataReader["ReserveName"].ToString().Trim(),
                            dataReader["ReserveHouse"].ToString().Trim(),
                            dataReader["StatusNo"].ToString().Trim(),
                            resideOnReserve,
                            int.Parse(dataReader["TrackID"].ToString()),
                            hasPhoto,
                            dataReader["cUserName"].ToString().Trim(),
                            credits,
                            int.Parse(dataReader["iActive_StudentStatusID"].ToString()),
                            dataReader["AboriginalStatus"].ToString().Trim(),
                            dataReader["LanguageHome"].ToString().Trim()
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }

        public static List<Student> loadStudentsFromThisTrack(SqlConnection connection, int trackID)
        {
            List<Student> returnMe = new List<Student>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            //sqlCommand.CommandText = "SELECT * FROM LSKY_ActiveStudents WHERE SchoolID='" + schoolID + "' ORDER BY Grade ASC;";
            sqlCommand.CommandText = "SELECT * FROM LSKY_ActiveStudents WHERE TrackID='" + trackID + "' ORDER BY LegalLastName ASC, LegalFirstName ASC;";

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

                    int credits = 0;
                    int.TryParse(dataReader["Credits"].ToString(), out credits);

                    returnMe.Add(new Student(
                            dataReader["FirstName"].ToString().Trim(),
                            dataReader["LastName"].ToString().Trim(),
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
                            dataReader["InStatus"].ToString().Trim(),
                            dataReader["InStatusCode"].ToString().Trim(),
                            HomeRoom,
                            DateTime.Parse(dataReader["InDate"].ToString()),
                            DateTime.Parse(dataReader["DateOfBirth"].ToString()),
                            dataReader["BandNo"].ToString().Trim(),
                            dataReader["BandName"].ToString().Trim(),
                            dataReader["ReserveName"].ToString().Trim(),
                            dataReader["ReserveHouse"].ToString().Trim(),
                            dataReader["StatusNo"].ToString().Trim(),
                            bool.Parse(dataReader["ResideOnReserve"].ToString()),
                            int.Parse(dataReader["TrackID"].ToString()),
                            hasPhoto,
                            dataReader["cUserName"].ToString().Trim(),
                            credits,
                            int.Parse(dataReader["iActive_StudentStatusID"].ToString()),
                            dataReader["AboriginalStatus"].ToString().Trim(),
                            dataReader["LanguageHome"].ToString().Trim()
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

                    int credits = 0;
                    int.TryParse(dataReader["Credits"].ToString(), out credits);

                    returnMe.Add(new Student(
                            dataReader["FirstName"].ToString().Trim(),
                            dataReader["LastName"].ToString().Trim(),
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
                            dataReader["InStatus"].ToString().Trim(),
                            dataReader["InStatusCode"].ToString().Trim(),
                            HomeRoom,
                            DateTime.Parse(dataReader["InDate"].ToString()),
                            DateTime.Parse(dataReader["DateOfBirth"].ToString()),
                            dataReader["BandNo"].ToString().Trim(),
                            dataReader["BandName"].ToString().Trim(),
                            dataReader["ReserveName"].ToString().Trim(),
                            dataReader["ReserveHouse"].ToString().Trim(),
                            dataReader["StatusNo"].ToString().Trim(),
                            bool.Parse(dataReader["ResideOnReserve"].ToString()),
                            int.Parse(dataReader["TrackID"].ToString()),
                            hasPhoto,
                            dataReader["cUserName"].ToString().Trim(),
                            credits,
                            int.Parse(dataReader["iActive_StudentStatusID"].ToString()),
                            dataReader["AboriginalStatus"].ToString().Trim(),
                            dataReader["LanguageHome"].ToString().Trim()
                            ));
                }
            }

            sqlCommand.Connection.Close();
            return returnMe;
        }

        public static List<Student> loadReserveStudentsFromThisSchool(SqlConnection connection, School school)
        {
            List<Student> returnMe = new List<Student>();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = "SELECT * FROM LSKY_ActiveStudents WHERE ResideOnReserve=1 AND SchoolID=@SchoolID ORDER BY SchoolID ASC, Grade ASC;";
            sqlCommand.Parameters.AddWithValue("@SchoolID", school.getGovIDAsString());
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

                    int credits = 0;
                    int.TryParse(dataReader["Credits"].ToString(), out credits);

                    Student newStudent = new Student(
                            dataReader["FirstName"].ToString().Trim(),
                            dataReader["LastName"].ToString().Trim(),
                            dataReader["LegalFirstName"].ToString().Trim(),
                            dataReader["LegalLastName"].ToString().Trim(),
                            dataReader["LegalMiddleName"].ToString(),
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
                            dataReader["InStatus"].ToString().Trim(),
                            dataReader["InStatusCode"].ToString().Trim(),
                            dataReader["HomeRoom"].ToString().Trim(),
                            DateTime.Parse(dataReader["InDate"].ToString()),
                            DateTime.Parse(dataReader["DateOfBirth"].ToString()),
                            dataReader["BandNo"].ToString().Trim(),
                            dataReader["BandName"].ToString().Trim(),
                            dataReader["ReserveName"].ToString().Trim(),
                            dataReader["ReserveHouse"].ToString().Trim(),
                            dataReader["StatusNo"].ToString().Trim(),
                            bool.Parse(dataReader["ResideOnReserve"].ToString()),
                            int.Parse(dataReader["TrackID"].ToString()),
                            hasPhoto,
                            dataReader["cUserName"].ToString().Trim(),
                            credits,
                            int.Parse(dataReader["iActive_StudentStatusID"].ToString()),
                            dataReader["AboriginalStatus"].ToString().Trim(),
                            dataReader["LanguageHome"].ToString().Trim()
                            );
                    returnMe.Add(newStudent);

                }
            }

            sqlCommand.Connection.Close();
            return returnMe;

        }

    }
}