using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SLReports
{
    public static class LSKYCommon
    {
        public static string internal_api_key = "6b05cb5705c07a4ca23a6bba779263ab983a5ae2";

        public static string boolToTrueFalse(bool thisBool)
        {
            if (thisBool)
            {
                return "True";
            }
            else
            {
                return "False";
            }
        }

        public static string boolToYesOrNoHTML(bool thisBool)
        {
            if (thisBool)
            {
                return "<span style=\"color: #007700;\">Yes</span>";
            }
            else
            {
                return "<span style=\"color: #770000;\">No</span>";
            }
        }

        public static string boolToYesOrNo(bool thisBool)
        {
            if (thisBool)
            {
                return "Yes";
            }
            else
            {
                return "No";
            }
        }

        public static int boolToOneOrZero(bool thisBool)
        {
            if (thisBool)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public static string removeSpaces(string working)
        {
            try
            {
                return Regex.Replace(working, @"[^\w]", "", RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            catch (RegexMatchTimeoutException)
            {
                return string.Empty;
            }
        }

        public static float translatePercentToOutcome(float percent)
        {
            float returnMe = -1;            

            if ((percent <= 100) && (percent >= 0))
            {
                if (percent < 40)
                {
                    returnMe = 1f;
                }
                else if (percent < 50)
                {
                    returnMe = 1.5f;
                }
                else if (percent < 60)
                {
                    returnMe = 2f;
                }
                else if (percent < 75)
                {
                    returnMe = 2.5f;
                }
                else if (percent < 90)
                {
                    returnMe = 3f;
                }
                else if (percent < 95)
                {
                    returnMe = 3.5f;
                }
                else if (percent <= 100)
                {
                    returnMe = 4f;
                }
            }

            return returnMe;
        }

        public static string getCurrentTimeStampForFilename()
        {
            return DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute;
        }

        public static string findClassNameForThisBlock(List<SchoolClass> classes, string block)
        {
            /* Try to parse the block */

            List<string> classNames = new List<string>();


            int blockNum = -1;
            if (int.TryParse(block, out blockNum))
            {
                foreach (SchoolClass thisclass in classes)
                {
                    if (thisclass.blockNumber == blockNum)
                    {
                        classNames.Add(thisclass.name);
                    }
                }
            }


            StringBuilder returnMe = new StringBuilder();
            for (int x = 0; x < classNames.Count; x++)
            {
                returnMe.Append(classNames[x]);
                if (x < classNames.Count - 1)
                {
                    returnMe.Append(", ");
                }
            }

            return returnMe.ToString();
        }

        public static Student loadStudentMarkData(SqlConnection connection, Student thisStudent, List<ReportPeriod> reportPeriods)
        {
            /*
             * Yes, this is really stupid and complicated, but it has to be beacuse of how data is organized in the schoollogic database.              
             * This function attempts to translate the data into a structure that makes more sense
             * */

            /* Find the earliest report period and the last report period, for attendance dates */
            DateTime earliestDate = DateTime.MaxValue;
            DateTime lastDate = DateTime.MinValue;

            List<int> detectedTermIDs = new List<int>();

            foreach (ReportPeriod rp in reportPeriods)
            {
                /* Find the earliest report period and the last report period, for attendance dates */
                if (rp.startDate < earliestDate)
                {
                    earliestDate = rp.startDate;
                }

                if (rp.endDate > lastDate)
                {
                    lastDate = rp.endDate;
                }

                /* Derive some terms from the given report periods while we are cycling through them */
                if (!detectedTermIDs.Contains(rp.termID))
                {
                    detectedTermIDs.Add(rp.termID);
                }
            }

            /* Derive some terms from the given report periods while we are cycling through them */
            List<Term> detectedTerms = new List<Term>();
            foreach (int termid in detectedTermIDs)
            {
                detectedTerms.Add(Term.loadThisTerm(connection, termid));
            }

            /* Put report periods into their respective terms */
            foreach (Term term in detectedTerms)
            {
                foreach (ReportPeriod rp in reportPeriods)
                {
                    if (rp.termID == term.ID)
                    {
                        term.ReportPeriods.Add(rp);
                    }
                }
            }



            Student student = thisStudent;
            if (student != null)
            {
                student.school = School.loadThisSchool(connection, thisStudent.getSchoolIDAsInt());
                student.track = Track.loadThisTrack(connection, student.getTrackID());
                student.track.terms = detectedTerms;

                /* Load attendance */
                student.absences = Absence.loadAbsencesForThisStudentAndTimePeriod(connection, thisStudent, earliestDate, lastDate);



                foreach (Term thisTerm in student.track.terms)
                {
                    /* Load enrolled classes */
                    thisTerm.Courses = SchoolClass.loadStudentEnrolledClasses(connection, student, thisTerm);

                    foreach (SchoolClass thisClass in thisTerm.Courses)
                    {

                        /* Put list of report periods into each class so we can easily reference it later */
                        thisClass.ReportPeriods = reportPeriods;

                        /* Load objectives and objective marks */
                        thisClass.Objectives = Objective.loadObjectivesForThisCourse(connection, thisClass);
                        thisClass.ObjectiveMarks = ObjectiveMark.loadObjectiveMarksForThisCourse(connection, thisTerm, student, thisClass);

                        /* Put objective marks in the corresonding objective */
                        foreach (ObjectiveMark objectivemark in thisClass.ObjectiveMarks)
                        {
                            foreach (Objective objective in thisClass.Objectives)
                            {
                                if (objectivemark.objectiveID == objective.id)
                                {
                                    objectivemark.objective = objective;
                                }
                            }

                            foreach (ReportPeriod rp in reportPeriods)
                            {
                                if (rp.ID == objectivemark.reportPeriodID)
                                {
                                    objectivemark.reportPeriod = rp;
                                }
                            }

                        }

                        foreach (Objective objective in thisClass.Objectives)
                        {
                            foreach (ObjectiveMark objectivemark in thisClass.ObjectiveMarks)
                            {
                                if (objective.id == objectivemark.objectiveID)
                                {
                                    objective.marks.Add(objectivemark);
                                }
                            }
                        }

                    }

                    /* Load class marks */

                    List<Mark> allMarks = new List<Mark>();
                    //thisTerm.ReportPeriods = ReportPeriod.loadReportPeriodsFromThisTerm(connection, thisTerm.ID);

                    foreach (ReportPeriod thisReportPeriod in thisTerm.ReportPeriods)
                    {
                        thisReportPeriod.marks = Mark.loadMarksFromThisReportPeriod(connection, thisReportPeriod, student);
                        foreach (Mark m in thisReportPeriod.marks)
                        {
                            allMarks.Add(m);
                        }
                    }

                    foreach (Mark m in allMarks)
                    {
                        foreach (SchoolClass c in thisTerm.Courses)
                        {
                            if (m.classID == c.classid)
                            {
                                c.Marks.Add(m);
                            }
                        }
                    }

                }

            }

            return student;
        }

        public static int parseInt(string thisString)
        {
            int returnMe = -1;

            if (int.TryParse(thisString, out returnMe))
            {
                return returnMe;
            }

            return -1;

        }

        public static string getMonthName(int monthNum)
        {
            string returnMe = "Unknown";
            switch (monthNum)
            {
                case 1:
                    returnMe = "January";
                    break;
                case 2:
                    returnMe = "February";
                    break;
                case 3:
                    returnMe = "March";
                    break;
                case 4:
                    returnMe = "April";
                    break;
                case 5:
                    returnMe = "May";
                    break;
                case 6:
                    returnMe = "June";
                    break;
                case 7:
                    returnMe = "July";
                    break;
                case 8:
                    returnMe = "August";
                    break;
                case 9:
                    returnMe = "September";
                    break;
                case 10:
                    returnMe = "October";
                    break;
                case 11:
                    returnMe = "November";
                    break;
                case 12:
                    returnMe = "December";
                    break;
            }
            return returnMe;
        }
    }
}