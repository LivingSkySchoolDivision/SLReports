using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace SLReports.INAC
{
    public static class LSKY_INAC
    {
        /// <summary>
        /// Attempts to figure out the number of days absent. The given student must have data loaded into it from the "loadStudentData" function in this class in order to work.
        /// </summary>
        /// <param name="student">The student object, filled with tracks, terms, courses, attendance, and contacts</param>
        /// <returns></returns>
        public static float getDaysAbsent(Student student)
        {
            string neverUsed = string.Empty;
            return getDaysAbsent(student, out neverUsed);
        }

        /// <summary>
        /// Attempts to figure out the number of days absent. The given student must have data loaded into it from the "loadStudentData" function in this class in order to work.
        /// </summary>
        /// <param name="student">The student object, filled with tracks, terms, courses, attendance, and contacts</param>
        /// <returns></returns>
        public static float getDaysAbsent(Student student, out String explaination)
        {
            StringBuilder calculationExplaination = new StringBuilder();
            float daysAbsent = -1;

            if (student.track != null)
            {
                // Should we be calculating daily absenses or class based absenses
                if (student.track.daily == true)
                {
                    calculationExplaination.Append("Track attendance is set to DAILY&#10;");
                    calculationExplaination.Append(" Absence count in blocks: " + student.absences.Count + "&#10;");
                    calculationExplaination.Append(" Blocks per day: " + student.track.dailyBlocksPerDay + "&#10;");
                    daysAbsent = (float)((float)student.absences.Count / (float)student.track.dailyBlocksPerDay);
                    calculationExplaination.Append(" " + (float)student.absences.Count + " / " + (float)student.track.dailyBlocksPerDay + " = " + daysAbsent);
                }
                else
                {
                    calculationExplaination.Append("Track attendance is set to PERIOD&#10;&#10;");
                    daysAbsent = 0;

                    // The required data should be preloaded into the student object...

                    // For each track and term
                    // Figure out how many classes per day this student actually has
                    // Calculate and add to the total                

                    foreach (Term term in student.track.terms)
                    {
                        calculationExplaination.Append(" Term: " + term.name + "&#10;");
                        calculationExplaination.Append("  Enrolled classes in this term: " + term.Courses.Count + "&#10;");
                        if (term.Courses.Count > 0)
                        {
                            // Get all absenses that fall within this term
                            List<Absence> thisTermAbsenses = new List<Absence>();
                            foreach (Absence abs in student.absences)
                            {
                                if ((abs.getDate() > term.startDate) && (abs.getDate() < term.endDate))
                                {
                                    thisTermAbsenses.Add(abs);
                                }
                            }
                            calculationExplaination.Append("  Absences in this term (in blocks): " + thisTermAbsenses.Count + "&#10;");
                            float daysAbsentThisTerm = (float)((float)thisTermAbsenses.Count / (float)term.Courses.Count);
                            daysAbsent += daysAbsentThisTerm;
                            calculationExplaination.Append("  " + (float)thisTermAbsenses.Count + " / " + (float)term.Courses.Count + " = " + daysAbsentThisTerm + "&#10;&#10;");
                        }
                    }
                }

            }
            else
            {
                throw new Exception("Student track is null");
            }
            explaination = calculationExplaination.ToString();
            return daysAbsent;
        }
        
        /// <summary>
        /// Filters a list of contacts to only the ones that would be displayed on the INAC report.
        /// </summary>
        /// <param name="contacts">A list of all contacts</param>
        /// <returns>A filtered list of contacts that match the INAC criteria (priority 1, lives with student)</returns>
        public static List<Contact> getINACGuardians(List<Contact> contacts)
        {
            List<Contact> guardians = new List<Contact>();
            
            foreach (Contact contact in contacts)
            {
                if ((contact.priority == 1) && (contact.livesWithStudent == true))
                {
                    guardians.Add(contact);
                }
            }

            return guardians;
        }

        /// <summary>
        /// Loads a student object with data suitable for the INAC report
        /// </summary>
        /// <param name="connection">A database connection</param>
        /// <param name="school"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static List<Student> loadStudentData(SqlConnection connection, School school, DateTime startDate, DateTime endDate)
        {
            List<Student> loadedStudentList = new List<Student>();
            List<Student> allLoadedStudents = Student.loadReserveStudentsFromThisSchol(connection, school);

            // Filter students to only those in a track that falls between the given dates
            foreach (Student student in allLoadedStudents)
            {
                student.track = Track.loadThisTrack(connection, student.getTrackID());
                if ((student.track.endDate > startDate) && (student.track.startDate < endDate))
                {
                    loadedStudentList.Add(student);
                }
            }

            // Load required data for students
            foreach (Student student in loadedStudentList)
            {
                // Load contacts for students
                student.contacts = Contact.loadContactsForStudent(connection, student);

                // Load absenses for students
                student.absences = Absence.loadAbsencesForThisStudentAndTimePeriod(connection, student, startDate, endDate);

                // Load any terms that fall within the specified dates
                student.track.terms = Term.loadTermsBetweenTheseDates(connection, student.track, startDate, endDate);

                // Load enrolled courses into the terms
                foreach (Term term in student.track.terms)
                {
                    term.Courses = SchoolClass.loadStudentEnrolledClasses(connection, student, term);
                }
            }

            return loadedStudentList;

        }


    }
}