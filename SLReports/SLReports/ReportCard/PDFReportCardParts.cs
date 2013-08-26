using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace SLReports.ReportCard
{
    public static class PDFReportCardParts
    {
        public static Font font_large = FontFactory.GetFont("Verdana", 15, BaseColor.BLACK);
        public static Font font_large_bold = FontFactory.GetFont("Verdana", 15, Font.BOLD, BaseColor.BLACK);
        public static Font font_large_italic = FontFactory.GetFont("Verdana", 15, Font.ITALIC, BaseColor.BLACK);

        public static Font font_body = FontFactory.GetFont("Verdana", 10, BaseColor.BLACK);
        public static Font font_body_bold = FontFactory.GetFont("Verdana", 10, Font.BOLD, BaseColor.BLACK);
        public static Font font_body_italic = FontFactory.GetFont("Verdana", 10, Font.ITALIC, BaseColor.BLACK);

        public static Font font_small = FontFactory.GetFont("Verdana", 8, BaseColor.BLACK);
        public static Font font_small_bold = FontFactory.GetFont("Verdana", 8, Font.BOLD, BaseColor.BLACK);
        public static Font font_small_bold_white = FontFactory.GetFont("Verdana", 8, Font.BOLD, BaseColor.WHITE);
        public static Font font_small_italic = FontFactory.GetFont("Verdana", 8, Font.ITALIC, BaseColor.BLACK);
        
        /// <summary>
        /// Displays a graphical number bar for outcomes
        /// </summary>
        /// <param name="content">A PdfContentByte object for the document</param>
        /// <param name="value">The value of the outcome mark to display</param>
        /// <returns></returns>
        /// 
        /// We are routing outcome bars through this routine so that we can change the style of the outcome bar in one place more easily
        public static iTextSharp.text.Image displayOutcomeBar(PdfContentByte content, String value)
        {
            return outcomeBar_Slider(content, value);
            //return outcomeBar_Thin(content, value);
            //return outcomeBar_Original(content, value);
            //return outcomeBar_Minimalist(content, value);
        }

        // Bar Colors
        private static BaseColor color_neutral = new BaseColor(70, 70, 70);
        private static BaseColor color_text_neutral = new BaseColor(255, 255, 255);


        private static BaseColor color_low = new BaseColor(255, 51, 0); // Red
        private static BaseColor color_text_low = new BaseColor(255, 255, 255);

        //private static BaseColor color_medium = new BaseColor(255, 119, 0); // Orange
        private static BaseColor color_medium = new BaseColor(255, 255, 50); // Yellow
        private static BaseColor color_text_medium = new BaseColor(0, 0, 0);

        private static BaseColor color_high = new BaseColor(0, 128, 0); // Green
        private static BaseColor color_text_high = new BaseColor(255, 255, 255);

        private static BaseColor color_max = color_high;
        private static BaseColor color_text_max = color_text_high;
            
        #region Outcome Bar Styles
        public static iTextSharp.text.Image outcomeBar_Original(PdfContentByte content, String value)
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
                    fillColor = color_low;
                }
                else if (parsedValue <= 2.25)
                {
                    fillColor = color_medium;
                }
                else if (parsedValue <= 3.5)
                {
                    fillColor = color_high;
                }
                else if (parsedValue <= 4)
                {
                    fillColor = color_max;
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

        public static iTextSharp.text.Image outcomeBar_JustNumbers(PdfContentByte content)
        {
            int width = 125;
            int height = 12;
            int maxvalue = 4;
            int rectancleCurveRadius = 2;

            int CanvasPaddingX = 1;
            int CanvasPaddingY = 1;

            /* Colors */
            BaseColor fillColor = new BaseColor(70, 70, 70);
            BaseColor borderColor = new BaseColor(0, 0, 0);

            PdfTemplate canvas = content.CreateTemplate(width + (CanvasPaddingX * 2), height + (CanvasPaddingY * 2));

            /* Background */
            canvas.Rectangle(0, 0, width, height);
            canvas.SetRGBColorFill(255, 255, 255);
            canvas.Fill();


            /* Overlay */
            /*
            canvas.SetColorStroke(borderColor);
            canvas.SetLineWidth(1);
            canvas.MoveTo((float)(width * 0.25) * 1, (float)CanvasPaddingY);
            canvas.LineTo((float)(width * 0.25) * 1, (float)height + CanvasPaddingY);
            canvas.MoveTo((float)(width * 0.25) * 2, (float)CanvasPaddingY);
            canvas.LineTo((float)(width * 0.25) * 2, (float)height + CanvasPaddingY);
            canvas.MoveTo((float)(width * 0.25) * 3, (float)CanvasPaddingY);
            canvas.LineTo((float)(width * 0.25) * 3, (float)height + CanvasPaddingY);
            canvas.Stroke();
            */

            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            canvas.BeginText();
            canvas.SetFontAndSize(bf, 10);

            canvas.SetColorFill(borderColor);

            canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "1", (float)((width * 0.25) / 2), (float)((height + CanvasPaddingY) / 2) - 2, 0);
            canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "2", (float)(((width * 0.25) / 2) + (width * 0.25)), (float)((height + CanvasPaddingY) / 2) - 2, 0);
            canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "3", (float)(((width * 0.25) / 2) + (width * 0.50)), (float)((height + CanvasPaddingY) / 2) - 2, 0);
            canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "4", (float)(((width * 0.25) / 2) + (width * 0.75)), (float)((height + CanvasPaddingY) / 2) - 2, 0);

            canvas.EndText();

            /* Border */
            canvas.SetRGBColorStroke(255, 255, 255);
            //canvas.RoundRectangle(CanvasPaddingX, CanvasPaddingY, width - CanvasPaddingX, height, rectancleCurveRadius);

            canvas.MoveTo(CanvasPaddingX, rectancleCurveRadius);
            canvas.LineTo(width - CanvasPaddingX, rectancleCurveRadius);

            canvas.SetColorStroke(borderColor);
            canvas.Stroke();

            return iTextSharp.text.Image.GetInstance(canvas); ;
        }

        public static iTextSharp.text.Image outcomeBar_Thin(PdfContentByte content, String value)
        {
            int width = 125;
            int height = 9;

            int CanvasPaddingX = 0;
            int CanvasPaddingY = 0;

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
                    fillColor = color_low;
                }
                else if (parsedValue <= 2.25)
                {
                    fillColor = color_medium;
                }
                else if (parsedValue <= 3.5)
                {
                    fillColor = color_high;
                }
                else if (parsedValue <= 4)
                {
                    fillColor = color_max;
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
                //canvas.RoundRectangle(CanvasPaddingX, CanvasPaddingY, width, height, rectancleCurveRadius);
                canvas.Rectangle(CanvasPaddingX, CanvasPaddingY, width, height);
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

        public static iTextSharp.text.Image outcomeBar_Slider(PdfContentByte content, String value)
        {
            int width = 125;
            int height = 12;
            int barMargin = 3;

            int CanvasPaddingX = 1;
            int CanvasPaddingY = 1;

            int maxvalue = 4;
            int rectancleCurveRadius = 2;

            /* Colors */
            BaseColor fillColor = color_neutral;
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
                    fillColor = color_low;
                    textColor = color_text_low;
                }
                else if (parsedValue <= 2.25)
                {
                    fillColor = color_medium;
                    textColor = color_text_medium;
                }
                else if (parsedValue <= 3.5)
                {
                    fillColor = color_high;
                    textColor = color_text_high;
                }
                else if (parsedValue <= 4)
                {
                    fillColor = color_max;
                    textColor = color_text_max;
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
                else if (value.ToLower() == "nym")
                {
                    canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "NOT YET MEETING", width / 2, (height / 2) - 2, 0);
                }
                else
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        value = "NO VALUE";
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

        public static iTextSharp.text.Image outcomeBar_Minimalist(PdfContentByte content, String value)
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

        public static PdfPTable schoolNamePlate(School school)
        {
            int cellpadding = 3;
            int border = Rectangle.NO_BORDER;

            PdfPTable schoolNamePlateTable = new PdfPTable(1);
            schoolNamePlateTable.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            schoolNamePlateTable.TotalWidth = 500f;
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

        public static PdfPTable namePlateTable(Student student, bool anonymize = false)
        {
            int cellpadding = 3;
            int border = Rectangle.NO_BORDER;

            if ((student == null) || (anonymize))
            {
                student = new Student("John", "Smith", "J", "000000000", "000000000", "Demo School", "00000", "X", "Saskatchewan", "North Battleford", "Fake St", "123", "", "H0H0H0", "3065551234", "Male", "Instatus", "Instatuscode", "Homeroom Teacher", DateTime.Now.AddDays(-1), DateTime.Now, "000", "Band name", "Reserve Name", "House #", "000000000", false, 000, false, "user.name", 20);
            }

            PdfPTable nameplateTable = new PdfPTable(3);
            nameplateTable.HorizontalAlignment = 1;
            nameplateTable.TotalWidth = 500f;
            nameplateTable.LockedWidth = true;
            nameplateTable.SpacingAfter = 50;

            float[] widths = new float[] { 100f, 125f, 225f };
            nameplateTable.SetWidths(widths);

            PdfPCell photoCell = new PdfPCell(new Phrase("(No Photo)", font_large_italic));

            //if (student.hasPhoto())
            {
                try
                {
                    iTextSharp.text.Image photo = iTextSharp.text.Image.GetInstance(@"https://sldata.lskysd.ca/SLReports/photos/GetPhoto.aspx?studentnumber=" + student.getStudentID() + "&apikey=" + LSKYCommon.internal_api_key);
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

        public static PdfPTable attendanceSummary(Student student)
        {
            PdfPTable attendanceTable = new PdfPTable(5);
            attendanceTable.HorizontalAlignment = 1;
            attendanceTable.TotalWidth = 500;
            attendanceTable.LockedWidth = true;
            attendanceTable.SpacingAfter = 50;
            attendanceTable.KeepTogether = true;

            float[] tableWidths = new float[] { 1f, 6f, 2f, 2f, 2f };
            attendanceTable.SetWidths(tableWidths);

            PdfPCell newCell = null;

            /* Gather stats */
            int totalExcused = 0;
            int totalUnexcused = 0;
            int totalLates = 0;
            int totalLateMinutes = 0;


            List<SchoolClass> allClasses = new List<SchoolClass>();
            foreach (Term term in student.track.terms)
            {
                allClasses.AddRange(term.Courses);
            }

            List<String> allCoursePeriods = new List<String>();

            allCoursePeriods.Clear();
            foreach (Absence abs in student.absences)
            {
                // Collect a list of courses
                if (!allCoursePeriods.Contains(abs.period))
                {
                    allCoursePeriods.Add(abs.period);
                }
            }

            allCoursePeriods.Sort();

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

            newCell = new PdfPCell(new Phrase("Class", font_body_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.BOTTOM_BORDER;
            newCell.PaddingBottom = 5;
            newCell.BorderWidth = 1;
            attendanceTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Lates", font_body_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.BOTTOM_BORDER;
            newCell.PaddingBottom = 5;
            newCell.BorderWidth = 1;
            attendanceTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Excused Absences", font_body_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.BOTTOM_BORDER;
            newCell.PaddingBottom = 5;
            newCell.BorderWidth = 1;
            attendanceTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Unexcused Absences", font_body_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.BOTTOM_BORDER;
            newCell.PaddingBottom = 5;
            newCell.BorderWidth = 1;
            attendanceTable.AddCell(newCell);

            /* Values */
            foreach (String courseName in allCoursePeriods)
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
                newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.NO_BORDER;
                attendanceTable.AddCell(newCell);


                newCell = new PdfPCell(new Phrase(LSKYCommon.findClassNameForThisBlock(allClasses, courseName), font_body));
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
                newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = Rectangle.NO_BORDER;
                attendanceTable.AddCell(newCell);

                newCell = new PdfPCell(new Phrase(numAbsUnexc.ToString(), font_body));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
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
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.TOP_BORDER;
            newCell.BorderWidth = 1;
            attendanceTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase(totalUnexcused.ToString(), font_body_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.TOP_BORDER;
            newCell.BorderWidth = 1;
            attendanceTable.AddCell(newCell);

            return attendanceTable;
        }

        public static PdfPTable courseAttendanceSummary(Student student, SchoolClass course)
        {
            PdfPTable attendanceTable = new PdfPTable(4);
            attendanceTable.HorizontalAlignment = 1;
            attendanceTable.TotalWidth = 500;
            attendanceTable.LockedWidth = true;
            attendanceTable.SpacingAfter = 35;
            attendanceTable.SpacingBefore = 10;
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
            newCell.Border = Rectangle.BOX;
            newCell.PaddingBottom = 5;
            attendanceTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Lates", font_body_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.BOX;
            newCell.PaddingBottom = 5;
            attendanceTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Excused Absences", font_body_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.BOX;
            newCell.PaddingBottom = 5;
            attendanceTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Unexcused Absences", font_body_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.BOX;
            newCell.PaddingBottom = 5;
            attendanceTable.AddCell(newCell);

            /* Values */
            newCell = new PdfPCell(new Phrase(course.name, font_small));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.BOX;
            newCell.PaddingBottom = 5;
            attendanceTable.AddCell(newCell);

            StringBuilder lateDisplay2 = new StringBuilder();
            lateDisplay2.Append(totalLates);

            if (totalLateMinutes > 0)
            {
                lateDisplay2.Append(" (" + totalLateMinutes + " minutes)");
            }

            newCell = new PdfPCell(new Phrase(lateDisplay2.ToString(), font_small));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.BOX;
            newCell.PaddingBottom = 5;
            attendanceTable.AddCell(newCell);


            newCell = new PdfPCell(new Phrase(totalExcused.ToString(), font_small));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.BOX;
            newCell.PaddingBottom = 5;
            attendanceTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase(totalUnexcused.ToString(), font_small));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.BOX;
            newCell.PaddingBottom = 5;
            attendanceTable.AddCell(newCell);

            return attendanceTable;
        }

        public static PdfPTable outcomeLegend(PdfContentByte content)
        {
            PdfPTable outcomeLegendTable = new PdfPTable(2);
            outcomeLegendTable.SpacingAfter = 25f;
            outcomeLegendTable.HorizontalAlignment = 1;
            outcomeLegendTable.TotalWidth = 500;
            outcomeLegendTable.LockedWidth = true;

            float[] widths = new float[] { 1f, 2f };
            outcomeLegendTable.SetWidths(widths);

            PdfPCell newCell = null;
            Paragraph description = null;

            newCell = new PdfPCell(new Phrase("Outcome value legend", font_large_bold));
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

        public static PdfPTable lifeSkillsLegend(PdfContentByte content, string grade)
        {
            SortedDictionary<int, string> potentialMarks = new SortedDictionary<int, string>();

            int gradeLevel = 0;
            if (!int.TryParse(grade, out gradeLevel)) 
            {
                gradeLevel = 0;
            }


            potentialMarks.Add(1, "Beginning to demonstrate these characteristics");
            potentialMarks.Add(2, "Occasionally demonstrates these characteristics");
            potentialMarks.Add(3, "Usually demonstrates these characteristics");
            potentialMarks.Add(4, "Consistently demonstrates these characteristics");

            SortedDictionary<string, string> lifeSkills = new SortedDictionary<string, string>();
            if (gradeLevel >= 10)
            {
                lifeSkills.Add("Engagement",    "Invested in learning, diligent in completing work.");
                lifeSkills.Add("Citizenship",   "Respectful, responsible, academically honest.");
                lifeSkills.Add("Collaborative", "Willing to work with all classmates, cooperative, willing to resolve conflict.");
                lifeSkills.Add("Leadership",    "Independent, takes initiative.");
                lifeSkills.Add("Self-Directed", "Arrives on time, prepared to advance learning, strong work habits.");
            }
            else if (gradeLevel >= 7)
            {
                lifeSkills.Add("Engagement",    "Active in learning, faces new challenges confidently, completes work.");
                lifeSkills.Add("Citizenship",   "Respectful to others and property, takes responsibility for actions and decisions.");
                lifeSkills.Add("Collaborative", "Willing to work with all classmates, encourages and includes others.");
                lifeSkills.Add("Leadership",    "Takes initiative, does the right thing.");
                lifeSkills.Add("Self-Directed", "Prepared for class, organized, and uses class time well.");

            }
            else if (gradeLevel >= 1)
            {
                lifeSkills.Add("Engagement", "Completes assignments, keeps trying when the work gets hard.");
                lifeSkills.Add("Citizenship", "Shows caring, follows class and school rules, takes responsibility for actions.");
                lifeSkills.Add("Collaborative", "Listens and works well with others, includes classmates at recess and in classroom.");
                lifeSkills.Add("Leadership", "Wants to learn and help others, good role model.");
                lifeSkills.Add("Self-Directed", "Stay son task, organized.");
            }
            else
            {
                lifeSkills.Add("Engagement", "Tries various activities, asks questions to satisfy curiosity, plays for extended periods of time.");
                lifeSkills.Add("Citizenship", "Respects others, follows classroom procedures, accepts responsibility for own actions.");
                lifeSkills.Add("Collaborative", "Plays and works well with others, offers and receives ideas.");
                lifeSkills.Add("Leadership", "Adapts to new situations, wants to learn and help others.");
                lifeSkills.Add("Self-Directed", "Problem solves, takes care of self and belongings, expresses emotions appropriately, adjusts to transitions.");
            }

            PdfPTable outcomeLegendTable = new PdfPTable(2);
            outcomeLegendTable.SpacingAfter = 25f;
            outcomeLegendTable.HorizontalAlignment = 1;
            outcomeLegendTable.TotalWidth = 500;
            outcomeLegendTable.LockedWidth = true;

            float[] widths = new float[] { 1f, 5f };
            outcomeLegendTable.SetWidths(widths);

            PdfPCell newCell = null;
            Paragraph description = null;

            newCell = new PdfPCell(new Phrase("Characteristics of Successful Learning Behaviors", font_large_bold));
            newCell.Border = 0;
            newCell.Colspan = 2;
            newCell.Padding = 2;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            outcomeLegendTable.AddCell(newCell);

            foreach (KeyValuePair<string, string> legendItem in lifeSkills)
            {
                newCell = new PdfPCell(new Phrase(legendItem.Key, font_body_bold));
                newCell.Border = 0;
                newCell.Padding = 5;
                newCell.PaddingLeft = 10;
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                outcomeLegendTable.AddCell(newCell);

                newCell = new PdfPCell(new Phrase(legendItem.Value, font_body));
                newCell.PaddingTop = 0;
                newCell.Padding = 5;
                newCell.AddElement(description);
                newCell.Border = 0;
                newCell.VerticalAlignment = 1;
                outcomeLegendTable.AddCell(newCell);
            }

            /*
            newCell = new PdfPCell(new Phrase("Successful Learning Behaviors Characteristics", font_large_bold));
            newCell.Border = 0;
            newCell.Colspan = 2;
            newCell.Padding = 2;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            outcomeLegendTable.AddCell(newCell);


            foreach (KeyValuePair<int, string> legendItem in potentialMarks)
            {
                newCell = new PdfPCell(displayOutcomeBar(content, legendItem.Key.ToString()));
                newCell.Border = 0;
                newCell.Padding = 2;
                newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                newCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                outcomeLegendTable.AddCell(newCell);

                newCell = new PdfPCell();
                description = new Paragraph();
                description.SpacingBefore = 0;
                description.SpacingAfter = 6;
                description.Add(new Phrase(legendItem.Key.ToString() + ": ", font_body_bold));
                description.Add(new Phrase(legendItem.Value, font_body));
                newCell.PaddingTop = 0;
                newCell.AddElement(description);
                newCell.Border = 0;
                newCell.VerticalAlignment = 1;
                outcomeLegendTable.AddCell(newCell);
            }
            */
            return outcomeLegendTable;
        }

        public static PdfPCell outcomeChunk(Outcome outcome, PdfContentByte content)
        {
            // Interesting note: If you add an element to a cell in the constructor it aligns differnetly than if you add it as an element

            string normalObjectiveCategoryName = "Outcome";           

            int ObjectivesTableBorder = 0;
            PdfPTable objectiveChunkTable = new PdfPTable(3);

            PdfPCell bufferCell = new PdfPCell();
            bufferCell.Border = ObjectivesTableBorder;

            // TODO: This may need to check for the exact name, but that doesn't exist in the data yet 
            // Currently normal objectives don't have a category, and "life skills" objectives do have a category

            if (
                (outcome.category.ToLower() == normalObjectiveCategoryName.ToLower()) ||
                (outcome.category.ToLower() == "")
                )
            {
                if (outcome.marks.Count > 0)
                {                    
                    PdfPCell objectiveMarksCell = new PdfPCell();

                    // Set up the description cell
                    PdfPCell objectiveDescriptionCell = new PdfPCell();

                    String outcomeDescription = outcome.notes;

                    if (outcome.notes.Length > 100)
                    {
                        outcomeDescription = outcome.notes.Substring(0, 100);

                    }

                    objectiveDescriptionCell.AddElement(new Phrase(outcomeDescription, font_small));
                    objectiveDescriptionCell.AddElement(new Phrase(Chunk.NEWLINE));
                    objectiveDescriptionCell.Border = ObjectivesTableBorder;
                    objectiveDescriptionCell.Padding = 0;
                    objectiveDescriptionCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    objectiveDescriptionCell.VerticalAlignment = PdfPCell.ALIGN_TOP;

                    // Set up the marks cell
                    PdfPTable marksTable = new PdfPTable(2);
                    marksTable.SpacingBefore = 5;
                    float[] marksTableWidths = new float[] { 2f, 1f };
                    marksTable.SetWidths(marksTableWidths);

                    foreach (OutcomeMark objectivemark in outcome.marks)
                    {
                        PdfPCell markCell = new PdfPCell();

                        // Attempt to figure out of the mark is an objective or a percent, and display an outcome bar if necessary
                        if (!string.IsNullOrEmpty(objectivemark.cMark))
                        {
                            PdfPCell Temp_MarkCell = new PdfPCell();
                            Temp_MarkCell.Border = ObjectivesTableBorder;
                            Temp_MarkCell.Padding = 0;
                            Temp_MarkCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                            Temp_MarkCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                            Temp_MarkCell.AddElement((displayOutcomeBar(content, objectivemark.cMark)));
                            markCell = Temp_MarkCell;
                        }
                        else
                        {
                            // If the numeric mark is between 0 and 4, assume that is it an outcome and display the bar
                            if (
                                (objectivemark.nMark <= 0) &&
                                (objectivemark.nMark >= 4)
                                )
                            {
                                PdfPCell Temp_MarkCell = new PdfPCell();
                                Temp_MarkCell.Border = ObjectivesTableBorder;
                                Temp_MarkCell.Padding = 0;
                                Temp_MarkCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                                Temp_MarkCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                Temp_MarkCell.AddElement((displayOutcomeBar(content, objectivemark.cMark)));
                                markCell = Temp_MarkCell;
                            }
                            else
                            {
                                PdfPCell Temp_ReportPeriodCell = new PdfPCell(new Phrase(Math.Round(objectivemark.nMark, 0) + "%", font_body_bold));
                                Temp_ReportPeriodCell.Border = ObjectivesTableBorder;
                                Temp_ReportPeriodCell.Padding = 0;
                                Temp_ReportPeriodCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                Temp_ReportPeriodCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                markCell = Temp_ReportPeriodCell;
                            }                            
                        }

                        // Display the report period
                        PdfPCell reportPeriodCell = new PdfPCell(new Phrase(objectivemark.reportPeriod.name, font_small_italic));                        
                        reportPeriodCell.Padding = 2;
                        reportPeriodCell.PaddingLeft = 10;
                        reportPeriodCell.Border = ObjectivesTableBorder;
                        reportPeriodCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                        reportPeriodCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;

                        marksTable.AddCell(markCell);
                        marksTable.AddCell(reportPeriodCell);
                    }

                    objectiveMarksCell.Border = ObjectivesTableBorder;
                    objectiveMarksCell.Padding = 0;
                    objectiveMarksCell.AddElement(marksTable);

                    // Build the final table to return
                    objectiveChunkTable.KeepTogether = false;
                    float[] objectivesTableWidths = new float[] { 0.10f, 3.65f, 2.75f };

                    objectiveChunkTable.AddCell(bufferCell);
                    objectiveChunkTable.AddCell(objectiveDescriptionCell);
                    objectiveChunkTable.AddCell(objectiveMarksCell);
                    //objectiveChunkTable.AddCell(marksTable);

                    objectiveChunkTable.SetWidths(objectivesTableWidths); 
                }                       
            }

            // Encapsulate the table in a cell object and return it
            PdfPCell objectiveChunkTableCell = new PdfPCell(objectiveChunkTable);
            objectiveChunkTableCell.Colspan = 2;
            objectiveChunkTableCell.Border = ObjectivesTableBorder;

            return objectiveChunkTableCell;
        }

        public static PdfPCell lifeSkillsChunk(List<Outcome> objectives, PdfContentByte content)
        {
            // Interesting note: If you add an element to a cell in the constructor it aligns differnetly than if you add it as an element

            string lifeSkillsCategoryName = "Successful Learner Behaviours";

            int lifeSkillsTableBorder = 0;

            PdfPCell bufferCell = new PdfPCell(new Phrase(""));
            bufferCell.Border = lifeSkillsTableBorder;

            // outcomeBar_JustNumbers()

            // TODO: This may need to check for the exact name, but that doesn't exist in the data yet 
            // Currently normal objectives don't have a category, and "life skills" objectives do have a category

            // Condense the list of objectives to just the ones we care about
            List<Outcome> lifeSkillsObjectives = new List<Outcome>();
            foreach (Outcome objective in objectives)
            {
                if (objective.category.ToLower() == lifeSkillsCategoryName.ToLower())
                {
                    lifeSkillsObjectives.Add(objective);
                }
            }

            if (lifeSkillsObjectives.Count > 0)
            {

                // Figure out life skills names and how many to display
                List<string> lifeSkillsNames = new List<string>();
                List<string> reportPeriodNames = new List<string>();
                foreach (Outcome objective in lifeSkillsObjectives)
                {
                    // Figure out life skills names and how many to display
                    if (!lifeSkillsNames.Contains(objective.subject))
                    {
                        lifeSkillsNames.Add(objective.subject);
                    }

                    foreach (OutcomeMark mark in objective.marks)
                    {
                        if (!reportPeriodNames.Contains(mark.reportPeriod.name))
                        {
                            reportPeriodNames.Add(mark.reportPeriod.name);
                        }
                    }
                }

                // Figure out how many report periods we need to display
                int numColumns = lifeSkillsNames.Count + 1;
                if (numColumns < 1)
                {
                    numColumns = 1;
                }

                PdfPTable lifeSkillChunkTable = new PdfPTable(numColumns);

                // Display column names

                PdfPCell rpHeadingCell = new PdfPCell(new Phrase("Report Period", font_small_bold));
                rpHeadingCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                rpHeadingCell.Border = lifeSkillsTableBorder;
                lifeSkillChunkTable.AddCell(rpHeadingCell);
                foreach (string lifeSkillName in lifeSkillsNames)
                {
                    PdfPCell columnHeadingCell = new PdfPCell(new Phrase(lifeSkillName, font_small_bold));
                    columnHeadingCell.Border = lifeSkillsTableBorder;
                    columnHeadingCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    lifeSkillChunkTable.AddCell(columnHeadingCell);
                }

                // Display number bars
                lifeSkillChunkTable.AddCell(bufferCell);
                foreach (string lifeSkillName in lifeSkillsNames)
                {
                    PdfPCell columnHeadingCell = new PdfPCell(outcomeBar_JustNumbers(content), true);
                    columnHeadingCell.Border = lifeSkillsTableBorder;
                    columnHeadingCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    lifeSkillChunkTable.AddCell(columnHeadingCell);
                }

                // Display outcome bars
                foreach (string reportPeriodName in reportPeriodNames)
                {
                    PdfPCell rpNamecell = new PdfPCell(new Phrase(reportPeriodName, font_small));
                    rpNamecell.Border = lifeSkillsTableBorder;
                    rpNamecell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    lifeSkillChunkTable.AddCell(rpNamecell);

                    foreach (string lifeSkillName in lifeSkillsNames)
                    {
                        PdfPCell markValueCell = new PdfPCell();
                        markValueCell.Border = lifeSkillsTableBorder;

                        foreach (Outcome objective in lifeSkillsObjectives)
                        {
                            if (objective.subject == lifeSkillName)
                            {
                                foreach (OutcomeMark objectivemark in objective.marks)
                                {
                                    if (objectivemark.reportPeriod.name == reportPeriodName)
                                    {
                                        // Figure out which mark to use, because high school objective marks will in the wrong place because schoologic is such a failure

                                        float markToDisplay = 0;

                                        float cMark_Parsed = -1;
                                        float.TryParse(objectivemark.cMark, out cMark_Parsed);

                                        // only marks between 1 and 4 are valid
                                        if ((cMark_Parsed >= 1) && (cMark_Parsed <= 4))
                                        {
                                            markToDisplay = cMark_Parsed;
                                        }
                                        else if ((objectivemark.nMark >= 1) && (objectivemark.nMark <= 4))
                                        {
                                            markToDisplay = objectivemark.nMark;
                                        }
                                        else if ((objectivemark.nMark >= 0.01) && (objectivemark.nMark <= 100))
                                        {
                                            markToDisplay = LSKYCommon.translatePercentToOutcome(objectivemark.nMark);
                                        }
                                        else
                                        {
                                            markToDisplay = -1;
                                        }

                                        markValueCell.AddElement(outcomeBar_Thin(content, markToDisplay.ToString()));

                                    }
                                }
                            }
                        }

                        lifeSkillChunkTable.AddCell(markValueCell);

                    }
                }


                // Encapsulate the table in a cell object and return it

                // Encapsulate the table in another table, so I can format it with whitespace on the left

                PdfPCell cellStyle = new PdfPCell();
                cellStyle.Border = lifeSkillsTableBorder;

                PdfPTable tableContainer = new PdfPTable(2);
                float[] tableContainer_Widths = new float[] { 0.10f, 6.4f };
                tableContainer.SetWidths(tableContainer_Widths);

                tableContainer.AddCell(bufferCell);
                PdfPCell finalTableCell = new PdfPCell(lifeSkillChunkTable);
                finalTableCell.Border = lifeSkillsTableBorder;

                tableContainer.AddCell(finalTableCell);


                PdfPCell objectiveChunkTableCell = new PdfPCell(tableContainer);
                objectiveChunkTableCell.Colspan = 2;
                objectiveChunkTableCell.Border = lifeSkillsTableBorder;

                return objectiveChunkTableCell;
            }
            else
            {
                PdfPCell blankCell = new PdfPCell(new Phrase(""));
                blankCell.Colspan = 2;
                blankCell.Border = lifeSkillsTableBorder;
                return blankCell;
            }
        }

        public static PdfPCell emptyCell()
        {
            PdfPCell newCell = new PdfPCell(new Paragraph(""));
            newCell.Border = 0;
            newCell.Padding = 5;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            return newCell;
        }

        public static PdfPTable classWithMarks(SchoolClass course, PdfContentByte content, bool anonymize = false)
        {          
            // Housekeeping first 
            // Check if the outcomes (including life skills outcomes) actually contain any marks
            // If not, don't bother displaying their section

            bool hasOutcomesWithMarks = false;
            bool hasComments = false;
            foreach (Outcome objective in course.Outcomes)
            {
                if (objective.marks.Count > 0)
                {
                    hasOutcomesWithMarks = true;
                }

                foreach (Mark mark in course.Marks)
                {
                    if (!string.IsNullOrEmpty(mark.comment))
                    {
                        hasComments = true;
                    }
                }
            }

            // Start building the table
            PdfPTable classTable = new PdfPTable(2);
            classTable.HorizontalAlignment = 1;
            classTable.TotalWidth = 500f;
            classTable.LockedWidth = true;
            //classTable.SpacingAfter = 35;
            classTable.KeepTogether = true;


            float[] widths = new float[] { 2f, 1f };


            classTable.SetWidths(widths);
            
            // Course Title
            PdfPTable classTitleTable = new PdfPTable(1);

            /*
            Paragraph classTitleParagraph = new Paragraph();
            classTitleParagraph.Add(new Phrase(course.name, font_large_bold));
            classTitleParagraph.Add(Chunk.NEWLINE);
            classTitleParagraph.Add(new Phrase("" + course.teacherName + "", font_small));
            */
            
            PdfPCell classTitleCell = new PdfPCell(new Phrase(course.name, font_large_bold));
            classTitleCell.Border = 0;
            classTitleCell.Padding = 0;
            classTitleCell.PaddingLeft = 0;

            PdfPCell classTeacherCell = new PdfPCell(new Phrase(course.teacherName, font_small));
            classTeacherCell.Border = 0;
            classTeacherCell.Padding = 0;
            classTeacherCell.PaddingLeft = 0;

            classTitleTable.AddCell(classTitleCell);
            classTitleTable.AddCell(classTeacherCell);

            PdfPCell classTitleTableContainer = new PdfPCell(classTitleTable);
            classTitleTableContainer.Border = 0;
            classTable.AddCell(classTitleTableContainer);


            // Course Mark (Adjusted Grade)

            // Only display an overall mark if:
            //  - Display the final report period if the class is grade 10, 11 or 12
            //  - Display all report periods if the class is not outcome based
            //    - display as an outcome for K-9
            //    - display as a percent for 10-12
            // Otherwise, never display anything                       
            // Class marks are always displayed as a percent                     

            

            // Class is not outcome based, so display the marks
            //  - If the class has a grade legend, this is an outcome
            //  - If the class does not have a grade legend, this is a percent
            
            // Class is outcome based and is 10-12
            //  - Display the final mark only
            //  - This is always a percent
            
            // Class is outcome based, and is k-9 (display nothing)
            

            
            if (
                (course.Marks.Count > 0) &&
                (course.hasObjectives()) &&
                (course.isHighSchoolLevel()) &&
                (course.term.FinalReportPeriod != null)
                )
            {
                // Class is outcome based and is 10-12
                //  - Display the final mark only
                //  - This is always a percent

                #region Outcome based 10-12 class

                // Find the mark from the final report period
                Mark finalMark = null;
                foreach (Mark mark in course.Marks) 
                {
                    if (mark.reportPeriodID == course.term.FinalReportPeriod.ID)
                    {
                        finalMark = mark;
                    }
                }

                if (finalMark != null)
                {
                    PdfPTable embeddedMarkTable = new PdfPTable(1);

                    // Parse the mark so that we can round it
                    double markToDisplay = 0;
                    double.TryParse(finalMark.nMark, out markToDisplay);

                    PdfPCell markCell = new PdfPCell(new Phrase(Math.Round(markToDisplay, 0) + "%", font_body_bold));
                    markCell.Border = Rectangle.BOX;
                    markCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;                    
                    markCell.Padding = 5;

                    PdfPCell titleCell = new PdfPCell(new Phrase("Final Mark", font_small));
                    titleCell.Border = Rectangle.BOX;
                    titleCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    titleCell.Padding = 2;
                    titleCell.PaddingBottom = 3;

                    
                    embeddedMarkTable.AddCell(titleCell);
                    embeddedMarkTable.AddCell(markCell);

                    PdfPCell embeddedMarkTableContainer = new PdfPCell(embeddedMarkTable);
                    embeddedMarkTableContainer.Border = 0;
                    classTable.AddCell(embeddedMarkTableContainer);
                }
                else
                {
                    classTable.AddCell(emptyCell());
                }
                #endregion

            }
            else if (
                (course.Marks.Count > 0) &&
                (!course.hasObjectives()) &&
                (course.term.FinalReportPeriod != null)
                )
            {
                // Class is not outcome based, so display the marks
                //  - If the class has a grade legend, this is an outcome
                //  - If the class does not have a grade legend, this is a percent

                #region Class without outcomes (displaying all report periods)
                
                // Get list of report periods to display
                List<ReportPeriod> loadedReportPeriods = new List<ReportPeriod>();
                foreach (Mark mark in course.Marks)
                {
                    if (!loadedReportPeriods.Contains(mark.reportPeriod))
                    {
                        loadedReportPeriods.Add(mark.reportPeriod);
                    }
                }

                PdfPTable embeddedMarkTable = new PdfPTable(loadedReportPeriods.Count);

                // Display the report period names
                foreach (ReportPeriod rp in loadedReportPeriods)
                {
                    PdfPCell reportPeriodNameCell = new PdfPCell(new Phrase(rp.name, font_small));
                    reportPeriodNameCell.Border = Rectangle.BOX;
                    reportPeriodNameCell.Padding = 2;
                    reportPeriodNameCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    embeddedMarkTable.AddCell(reportPeriodNameCell);
                }

                // Display the marks
                foreach (ReportPeriod rp in loadedReportPeriods)
                {
                    Mark MarkForThisReportPeriod = null;
                    foreach (Mark mark in course.Marks)
                    {
                        if (mark.reportPeriodID == rp.ID) 
                        {
                            MarkForThisReportPeriod = mark;
                        }
                    }

                    if (MarkForThisReportPeriod != null) 
                    {
                        // Should we display a percent or an outcome (1-4) mark?
                        //  - If a grade legend exists, use outcome marks (cMark)
                        //  - If a grade legend does not exist, use percent marks (nMark)
                        Paragraph markValue = new Paragraph();

                        if (course.isOutcomeBased()) {
                           // Display an outcome mark for each valid report period
                            markValue.Add(new Phrase(MarkForThisReportPeriod.cMark, font_body_bold));

                        } else {
                           // Display a percent mark for each valid report period
                           double parsedMark = 0;
                           double.TryParse(MarkForThisReportPeriod.nMark, out parsedMark);
                           markValue.Add(new Phrase(Math.Round(parsedMark, 0) + "%", font_body_bold));
                        }

                        PdfPCell markCell = new PdfPCell(markValue);
                        markCell.Border = Rectangle.BOX;                        
                        markCell.Padding = 5;
                        markCell.PaddingBottom = 7;
                        markCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                        embeddedMarkTable.AddCell(markCell);


                    } else {
                        embeddedMarkTable.AddCell(emptyCell());
                    }

                }

                          
                

                PdfPCell embeddedMarkTableContainer = new PdfPCell(embeddedMarkTable);
                embeddedMarkTableContainer.Border = 0;
                classTable.AddCell(embeddedMarkTableContainer);

                #endregion

            }
            else
            {
                // Class is outcome based, and is k-9 (display nothing)
                // or
                // There are no marks for this class
                                
                PdfPCell blankCell = new PdfPCell(new Paragraph(""));
                blankCell.Border = 0;
                blankCell.Padding = 5;
                blankCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                classTable.AddCell(blankCell);
            }



            //Display outcomes
            if (hasOutcomesWithMarks)
            {
                Paragraph outcomeParagraph = new Paragraph();
                outcomeParagraph.Add(new Phrase("Outcomes:", font_body_bold));
                PdfPCell outcomeCell = new PdfPCell(outcomeParagraph);
                outcomeCell.Border = 0;
                outcomeCell.Padding = 5;
                outcomeCell.Colspan = 2;
                classTable.AddCell(outcomeCell);

                /* Outcome marks */
                foreach (Outcome objective in course.Outcomes)
                {
                    // This is broken, and I need to test something else - fix me later
                    classTable.AddCell(outcomeChunk(objective, content));
                }
            }

            // Life skills
            if (hasOutcomesWithMarks)
            {
                Paragraph lifeskillsParagraph = new Paragraph();
                lifeskillsParagraph.Add(new Phrase("Successful Learning Behaviors:", font_body_bold));
                PdfPCell lifeSkillsCell = new PdfPCell(lifeskillsParagraph);
                lifeSkillsCell.Border = 0;
                lifeSkillsCell.Padding = 5;
                lifeSkillsCell.Colspan = 2;
                classTable.AddCell(lifeSkillsCell);
                classTable.AddCell(lifeSkillsChunk(course.Outcomes, content));
            }

            // Comments
            if (hasComments)
            {
                Paragraph commentTitleParagraph = new Paragraph();
                commentTitleParagraph.Add(new Phrase("\nComments:\n", font_body_bold));

                PdfPCell commentTitleCell = new PdfPCell(commentTitleParagraph);
                commentTitleCell.Border = 0;
                commentTitleCell.Padding = 5;
                commentTitleCell.Colspan = 2;
                classTable.AddCell(commentTitleCell);

                Paragraph commentsParagraph = new Paragraph();
                foreach (Mark mark in course.Marks)
                {
                    if (!string.IsNullOrEmpty(mark.comment))
                    {
                        commentsParagraph.Add(new Phrase(mark.reportPeriod.name + ": ", font_small_bold));
                        if (anonymize)
                        {
                            commentsParagraph.Add(new Phrase("Comment would go here", font_small));
                        }
                        else
                        {
                            commentsParagraph.Add(new Phrase(mark.comment, font_small));
                        }
                        commentsParagraph.Add(Chunk.NEWLINE);
                    }
                }
                PdfPCell commentsCell = new PdfPCell(commentsParagraph);
                commentsCell.Border = 0;
                commentsCell.Padding = 5;
                commentsCell.PaddingLeft = 10;
                commentsCell.Colspan = 2;
                classTable.AddCell(commentsCell);
            }
            return classTable;
        }

        public static MemoryStream GeneratePDF(List<Student> students, bool anonymize = false)
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
            //PageEventHandler.bottomCenter = "Printed " + DateTime.Now.ToLongDateString();

            foreach (Student student in students)
            {
                if (!anonymize)
                {
                    PageEventHandler.bottomLeft = student.getDisplayName();
                }
                else
                {
                    PageEventHandler.bottomLeft = "Student Name";
                }
                ReportCard.Add(PDFReportCardParts.schoolNamePlate(student.school));
                ReportCard.Add(PDFReportCardParts.namePlateTable(student, anonymize));
                ReportCard.Add(PDFReportCardParts.lifeSkillsLegend(content, student.getGrade()));
                ReportCard.Add(PDFReportCardParts.outcomeLegend(content));
                ReportCard.NewPage();
                ReportCard.Add(new Phrase(string.Empty));


                foreach (Term term in student.track.terms)
                {
                    foreach (SchoolClass course in term.Courses)
                    {
                        ReportCard.Add(PDFReportCardParts.classWithMarks(course, content, anonymize));
                        if (!student.track.daily)
                        {
                            ReportCard.Add(PDFReportCardParts.courseAttendanceSummary(student, course));
                        }
                    }
                }

                ReportCard.Add(PDFReportCardParts.attendanceSummary(student));

                PageEventHandler.ResetPageNumbers(ReportCard);
            }

            ReportCard.Close();
            return memstream;
        }
        
    }
}