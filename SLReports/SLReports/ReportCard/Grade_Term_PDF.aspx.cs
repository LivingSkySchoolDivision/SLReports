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
    public partial class Grade_Term_PDF : System.Web.UI.Page
    {
        private string api_key = "6b05cb5705c07a4ca23a6bba779263ab983a5ae2";
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

        protected PdfPTable namePlateTable(Student student)
        {
            int cellpadding = 3;
            int border = Rectangle.NO_BORDER;

            if (student == null)
            {
                student = new Student("John", "Smith", "J", "00000", "00000", "School Name", "00000", "Grade 15", "Saskatchewan", "North Battleford", "Fake St", "123", "", "H0H0H0", "3065551234", "Male", "Instatus", "Instatuscode", "Homeroom here", DateTime.Now.AddDays(-1), DateTime.Now, 00000, false, null);
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
                    iTextSharp.text.Image photo = iTextSharp.text.Image.GetInstance(@"https://sldata.lskysd.ca/SLReports/photos/GetPhoto.aspx?studentnumber=" + student.getStudentID() + "&apikey=" + api_key);
                    photo.Border = Rectangle.BOX;
                    photo.BorderWidth = 1;
                    photoCell.PaddingRight = 10f;

                    photoCell = new PdfPCell(photo);
                }
                catch (Exception ex)
                {
                    photoCell = new PdfPCell(new Phrase(ex.Message, font_small));
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

            /*
            description.Add(new Phrase(term.name, font_body));
            description.Add(new Phrase(" (" + term.startDate.ToLongDateString() + " to " + term.endDate.ToLongDateString() + ")", font_body_italic));

            newCell = new PdfPCell(description);
            newCell.Border = border;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.Padding = cellpadding;
            nameplateTable.AddCell(newCell);
            */

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

        protected PdfPTable courseAttendanceSummary(Student student, SchoolClass course)
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

            newCell = new PdfPCell(new Phrase("Outcome Legend", font_large_bold));
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

        protected PdfPTable classWithMarks(SchoolClass course, PdfContentByte content)
        {

            PdfPTable classTable = new PdfPTable(3);
            classTable.HorizontalAlignment = 1;
            classTable.TotalWidth = 500f;
            classTable.LockedWidth = true;
            classTable.SpacingAfter = 35;
            classTable.KeepTogether = true;            

            float[] widths = new float[] { 1.75f, 2f, 2f };
            classTable.SetWidths(widths);


            PdfPCell newCell = null;  // Going to reuse this for each cell, because i'm lazy and don't want to create a billion extra objects...
            Paragraph newP = null; // Ditto (newP[aragraph])

            /* Course title and numeric mark */
            newP = new Paragraph();
            newP.Add(new Phrase(course.name, font_large_bold));
            newP.Add(new Phrase(" (" + course.teacherName + ")", font_body));
            newCell = new PdfPCell(newP);
            newCell.Border = Rectangle.BOX;
            newCell.Padding = 5;
            newCell.Colspan = 2;
            classTable.AddCell(newCell);

            newCell = new PdfPCell();
            newCell.Border = 0;
            newCell.Padding = 5;

            if (course.Marks.Count > 0)
            {
                /* Create a nested table to put in the mark cell */
                PdfPTable markTable = new PdfPTable(course.Marks.Count);

                foreach (Mark mark in course.Marks)
                {                    
                    Paragraph markValueParagraph = new Paragraph();

                    /* figure out what to display for the mark */
                    /*  - if The class is a high school class, it should be a percent */
                    /*  - Otherwise, display the Outcome/Objective mark */

                    if (course.isHighSchoolClass())
                    {
                        if (!string.IsNullOrEmpty(mark.nMark))
                        {
                            double nMarkValue = -1;
                            Double.TryParse(mark.nMark, out nMarkValue);
                            markValueParagraph.Add(new Phrase(Math.Round(nMarkValue) + "%", font_body_bold));
                        }
                        else
                        {
                            markValueParagraph.Add(new Phrase("No Value", font_small_italic));
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(mark.cMark))
                        {
                            markValueParagraph.Add(new Phrase(mark.cMark, font_large_bold));
                        }
                        else
                        {
                            markValueParagraph.Add(new Phrase("No Value", font_small_italic));
                        }
                    }


                    newCell = new PdfPCell(markValueParagraph);
                    newCell.Border = Rectangle.BOX;
                    newCell.Padding = 5;
                    newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    markTable.AddCell(newCell);
                    
                }

                foreach (Mark mark in course.Marks)
                {                    
                    newCell = new PdfPCell(new Phrase(mark.reportPeriod.name, font_small));
                    newCell.Border = Rectangle.BOX;
                    newCell.Padding = 2;
                    newCell.PaddingBottom = 3;
                    newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    markTable.AddCell(newCell);                    
                }

                classTable.AddCell(new PdfPCell(markTable));
            }
            else
            {
                newCell = new PdfPCell(new Paragraph(""));
                newCell.Border = Rectangle.BOX;
                newCell.Padding = 5;
                newCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                classTable.AddCell(newCell);
            }

            

            /* Marks and Outcomes Heading */
            if (course.hasObjectives())
            {
                newP = new Paragraph();
                newP.Add(new Phrase("Outcomes:", font_body_bold));
                newCell = new PdfPCell(newP);
                newCell.Border = Rectangle.BOX;
                newCell.Padding = 5;
                newCell.Colspan = 3;
                classTable.AddCell(newCell);
            }

            /* Outcome marks */
            foreach (ReportPeriod reportPeriod in course.ReportPeriods)
            {
                foreach (ObjectiveMark objectivem in course.ObjectiveMarks)
                {
                    newCell = new PdfPCell(displayOutcomeBar(content, objectivem.cMark));
                    newCell.Padding = 5;                    
                    //newCell.PaddingLeft = 0;

                    newCell.Border = Rectangle.BOX;
                    classTable.AddCell(newCell);

                    newP = new Paragraph();
                    newP.Add(new Phrase(objectivem.description, font_small));
                    newCell = new PdfPCell(newP);
                    //newCell.Border = Rectangle.BOX;
                    newCell.Border = Rectangle.BOX;
                    newCell.Padding = 5;
                    newCell.Colspan = 2;
                    classTable.AddCell(newCell);
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
                    newP.Add(new Phrase(mark.reportPeriod.name + ": ", font_small_bold));
                    newP.Add(new Phrase(mark.comment, font_small));
                    newP.Add(Chunk.NEWLINE);
                    newP.Add(Chunk.NEWLINE);
                }
            }

            newCell = new PdfPCell(newP);
            newCell.Border = Rectangle.BOX;
            newCell.Padding = 5;
            newCell.Colspan = 3;
            classTable.AddCell(newCell);

            return classTable;
        }

        protected void sendPDF(System.IO.MemoryStream PDFData, string filename)
        {
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "attachment; filename="+filename+"");

            Response.OutputStream.Write(PDFData.GetBuffer(), 0, PDFData.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();
            Response.End();

        }

        protected MemoryStream GeneratePDF(List<Student> students)
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
            PageEventHandler.bottomCenter = "Printed " + DateTime.Now.ToLongDateString();

            foreach (Student student in students)
            {
                PageEventHandler.bottomLeft = student.getDisplayName();                
                ReportCard.Add(schoolNamePlate(student.school));
                ReportCard.Add(namePlateTable(student));
                //ReportCard.Add(outcomeLegend(content));
                //ReportCard.Add(lifeSkillsLegend(content));
                //ReportCard.NewPage();
                ReportCard.Add(new Phrase(string.Empty));

                foreach (Term term in student.track.terms)
                {
                    foreach (SchoolClass course in term.Courses)
                    {
                        ReportCard.Add(classWithMarks(course, content));
                        if (!student.track.daily)
                        {
                            ReportCard.Add(courseAttendanceSummary(student, course));
                        }
                    }
                }

                ReportCard.Add(attendanceSummary(student));

                PageEventHandler.ResetPageNumbers(ReportCard);
            }

            ReportCard.Close();
            return memstream;
        }

        private void displayError(string error)
        {
            Response.Write(error);
        }

        private Student loadStudentData(SqlConnection connection, Student thisStudent, List<ReportPeriod> reportPeriods)
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
                        }

                        foreach (Objective objective in thisClass.Objectives)
                        {
                            foreach (ObjectiveMark objectivemark in thisClass.ObjectiveMarks)
                            {
                                if (objective.id == objectivemark.objectiveID)
                                {
                                    objective.mark = objectivemark;
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

        protected void Page_Load(object sender, EventArgs e)
        {
            List<Student> students = new List<Student>();
            List<Student> displayedStudents = new List<Student>();

            List<ReportPeriod> selectedReportPeriods = new List<ReportPeriod>();
            


            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogic2013"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                /* Debugging info */
                //selectedTerm = Term.loadThisTerm(connection, 20);
                selectedReportPeriods.Add(ReportPeriod.loadThisReportPeriod(connection, 258));
                selectedReportPeriods.Add(ReportPeriod.loadThisReportPeriod(connection, 257));
                selectedReportPeriods.Add(ReportPeriod.loadThisReportPeriod(connection, 256));

                //students.Add(Student.loadThisStudent(connection, "11871"));
                students.Add(Student.loadThisStudent(connection, "11804"));
                students.Add(Student.loadThisStudent(connection, "12511"));
            }


            selectedReportPeriods.Sort();
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                foreach (Student student in students)
                {                
                    displayedStudents.Add(loadStudentData(connection, student, selectedReportPeriods));
                }
                students.Clear();
            }


            if (Request.QueryString["debug"] == "true")
            {

                foreach (Student student in displayedStudents)
                {
                    Response.Write("<BR><hr><BR><b>" + student + "</B>");
                    Response.Write("<BR><b>Absense entries: </b>" + student.absences.Count);
                    Response.Write("<BR>&nbsp;<b>Track:</b> " + student.track);
                    foreach (Term term in student.track.terms)
                    {
                        Response.Write("<BR>&nbsp;<b>Term:</b> " + term);
                        foreach (ReportPeriod rp in term.ReportPeriods)
                        {
                            Response.Write("<BR>&nbsp;&nbsp;<b>Report Period:</b> " + rp);
                            Response.Write("<BR>&nbsp;&nbsp;&nbsp;<b>Marks: </b> " + rp.marks.Count);
                            foreach (Mark mark in rp.marks)
                            {
                                Response.Write("<BR>&nbsp;&nbsp;&nbsp;&nbsp;<b>Mark: </b> " + mark);
                            }
                        }

                        Response.Write("<BR><BR>&nbsp;&nbsp;<b>Classes:</b> " + term.Courses.Count);
                        foreach (SchoolClass c in term.Courses)
                        {
                            Response.Write("<BR><BR>&nbsp;&nbsp;<b>Class:</b> " + c);
                            Response.Write("<BR>&nbsp;&nbsp;&nbsp;<b>Teacher:</b> " + c.teacherName);
                            Response.Write("<BR>&nbsp;&nbsp;&nbsp;<b>Marks:</b> " + c.Marks.Count);
                            Response.Write("<BR>&nbsp;&nbsp;&nbsp;<b>Objective Marks:</b> " + c.ObjectiveMarks.Count);
                            foreach (ReportPeriod rp in term.ReportPeriods)
                            {
                                Response.Write("<BR>&nbsp;&nbsp;&nbsp;<b>Report Period:</b> " + rp);
                                foreach (Mark m in c.Marks)
                                {
                                    if (m.reportPeriodID == rp.ID)
                                    {
                                        Response.Write("<BR>&nbsp;&nbsp;&nbsp;&nbsp;<b>Mark</b> " + m);
                                    }
                                }

                                foreach (ObjectiveMark om in c.ObjectiveMarks)
                                {
                                    if (om.reportPeriodID == rp.ID)
                                    {
                                        Response.Write("<BR>&nbsp;&nbsp;&nbsp;&nbsp;<b>ObjectiveMark:</b> " + om);
                                    }
                                }
                            }
                            Response.Write("<BR>&nbsp;&nbsp;&nbsp;<b>Objectives:</b> " + c.Objectives.Count);
                            foreach (Objective o in c.Objectives)
                            {
                                Response.Write("<BR>&nbsp;&nbsp;&nbsp;&nbsp;<b>Objective:</b> " + o);
                            }

                            
                        }
                    }
                }
            }
            else
            {
                String selectedGrade = "TestGrade";
                String fileName = "ReportCards_" + selectedGrade + "_" + DateTime.Today.Year + "_" + DateTime.Today.Month + "_" + DateTime.Today.Day + ".pdf";
                sendPDF(GeneratePDF(displayedStudents), fileName);
            }
            

        }
    }
}