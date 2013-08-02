using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace SLReports
{
    public static class LSKYCommon
    {
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

    }
}