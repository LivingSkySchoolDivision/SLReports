using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SLReports.Attendance
{
    public static class AttendancePDFParts
    {
        private static iTextSharp.text.Image lskyLogo = iTextSharp.text.Image.GetInstance(@"https://sldata.lskysd.ca/SLReports/Logo_Circle_Notext_Trans.png");

        private static Font font_large = FontFactory.GetFont("Verdana", 15, BaseColor.BLACK);
        private static Font font_large_bold = FontFactory.GetFont("Verdana", 15, Font.BOLD, BaseColor.BLACK);
        private static Font font_large_italic = FontFactory.GetFont("Verdana", 15, Font.ITALIC, BaseColor.BLACK);


        private static Font font_body_larger = FontFactory.GetFont("Verdana", 12, BaseColor.BLACK);
        private static Font font_body_larger_bold = FontFactory.GetFont("Verdana", 12, Font.BOLD, BaseColor.BLACK);

        private static Font font_body = FontFactory.GetFont("Verdana", 8, BaseColor.BLACK);
        private static Font font_body_bold = FontFactory.GetFont("Verdana", 8, Font.BOLD, BaseColor.BLACK);
        private static Font font_body_italic = FontFactory.GetFont("Verdana", 8, Font.ITALIC, BaseColor.BLACK);

        private static Font font_small = FontFactory.GetFont("Verdana", 8, BaseColor.BLACK);
        private static Font font_small_bold = FontFactory.GetFont("Verdana", 8, Font.BOLD, BaseColor.BLACK);
        private static Font font_small_italic = FontFactory.GetFont("Verdana", 8, Font.ITALIC, BaseColor.BLACK);


        public static PdfPTable livingSkyHeading()
        {
            PdfPTable titleTable = new PdfPTable(2);
            titleTable.SpacingAfter = 10f;
            titleTable.HorizontalAlignment = 1;
            titleTable.TotalWidth = 300f;
            titleTable.LockedWidth = true;


            float[] widths = new float[] { 25f, 275f };
            titleTable.SetWidths(widths);

            lskyLogo.ScaleAbsolute(22f, 22f);


            PdfPCell newCell = null;
            newCell = new PdfPCell(lskyLogo);
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            titleTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Living Sky School Division No. 202", font_large_bold));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            titleTable.AddCell(newCell);

            return titleTable;
        }

        public static PdfPTable pageTitle(PdfContentByte content, DateTime from, DateTime to)
        {
            PdfPTable titleTable = new PdfPTable(1);
            titleTable.SpacingAfter = 10f;
            titleTable.HorizontalAlignment = 1;
            titleTable.TotalWidth = 450f;
            titleTable.LockedWidth = true;


            PdfPCell newCell = null;
            newCell = new PdfPCell(new Phrase("Detailed Student Attendance", font_large_bold));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            titleTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase(from.ToLongDateString() + " to " + to.ToLongDateString(), font_body));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            titleTable.AddCell(newCell);

            return titleTable;
        }

        public static PdfPTable studentNamePlate(Student student)
        {

            PdfPTable nameplateTable = new PdfPTable(4);
            nameplateTable.SpacingAfter = 10f;
            nameplateTable.HorizontalAlignment = 1;
            nameplateTable.TotalWidth = 500f;
            nameplateTable.LockedWidth = true;

            //float[] widths = new float[] { 125, 175, 125, 175, 125, 175 };
            float[] widths = new float[] { 125, 525, 125, 125 };
            nameplateTable.SetWidths(widths);

            PdfPCell newCell = null;

            /*
            Paragraph displayNameParagraph = new Paragraph();
            displayNameParagraph.Add(new Phrase(student.getDisplayName(), font_body_larger_bold));
            displayNameParagraph.Add(Chunk.NEWLINE);
            if (!string.IsNullOrEmpty(student.getApartmentNo()))
            {
                displayNameParagraph.Add(new Phrase("Apt " + student.getApartmentNo(), font_body));
                displayNameParagraph.Add(Chunk.NEWLINE);
            }
            displayNameParagraph.Add(new Phrase(student.getHouseNo(), font_body));
            displayNameParagraph.Add(Chunk.NEWLINE);
            displayNameParagraph.Add(new Phrase(student.getCity(), font_body));
            displayNameParagraph.Add(Chunk.NEWLINE);
            displayNameParagraph.Add(new Phrase(student.getRegion() + ", " + student.getCountry(), font_body));
            displayNameParagraph.Add(Chunk.NEWLINE);
            displayNameParagraph.Add(new Phrase(student.getPostalCode(), font_body));

            newCell = new PdfPCell(displayNameParagraph);
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.Colspan = 2;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);
            */

            newCell = new PdfPCell(new Phrase("Student", font_body_bold));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase(student.getDisplayName(), font_body));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Student Number", font_body_bold));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase(student.getStudentID(), font_body));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);


            newCell = new PdfPCell(new Phrase("School", font_body_bold));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase(student.getSchoolName(), font_body));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Government ID", font_body_bold));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase(student.getGovernmentID(), font_body));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);            

            /*
            newCell = new PdfPCell(new Phrase("", font_body_bold));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("", font_body));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);
            */

            newCell = new PdfPCell(new Phrase("Contact", font_body_bold));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);

            StringBuilder contactList = new StringBuilder();
            foreach (Contact contact in student.getContacts())
            {
                if (contact.livesWithStudent)
                {
                    contactList.Append(contact.firstName + " " + contact.lastName + " " + formatPhoneNumber(contact.telephone) + " (" + contact.relation + ")\n");
                }
            }

            newCell = new PdfPCell(new Phrase(contactList.ToString(), font_body));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Grade", font_body_bold));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase(student.getGrade(), font_body));
            newCell.Padding = 2;
            newCell.Border = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            nameplateTable.AddCell(newCell);
                        
            return nameplateTable;

        }

        public static PdfPTable attendanceSummary(Student student)
        {
            if (student.absences.Count > 0)
            {
                PdfPTable summaryTable = new PdfPTable(4);
                summaryTable.SpacingAfter = 25f;
                summaryTable.HorizontalAlignment = 1;
                summaryTable.TotalWidth = 500f;
                summaryTable.LockedWidth = true;
                summaryTable.KeepTogether = true;

                PdfPCell newCell = null;
                
                /* Figure out some statistics to display */
                List<String> allPeriods = new List<String>();

                foreach (Absence abs in student.absences)
                {
                    if (!allPeriods.Contains(abs.period))
                    {
                        allPeriods.Add(abs.period);
                    }
                }
                allPeriods.Sort();

                /* Headings */
                newCell = new PdfPCell(new Phrase("Attendance Summary", font_body_bold));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.NO_BORDER;
                newCell.Colspan = 5;
                newCell.PaddingBottom = 5;
                summaryTable.AddCell(newCell);

                newCell = new PdfPCell(new Phrase("Period", font_body_bold));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.BOTTOM_BORDER;
                newCell.PaddingBottom = 5;
                newCell.BorderWidth = 1;
                summaryTable.AddCell(newCell);

                newCell = new PdfPCell(new Phrase("Lates", font_body_bold));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.BOTTOM_BORDER;
                newCell.PaddingBottom = 5;
                newCell.BorderWidth = 1;
                summaryTable.AddCell(newCell);

                newCell = new PdfPCell(new Phrase("Excused Absences", font_body_bold));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.BOTTOM_BORDER;
                newCell.PaddingBottom = 5;
                newCell.BorderWidth = 1;
                summaryTable.AddCell(newCell);

                newCell = new PdfPCell(new Phrase("Not Excused Absences", font_body_bold));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.BOTTOM_BORDER;
                newCell.PaddingBottom = 5;
                newCell.BorderWidth = 1;
                summaryTable.AddCell(newCell);


                int totalLates = 0;
                int totalAbsExc = 0;
                int totalAbsUnexc = 0;
                int totalMinutesLate = 0;

                /* Values */
                foreach (String periodName in allPeriods)
                {
                    int numLates = 0;
                    int numAbsExc = 0;
                    int numAbsUnexc = 0;
                    int numMinutesLate = 0;

                    foreach (Absence abs in student.absences)
                    {
                        if (abs.period == periodName)
                        {
                            if (abs.getStatus().ToLower() == "late")
                            {
                                numLates++;
                                totalLates++;
                                numMinutesLate += abs.getMinutes();
                                totalMinutesLate += abs.getMinutes();
                            }
                            else
                            {
                                if (abs.excused)
                                {
                                    numAbsExc++;
                                    totalAbsExc++;
                                }
                                else
                                {
                                    numAbsUnexc++;
                                    totalAbsUnexc++;
                                }
                            }
                        }
                    }

                    newCell = new PdfPCell(new Phrase(periodName, font_body));
                    newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    newCell.Border = Rectangle.NO_BORDER;
                    summaryTable.AddCell(newCell);

                    StringBuilder lateDisplay = new StringBuilder();
                    lateDisplay.Append(numLates);

                    if (numMinutesLate > 0)
                    {
                        lateDisplay.Append(" (" + numMinutesLate + " minutes)");
                    }

                    newCell = new PdfPCell(new Phrase(lateDisplay.ToString(), font_body));
                    newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    newCell.Border = Rectangle.NO_BORDER;
                    summaryTable.AddCell(newCell);


                    newCell = new PdfPCell(new Phrase(numAbsExc.ToString(), font_body));
                    newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    newCell.Border = Rectangle.NO_BORDER;
                    summaryTable.AddCell(newCell);

                    newCell = new PdfPCell(new Phrase(numAbsUnexc.ToString(), font_body));
                    newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    newCell.Border = Rectangle.NO_BORDER;
                    summaryTable.AddCell(newCell);

                }

                /* Totals */
                newCell = new PdfPCell(new Phrase("Total", font_body_bold));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.TOP_BORDER;
                newCell.BorderWidth = 1;
                summaryTable.AddCell(newCell);

                StringBuilder lateDisplay2 = new StringBuilder();
                lateDisplay2.Append(totalLates);

                if (totalMinutesLate > 0)
                {
                    lateDisplay2.Append(" (" + totalMinutesLate + " minutes)");
                }

                newCell = new PdfPCell(new Phrase(lateDisplay2.ToString(), font_body_bold));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.TOP_BORDER;
                newCell.BorderWidth = 1;
                summaryTable.AddCell(newCell);

                newCell = new PdfPCell(new Phrase(totalAbsExc.ToString(), font_body_bold));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.TOP_BORDER;
                newCell.BorderWidth = 1;
                summaryTable.AddCell(newCell);

                newCell = new PdfPCell(new Phrase(totalAbsUnexc.ToString(), font_body_bold));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.TOP_BORDER;
                newCell.BorderWidth = 1;
                summaryTable.AddCell(newCell);

                return summaryTable;
            }
            else
            {
                return new PdfPTable(1);
            }
        }

        private static String excusedToEnglish(bool thisBool)
        {
            if (thisBool)
            {
                return "Excused";
            }
            else
            {
                return "Not Excused";
            }
        }

        public static PdfPTable attendanceTable(Student student)
        {
            if (student.absences.Count > 0)
            {

                PdfPTable attendanceTable = new PdfPTable(6);
                attendanceTable.SpacingAfter = 25f;
                attendanceTable.HorizontalAlignment = 1;
                attendanceTable.TotalWidth = 500f;
                attendanceTable.LockedWidth = true;

                float[] widths = new float[] { 50, 100, 100, 80, 55, 115 };
                attendanceTable.SetWidths(widths);

                PdfPCell newCell = null;

                // copy and paste these
                //newCell.Padding = 2;
                //newCell.Border = 0;

                /* Headings */
                newCell = new PdfPCell(new Phrase("Date", font_body_bold));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.BOTTOM_BORDER;
                newCell.PaddingBottom = 5;
                newCell.BorderWidth = 2;
                attendanceTable.AddCell(newCell);

                newCell = new PdfPCell(new Phrase("Period / Class", font_body_bold));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.BOTTOM_BORDER;
                newCell.PaddingBottom = 5;
                newCell.BorderWidth = 2;
                attendanceTable.AddCell(newCell);

                newCell = new PdfPCell(new Phrase("Status", font_body_bold));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.BOTTOM_BORDER;
                newCell.PaddingBottom = 5;
                newCell.BorderWidth = 2;
                attendanceTable.AddCell(newCell);

                newCell = new PdfPCell(new Phrase("Reason", font_body_bold));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.BOTTOM_BORDER;
                newCell.PaddingBottom = 5;
                newCell.BorderWidth = 2;
                attendanceTable.AddCell(newCell);

                newCell = new PdfPCell(new Phrase("Is Excused", font_body_bold));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.BOTTOM_BORDER;
                newCell.PaddingBottom = 5;
                newCell.BorderWidth = 2;
                attendanceTable.AddCell(newCell);

                newCell = new PdfPCell(new Phrase("Comment", font_body_bold));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.BOTTOM_BORDER;
                newCell.PaddingBottom = 5;
                newCell.BorderWidth = 2;
                attendanceTable.AddCell(newCell);

                DateTime lastDay = DateTime.Now;
                BaseColor[] backgroundColors = { new BaseColor(255, 255, 255), new BaseColor(235, 235, 235) };
                int colorindex = 0;

                foreach (Absence abs in student.absences)
                {
                    newCell = new PdfPCell(new Phrase(abs.getDate().ToShortDateString(), font_body));
                    newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    newCell.Border = Rectangle.BOTTOM_BORDER;

                    if ((abs.getDate().Day == lastDay.Day) && (abs.getDate().Month == lastDay.Month) && (abs.getDate().Year == lastDay.Year))
                    {
                        newCell.AddElement(new Phrase(""));
                    }
                    else
                    {
                        colorindex++;
                        if (colorindex >= backgroundColors.Length)
                        {
                            colorindex = 0;
                        }
                    }

                    lastDay = abs.getDate();

                    newCell.BackgroundColor = backgroundColors[colorindex];

                    attendanceTable.AddCell(newCell);

                    /* Block */
                    /* Check if course has a name */

                    StringBuilder blockName = new StringBuilder();

                    blockName.Append(abs.getPeriod());

                    if (!string.IsNullOrEmpty(abs.getCourseName()))
                    {
                        blockName.Append(" (" + abs.getCourseName() + ")");
                    }

                    newCell = new PdfPCell(new Phrase(blockName.ToString(), font_body));
                    newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    newCell.Border = Rectangle.BOTTOM_BORDER;
                    newCell.BackgroundColor = backgroundColors[colorindex];
                    attendanceTable.AddCell(newCell);


                    StringBuilder status = new StringBuilder();
                    status.Append(abs.getStatus());
                    if (abs.getMinutes() > 0)
                    {
                        status.Append(" (" + abs.getMinutes() + " min)");
                    }

                    newCell = new PdfPCell(new Phrase(status.ToString(), font_body));
                    newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    newCell.Border = Rectangle.BOTTOM_BORDER;
                    newCell.BackgroundColor = backgroundColors[colorindex];
                    attendanceTable.AddCell(newCell);

                    newCell = new PdfPCell(new Phrase(abs.getReason(), font_body));
                    newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    newCell.Border = Rectangle.BOTTOM_BORDER;
                    newCell.BackgroundColor = backgroundColors[colorindex];
                    attendanceTable.AddCell(newCell);

                    newCell = new PdfPCell(new Phrase(excusedToEnglish(abs.excused), font_body));
                    newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    newCell.Border = Rectangle.BOTTOM_BORDER;
                    newCell.BackgroundColor = backgroundColors[colorindex];
                    attendanceTable.AddCell(newCell);

                    newCell = new PdfPCell(new Phrase(abs.getComment(), font_body));
                    newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                    newCell.Border = Rectangle.BOTTOM_BORDER;
                    newCell.BackgroundColor = backgroundColors[colorindex];
                    attendanceTable.AddCell(newCell);
                }



                return attendanceTable;
            }
            else
            {
                /* No absences */
                PdfPTable attendanceTable = new PdfPTable(1);
                attendanceTable.SpacingAfter = 25f;
                attendanceTable.HorizontalAlignment = 1;
                attendanceTable.TotalWidth = 500f;
                attendanceTable.LockedWidth = true;


                PdfPCell newCell = null;

                newCell = new PdfPCell(new Phrase("- No absences within the specified time period - ", font_body));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.NO_BORDER;
                attendanceTable.AddCell(newCell);

                return attendanceTable;
            }

        }

        private static string formatPhoneNumber(string unformatted)
        {
            if (unformatted.Length == 10)
            {
                string areaCode = unformatted.Substring(0, 3);
                string exchange = unformatted.Substring(3, 3);
                string number = unformatted.Substring(6, 4);

                return "(" + areaCode + ") " + exchange + "-" + number;
            }
            else
            {
                return unformatted;
            }
        }
    }
}