using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.ReportCard
{
    public partial class SingleReportPeriodPDF : System.Web.UI.Page
    {
        private Font font_large = FontFactory.GetFont("Verdana", 15, BaseColor.BLACK);
        private Font font_large_bold = FontFactory.GetFont("Verdana", 15, Font.BOLD, BaseColor.BLACK);
        private Font font_large_italic = FontFactory.GetFont("Verdana", 15, Font.ITALIC, BaseColor.BLACK);

        private Font font_body = FontFactory.GetFont("Verdana", 10, BaseColor.BLACK);
        private Font font_body_bold = FontFactory.GetFont("Verdana", 10, Font.BOLD, BaseColor.BLACK);
        private Font font_body_italic = FontFactory.GetFont("Verdana", 10, Font.ITALIC, BaseColor.BLACK);

        private Font font_small = FontFactory.GetFont("Verdana", 8, BaseColor.BLACK);
        private Font font_small_bold = FontFactory.GetFont("Verdana", 8, Font.BOLD, BaseColor.BLACK);
        private Font font_small_bold_white = FontFactory.GetFont("Verdana", 8, Font.BOLD, BaseColor.WHITE);
        private Font font_small_italic = FontFactory.GetFont("Verdana", 8, Font.ITALIC, BaseColor.BLACK);

        private Student selectedStudent = null;
        private ReportPeriod selectedReportPeriod = null;

        protected void Page_Init(object sender, EventArgs e)
        {
        }
       
        /// <summary>
        /// Displays a graphical number bar for outcomes
        /// </summary>
        /// <param name="content">A PdfContentByte object for the document</param>
        /// <param name="value">The value of the outcome mark to display</param>
        /// <returns></returns>
        /// 
        /// We are routing outcome bars through this routine so that we can change the style of the outcome bar in one place more easily
        private iTextSharp.text.Image displayOutcomeBar(PdfContentByte content, String value)
        {
            return outcomeBar_Slider(content, value);
        }

        #region Outcome Bar Styles
        protected iTextSharp.text.Image outcomeBar_Original(PdfContentByte content, String value)
        {
            int width = 125;
            int height = 10;
            int maxvalue = 4;
            int rectancleCurveRadius = 2;

            int CanvasPaddingX = 1;
            int CanvasPaddingY = 1;

            /* Colors */
            BaseColor fillColor = new BaseColor(70, 70, 70);
            BaseColor borderColor = new BaseColor(0, 0, 0);

            Single parsedValue = -1;

            PdfTemplate canvas = content.CreateTemplate(width + (CanvasPaddingX * 2), height + (CanvasPaddingY * 2));

            if (Single.TryParse(value, out parsedValue))
            {
                Single percentFill = parsedValue / maxvalue;

                /* Background */
                canvas.Rectangle(0, 0, width, height);
                canvas.SetRGBColorFill(255, 255, 255);
                canvas.Fill();

                /* Determine fill color  based on value */
                if (parsedValue <= 1)
                {
                    fillColor = new BaseColor(255, 51, 0);
                }
                else if (parsedValue <= 2.25)
                {
                    fillColor = new BaseColor(255, 165, 0);
                }
                else if (parsedValue <= 3.5)
                {
                    fillColor = new BaseColor(0, 128, 0);
                }
                else if (parsedValue <= 4)
                {
                    fillColor = new BaseColor(0, 128, 0);
                }


                /* Fill */
                if ((parsedValue > 0) && (parsedValue <= 4))
                {
                    canvas.RoundRectangle(CanvasPaddingX, CanvasPaddingY, (width * percentFill) - CanvasPaddingX, height, rectancleCurveRadius);
                    if (parsedValue < 4)
                    {
                        canvas.Rectangle(CanvasPaddingX + rectancleCurveRadius, CanvasPaddingY, (width * percentFill) - rectancleCurveRadius - CanvasPaddingX, height);
                    }
                    canvas.SetColorFill(fillColor);
                    canvas.Fill();
                }

                /* Overlay */
                canvas.SetColorStroke(borderColor);
                canvas.SetLineWidth(1);
                canvas.MoveTo((float)(width * 0.25) * 1, (float)CanvasPaddingY);
                canvas.LineTo((float)(width * 0.25) * 1, (float)height + CanvasPaddingY);
                canvas.MoveTo((float)(width * 0.25) * 2, (float)CanvasPaddingY);
                canvas.LineTo((float)(width * 0.25) * 2, (float)height + CanvasPaddingY);
                canvas.MoveTo((float)(width * 0.25) * 3, (float)CanvasPaddingY);
                canvas.LineTo((float)(width * 0.25) * 3, (float)height + CanvasPaddingY);
                canvas.Stroke();

                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                canvas.BeginText();
                canvas.SetFontAndSize(bf, 8);

                canvas.SetColorFill(borderColor);

                canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "1", (float)((width * 0.25) / 2), (float)((height + CanvasPaddingY) / 2) - 2, 0);
                canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "2", (float)(((width * 0.25) / 2) + (width * 0.25)), (float)((height + CanvasPaddingY) / 2) - 2, 0);
                canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "3", (float)(((width * 0.25) / 2) + (width * 0.50)), (float)((height + CanvasPaddingY) / 2) - 2, 0);
                canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "4", (float)(((width * 0.25) / 2) + (width * 0.75)), (float)((height + CanvasPaddingY) / 2) - 2, 0);

                canvas.EndText();


            }
            else
            {
                /* IE */
                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                canvas.SetRGBColorFill(0, 0, 0);
                canvas.BeginText();
                canvas.SetFontAndSize(bf, 8);

                if (value.ToLower() == "ie")
                {
                    canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "INSUFFICIENT EVIDENCE", width / 2, (height / 2) - 2, 0);
                }
                else
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        value = "INVALID VALUE";
                    }
                    canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, value, width / 2, (height / 2) - 2, 0);
                }
                canvas.EndText();
            }

            /* Border */

            canvas.SetRGBColorStroke(255, 255, 255);
            canvas.RoundRectangle(CanvasPaddingX, CanvasPaddingY, width - CanvasPaddingX, height, rectancleCurveRadius);
            canvas.SetColorStroke(borderColor);
            canvas.Stroke();


            return iTextSharp.text.Image.GetInstance(canvas); ;
        }
        protected iTextSharp.text.Image outcomeBar_Thin(PdfContentByte content, String value)
        {
            int width = 125;
            int height = 6;

            int CanvasPaddingX = 1;
            int CanvasPaddingY = 1;

            int maxvalue = 4;
            int rectancleCurveRadius = 2;

            /* Colors */
            BaseColor fillColor = new BaseColor(70, 70, 70);
            BaseColor borderColor = new BaseColor(0, 0, 0);

            Single parsedValue = -1;

            PdfTemplate canvas = content.CreateTemplate(width + (CanvasPaddingX * 2), height + (CanvasPaddingY * 2));

            /* Background */
            canvas.Rectangle(0, 0, width + (CanvasPaddingX * 2), height + (CanvasPaddingY * 2));
            canvas.SetRGBColorFill(255, 255, 255);
            canvas.Fill();

            if (Single.TryParse(value, out parsedValue))
            {
                Single percentFill = parsedValue / maxvalue;

                /* Determine fill color  based on value */
                if (parsedValue <= 1)
                {
                    fillColor = new BaseColor(255, 51, 0);
                }
                else if (parsedValue <= 2.25)
                {
                    fillColor = new BaseColor(255, 165, 0);
                }
                else if (parsedValue <= 3.5)
                {
                    fillColor = new BaseColor(0, 128, 0);
                }
                else if (parsedValue <= 4)
                {
                    fillColor = new BaseColor(0, 128, 0);
                }

                /* Fill */
                if ((parsedValue > 0) && (parsedValue <= 4))
                {
                    canvas.RoundRectangle(CanvasPaddingX, CanvasPaddingY, width * percentFill, height, rectancleCurveRadius);
                    if (parsedValue < 4)
                    {
                        canvas.Rectangle(CanvasPaddingX + rectancleCurveRadius, CanvasPaddingY, (width * percentFill) - rectancleCurveRadius, height);
                    }
                    canvas.SetColorFill(fillColor);
                    canvas.Fill();
                }

                /* Tick marks */
                canvas.SetColorStroke(borderColor);
                canvas.SetLineWidth(1);
                canvas.MoveTo((float)(width * 0.25) * 1, (float)CanvasPaddingY);
                canvas.LineTo((float)(width * 0.25) * 1, (float)height + CanvasPaddingY);
                canvas.MoveTo((float)(width * 0.25) * 2, (float)CanvasPaddingY);
                canvas.LineTo((float)(width * 0.25) * 2, (float)height + CanvasPaddingY);
                canvas.MoveTo((float)(width * 0.25) * 3, (float)CanvasPaddingY);
                canvas.LineTo((float)(width * 0.25) * 3, (float)height + CanvasPaddingY);
                canvas.Stroke();

                /* Border */
                canvas.SetRGBColorStroke(255, 255, 255);
                canvas.RoundRectangle(CanvasPaddingX, CanvasPaddingY, width, height, rectancleCurveRadius);
                canvas.SetColorStroke(borderColor);
                canvas.Stroke();

            }
            else
            {
                /* IE */
                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                canvas.SetRGBColorFill(0, 0, 0);
                canvas.BeginText();
                canvas.SetFontAndSize(bf, 6);

                if (value.ToLower() == "ie")
                {
                    canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "INSUFFICIENT EVIDENCE", width / 2, (height / 2) - 2, 0);
                }
                else
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        value = "INVALID VALUE";
                    }
                    canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, value, width / 2, (height / 2) - 2, 0);
                }
                canvas.EndText();
            }

            return iTextSharp.text.Image.GetInstance(canvas); ;
        }
        protected iTextSharp.text.Image outcomeBar_Slider(PdfContentByte content, String value)
        {
            int width = 125;
            int height = 12;
            int barMargin = 3;

            int CanvasPaddingX = 1;
            int CanvasPaddingY = 1;

            int maxvalue = 4;
            int rectancleCurveRadius = 2;

            /* Colors */
            BaseColor fillColor = new BaseColor(70, 70, 70);
            BaseColor borderColor = new BaseColor(0, 0, 0);
            BaseColor textColor = new BaseColor(255, 255, 255);

            Single parsedValue = -1;

            PdfTemplate canvas = content.CreateTemplate(width + 2, height + 2);

            if (Single.TryParse(value, out parsedValue))
            {
                if (parsedValue > maxvalue)
                {
                    parsedValue = maxvalue;
                }

                Single percentFill = parsedValue / maxvalue;

                /* Determine fill color  based on value */
                if (parsedValue <= 1)
                {
                    fillColor = new BaseColor(255, 51, 0); // Red
                }
                else if (parsedValue <= 2.25)
                {
                    fillColor = new BaseColor(255, 119, 0); // Orange
                }
                else if (parsedValue <= 3.5)
                {
                    fillColor = new BaseColor(0, 128, 0); // green
                }
                else if (parsedValue <= 4)
                {
                    fillColor = new BaseColor(0, 128, 0); // Green
                }

                /* Fill */
                if ((parsedValue > 0) && (parsedValue <= 4))
                {
                    canvas.RoundRectangle(CanvasPaddingX, barMargin, width * percentFill, height - (barMargin * 2), rectancleCurveRadius);
                    if (parsedValue < 4)
                    {
                        canvas.Rectangle(CanvasPaddingX + rectancleCurveRadius, barMargin, (width * percentFill) - rectancleCurveRadius, height - (barMargin * 2));
                    }
                    canvas.SetColorFill(fillColor);
                    canvas.Fill();
                }


                /* Border */
                canvas.SetRGBColorStroke(255, 255, 255);
                canvas.RoundRectangle(CanvasPaddingX, barMargin, width, height - (barMargin * 2), rectancleCurveRadius);
                canvas.SetColorStroke(borderColor);
                canvas.Stroke();


                /* Number indicator / Overlay */
                int indicatorWidth = 15;
                float indicatorX = (width * percentFill) - (indicatorWidth / 2) + CanvasPaddingX;
                float indicatorY = CanvasPaddingY;

                if (indicatorX < (CanvasPaddingX))
                {
                    indicatorX = CanvasPaddingX;
                }

                if ((indicatorX + indicatorWidth) > width)
                {
                    indicatorX = width - indicatorWidth + CanvasPaddingX;
                }

                /* Indicator fill */
                canvas.RoundRectangle(indicatorX, indicatorY, indicatorWidth, height - (CanvasPaddingY * 2), rectancleCurveRadius);
                canvas.SetColorFill(fillColor);
                canvas.Fill();

                /* Indicator border */
                canvas.RoundRectangle(indicatorX, indicatorY, indicatorWidth, height - (CanvasPaddingY * 2), rectancleCurveRadius);
                canvas.SetColorStroke(borderColor);
                canvas.Stroke();

                /* Indicator text */
                canvas.SetColorFill(textColor);
                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                canvas.BeginText();
                canvas.MoveText(indicatorX + (indicatorWidth / 2) - (bf.GetWidthPoint(parsedValue.ToString(), 8) / 2), (height / 2) - (CanvasPaddingY * 2));
                canvas.SetFontAndSize(bf, 8);
                canvas.ShowText(parsedValue.ToString());
                //canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "X", indicatorX + indicatorWidth, CanvasStartY, 0);
                canvas.EndText();
            }
            else
            {
                /* IE */
                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                canvas.SetRGBColorFill(0, 0, 0);
                canvas.BeginText();
                canvas.SetFontAndSize(bf, 8);

                if (value.ToLower() == "ie")
                {
                    canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "INSUFFICIENT EVIDENCE", width / 2, (height / 2) - 2, 0);
                }
                else
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        value = "INVALID VALUE";
                    }
                    canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, value, width / 2, (height / 2) - 2, 0);
                }
                canvas.EndText();

                /* Border */
                canvas.SetRGBColorStroke(255, 255, 255);
                canvas.RoundRectangle(CanvasPaddingX, CanvasPaddingY, width, height, rectancleCurveRadius);
                canvas.SetColorStroke(borderColor);
                canvas.Stroke();

            }

            return iTextSharp.text.Image.GetInstance(canvas); ;
        }
        protected iTextSharp.text.Image outcomeBar_Minimalist(PdfContentByte content, String value)
        {
            int width = 125;
            int height = 6;

            int CanvasPaddingX = 1;
            int CanvasPaddingY = 1;

            int maxvalue = 4;
            int rectancleCurveRadius = 2;

            /* Colors */
            BaseColor fillColor = new BaseColor(70, 70, 70);
            BaseColor borderColor = new BaseColor(0, 0, 0);

            Single parsedValue = -1;

            PdfTemplate canvas = content.CreateTemplate(width + (CanvasPaddingX * 2), height + (CanvasPaddingY * 2));

            /* Background */
            canvas.Rectangle(0, 0, width + (CanvasPaddingX * 2), height + (CanvasPaddingY * 2));
            canvas.SetRGBColorFill(255, 255, 255);
            canvas.Fill();

            if (Single.TryParse(value, out parsedValue))
            {
                Single percentFill = parsedValue / maxvalue;

                /* Determine fill color  based on value */
                if (parsedValue <= 1)
                {
                    fillColor = new BaseColor(255, 51, 0);
                }
                else if (parsedValue <= 2.25)
                {
                    fillColor = new BaseColor(255, 165, 0);
                }
                else if (parsedValue <= 3.5)
                {
                    fillColor = new BaseColor(0, 128, 0);
                }
                else if (parsedValue <= 4)
                {
                    fillColor = new BaseColor(0, 128, 0);
                }

                /* Fill */
                if ((parsedValue > 0) && (parsedValue <= 4))
                {
                    canvas.Rectangle(CanvasPaddingX, CanvasPaddingY, (width * percentFill) - rectancleCurveRadius, height);
                    canvas.SetColorFill(fillColor);
                    canvas.Fill();
                }

            }
            else
            {
                /* IE */
                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                canvas.SetRGBColorFill(0, 0, 0);
                canvas.BeginText();
                canvas.SetFontAndSize(bf, 6);

                if (value.ToLower() == "ie")
                {
                    canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "INSUFFICIENT EVIDENCE", width / 2, (height / 2) - 2, 0);
                }
                else
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        value = "INVALID VALUE";
                    }
                    canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, value, width / 2, (height / 2) - 2, 0);
                }
                canvas.EndText();
            }

            return iTextSharp.text.Image.GetInstance(canvas); ;
        }
        #endregion

        protected PdfPTable schoolNamePlate(School school)
        {
            int cellpadding = 3;
            int border = Rectangle.NO_BORDER;

            PdfPTable schoolNamePlateTable = new PdfPTable(1);
            schoolNamePlateTable.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            schoolNamePlateTable.TotalWidth = 425;
            schoolNamePlateTable.LockedWidth = true;
            schoolNamePlateTable.SpacingAfter = 50;

            //float[] widths = new float[] { 100f, 125f, 225f };
            //schoolNamePlateTable.SetWidths(widths);
            PdfPCell newCell = null;
            newCell = new PdfPCell(new Phrase(school.getName(), font_large_bold));
            newCell.VerticalAlignment = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.Padding = cellpadding;
            newCell.Border = border;
            schoolNamePlateTable.AddCell(newCell);
            
            newCell = new PdfPCell(new Phrase(school.address, font_body));
            newCell.VerticalAlignment = 0;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.Padding = cellpadding;
            newCell.Border = border;
            schoolNamePlateTable.AddCell(newCell);


            return schoolNamePlateTable;

        }

        protected PdfPTable namePlateTable(Student student, ReportPeriod period)
        {
            int cellpadding = 3;
            int border = Rectangle.NO_BORDER;

            if (student == null)
            {
                student = new Student("John", "Smith", "J", "00000", "00000", "School Name", "00000", "Grade 15", "Saskatchewan", "North Battleford", "Fake St", "123", "", "H0H0H0", "3065551234", "Male", "Instatus", "Instatuscode", "Homeroom here", DateTime.Now.AddDays(-1), DateTime.Now, 00000, false);
            }

            PdfPTable nameplateTable = new PdfPTable(3);
            nameplateTable.HorizontalAlignment = 1;
            nameplateTable.TotalWidth = 425;
            nameplateTable.LockedWidth = true;
            nameplateTable.SpacingAfter = 50;

            float[] widths = new float[] { 100f, 125f, 225f };
            nameplateTable.SetWidths(widths);

            PdfPCell photoCell = new PdfPCell(new Phrase("(No Photo)", font_large_italic));

            //if (student.hasPhoto())
            {
                try
                {
                    iTextSharp.text.Image photo = iTextSharp.text.Image.GetInstance(@"https://sldata.lskysd.ca/SLReports/photos/GetPhoto.aspx?studentnumber=" + student.getStudentID());
                    photo.Border = Rectangle.BOX;
                    photo.BorderWidth = 1;
                    photoCell.PaddingRight = 10f;

                    photoCell = new PdfPCell(photo);
                }
                catch (Exception ex)
                {
                    photoCell = new PdfPCell(new Phrase(ex.Message, font_large_italic));
                };
            }

            photoCell.Border = border;
            photoCell.MinimumHeight = 300f;
            photoCell.Rowspan = 10;
            photoCell.VerticalAlignment = 1;
            photoCell.HorizontalAlignment = 1;
            nameplateTable.AddCell(photoCell);

            /*
            PdfPCell StudentNameCell_Header = new PdfPCell(new Phrase("Student", font_body_bold));
            StudentNameCell_Header.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            StudentNameCell_Header.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            StudentNameCell_Header.Padding = cellpadding;
            StudentNameCell_Header.Border = border;
            nameplateTable.AddCell(StudentNameCell_Header);
            */

            PdfPCell newCell = null;

            newCell = new PdfPCell(new Phrase(student.getDisplayName(), font_large_bold));
            newCell.Border = border;
            newCell.Colspan = 2;
            newCell.Padding = cellpadding;
            newCell.PaddingBottom = 10;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            nameplateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Student Number", font_body_bold));
            newCell.Border = border;
            newCell.Padding = cellpadding;
            newCell.PaddingLeft = 10;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            nameplateTable.AddCell(newCell);
            newCell = new PdfPCell(new Phrase(student.getStudentID(), font_body));
            newCell.Border = border;
            newCell.Padding = cellpadding;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            nameplateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("School", font_body_bold));
            newCell.Border = border;
            newCell.Padding = cellpadding;
            newCell.PaddingLeft = 10;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            nameplateTable.AddCell(newCell);
            newCell = new PdfPCell(new Phrase(student.getSchoolName(), font_body));
            newCell.Border = border;
            newCell.Padding = cellpadding;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            nameplateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Grade", font_body_bold));
            newCell.Border = border;
            newCell.Padding = cellpadding;
            newCell.PaddingLeft = 10;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            nameplateTable.AddCell(newCell);
            newCell = new PdfPCell(new Phrase(student.getGrade(), font_body));
            newCell.Border = border;
            newCell.Padding = cellpadding;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            nameplateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Home Room", font_body_bold));
            newCell.Border = border;
            newCell.Padding = cellpadding;
            newCell.PaddingLeft = 10;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            nameplateTable.AddCell(newCell);
            newCell = new PdfPCell(new Phrase(student.getHomeRoom(), font_body));
            newCell.Border = border;
            newCell.Padding = cellpadding;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            nameplateTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Report Period", font_body_bold));
            newCell.Border = border;
            newCell.Padding = cellpadding;
            newCell.PaddingLeft = 10;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            nameplateTable.AddCell(newCell);
            Paragraph description = new Paragraph();

            description.Add(new Phrase(period.name, font_body));
            description.Add(new Phrase(" (" + period.startDate.ToLongDateString() + " to " + period.endDate.ToLongDateString() + ")", font_body_italic));

            newCell = new PdfPCell(description);
            newCell.Border = border;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.Padding = cellpadding;
            nameplateTable.AddCell(newCell);


            return nameplateTable;

        }

        protected PdfPTable attendanceSummary(Student student)
        {
            PdfPTable attendanceTable = new PdfPTable(4);
            attendanceTable.HorizontalAlignment = 1;
            attendanceTable.TotalWidth = 500;
            attendanceTable.LockedWidth = true;
            attendanceTable.SpacingAfter = 50;
            attendanceTable.KeepTogether = true;

            PdfPCell newCell = null;

            /* Gather stats */
            int totalAbs = 0;
            int totalExcused = 0;
            int totalUnexcused = 0;
            int totalLates = 0;
            int totalLateMinutes = 0;

            List<String> allCourses = new List<String>();

            allCourses.Clear();
            foreach (Absence abs in student.absences)
            {
                // Collect a list of courses
                if (!allCourses.Contains(abs.period))
                {
                    allCourses.Add(abs.period);
                }
            }

            allCourses.Sort();

            newCell = new PdfPCell(new Paragraph("Attendance Summary\n\n", font_large_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.NO_BORDER;
            newCell.Colspan = 5;
            newCell.PaddingBottom = 5;
            attendanceTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Period", font_body_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.BOTTOM_BORDER;
            newCell.PaddingBottom = 5;
            newCell.BorderWidth = 1;
            attendanceTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Lates", font_body_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.BOTTOM_BORDER;
            newCell.PaddingBottom = 5;
            newCell.BorderWidth = 1;
            attendanceTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Excused Absences", font_body_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.BOTTOM_BORDER;
            newCell.PaddingBottom = 5;
            newCell.BorderWidth = 1;
            attendanceTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Unexcused Absences", font_body_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.BOTTOM_BORDER;
            newCell.PaddingBottom = 5;
            newCell.BorderWidth = 1;
            attendanceTable.AddCell(newCell);

            /* Values */
            foreach (String courseName in allCourses)
            {
                int numLates = 0;
                int numAbsExc = 0;
                int numAbsUnexc = 0;
                int numMinutesLate = 0;

                foreach (Absence abs in student.absences)
                {
                    if (abs.period == courseName)
                    {
                        if (abs.getStatus().ToLower() == "late")
                        {
                            numLates++;
                            totalLates++;
                            numMinutesLate += abs.getMinutes();
                            totalLateMinutes += abs.getMinutes();
                        }
                        else
                        {
                            if (abs.excused)
                            {
                                numAbsExc++;
                                totalExcused++;
                            }
                            else
                            {
                                numAbsUnexc++;
                                totalUnexcused++;
                            }
                        }
                    }
                }

                newCell = new PdfPCell(new Phrase(courseName, font_body));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.NO_BORDER;
                attendanceTable.AddCell(newCell);

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
                attendanceTable.AddCell(newCell);


                newCell = new PdfPCell(new Phrase(numAbsExc.ToString(), font_body));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.NO_BORDER;
                attendanceTable.AddCell(newCell);

                newCell = new PdfPCell(new Phrase(numAbsUnexc.ToString(), font_body));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.NO_BORDER;
                attendanceTable.AddCell(newCell);
            }


            /* Totals */
            newCell = new PdfPCell(new Phrase("Total", font_body_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.TOP_BORDER;
            newCell.BorderWidth = 1;
            attendanceTable.AddCell(newCell);

            StringBuilder lateDisplay2 = new StringBuilder();
            lateDisplay2.Append(totalLates);

            if (totalLateMinutes > 0)
            {
                lateDisplay2.Append(" (" + totalLateMinutes + " minutes)");
            }

            newCell = new PdfPCell(new Phrase(lateDisplay2.ToString(), font_body_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.TOP_BORDER;
            newCell.BorderWidth = 1;
            attendanceTable.AddCell(newCell);


            newCell = new PdfPCell(new Phrase(totalExcused.ToString(), font_body_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.TOP_BORDER;
            newCell.BorderWidth = 1;
            attendanceTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase(totalUnexcused.ToString(), font_body_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.TOP_BORDER;
            newCell.BorderWidth = 1;
            attendanceTable.AddCell(newCell);
            

            return attendanceTable;

        }

        protected PdfPTable courseAttendanceSummary(Student student, Course course)
        {
            PdfPTable attendanceTable = new PdfPTable(4);
            attendanceTable.HorizontalAlignment = 1;
            attendanceTable.TotalWidth = 475;
            attendanceTable.LockedWidth = true;
            attendanceTable.SpacingAfter = 50;
            attendanceTable.KeepTogether = true;

            PdfPCell newCell = null;

            /* Gather stats */
            int totalAbs = 0;
            int totalExcused = 0;
            int totalUnexcused = 0;
            int totalLates = 0;
            int totalLateMinutes = 0;

            foreach (Absence abs in student.absences)
            {
                if (abs.getCourseName() == course.name) 
                {
                    if (abs.getStatus().ToLower() == "late")
                    {
                        totalLates++;
                        totalLateMinutes += abs.getMinutes();
                    }
                    else
                    {
                        totalAbs++;
                        if (abs.excused)
                        {
                            totalExcused++;
                        }
                        else
                        {
                            totalUnexcused++;
                        }
                    }
                }
            }

            /* Headings */
            newCell = new PdfPCell(new Phrase("Course Attendance", font_body_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.BOTTOM_BORDER;
            newCell.PaddingBottom = 5;
            newCell.BorderWidth = 1;
            attendanceTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Lates", font_body_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.BOTTOM_BORDER;
            newCell.PaddingBottom = 5;
            newCell.BorderWidth = 1;
            attendanceTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Excused Absences", font_body_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.BOTTOM_BORDER;
            newCell.PaddingBottom = 5;
            newCell.BorderWidth = 1;
            attendanceTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Unexcused Absences", font_body_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.BOTTOM_BORDER;
            newCell.PaddingBottom = 5;
            newCell.BorderWidth = 1;
            attendanceTable.AddCell(newCell);

            /* Totals */
            newCell = new PdfPCell(new Phrase(course.name, font_body_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.TOP_BORDER;
            newCell.BorderWidth = 1;
            attendanceTable.AddCell(newCell);

            StringBuilder lateDisplay2 = new StringBuilder();
            lateDisplay2.Append(totalLates);

            if (totalLateMinutes > 0)
            {
                lateDisplay2.Append(" (" + totalLateMinutes + " minutes)");
            }

            newCell = new PdfPCell(new Phrase(lateDisplay2.ToString(), font_small));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.TOP_BORDER;
            newCell.BorderWidth = 1;
            attendanceTable.AddCell(newCell);


            newCell = new PdfPCell(new Phrase(totalExcused.ToString(), font_small));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.TOP_BORDER;
            newCell.BorderWidth = 1;
            attendanceTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase(totalUnexcused.ToString(), font_small));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.TOP_BORDER;
            newCell.BorderWidth = 1;
            attendanceTable.AddCell(newCell);



            return attendanceTable;

        }

        protected PdfPTable outcomeLegend(PdfContentByte content)
        {
            PdfPTable outcomeLegendTable = new PdfPTable(2);
            outcomeLegendTable.SpacingAfter = 25f;
            outcomeLegendTable.HorizontalAlignment = 1;
            outcomeLegendTable.TotalWidth = 450f;
            outcomeLegendTable.LockedWidth = true;

            float[] widths = new float[] { 1f, 2f };
            outcomeLegendTable.SetWidths(widths);

            PdfPCell newCell = null;
            Paragraph description = null;

            newCell = new PdfPCell(new Phrase("Outcome Legend",font_large_bold));
            newCell.Border = 0;
            newCell.Colspan = 2;
            newCell.Padding = 2;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell(displayOutcomeBar(content, "4"));
            newCell.Border = 0;
            newCell.Padding = 2;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell();
            description = new Paragraph();
            description.SpacingBefore = 0;
            description.SpacingAfter = 6;
            description.Add(new Phrase("4: Master: ", font_body_bold));
            description.Add(new Phrase("Insightful understanding of the outcome", font_body));
            newCell.PaddingTop = 0;
            newCell.AddElement(description);
            newCell.Border = 0;
            newCell.VerticalAlignment = 1;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell(displayOutcomeBar(content, "3"));
            newCell.Border = 0;
            newCell.Padding = 2;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell();
            description = new Paragraph();
            description.SpacingBefore = 0;
            description.SpacingAfter = 6;
            description.Add(new Phrase("3: Proficient: ", font_body_bold));
            description.Add(new Phrase("A well developed understanding of the outcome", font_body));
            newCell.PaddingTop = 0;
            newCell.AddElement(description);
            newCell.Border = 0;
            newCell.VerticalAlignment = 1;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell(displayOutcomeBar(content, "2"));
            newCell.Border = 0;
            newCell.Padding = 2;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell();
            description = new Paragraph();
            description.SpacingBefore = 0;
            description.SpacingAfter = 6;
            description.Add(new Phrase("2: Approaching: ", font_body_bold));
            description.Add(new Phrase("A basic understanding", font_body));
            newCell.PaddingTop = 0;
            newCell.AddElement(description);
            newCell.Border = 0;
            newCell.VerticalAlignment = 1;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell(displayOutcomeBar(content, "1"));
            newCell.Border = 0;
            newCell.Padding = 2;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell();
            description = new Paragraph();
            description.SpacingBefore = 0;
            description.SpacingAfter = 6;
            description.Add(new Phrase("1: Beginning: ", font_body_bold));
            description.Add(new Phrase("A partial understanding", font_body));
            newCell.PaddingTop = 0;
            newCell.AddElement(description);
            newCell.Border = 0;
            newCell.VerticalAlignment = 1;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell(displayOutcomeBar(content, "IE"));
            newCell.Border = 0;
            newCell.Padding = 2;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell();
            description = new Paragraph();
            description.SpacingBefore = 0;
            description.SpacingAfter = 6;
            description.Add(new Phrase("IE: ", font_body_bold));
            description.Add(new Phrase("Insufficient Evidence", font_body));
            newCell.PaddingTop = 0;
            newCell.AddElement(description);
            newCell.Border = 0;
            newCell.VerticalAlignment = 1;
            outcomeLegendTable.AddCell(newCell);

            return outcomeLegendTable;
        }
        protected PdfPTable lifeSkillsLegend(PdfContentByte content)
        {
            PdfPTable outcomeLegendTable = new PdfPTable(2);
            outcomeLegendTable.SpacingAfter = 25f;
            outcomeLegendTable.HorizontalAlignment = 1;
            outcomeLegendTable.TotalWidth = 450f;
            outcomeLegendTable.LockedWidth = true;

            float[] widths = new float[] { 1f, 2f };
            outcomeLegendTable.SetWidths(widths);

            PdfPCell newCell = null;
            Paragraph description = null;

            newCell = new PdfPCell(new Phrase("Life Skills Legend", font_large_bold));
            newCell.Border = 0;
            newCell.Colspan = 2;
            newCell.Padding = 2;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell(displayOutcomeBar(content, "4"));
            newCell.Border = 0;
            newCell.Padding = 2;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell();
            description = new Paragraph();
            description.SpacingBefore = 0;
            description.SpacingAfter = 6;
            description.Add(new Phrase("4: ", font_body_bold));
            description.Add(new Phrase("Consistently exhibits", font_body));
            newCell.PaddingTop = 0;
            newCell.AddElement(description);
            newCell.Border = 0;
            newCell.VerticalAlignment = 1;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell(displayOutcomeBar(content, "3"));
            newCell.Border = 0;
            newCell.Padding = 2;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell();
            description = new Paragraph();
            description.SpacingBefore = 0;
            description.SpacingAfter = 6;
            description.Add(new Phrase("3: ", font_body_bold));
            description.Add(new Phrase("Usually exhibits", font_body));
            newCell.PaddingTop = 0;
            newCell.AddElement(description);
            newCell.Border = 0;
            newCell.VerticalAlignment = 1;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell(displayOutcomeBar(content, "2"));
            newCell.Border = 0;
            newCell.Padding = 2;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell();
            description = new Paragraph();
            description.SpacingBefore = 0;
            description.SpacingAfter = 6;
            description.Add(new Phrase("2: ", font_body_bold));
            description.Add(new Phrase("Occasionally exhibits", font_body));
            newCell.PaddingTop = 0;
            newCell.AddElement(description);
            newCell.Border = 0;
            newCell.VerticalAlignment = 1;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell(displayOutcomeBar(content, "1"));
            newCell.Border = 0;
            newCell.Padding = 2;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell();
            description = new Paragraph();
            description.SpacingBefore = 0;
            description.SpacingAfter = 6;
            description.Add(new Phrase("1: ", font_body_bold));
            description.Add(new Phrase("Beginning to exhibit", font_body));
            newCell.PaddingTop = 0;
            newCell.AddElement(description);
            newCell.Border = 0;
            newCell.VerticalAlignment = 1;
            outcomeLegendTable.AddCell(newCell);

            newCell = new PdfPCell(displayOutcomeBar(content, "IE"));
            newCell.Border = 0;
            newCell.Padding = 2;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            outcomeLegendTable.AddCell(newCell);
           

            return outcomeLegendTable;
        }
       

        protected PdfPTable classWithMarks(Course course, PdfContentByte content)
        {
            PdfPTable classTable = new PdfPTable(3);
            classTable.HorizontalAlignment = 1;
            classTable.TotalWidth = 500f;
            classTable.LockedWidth = true;
            classTable.SpacingAfter = 35;
            classTable.KeepTogether = true;

            float[] widths = new float[] { 2f, 4f, 1f};
            classTable.SetWidths(widths);


            PdfPCell newCell = null;  // Going to reuse this for each cell, because i'm lazy and don't want to create a billion extra objects...
            Paragraph newP = null; // Ditto (newP[aragraph])
               
            /* Course title and numeric mark */            
            newP = new Paragraph();
            newP.Add(new Phrase(course.name, font_large_bold));
            newP.Add(new Phrase(" (" + course.teacherName + ")", font_body));
            newCell = new PdfPCell(newP);
            newCell.Border = 0;
            newCell.Padding = 5;
            newCell.Colspan = 2;
            classTable.AddCell(newCell);

            newCell = new PdfPCell();
            newCell.Border = 0;
            newCell.Padding = 5;

            //if (!course.hasObjectives())
            {
                foreach (Mark mark in course.Marks)
                {
                    Paragraph markValueParagraph = new Paragraph();
                    if (double.Parse(mark.numberMark) > 0)
                    {
                        markValueParagraph.Add(mark.numberMark.ToString() + "%");
                    }

                    newCell = new PdfPCell(markValueParagraph);
                    newCell.Border = 0;
                    newCell.Padding = 5;
                    newCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;  
                }
            }

            classTable.AddCell(newCell);

            /* Marks and Outcomes Heading */
            if (course.hasObjectives())
            {
                newP = new Paragraph();
                newP.Add(new Phrase("Outcomes:", font_body_bold));
                newCell = new PdfPCell(newP);
                newCell.Border = 0;
                newCell.Padding = 5;
                newCell.Colspan = 3;
                classTable.AddCell(newCell);
            }

            /* Outcome marks */
            foreach (ReportPeriod reportPeriod in course.ReportPeriods)
            {
                foreach (ObjectiveMark objectivem in course.ObjectiveMarks)
                {
                    if ((objectivem.courseID == course.courseid) && (objectivem.reportPeriodID == reportPeriod.ID))
                    {
                        if (!string.IsNullOrEmpty(objectivem.mark))
                        {

                            newCell = new PdfPCell(displayOutcomeBar(content, objectivem.mark));
                            newCell.Padding = 5;
                            //newCell.PaddingLeft = 0;

                            newCell.Border = 0;
                            classTable.AddCell(newCell);

                            newP = new Paragraph();
                            newP.Add(new Phrase(objectivem.description, font_small));
                            newCell = new PdfPCell(newP);
                            newCell.Border = 0;
                            newCell.Padding = 5;
                            newCell.Colspan = 2;
                            classTable.AddCell(newCell);
                        }
                    }
                }
            }
            
            /* Life Skills */

            /* Comments */            
            newP = new Paragraph();
            newP.Add(new Phrase("Comments:\n", font_body_bold));
            newP.Add(Chunk.NEWLINE);
            foreach (Mark mark in course.Marks)
            {
                if (!string.IsNullOrEmpty(mark.comment))
                {
                    newP.Add(new Phrase(mark.comment, font_small));
                    newP.Add(Chunk.NEWLINE);
                    newP.Add(Chunk.NEWLINE);
                }
            }

            newCell = new PdfPCell(newP);
            newCell.Border = 0;
            newCell.Padding = 5;
            newCell.Colspan = 3;
            classTable.AddCell(newCell);

            return classTable;
        }

        protected void sendPDF(System.IO.MemoryStream PDFData)
        {
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "attachment; filename=ReportCard.pdf");
            
            Response.OutputStream.Write(PDFData.GetBuffer(), 0, PDFData.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();
            Response.End();

        }
        
        protected MemoryStream GeneratePDF(Student student, ReportPeriod period)
        {
            MemoryStream memstream = new MemoryStream();
            Document ReportCard = new Document(PageSize.LETTER);
            PdfWriter writer = PdfWriter.GetInstance(ReportCard, memstream);

            ReportCard.Open();
            PdfContentByte content = writer.DirectContent;

            PdfPageEventHandler PageEventHandler = new PdfPageEventHandler();
            writer.PageEvent = PageEventHandler;
            PageEventHandler.DoubleSidedMode = true;
            PageEventHandler.ShowOnFirstPage = false;
            PageEventHandler.bottomLeft = student.getDisplayName();
            PageEventHandler.bottomCenter = period.name;
            
            ReportCard.Add(schoolNamePlate(student.school));
            ReportCard.Add(namePlateTable(student, period));
            ReportCard.Add(outcomeLegend(content));
            ReportCard.Add(lifeSkillsLegend(content));
            
            ReportCard.NewPage();

            ReportCard.Add(new Phrase(string.Empty));

            foreach (Term term in student.track.terms)
            {
                foreach (Course course in term.Courses)
                {
                    ReportCard.Add(classWithMarks(course, content));
                    if (!student.track.daily)
                    {
                        ReportCard.Add(courseAttendanceSummary(student, course));
                    }
                }
            }

            ReportCard.Add(attendanceSummary(student));

            ReportCard.Close();

            return memstream;
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;

            try
            {
                if (!String.IsNullOrEmpty(Request.QueryString["studentid"]))
                {
                    int studentID = -1;
                    if (int.TryParse(Request.QueryString["studentid"], out studentID))
                    {
                        if (!String.IsNullOrEmpty(Request.QueryString["reportperiod"]))
                        {
                            int reportPeriodID = -1;
                            if (int.TryParse(Request.QueryString["reportperiod"], out reportPeriodID))
                            {
                                using (SqlConnection connection = new SqlConnection(dbConnectionString))
                                {
                                    selectedStudent = Student.loadThisStudent(connection, studentID.ToString());

                                    #region Load data for the student
                                    if (selectedStudent != null)
                                    {
                                        selectedStudent.school = School.loadThisSchool(connection, int.Parse(selectedStudent.getSchoolID()));

                                        /* Get student attendance */
                                        ReportPeriod rp = ReportPeriod.loadThisReportPeriod(connection, reportPeriodID);
                                        selectedStudent.absences = Absence.loadAbsencesForThisStudentAndTimePeriod(connection, selectedStudent, rp.startDate, rp.endDate);

                                        /* Get student track, and determine the terms and report periods */
                                        selectedStudent.track = Track.loadThisTrack(connection, selectedStudent.getTrackID());

                                        /* Populate the track with terms */
                                        selectedStudent.track.terms = Term.loadTermsFromThisTrack(connection, selectedStudent.track);

                                        /* Populate the terms with report periods */
                                        foreach (Term t in selectedStudent.track.terms)
                                        {
                                            List<ObjectiveMark> TermObjectiveMarks = ObjectiveMark.loadObjectiveMarksForThisStudent(connection, t, selectedStudent);

                                            t.ReportPeriods = ReportPeriod.loadReportPeriodsFromThisTerm(connection, t);

                                            Dictionary<int, Course> termCourses = new Dictionary<int, Course>();
                                            termCourses.Clear();

                                            /* Load marks into the report period */
                                            foreach (ReportPeriod r in t.ReportPeriods)
                                            {
                                                if (r.ID == reportPeriodID)
                                                {
                                                    r.marks = Mark.loadMarksFromThisReportPeriod(connection, r, selectedStudent);
                                                    selectedReportPeriod = r;

                                                    Dictionary<int, Course> allcourses = new Dictionary<int, Course>();
                                                    foreach (Mark m in r.marks)
                                                    {
                                                        if (!allcourses.ContainsKey(m.courseID))
                                                        {
                                                            allcourses.Add(m.courseID, new Course(m.className, m.classID, m.courseID, m.teacherFirst, m.teacherLast, m.teacherTitle));
                                                        }

                                                        if (!termCourses.ContainsKey(m.courseID))
                                                        {
                                                            termCourses.Add(m.courseID, new Course(m.className, m.classID, m.courseID, m.teacherFirst, m.teacherLast, m.teacherTitle));
                                                        }
                                                    }


                                                    foreach (KeyValuePair<int, Course> kvp in termCourses)
                                                    {
                                                        Course c = kvp.Value;
                                                        Dictionary<int, ReportPeriod> detectedReportPeriods = new Dictionary<int, ReportPeriod>();

                                                        foreach (Mark m in r.marks)
                                                        {
                                                            if (!detectedReportPeriods.ContainsKey(m.reportPeriodID))
                                                            {
                                                                detectedReportPeriods.Add(m.reportPeriodID, m.reportPeriod);
                                                            }
                                                        }

                                                        foreach (KeyValuePair<int, ReportPeriod> drp in detectedReportPeriods)
                                                        {
                                                            c.ReportPeriods.Add(drp.Value);


                                                        }

                                                        foreach (Mark m in r.marks)
                                                        {
                                                            if (m.courseID == c.courseid)
                                                            {
                                                                c.Marks.Add(m);
                                                            }
                                                        }




                                                        c.ObjectiveMarks = ObjectiveMark.loadObjectiveMarksForThisCourse(connection, t, selectedStudent, c);
                                                        c.Objectives = Objective.loadObjectivesForThisCourse(connection, c);

                                                        foreach (ObjectiveMark om in c.ObjectiveMarks)
                                                        {
                                                            foreach (Objective o in c.Objectives)
                                                            {
                                                                if (om.objectiveID == o.id)
                                                                {
                                                                    om.objective = o;
                                                                }
                                                            }
                                                        }


                                                        foreach (Objective o in c.Objectives)
                                                        {
                                                            foreach (ObjectiveMark om in TermObjectiveMarks)
                                                            {
                                                                if (om.objectiveID == o.id)
                                                                {
                                                                    o.mark = om;
                                                                }
                                                            }
                                                        }
                                                    }

                                                    foreach (KeyValuePair<int, Course> kvp in termCourses)
                                                    {
                                                        t.Courses.Add(kvp.Value);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    #region Generate the PDF
                                    sendPDF(GeneratePDF(selectedStudent, selectedReportPeriod));
                                    #endregion
                                }
                            }
                            else
                            {
                                Response.Write("Invalid or missing report period ID");
                            }
                        }
                        else
                        {
                            Response.Write("Invalid or missing report period ID");
                        }

                    }
                    else
                    {
                        Response.Write("Invalid or missing student ID");
                    }
                }
                else
                {
                    Response.Write("Invalid or missing student ID");
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message + "<BR>" + ex.StackTrace);
            }
        }
    }
}