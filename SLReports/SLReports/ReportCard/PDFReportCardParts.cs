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
        #region Options for look and feel
        public enum OutcomeMarkDisplayStyle
        {
            AsPercent,
            AsNumber,
            AsOutcomeBar
        }

        public enum ClassMarksToDisplay
        {
            None,
            FinalReportPeriodOnly,
            AllReportPeriods
        }

        public enum ClassMarkDisplayStyle
        {
            AsPercent,
            AsNumber,
            AsOutcomeBar
        }

        public enum OutcomeBarStyle
        {
            Slider,
            SliderWithoutNumber,
            LifeSkills,
            Minimal,
            JustNumber,
            NumberBar,
            Table
        }
        #endregion

        #region Globals (Standard colors and fonts)
        public static String ReportCardDatabase = LSKYCommon.dbConnectionString_SchoolLogicTest;
        //public static String ReportCardDatabase = LSKYCommon.dbConnectionString_SchoolLogic;

        private static iTextSharp.text.Image lskyLogo = iTextSharp.text.Image.GetInstance(@"https://sldata.lskysd.ca/SLReports/lsky_logo_text.png");

        private static Font font_large = FontFactory.GetFont("Verdana", 14, BaseColor.BLACK);
        private static Font font_large_bold = FontFactory.GetFont("Verdana", 14, Font.BOLD, BaseColor.BLACK);
        private static Font font_large_italic = FontFactory.GetFont("Verdana", 14, Font.ITALIC, BaseColor.BLACK);

        private static Font font_body = FontFactory.GetFont("Verdana", 10, BaseColor.BLACK);
        private static Font font_body_bold = FontFactory.GetFont("Verdana", 10, Font.BOLD, BaseColor.BLACK);
        private static Font font_body_italic = FontFactory.GetFont("Verdana", 10, Font.ITALIC, BaseColor.BLACK);

        private static Font font_small = FontFactory.GetFont("Verdana", 8, BaseColor.BLACK);
        private static Font font_small_bold = FontFactory.GetFont("Verdana", 8, Font.BOLD, BaseColor.BLACK);
        private static Font font_small_bold_white = FontFactory.GetFont("Verdana", 8, Font.BOLD, BaseColor.WHITE);
        private static Font font_small_italic = FontFactory.GetFont("Verdana", 8, Font.ITALIC, BaseColor.BLACK);

        private static Font font_very_small = FontFactory.GetFont("Verdana", 5, BaseColor.BLACK);
        private static Font font_very_small_bold = FontFactory.GetFont("Verdana", 5, Font.BOLD, BaseColor.BLACK);       
                
        #endregion

        /// <summary>
        /// Returns a BaseColor appropriate for the given value. Used for text.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static BaseColor GetTextColor(Single value)
        {
            BaseColor color_text_neutral = new BaseColor(255, 255, 255);
            BaseColor color_text_max = new BaseColor(255, 255, 255);
            BaseColor color_text_low = new BaseColor(255, 255, 255);
            BaseColor color_text_medium = new BaseColor(0, 0, 0);
            BaseColor color_text_high = new BaseColor(255, 255, 255);

            if (value <= 1)
            {
                return color_text_low;
            }
            else if (value <= 2.25)
            {
                return color_text_medium;
            }
            else if (value <= 3.5)
            {
                return color_text_high;
            }
            else if (value <= 4)
            {
                return color_text_max;
            }

            return color_text_neutral;

        }

        /// <summary>
        /// Returns a BaseColor appropriate for the given value. Used for fill.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static BaseColor GetFillColor(Single value)
        {
            BaseColor color_neutral = new BaseColor(70, 70, 70);
            BaseColor color_low = new BaseColor(255, 51, 0); // Red
            BaseColor color_medium = new BaseColor(255, 255, 50); // Yellow
            BaseColor color_high = new BaseColor(0, 128, 0); // Green
            BaseColor color_max = new BaseColor(21, 137, 255);

            if (value <= 1)
            {
                return color_low;
            }
            else if (value <= 2.25)
            {
                return color_medium;
            }
            else if (value <= 3.5)
            {
                return color_high;
            }
            else if (value <= 4)
            {
                return color_max;
            }

            return color_neutral;
        }

        /// <summary>
        /// Returns a BaseColor appropriate for the given value. Used for Borders.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static BaseColor GetBorderColor(Single value)
        {
            return new BaseColor(0, 0, 0);
        }

        /// <summary>
        /// Displays a graphical number bar for outcomes
        /// </summary>
        /// <param name="content">A PdfContentByte object for the document</param>
        /// <param name="value">The value of the outcome mark to display</param>
        /// <returns></returns>
        private static iTextSharp.text.Image displayOutcomeBar(PdfContentByte content, String value, OutcomeBarStyle style = OutcomeBarStyle.Slider)
        {
            if (style == OutcomeBarStyle.JustNumber)
            {
                return outcomeBar_JustNumber(content, value);
            }
            else if (style == OutcomeBarStyle.Minimal)
            {
                return outcomeBar_Minimalist(content, value);
            }
            else if (style == OutcomeBarStyle.LifeSkills)
            {
                return outcomeBar_LifeSkills(content, value);
            }
            else if (style == OutcomeBarStyle.Slider)
            {
                return outcomeBar_Slider(content, value);
            }
            else if (style == OutcomeBarStyle.SliderWithoutNumber)
            {
                return outcomeBar_Slider_NoNumbers(content, value);
            }
            else if (style == OutcomeBarStyle.Table)
            {
                return outcomeBar_Original(content, value);
            }

            return outcomeBar_Slider(content, value);
        }

        #region Outcome Bar Styles
        
        public static iTextSharp.text.Image outcomeBar_Original(PdfContentByte content, String value)
        {
            int width = 125;
            int height = 10;
            int maxvalue = 4;
            int rectancleCurveRadius = 2;

            int CanvasPaddingX = 1;
            int CanvasPaddingY = 1;
            
            Single parsedValue = -1;

            PdfTemplate canvas = content.CreateTemplate(width + (CanvasPaddingX * 2), height + (CanvasPaddingY * 2));

            BaseColor borderColor = new BaseColor(0, 0, 0);


            if (Single.TryParse(value, out parsedValue))
            {
                Single percentFill = parsedValue / maxvalue;
                BaseColor fillColor = GetFillColor(parsedValue);
                borderColor = GetBorderColor(parsedValue);

                // Background
                canvas.Rectangle(0, 0, width, height);
                canvas.SetRGBColorFill(255, 255, 255);
                canvas.Fill();

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

        public static iTextSharp.text.Image outcomeBar_NumberBar(PdfContentByte content)
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

        public static iTextSharp.text.Image outcomeBar_LifeSkills(PdfContentByte content, String value)
        {
            int width = 125;
            int height = 9;

            int CanvasPaddingX = 0;
            int CanvasPaddingY = 0;

            int maxvalue = 4;
            int rectancleCurveRadius = 2;

            Single parsedValue = -2;

            PdfTemplate canvas = content.CreateTemplate(width + (CanvasPaddingX * 2), height + (CanvasPaddingY * 2));

            BaseColor borderColor = new BaseColor(0.4f, 0.4f, 0.4f);
            
            /* Background */
            canvas.Rectangle(0, 0, width + (CanvasPaddingX * 2), height + (CanvasPaddingY * 2));
            canvas.SetRGBColorFill(255, 255, 255);
            canvas.Fill();

            if (Single.TryParse(value, out parsedValue))
            {
                if ((parsedValue >= 1) && (parsedValue <= 4))
                {
                    Single percentFill = parsedValue / maxvalue;
                    BaseColor fillColor = GetFillColor(parsedValue);

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

                    
                }
                else
                {
                    borderColor = new BaseColor(1f, 0f, 0f);
                    BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    canvas.SetRGBColorFill(0, 0, 0);
                    canvas.BeginText();
                    canvas.SetFontAndSize(bf, 6);
                    canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "INVALID DATA", width / 2, (height / 2) - 2, 0);
                    canvas.EndText();
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

            /* Border */
            canvas.SetRGBColorStroke(255, 255, 255);
            canvas.RoundRectangle(CanvasPaddingX, CanvasPaddingY, width, height, rectancleCurveRadius);
            //canvas.Rectangle(CanvasPaddingX, CanvasPaddingY, width, height);
            canvas.SetColorStroke(borderColor);
            canvas.Stroke();

            return iTextSharp.text.Image.GetInstance(canvas); ;
        }

        public static iTextSharp.text.Image outcomeBar_Slider(PdfContentByte content, String value)
        {
            int width = 125;
            int height = 12;
            int barMargin = 3;
            int indicatorWidth = 15 + (value.Length * 2);

            int CanvasPaddingX = 1;
            int CanvasPaddingY = 1;

            int maxvalue = 4;
            int rectancleCurveRadius = 2;
            
            Single parsedValue = -1;

            PdfTemplate canvas = content.CreateTemplate(width + 2, height + 2);
            BaseColor borderColor = new BaseColor(0, 0, 0);

            if (Single.TryParse(value, out parsedValue))
            {
                if (parsedValue > maxvalue)
                {
                    parsedValue = maxvalue;
                }

                Single percentFill = parsedValue / maxvalue;
                BaseColor fillColor = GetFillColor(parsedValue);
                borderColor = GetBorderColor(parsedValue);
                BaseColor textColor = GetTextColor(parsedValue);

                

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

        public static iTextSharp.text.Image outcomeBar_Slider_NoNumbers(PdfContentByte content, String value)
        {
            int width = 125;
            int height = 12;
            int barMargin = 3;

            int CanvasPaddingX = 1;
            int CanvasPaddingY = 1;

            int maxvalue = 4;
            int rectancleCurveRadius = 2;

            Single parsedValue = -1;

            PdfTemplate canvas = content.CreateTemplate(width + 2, height + 2);
            BaseColor borderColor = new BaseColor(0, 0, 0);

            if (Single.TryParse(value, out parsedValue))
            {
                if (parsedValue > maxvalue)
                {
                    parsedValue = maxvalue;
                }

                Single percentFill = parsedValue / maxvalue;
                BaseColor fillColor = GetFillColor(parsedValue);
                borderColor = GetBorderColor(parsedValue);
                BaseColor textColor = GetTextColor(parsedValue);

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


            Single parsedValue = -1;

            PdfTemplate canvas = content.CreateTemplate(width + (CanvasPaddingX * 2), height + (CanvasPaddingY * 2));

            /* Background */
            canvas.Rectangle(0, 0, width + (CanvasPaddingX * 2), height + (CanvasPaddingY * 2));
            canvas.SetRGBColorFill(255, 255, 255);
            canvas.Fill();

            if (Single.TryParse(value, out parsedValue))
            {
                Single percentFill = parsedValue / maxvalue;
                BaseColor fillColor = GetFillColor(parsedValue);
                BaseColor borderColor = GetBorderColor(parsedValue);
                BaseColor textColor = GetTextColor(parsedValue);

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

        public static iTextSharp.text.Image outcomeBar_JustNumber(PdfContentByte content, String value, bool fillBackground = false)
        {
            int width = 125;
            int height = 25; // This should be an "odd" number, so that things centered vertically are actually centered
            int barWidth = 30;
            int CanvasPaddingX = 1;
            int CanvasPaddingY = 1;

            int rectancleCurveRadius = 2;

            string barContent = value;

            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            PdfTemplate canvas = content.CreateTemplate(width + 2, height + 2);
            canvas.SetFontAndSize(bf, 14);

            if (value.ToLower() == "ie")
            {
                canvas.SetFontAndSize(bf, 10);
                barContent = "Insufficient Evidence";
                barWidth = width;
            }

            if (value.ToLower() == "nym")
            {
                canvas.SetFontAndSize(bf, 10);
                barContent = "Not Yet Meeting";
                barWidth = width;
            }            

            // Colors
            BaseColor borderColor = new BaseColor(0.7f, 0.7f, 0.7f, 0.5f);
            BaseColor backgroundColor = new BaseColor(0.9f, 0.9f, 0.9f, 0.25f);
            BaseColor textColor = new BaseColor(0, 0, 0);

            // Background
            if (fillBackground)
            {
                Single parsedValue = -1;
                if (Single.TryParse(value, out parsedValue))
                {
                    BaseColor fillColor = GetFillColor(parsedValue);

                    canvas.RoundRectangle((width / 2) - (barWidth / 2), CanvasPaddingY, barWidth, height, rectancleCurveRadius);
                    canvas.SetColorFill(fillColor);
                    canvas.Fill();
                }
            }
            else
            {
                canvas.RoundRectangle((width / 2) - (barWidth / 2), CanvasPaddingY, barWidth, height, rectancleCurveRadius);
                canvas.SetColorFill(backgroundColor);
                canvas.Fill();
            }
            

            // Text
            canvas.BeginText();
            canvas.SetColorFill(textColor);
            canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, barContent, width / 2, (height / 2) - 3, 0);
            canvas.EndText();

            // Border
            canvas.SetRGBColorStroke(255, 255, 255);
            canvas.RoundRectangle((width / 2) - (barWidth /2), CanvasPaddingY, barWidth, height, rectancleCurveRadius);
            canvas.SetColorStroke(borderColor);
            canvas.Stroke();

            
            return iTextSharp.text.Image.GetInstance(canvas); ;
        }
        
        #endregion

        /// <summary>
        /// Determines which number to display on the report card
        /// </summary>
        /// <param name="grade">The grade level of the class</param>
        /// <param name="nMark">The nMark value from the mark record</param>
        /// <param name="cMark">The cMark value from the mark record</param>
        /// <returns>Returns a mark (as a string), or an empty string if there is no valid mark</returns>
        private static string getNumberToDisplay(string grade, int nMark, string cMark)
        {
            string returnMe = string.Empty;

            // Parse the grade into a number ('K' and 'PK' should parse to zero)
            int gradeInt = 0;
            int.TryParse(grade, out gradeInt);

            // k-9 should always use the cMark
            if (gradeInt > 10)
            {
                return cMark;
            }

            // 10-12 should use the nMark if it is non-zero, then the cMark if it is not null            
            if (gradeInt >= 10)
            {
                if (nMark > 0)
                {
                    returnMe = nMark.ToString();
                }
                else if (!string.IsNullOrEmpty(cMark))
                {
                    returnMe = cMark;
                }
            }
             
            return returnMe;
        }

        private static PdfPTable schoolNamePlate(School school)
        {
            int cellpadding = 3;
            int border = Rectangle.NO_BORDER;

            PdfPTable schoolNamePlateTable = new PdfPTable(3);
            schoolNamePlateTable.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            schoolNamePlateTable.TotalWidth = 500f;
            schoolNamePlateTable.LockedWidth = true;
            schoolNamePlateTable.SpacingAfter = 40;
            float[] widths = new float[] { 50f, 400f, 50f };
            schoolNamePlateTable.SetWidths(widths);

            PdfPCell schoolLogoCell = new PdfPCell();
            schoolLogoCell.Border = 0;
            schoolLogoCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            schoolNamePlateTable.AddCell(schoolLogoCell);
            
            Paragraph SchoolNameParagraph = new Paragraph();
            SchoolNameParagraph.Add(new Phrase(school.getName(), font_large_bold));
            SchoolNameParagraph.Add(Chunk.NEWLINE);
            SchoolNameParagraph.Add(Chunk.NEWLINE);
            SchoolNameParagraph.Add(new Phrase(school.address, font_body));
            PdfPCell SchoolNameCell = new PdfPCell(SchoolNameParagraph);
            SchoolNameCell.VerticalAlignment = 0;
            SchoolNameCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            SchoolNameCell.Padding = cellpadding;
            SchoolNameCell.Border = border;
            
            schoolNamePlateTable.AddCell(SchoolNameCell);

            lskyLogo.ScalePercent(5);
            PdfPCell divisionLogoCell = new PdfPCell(lskyLogo);
            divisionLogoCell.Border = 0;
            divisionLogoCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            schoolNamePlateTable.AddCell(divisionLogoCell);


            return schoolNamePlateTable;

        }

        private static PdfPTable namePlateTable(Student student, bool anonymize = false, bool showPlaceholderPhotos = false)
        {
            Font font_StudentName = FontFactory.GetFont("Verdana", 22, Font.BOLD, BaseColor.BLACK);
            Font font_title = FontFactory.GetFont("Verdana", 10, Font.BOLD, BaseColor.BLACK);
            Font font_value = FontFactory.GetFont("Verdana", 10, Font.NORMAL, BaseColor.BLACK);

            int border = 0;

            if ((student == null) || (anonymize))
            {
                student = new Student("John", "Smith", "John", "Smith", "J", "Demo", "000000000", "Demo School", "00000", "X", "Saskatchewan", "North Battleford", "Fake St", "123", "", "H0H0H0", "3065551234", "Male", "Instatus", "Instatuscode", "Homeroom Teacher", DateTime.Now.AddDays(-1), DateTime.Now, "000", "Band name", "Reserve Name", "House #", "000000000", false, 000, false, "user.name", 20, 0, "", "English");
            }

            PdfPTable nameplateTable = new PdfPTable(2);
            nameplateTable.HorizontalAlignment = 1;
            nameplateTable.TotalWidth = 500f;
            nameplateTable.LockedWidth = true;
            nameplateTable.SpacingAfter = 50;
            float[] widths = new float[] { 350f, 150f };
            nameplateTable.SetWidths(widths);

            PdfPCell studentNamecell = new PdfPCell(new Phrase(student.getDisplayName(), font_StudentName));
            studentNamecell.Border = border;
            nameplateTable.AddCell(studentNamecell);


            PdfPCell photoCell = new PdfPCell(new Phrase("", font_large_italic));            
            if ((student.hasPhoto()) || (showPlaceholderPhotos))
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

            // Put student info table in a nested table for easier formatting

            Dictionary<string, string> studentInformation = new Dictionary<string, string>();
            studentInformation.Add("Student Number", student.getStudentID());
            if (!string.IsNullOrEmpty(student.getHomeRoom()))
            {
                studentInformation.Add("Homeroom", student.getHomeRoom());
            }

            studentInformation.Add("Grade", student.getGradeFormatted());

            if (student.creditsEarned > 0)
            {
                studentInformation.Add("Credits Earned", student.creditsEarned.ToString());
            }
                        
            PdfPTable studentInfoTable = new PdfPTable(2);
            float[] studentInfoTableWidths = new float[] { 115f, 235f };
            studentInfoTable.SetWidths(studentInfoTableWidths);            

            foreach (KeyValuePair<string, string> kvp in studentInformation)
            {
                PdfPCell titleCell = new PdfPCell(new Phrase(kvp.Key, font_title));                
                titleCell.Border = border;
                titleCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                titleCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                titleCell.PaddingTop = 10;

                PdfPCell valueCell = new PdfPCell(new Phrase(kvp.Value, font_value));
                valueCell.Border = border;
                valueCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                valueCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                valueCell.PaddingTop = 10;

                studentInfoTable.AddCell(titleCell);
                studentInfoTable.AddCell(valueCell);
            }


            // Cell to wrap the student info table in
            PdfPCell studentInfoCell = new PdfPCell(studentInfoTable);
            studentInfoCell.PaddingLeft = 10;
            studentInfoCell.Border = border;
            nameplateTable.AddCell(studentInfoCell);
            


            return nameplateTable;

        }

        private static PdfPTable reportNamePlate(List<ReportPeriod> reportPeriods) {
            
            // Find the earliest date from the given report periods
            // Find the latest date from the given report periods
            DateTime earliestDate = DateTime.MaxValue;            
            DateTime latestDate = DateTime.MinValue;
            foreach (ReportPeriod rp in reportPeriods)
            {
                if (rp.startDate < earliestDate)
                {
                    earliestDate = rp.startDate;
                }

                if (rp.endDate > latestDate)
                {
                    latestDate = rp.endDate;
                }
            }


            BaseColor backgroundColor = new BaseColor(0.99f, 0.99f, 0.99f, 0.2f);
            
            Font font_title = FontFactory.GetFont("Verdana", 11, Font.NORMAL, BaseColor.BLACK);
            Font font_date = FontFactory.GetFont("Verdana", 14, Font.NORMAL, BaseColor.BLACK);
            Font font_date_bold = FontFactory.GetFont("Verdana", 14, Font.BOLD, BaseColor.BLACK);

            PdfPTable reportNamePlateTable = new PdfPTable(1);
            reportNamePlateTable.HorizontalAlignment = 1;
            reportNamePlateTable.TotalWidth = 500f;
            reportNamePlateTable.LockedWidth = true;
            reportNamePlateTable.SpacingAfter = 50;            

            Paragraph reportName = new Paragraph();
            reportName.Add(new Phrase("Progress report for", font_title));
            reportName.Add(Chunk.NEWLINE);
            reportName.Add(new Phrase(earliestDate.ToLongDateString(), font_date_bold));
            reportName.Add(new Phrase(" to ", font_date));
            reportName.Add(new Phrase(latestDate.ToLongDateString(), font_date_bold));

            PdfPCell reportNameCell = new PdfPCell(reportName);
            reportNameCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            reportNameCell.Border = Rectangle.BOX;
            reportNameCell.BorderColor = new BaseColor(0.4f, 0.4f, 0.4f);
            reportNameCell.BackgroundColor = new BaseColor(0.9f, 0.9f, 0.9f);
            reportNamePlateTable.AddCell(reportNameCell);
            



            return reportNamePlateTable;
        }

        private static PdfPTable attendanceSummary(Student student)
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


            newCell = new PdfPCell(new Phrase("", font_body_bold)); // Class
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


                //newCell = new PdfPCell(new Phrase(LSKYCommon.findClassNameForThisBlock(allClasses, courseName), font_body));
                newCell = new PdfPCell(new Phrase("", font_body));
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

        private static PdfPTable courseAttendanceSummary(Student student, SchoolClass course)
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

        private static PdfPTable outcomeLegend(PdfContentByte content, OutcomeBarStyle barStyle)
        {
            Font font_legend = FontFactory.GetFont("Verdana", 7, BaseColor.BLACK);
            Font font_legend_bold = FontFactory.GetFont("Verdana", 7, Font.BOLD, BaseColor.BLACK);

            PdfPTable outcomeLegendTable = new PdfPTable(2);
            outcomeLegendTable.SpacingAfter = 25f;
            outcomeLegendTable.HorizontalAlignment = 1;
            outcomeLegendTable.TotalWidth = 250;
            outcomeLegendTable.LockedWidth = true;

            float[] widths = new float[] { 1.25f, 0.75f };
            outcomeLegendTable.SetWidths(widths);

            PdfPCell Cell_Title = new PdfPCell(new Phrase("Achievement Legend", font_large_bold));
            Cell_Title.Border = 0;
            Cell_Title.Colspan = 2;
            Cell_Title.Padding = 2;
            Cell_Title.PaddingBottom = 20;
            Cell_Title.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            Cell_Title.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            outcomeLegendTable.AddCell(Cell_Title);

            var legendItems = new[]{
                new { Value = "4", Title = "Master", Description = "Insightful understanding of the outcome" },
                new { Value = "3", Title = "Proficient", Description = "A well developed understanding of the outcome" },
                new { Value = "2", Title = "Approaching", Description = "A basic understanding of the outcome" },
                new { Value = "1", Title = "Beginning", Description = "A partial understanding of the outcome" },                
                new { Value = "IE", Title = "Insufficient Evidence", Description = "" },                
                new { Value = "NYM", Title = "Not Yet Meeting", Description = "" }
            };
            
            foreach (var item in legendItems)
            {
                PdfPCell Cell_LegendItemTitle = new PdfPCell(new Phrase(item.Title, font_body_bold));
                Cell_LegendItemTitle.Border = 0;
                Cell_LegendItemTitle.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                Cell_LegendItemTitle.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                outcomeLegendTable.AddCell(Cell_LegendItemTitle);

                PdfPCell Cell_Bar = new PdfPCell();
                //Cell_Bar = new PdfPCell(displayOutcomeBar(content, item.Value, barStyle));
                Cell_Bar.AddElement(displayOutcomeBar(content, item.Value, barStyle));
                Cell_Bar.Border = 0;            
                Cell_Bar.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                Cell_Bar.VerticalAlignment = PdfPCell.ALIGN_TOP;
                outcomeLegendTable.AddCell(Cell_Bar);

                

                PdfPCell Cell_LegendItemDescription = new PdfPCell(new Phrase(item.Description, font_body));
                Cell_LegendItemDescription.Padding = 0;
                Cell_LegendItemDescription.PaddingBottom = 20;
                Cell_LegendItemDescription.PaddingLeft = 10;
                Cell_LegendItemDescription.Border = 0;
                Cell_LegendItemDescription.Colspan = 2;
                Cell_LegendItemDescription.VerticalAlignment = PdfPCell.ALIGN_TOP;
                Cell_LegendItemDescription.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                outcomeLegendTable.AddCell(Cell_LegendItemDescription);


            }

            return outcomeLegendTable;
        }

        private static PdfPTable lifeSkillsLegend(PdfContentByte content, string grade)
        {            
            Font font_legend_title = FontFactory.GetFont("Verdana", 12, Font.BOLD, BaseColor.BLACK);
            Font font_legend = FontFactory.GetFont("Verdana", 9, BaseColor.BLACK);
            Font font_legend_bold = FontFactory.GetFont("Verdana", 9, Font.BOLD, BaseColor.BLACK);

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
                lifeSkills.Add("Self-Directed", "Stays on task, organized.");
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
            outcomeLegendTable.TotalWidth = 250;
            outcomeLegendTable.LockedWidth = true;

            float[] widths = new float[] { 1f, 5f };
            outcomeLegendTable.SetWidths(widths);
            
            PdfPCell titleCell = new PdfPCell(new Phrase("Characteristics of Successful Learning Behaviours", font_legend_title));
            titleCell.Border = 0;
            titleCell.Colspan = 2;
            titleCell.Padding = 2;
            titleCell.PaddingBottom = 10;
            titleCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            titleCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            outcomeLegendTable.AddCell(titleCell);

            foreach (KeyValuePair<string, string> legendItem in lifeSkills)
            {
                Paragraph lifeSkillDescription = new Paragraph();
                Phrase keyPhrase = new Phrase(legendItem.Key, font_legend_bold);
                lifeSkillDescription.Add(keyPhrase);
                lifeSkillDescription.Add(Chunk.NEWLINE);
                lifeSkillDescription.Add(new Phrase(legendItem.Value, font_legend));

                PdfPCell lifeskillCell = new PdfPCell(lifeSkillDescription);
                lifeskillCell.SetLeading(0, 1.25f);
                lifeskillCell.Border = 0;
                lifeskillCell.Colspan = 2;
                lifeskillCell.Padding = 5;
                lifeskillCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                lifeskillCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                outcomeLegendTable.AddCell(lifeskillCell);
            }
            
            return outcomeLegendTable;
        }

        private static PdfPTable legend(PdfContentByte content, string grade, OutcomeBarStyle barStyle)
        {
            PdfPTable legendTable = new PdfPTable(2);
            legendTable.HorizontalAlignment = 1;
            legendTable.TotalWidth = 550f;
            legendTable.LockedWidth = true;
            legendTable.SpacingAfter = 50;
            float[] widths = new float[] { 250f, 250f };
            legendTable.SetWidths(widths);


            PdfPCell lifeskills_column = new PdfPCell(lifeSkillsLegend(content, grade));
            lifeskills_column.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            lifeskills_column.Border = Rectangle.RIGHT_BORDER;
            lifeskills_column.BorderColor = new BaseColor(190, 190, 190);
            legendTable.AddCell(lifeskills_column);

            PdfPCell outcomes_column = new PdfPCell(outcomeLegend(content, barStyle));
            outcomes_column.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            outcomes_column.Border = 0;

            legendTable.AddCell(outcomes_column);

            return legendTable;
        }

        private static PdfPCell outcomeChunk(Outcome outcome, PdfContentByte content, OutcomeBarStyle barStyle = OutcomeBarStyle.Slider)
        {                      
            int ObjectivesTableDebuggingBorder = 0;

            // The container for this objective
            PdfPTable objectiveChunkTable = new PdfPTable(3);                        
           
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
                //objectiveDescriptionCell.AddElement(new Phrase(Chunk.NEWLINE));
                objectiveDescriptionCell.Border = ObjectivesTableDebuggingBorder;
                objectiveDescriptionCell.PaddingBottom = 7;
                objectiveDescriptionCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                objectiveDescriptionCell.VerticalAlignment = PdfPCell.ALIGN_TOP;

                // Set up the marks cell
                PdfPTable marksTable = new PdfPTable(2);
                //marksTable.SpacingBefore = 5;
                float[] marksTableWidths = new float[] { 1f, 2f };
                marksTable.SetWidths(marksTableWidths);

                foreach (OutcomeMark objectivemark in outcome.marks)
                {
                    PdfPCell markCell = new PdfPCell();                        

                    // Attempt to figure out of the mark is an objective or a percent, and display an outcome bar if necessary

                    // If there is a "cMark", display it (this is the "alpha" mark)
                    if (!string.IsNullOrEmpty(objectivemark.cMark))
                    {
                        PdfPCell Temp_MarkCell = new PdfPCell();
                        Temp_MarkCell.Border = ObjectivesTableDebuggingBorder;
                        Temp_MarkCell.Padding = 0;
                        Temp_MarkCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                        Temp_MarkCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                        Temp_MarkCell.AddElement((displayOutcomeBar(content, objectivemark.cMark, barStyle)));
                        markCell = Temp_MarkCell;
                    }
                    else
                    {
                        // Otherwise display the percentage

                        // If the numeric mark is between 0 and 4, assume that is it an outcome and display the bar
                        if (
                            (objectivemark.nMark <= 0) &&
                            (objectivemark.nMark >= 4)
                            )
                        {
                            PdfPCell Temp_MarkCell = new PdfPCell();
                            //Temp_MarkCell.Border = ObjectivesTableDebuggingBorder;
                            Temp_MarkCell.Border = Rectangle.BOX;
                            Temp_MarkCell.Padding = 1;
                            Temp_MarkCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                            Temp_MarkCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                            Temp_MarkCell.AddElement((displayOutcomeBar(content, objectivemark.cMark, barStyle)));
                            markCell = Temp_MarkCell;
                        }
                        else
                        {
                            PdfPCell Temp_ReportPeriodCell = new PdfPCell(new Phrase(Math.Round(objectivemark.nMark, 0) + "%", font_body_bold));
                            Temp_ReportPeriodCell.Border = ObjectivesTableDebuggingBorder;
                            Temp_ReportPeriodCell.Padding = 2;
                            Temp_ReportPeriodCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            Temp_ReportPeriodCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                            markCell = Temp_ReportPeriodCell;
                        }                            
                    }

                    // Display the report period
                    PdfPCell reportPeriodCell = new PdfPCell(new Phrase(objectivemark.reportPeriod.name + ":", font_very_small_bold));
                    reportPeriodCell.Padding = 2;
                    reportPeriodCell.PaddingLeft = 10;
                    reportPeriodCell.Border = ObjectivesTableDebuggingBorder;
                    reportPeriodCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    reportPeriodCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                        
                    marksTable.AddCell(reportPeriodCell);
                    marksTable.AddCell(markCell);
                }

                objectiveMarksCell.Border = ObjectivesTableDebuggingBorder;
                objectiveMarksCell.Padding = 0;
                objectiveMarksCell.AddElement(marksTable);

                // Build the final table to return
                objectiveChunkTable.KeepTogether = false;
                float[] objectivesTableWidths = new float[] { 0.10f, 3.65f, 2.75f };

                // Add a buffer between objectives
                PdfPCell bufferCell = new PdfPCell();
                bufferCell.Border = ObjectivesTableDebuggingBorder;
                objectiveChunkTable.AddCell(bufferCell);

                objectiveChunkTable.AddCell(objectiveDescriptionCell);
                objectiveChunkTable.AddCell(objectiveMarksCell);
                //objectiveChunkTable.AddCell(marksTable);

                objectiveChunkTable.SetWidths(objectivesTableWidths); 
            }                       
            

            // Encapsulate the table in a cell object and return it
            PdfPCell objectiveChunkTableCell = new PdfPCell(objectiveChunkTable);
            objectiveChunkTableCell.Colspan = 2;
            objectiveChunkTableCell.Border = ObjectivesTableDebuggingBorder;

            return objectiveChunkTableCell;
        }

        private static PdfPCell lifeSkillsChunk(List<Outcome> objectives, PdfContentByte content)
        {
            int lifeSkillsTableBorder = 0;

            PdfPCell bufferCell = new PdfPCell(new Phrase(""));
            bufferCell.Border = lifeSkillsTableBorder;
            
            // Condense the list of objectives to just the ones we care about
            List<Outcome> lifeSkillsObjectives = new List<Outcome>();
            foreach (Outcome objective in objectives)
            {
                lifeSkillsObjectives.Add(objective);                
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

                // Sort the list of life skills so that they are displayed alphabetically
                lifeSkillsNames.Sort();

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
                    PdfPCell columnHeadingCell = new PdfPCell(outcomeBar_NumberBar(content), true);
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

                                        markValueCell.AddElement(outcomeBar_LifeSkills(content, markToDisplay.ToString()));

                                    }
                                }
                            }
                        }

                        lifeSkillChunkTable.AddCell(markValueCell);

                    }
                }
                
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
                
        private static PdfPCell emptyCell()
        {
            PdfPCell newCell = new PdfPCell(new Paragraph(""));
            newCell.Border = 0;
            newCell.Padding = 5;
            newCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            return newCell;
        }

        private static PdfPTable classWithMarks(SchoolClass course, PdfContentByte content, bool anonymize = false, OutcomeBarStyle barStyle = OutcomeBarStyle.Slider)
        {          
            // Housekeeping first 

            // Default display modes
            ClassMarkDisplayStyle classMarkDisplayStyle = ClassMarkDisplayStyle.AsPercent;
            ClassMarksToDisplay classMarksToDisplay = ClassMarksToDisplay.None;
            
                        
            // Table that the whole "class" will be displayed in
            PdfPTable classTable = new PdfPTable(2);
            classTable.HorizontalAlignment = 1;
            classTable.TotalWidth = 500f;
            classTable.LockedWidth = true;
            classTable.SpacingAfter = 25;
            classTable.KeepTogether = true;
            float[] widths = new float[] { 2f, 1f };
            classTable.SetWidths(widths);


            // *********************************
            // * Course Title cell
            // *********************************
            PdfPTable classTitleTable = new PdfPTable(1);                        
            PdfPCell classTitleCell = new PdfPCell(new Phrase(course.name, font_large_bold));
            classTitleCell.Border = 0;
            classTitleCell.Padding = 0;
            classTitleCell.SetLeading(0, 1.25f);
            classTitleCell.PaddingLeft = 0;
            classTitleTable.AddCell(classTitleCell);

            // *********************************
            // * Teacher name cell
            // *********************************
            PdfPCell classTeacherCell = new PdfPCell(new Phrase(course.teacherName, font_small));
            if (anonymize)
            {
                classTeacherCell = new PdfPCell(new Phrase("Mr. Teacher", font_small));
            }            
            classTeacherCell.Border = 0;
            classTeacherCell.SetLeading(0, 1.25f);
            classTeacherCell.Padding = 0;
            classTeacherCell.PaddingLeft = 0;            
            classTitleTable.AddCell(classTeacherCell);


            PdfPCell classTitleTableContainer = new PdfPCell(classTitleTable);
            classTitleTableContainer.Border = 0;
            classTable.AddCell(classTitleTableContainer);


            // *********************************
            // * Course Mark (Adjusted Grade)
            // *********************************

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
                (course.hasOutcomes()) &&
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
                (!course.hasOutcomes())
                )
            {
                // Class is not outcome based, so display all of the marks

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

            // *********************************
            // *  Outcomes
            // *********************************

            // Find out how many outcomes actually have marks in them            
            if (course.OutcomeMarks.Count > 0)
            {
                // "Outcomes" title
                Paragraph outcomeParagraph = new Paragraph();
                outcomeParagraph.Add(new Phrase("Outcomes:", font_body_bold));
                PdfPCell outcomeCell = new PdfPCell(outcomeParagraph);
                outcomeCell.Border = 0;
                outcomeCell.Padding = 5;
                outcomeCell.Colspan = 2;
                classTable.AddCell(outcomeCell);
                                

                // Outcome entries
                // - Check for marks and category name before displaying the outcome
                foreach (Outcome objective in course.Outcomes)
                {
                    if (objective.marks.Count > 0)
                    {
                        classTable.AddCell(outcomeChunk(objective, content, barStyle));                        
                    }
                }
            }

            /*
            Paragraph lifeskillsParagraph = new Paragraph();
            lifeskillsParagraph.Add(new Phrase("Successful Learning Behaviours:", font_body_bold));
            PdfPCell lifeSkillsCell = new PdfPCell(lifeskillsParagraph);
            lifeSkillsCell.Border = 0;
            lifeSkillsCell.Padding = 5;
            lifeSkillsCell.Colspan = 2;
            classTable.AddCell(lifeSkillsCell);
             */

            // *********************************
            // * Life skills / Successful Learning Behaviors / SLBs
            // *********************************
            
            int lifeSkillsWithMarks = 0;
            foreach (Outcome o in course.LifeSkills)
            {
                lifeSkillsWithMarks += o.marks.Count;
            }

            if (lifeSkillsWithMarks > 0)
            {                    
                classTable.AddCell(lifeSkillsChunk(course.LifeSkills, content));
            }
            
            
            // *********************************
            // * Comments (from the Marks table)
            // *********************************
            
            // Check for comments
            bool hasAcademicComments = false;

            foreach (Mark m in course.Marks)
            {
                if (!string.IsNullOrEmpty(m.comment))
                {
                    hasAcademicComments = true;
                }
            }

            if (hasAcademicComments)
            {
                Paragraph commentTitleParagraph = new Paragraph();
                commentTitleParagraph.Add(new Phrase("Comments:", font_body_bold));

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
                            commentsParagraph.Add(new Phrase(LSKYCommon.getRandomLipsumString(), font_small));
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
                commentsCell.PaddingLeft = 15;
                commentsCell.Colspan = 2;
                classTable.AddCell(commentsCell);
            }

            return classTable;
        }

        public static MemoryStream GeneratePDF(List<Student> students, List<ReportPeriod> reportPeriods, bool anonymize = false, bool showPlaceholderPhotos = false, bool doubleSidedMode = true, OutcomeBarStyle barStyle = OutcomeBarStyle.Slider)
        {
            MemoryStream memstream = new MemoryStream();
            Document ReportCard = new Document(PageSize.LETTER);
            PdfWriter writer = PdfWriter.GetInstance(ReportCard, memstream);

            ReportCard.Open();
            PdfContentByte content = writer.DirectContent;

            PdfPageEventHandler PageEventHandler = new PdfPageEventHandler();
            writer.PageEvent = PageEventHandler;
            PageEventHandler.DoubleSidedMode = doubleSidedMode;
            PageEventHandler.ShowOnFirstPage = false;

            // Add a watermark to the first page (if applicable)
            /*            
            if (showWatermark)
            {
                int imgWidth = 500;
                int imgHeight = 500;
                iTextSharp.text.Image watermark = iTextSharp.text.Image.GetInstance(@"https://sldata.lskysd.ca/SLReports/lsky_logo_watermark.png");
                watermark.ScaleToFit(imgWidth, imgHeight);
                watermark.Alignment = iTextSharp.text.Image.UNDERLYING;
                watermark.SetAbsolutePosition(((ReportCard.Right + ReportCard.RightMargin) / 2) - (watermark.ScaledWidth / 2), (ReportCard.Top / 2) - (watermark.ScaledHeight / 2));
                ReportCard.Add(watermark);
            }
            */
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
                
                // Determine bar style based on grade number
                OutcomeBarStyle determinedBarStyle = barStyle;
                int gradeParsed = 0;
                if (int.TryParse(student.getGradeFormatted(), out gradeParsed))
                {
                    if (gradeParsed >= 10)
                    {
                        determinedBarStyle = OutcomeBarStyle.JustNumber;
                    }
                    else
                    {
                        determinedBarStyle = OutcomeBarStyle.Slider;
                    }
                }

                // Cover page
                ReportCard.Add(PDFReportCardParts.schoolNamePlate(student.school));
                ReportCard.Add(PDFReportCardParts.namePlateTable(student, anonymize, showPlaceholderPhotos));
                ReportCard.Add(PDFReportCardParts.reportNamePlate(reportPeriods));
                ReportCard.Add(PDFReportCardParts.legend(content, student.getGrade(), determinedBarStyle));
                //ReportCard.Add(PDFReportCardParts.lifeSkillsLegend(content, student.getGrade()));
                //ReportCard.Add(PDFReportCardParts.outcomeLegend(content, barStyle));
                ReportCard.NewPage();

                // Start course list
                ReportCard.Add(new Phrase(string.Empty));
                foreach (Term term in student.track.terms)
                {
                    foreach (SchoolClass course in term.Courses)
                    {
                        if ((course.Marks.Count > 0) || (course.OutcomeMarks.Count > 0) || (course.LifeSkillMarks.Count > 0))
                        {
                            ReportCard.Add(PDFReportCardParts.classWithMarks(course, content, anonymize, determinedBarStyle));
                            if (!student.track.daily)
                            {
                                //ReportCard.Add(PDFReportCardParts.courseAttendanceSummary(student, course));
                            }
                        }
                    }
                }


                // Attendance summary
                ReportCard.Add(PDFReportCardParts.attendanceSummary(student));

                // Report period comments (if applicable)
                // TODO

                PageEventHandler.ResetPageNumbers(ReportCard);
            }

            ReportCard.Close();
            return memstream;
        }
        
    }
}