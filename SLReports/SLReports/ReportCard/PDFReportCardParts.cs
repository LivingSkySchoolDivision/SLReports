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

        #region Globals (Standard colors and fonts)

        // LSKY Logo
        // This is a static variable to drastically reduce the size of the PDF produced. The way PDFs work is, if a new image object is created each time, it is treated as a seperate image. 
        // If an image is created once and referenced each time, the actual image is only included in the file once. This can save upwards of 50-100mb, depending on how many times the image is used.
        private static iTextSharp.text.Image lskyLogo = iTextSharp.text.Image.GetInstance(@"https://sldata.lskysd.ca/SLReports/lsky_logo_text.png");

        // Database to use (so we can switch it easily for testing)
        //public static String ReportCardDatabase = LSKYCommon.dbConnectionString_SchoolLogic;
        public static String ReportCardDatabase = LSKYCommon.dbConnectionString_SchoolLogic;
        
        // Common Fonts
        
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
        private static Font font_very_small = FontFactory.GetFont("Verdana", 6, BaseColor.BLACK);
        private static Font font_very_small_bold = FontFactory.GetFont("Verdana", 6, Font.BOLD, BaseColor.BLACK);       
              
        // Spacing between different sections of the page
        const int standardElementSpacing = 8;       
        
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
        private static iTextSharp.text.Image displayOutcomeBar(PdfContentByte content, String value, OutcomeBarStyle style, bool shortStrings = false)
        {
            if (style == OutcomeBarStyle.JustNumber)
            {
                return outcomeBar_JustNumber(content, value, shortStrings);
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

            BaseColor borderColor = new BaseColor(0, 0, 0);
            
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

                    //Redraw a red background
                    canvas.Rectangle(0, 0, width + (CanvasPaddingX * 2), height + (CanvasPaddingY * 2));
                    canvas.SetColorFill(new BaseColor(1f,0,0,0.5f));
                    canvas.Fill();

                    BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    canvas.SetColorFill(new BaseColor(1f, 1f, 1f, 1f));
                    canvas.BeginText();
                    canvas.SetFontAndSize(bf, 8);
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
            //canvas.RoundRectangle(CanvasPaddingX, CanvasPaddingY, width, height, rectancleCurveRadius);
            canvas.Rectangle(CanvasPaddingX, CanvasPaddingY, width, height);
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

        public static iTextSharp.text.Image outcomeBar_JustNumber(PdfContentByte content, String value, bool shortStrings = false, bool fillBackground = false)
        {
            int width = 125;
            int height = 25; // This should be an "odd" number, so that things centered vertically are actually centered
            int barWidth = 30;
            int CanvasPaddingX = 1;
            int CanvasPaddingY = 1;

            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            PdfTemplate canvas = content.CreateTemplate(width + 2, height + 2);
            canvas.SetFontAndSize(bf, 14);
            string barContent = value;
            if (!shortStrings)
            {
                if (value.ToLower() == "ie")
                {
                    width = 125;
                    canvas = content.CreateTemplate(width + 2, height + 2);
                    canvas.SetFontAndSize(bf, 10);
                    barContent = "Insufficient Evidence";
                    width = 125;
                    barWidth = width;
                }

                if (value.ToLower() == "nym")
                {
                    width = 125;
                    canvas = content.CreateTemplate(width + 2, height + 2);
                    canvas.SetFontAndSize(bf, 10);
                    barContent = "Not Yet Meeting";
                    barWidth = width;
                }
            }

            int rectancleCurveRadius = 2;



            

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
        /// Parses the comment fields, replacing variable placeholders with data 
        /// </summary>
        /// <param name="comments">Comments from the database</param>
        /// <returns></returns>
        private static string parseCommentsWithVariables(Student student, string comments)
        {
            StringBuilder returnMe = new StringBuilder();

            returnMe.Append(comments);
            // I will add more of these as I see them - so far this is the only one that has come up
            returnMe.Replace("<|FirstNameStudent|>", student.getFirstName());

            return returnMe.ToString();
        }

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
        
        /// <summary>
        /// The "school" section of the page, with school names, logos, and address
        /// </summary>
        /// <param name="school"></param>
        /// <returns></returns>
        private static PdfPTable schoolNamePlate(School school)
        {
            // Fonts for this section            
            Font font_schoolName = FontFactory.GetFont("Verdana", 12, BaseColor.BLACK);
            Font font_schoolInfo = FontFactory.GetFont("Verdana", 8, Font.NORMAL, BaseColor.BLACK);

            int cellpadding = 3;
            int border = Rectangle.NO_BORDER;

            PdfPTable schoolNamePlateTable = new PdfPTable(3);
            schoolNamePlateTable.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            schoolNamePlateTable.TotalWidth = 500f;
            schoolNamePlateTable.LockedWidth = true;
            schoolNamePlateTable.SpacingAfter = standardElementSpacing;
            float[] widths = new float[] { 50f, 400f, 50f };
            schoolNamePlateTable.SetWidths(widths);

            if (!string.IsNullOrEmpty(school.logoPath))
            {
                try
                {
                    iTextSharp.text.Image schoolLogo = iTextSharp.text.Image.GetInstance(@"https://sis.lskysd.ca/SchoolLogic/Images/" + school.logoPath);
                    schoolLogo.ScaleAbsolute(35, 35);
                    PdfPCell schoolLogoCell = new PdfPCell(schoolLogo);
                    schoolLogoCell.Border = 0;
                    schoolLogoCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    schoolNamePlateTable.AddCell(schoolLogoCell);
                }
                catch
                {
                    PdfPCell schoolLogoCell = new PdfPCell();
                    schoolLogoCell.Border = 0;
                    schoolLogoCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    schoolNamePlateTable.AddCell(schoolLogoCell);
                }
            }
            else
            {
                PdfPCell schoolLogoCell = new PdfPCell();
                schoolLogoCell.Border = 0;
                schoolLogoCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                schoolNamePlateTable.AddCell(schoolLogoCell);
            }            
            
            Paragraph SchoolNameParagraph = new Paragraph();
            SchoolNameParagraph.Add(new Phrase(school.getName(), font_schoolName));
            SchoolNameParagraph.Add(Chunk.NEWLINE);
            SchoolNameParagraph.Add(new Phrase(school.address, font_schoolInfo));
            PdfPCell SchoolNameCell = new PdfPCell(SchoolNameParagraph);
            SchoolNameCell.VerticalAlignment = 0;
            SchoolNameCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            SchoolNameCell.Padding = cellpadding;
            SchoolNameCell.Border = border;
            
            schoolNamePlateTable.AddCell(SchoolNameCell);

            lskyLogo.ScalePercent(3f);
            PdfPCell divisionLogoCell = new PdfPCell(lskyLogo);
            divisionLogoCell.Border = 0;
            divisionLogoCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            schoolNamePlateTable.AddCell(divisionLogoCell);

            return schoolNamePlateTable;

        }

        /// <summary>
        /// The student's name, photo, and other general information about the student
        /// </summary>
        /// <param name="student"></param>
        /// <param name="anonymize"></param>
        /// <param name="showPhotos"></param>
        /// <returns></returns>
        private static PdfPTable namePlateTable(Student student, bool anonymize, bool showPhotos)
        {
            Font font_StudentName = FontFactory.GetFont("Verdana", 22, Font.BOLD, BaseColor.BLACK);
            Font font_title = FontFactory.GetFont("Verdana", 10, Font.BOLD, BaseColor.BLACK);
            Font font_value = FontFactory.GetFont("Verdana", 10, Font.NORMAL, BaseColor.BLACK);

            int border = 0;

            if ((student == null) || (anonymize))
            {
                student = new Student("John", "Smith", "John", "Smith", "J", "Demo", "000000000", "Demo School", "00000", student.getGradeFormatted(), "Saskatchewan", "North Battleford", "Fake St", "123", "", "H0H0H0", "3065551234", "Male", "Instatus", "Instatuscode", "Homeroom Teacher", DateTime.Now.AddDays(-1), DateTime.Now, "000", "Band name", "Reserve Name", "House #", "000000000", false, 000, false, "user.name", 20, 0, "", "English", "PARKING", "LOCKER", "COMBINATION");
            }

            PdfPTable nameplateTable = new PdfPTable(2);
            nameplateTable.HorizontalAlignment = 1;
            nameplateTable.TotalWidth = 500f;
            nameplateTable.LockedWidth = true;
            nameplateTable.SpacingAfter = standardElementSpacing;
            float[] widths = new float[] { 350f, 150f };
            nameplateTable.SetWidths(widths);

            PdfPCell studentNamecell = new PdfPCell(new Phrase(student.getDisplayName(), font_StudentName));
            studentNamecell.Border = border;
            nameplateTable.AddCell(studentNamecell);


            PdfPCell photoCell = new PdfPCell(new Phrase("", font_large_italic));
            if (showPhotos)
            {
                if (student.hasPhoto() || anonymize)
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


            PdfPTable studentInfoTable = new PdfPTable(2);
            float[] studentInfoTableWidths = new float[] { 115f, 235f };
            studentInfoTable.SetWidths(studentInfoTableWidths);

            foreach (KeyValuePair<string, string> kvp in studentInformation)
            {
                PdfPCell titleCell = new PdfPCell(new Phrase(kvp.Key, font_title));
                titleCell.Border = border;
                titleCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                titleCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;

                PdfPCell valueCell = new PdfPCell(new Phrase(kvp.Value, font_value));
                valueCell.Border = border;
                valueCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                valueCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;

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

        private static PdfPTable namePlateTable_Old(Student student, bool anonymize, bool showPhotos)
        {
            Font font_StudentName = FontFactory.GetFont("Verdana", 22, Font.BOLD, BaseColor.BLACK);
            Font font_title = FontFactory.GetFont("Verdana", 10, Font.BOLD, BaseColor.BLACK);
            Font font_value = FontFactory.GetFont("Verdana", 10, Font.NORMAL, BaseColor.BLACK);

            int border = 0;

            if ((student == null) || (anonymize))
            {
                student = new Student("John", "Smith", "John", "Smith", "J", "Demo", "000000000", "Demo School", "00000", student.getGradeFormatted(), "Saskatchewan", "North Battleford", "Fake St", "123", "", "H0H0H0", "3065551234", "Male", "Instatus", "Instatuscode", "Homeroom Teacher", DateTime.Now.AddDays(-1), DateTime.Now, "000", "Band name", "Reserve Name", "House #", "000000000", false, 000, false, "user.name", 20, 0, "", "English", "PARKING", "LOCKER", "COMBINATION");
            }

            PdfPTable nameplateTable = new PdfPTable(2);
            nameplateTable.HorizontalAlignment = 1;
            nameplateTable.TotalWidth = 500f;
            nameplateTable.LockedWidth = true;
            nameplateTable.SpacingAfter = standardElementSpacing;
            float[] widths = new float[] { 350f, 150f };
            nameplateTable.SetWidths(widths);

            PdfPCell studentNamecell = new PdfPCell(new Phrase(student.getDisplayName(), font_StudentName));
            studentNamecell.Border = border;
            nameplateTable.AddCell(studentNamecell);


            PdfPCell photoCell = new PdfPCell(new Phrase("", font_large_italic));
            if (showPhotos)
            {
                if (student.hasPhoto() || anonymize)
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

        /// <summary>
        /// Title of the document - this section displays that this is a progress report, and what date ranges are covered in the report
        /// </summary>
        /// <param name="reportPeriods"></param>
        /// <returns></returns>
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
            
            Font font_title = FontFactory.GetFont("Verdana", 8, Font.NORMAL, BaseColor.BLACK);
            Font font_date = FontFactory.GetFont("Verdana", 12, Font.NORMAL, BaseColor.BLACK);
            Font font_date_bold = FontFactory.GetFont("Verdana", 12, Font.BOLD, BaseColor.BLACK);

            PdfPTable reportNamePlateTable = new PdfPTable(1);
            reportNamePlateTable.HorizontalAlignment = 1;
            reportNamePlateTable.TotalWidth = 500f;
            reportNamePlateTable.LockedWidth = true;
            reportNamePlateTable.SpacingAfter = standardElementSpacing + 5;            

            Paragraph reportName = new Paragraph();
            reportName.Add(new Phrase("Progress report", font_title));
            reportName.Add(Chunk.NEWLINE);
            //reportName.Add(new Phrase(earliestDate.ToLongDateString(), font_date_bold));            
            //reportName.Add(new Phrase(" to ", font_date));
            //reportName.Add(new Phrase("Report period ending ", font_date));
            reportName.Add(new Phrase(latestDate.ToLongDateString(), font_date_bold));

            PdfPCell reportNameCell = new PdfPCell(reportName);
            reportNameCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            reportNameCell.Border = Rectangle.BOX;
            reportNameCell.BorderColor = new BaseColor(0.6f, 0.6f, 0.6f);
            reportNameCell.BackgroundColor = new BaseColor(0.97f, 0.97f, 0.97f);
            reportNameCell.Padding = 5;
            reportNamePlateTable.AddCell(reportNameCell);

            return reportNamePlateTable;
        }
           
        /// <summary>
        /// Formats a number for use in the absense summary at the end of the report card. Mostly because I want to hide zeros for readability
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static string absenseNumberFormatter(int number)
        {
            if (number > 0)
            {
                return number.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Lists all absence entries, organized by class name
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        private static PdfPTable attendanceSummaryByClassName(Student student, bool showAllClasses)
        {
            // Sort attendance entries by class name
            SortedDictionary<string, List<Absence>> absencesByClass = new SortedDictionary<string, List<Absence>>();

            // Get a list of all enrolled classes and add them to the list
            if (showAllClasses)
            {
                foreach (Term term in student.track.terms)
                {
                    foreach (SchoolClass course in term.Courses)
                    {
                        if (!absencesByClass.ContainsKey(course.name))
                        {
                            absencesByClass.Add(course.name, new List<Absence>());
                        }
                    }
                }
            }
            else
            {
                foreach (Absence abs in student.absences)
                {
                    if (!absencesByClass.ContainsKey(abs.getCourseName()))
                    {
                        absencesByClass.Add(abs.getCourseName(), new List<Absence>());
                    }
                }
            }

            foreach (Absence abs in student.absences)
            {
                if (!absencesByClass.ContainsKey(abs.getCourseName()))
                {
                    absencesByClass.Add(abs.getCourseName(), new List<Absence>());
                }
                absencesByClass[abs.getCourseName()].Add(abs);
            }

            PdfPTable attendanceTable = new PdfPTable(4);
            attendanceTable.HorizontalAlignment = 1;
            attendanceTable.TotalWidth = 500;
            attendanceTable.LockedWidth = true;
            attendanceTable.SpacingAfter = standardElementSpacing;
            attendanceTable.KeepTogether = true;

            float[] tableWidths = new float[] { 6f, 2f, 2f, 2f };
            attendanceTable.SetWidths(tableWidths);

            PdfPCell newCell = null;

            // Total stats
            int totalExcused = 0;
            int totalUnexcused = 0;
            int totalLates = 0;
            int totalLateMinutes = 0;

            // Title (full colspan)
            newCell = new PdfPCell(new Paragraph("Absence Summary", font_large_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            newCell.Border = Rectangle.NO_BORDER;
            newCell.Colspan = 4;
            newCell.PaddingBottom = 5;
            attendanceTable.AddCell(newCell);


            newCell = new PdfPCell(new Phrase("Class or Block", font_body_bold));
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

            // By class values
            foreach (KeyValuePair<string, List<Absence>> absByclass in absencesByClass)
            {
                BaseColor classBorderColor = new BaseColor(0.8f, 0.8f, 0.8f);
                int classBorders = Rectangle.BOTTOM_BORDER;

                int numLates = 0;
                int numAbsExc = 0;
                int numAbsUnexc = 0;
                int numMinutesLate = 0;

                foreach (Absence abs in absByclass.Value)
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

                newCell = new PdfPCell(new Phrase(absByclass.Key, font_body));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.SetLeading(0, 1.25f);
                newCell.Border = classBorders;
                newCell.BorderColor = classBorderColor;
                attendanceTable.AddCell(newCell);

                StringBuilder lateDisplay = new StringBuilder();
                if (numLates > 0)
                {
                    lateDisplay.Append(numLates);
                }

                if (numMinutesLate > 0)
                {
                    lateDisplay.Append(" (" + numMinutesLate + " minutes)");
                }

                newCell = new PdfPCell(new Phrase(lateDisplay.ToString(), font_body));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = classBorders;
                newCell.BorderColor = classBorderColor;
                attendanceTable.AddCell(newCell);

                newCell = new PdfPCell(new Phrase(absenseNumberFormatter(numAbsExc), font_body));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = classBorders;
                newCell.BorderColor = classBorderColor;
                attendanceTable.AddCell(newCell);

                newCell = new PdfPCell(new Phrase(absenseNumberFormatter(numAbsUnexc), font_body));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = classBorders;
                newCell.BorderColor = classBorderColor;
                attendanceTable.AddCell(newCell);
            }

            // Display totals
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

        /// <summary>
        /// Lists all absence entries organized by block
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        private static PdfPTable attendanceSummaryByPeriod(Student student)
        {
            // Sort attendance entries by class name
            SortedDictionary<int, List<Absence>> absencesByClass = new SortedDictionary<int, List<Absence>>();

            // Get a list of all enrolled classes and add them to the list
            foreach (Absence abs in student.absences)
            {
                if (!absencesByClass.ContainsKey(abs.getBlock()))
                {
                    absencesByClass.Add(abs.getBlock(), new List<Absence>());
                }
                absencesByClass[abs.getBlock()].Add(abs);
            }

            PdfPTable attendanceTable = new PdfPTable(4);
            attendanceTable.HorizontalAlignment = 1;
            attendanceTable.TotalWidth = 500;
            attendanceTable.LockedWidth = true;
            attendanceTable.SpacingAfter = standardElementSpacing;
            attendanceTable.KeepTogether = true;

            float[] tableWidths = new float[] { 6f, 2f, 2f, 2f };
            attendanceTable.SetWidths(tableWidths);

            PdfPCell newCell = null;

            // Total stats
            int totalExcused = 0;
            int totalUnexcused = 0;
            int totalLates = 0;
            int totalLateMinutes = 0;

            // Title (full colspan)
            //newCell = new PdfPCell(new Paragraph("Absence Summary (By Period)", font_large_bold));
            //newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            //newCell.VerticalAlignment = PdfPCell.ALIGN_BOTTOM;
            //newCell.Border = Rectangle.NO_BORDER;
            //newCell.Colspan = 4;
            //newCell.PaddingBottom = 5;
            //attendanceTable.AddCell(newCell);


            newCell = new PdfPCell(new Phrase("Absence Summary by Period", font_body_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_BOTTOM;
            newCell.Border = Rectangle.BOTTOM_BORDER;
            newCell.PaddingBottom = 5;
            newCell.BorderWidth = 1;
            attendanceTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Lates", font_body_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            newCell.VerticalAlignment = PdfPCell.ALIGN_BOTTOM;
            newCell.Border = Rectangle.BOTTOM_BORDER;
            newCell.PaddingBottom = 5;
            newCell.BorderWidth = 1;
            attendanceTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Excused Absences", font_body_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_BOTTOM;
            newCell.Border = Rectangle.BOTTOM_BORDER;
            newCell.PaddingBottom = 5;
            newCell.BorderWidth = 1;
            attendanceTable.AddCell(newCell);

            newCell = new PdfPCell(new Phrase("Unexcused Absences", font_body_bold));
            newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            newCell.VerticalAlignment = PdfPCell.ALIGN_BOTTOM;
            newCell.Border = Rectangle.BOTTOM_BORDER;
            newCell.PaddingBottom = 5;
            newCell.BorderWidth = 1;
            attendanceTable.AddCell(newCell);

            // By class values
            foreach (KeyValuePair<int, List<Absence>> absByclass in absencesByClass)
            {
                BaseColor classBorderColor = new BaseColor(0.8f, 0.8f, 0.8f);
                int classBorders = Rectangle.BOTTOM_BORDER;

                int numLates = 0;
                int numAbsExc = 0;
                int numAbsUnexc = 0;
                int numMinutesLate = 0;

                string periodName = absByclass.Key.ToString();

                foreach (Absence abs in absByclass.Value)
                {
                    // Figure out what the periods might be called
                    if (abs.attendanceBlock != null)
                    {
                        periodName = abs.attendanceBlock.name;
                    }

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

                newCell = new PdfPCell(new Phrase(periodName, font_body));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.SetLeading(0, 1.25f);
                newCell.Border = classBorders;
                newCell.BorderColor = classBorderColor;
                attendanceTable.AddCell(newCell);

                StringBuilder lateDisplay = new StringBuilder();
                if (numLates > 0)
                {
                    lateDisplay.Append(numLates);
                }

                if (numMinutesLate > 0)
                {
                    lateDisplay.Append(" (" + numMinutesLate + " minutes)");
                }

                newCell = new PdfPCell(new Phrase(lateDisplay.ToString(), font_body));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = classBorders;
                newCell.BorderColor = classBorderColor;
                attendanceTable.AddCell(newCell);

                newCell = new PdfPCell(new Phrase(absenseNumberFormatter(numAbsExc), font_body));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = classBorders;
                newCell.BorderColor = classBorderColor;
                attendanceTable.AddCell(newCell);

                newCell = new PdfPCell(new Phrase(absenseNumberFormatter(numAbsUnexc), font_body));
                newCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                newCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                newCell.Border = classBorders;
                newCell.BorderColor = classBorderColor;
                attendanceTable.AddCell(newCell);
            }

            // Display totals
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

        /// <summary>
        /// Lists report period comments - also called "report CARD comments" within school logic
        /// </summary>
        /// <param name="comments"></param>
        /// <returns></returns>
        private static PdfPTable reportPeriodCommentsSection(List<ReportPeriodComment> comments, bool anon)
        {
            PdfPTable commentsTable = new PdfPTable(1);
            commentsTable.HorizontalAlignment = 1;
            commentsTable.TotalWidth = 500;
            commentsTable.LockedWidth = true;
            commentsTable.SpacingAfter = standardElementSpacing;
            commentsTable.KeepTogether = true;
            
            // Title
            PdfPCell titleCell = new PdfPCell(new Paragraph("Overall Comments", font_large_bold));
            titleCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            titleCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            titleCell.Border = Rectangle.NO_BORDER;
            titleCell.Colspan = 5;
            titleCell.PaddingBottom = 2;
            commentsTable.AddCell(titleCell);

            // Comments
            foreach (ReportPeriodComment comment in comments)
            {
                Paragraph commentParagraph = new Paragraph();
                if (comments.Count > 1)
                {
                    commentParagraph.Add(new Phrase(comment.reportPeriodName + ": ", font_small_bold));
                }

                if (anon)
                {
                    commentParagraph.Add(new Phrase(LSKYCommon.getRandomLipsumString(), font_small));
                }
                else
                {
                    commentParagraph.Add(new Phrase(comment.comment, font_small));
                }

                PdfPCell commentCell = new PdfPCell(commentParagraph);
                commentCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                commentCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                commentCell.SetLeading(0, 1.25f);
                commentCell.Border = 0;
                commentCell.PaddingBottom = 5;
                commentCell.PaddingLeft = 5;
                commentsTable.AddCell(commentCell);
            }


            return commentsTable;
        }
                
        /// <summary>
        /// This is a brief absence summary attached to the bottom of each "class" section of the page, displaying absences for just that class
        /// </summary>
        /// <param name="student"></param>
        /// <param name="course"></param>
        /// <returns></returns>
        private static PdfPTable courseAttendanceSummary(Student student, SchoolClass course)
        {
            // Fonts for this section
            Font font_attendance = FontFactory.GetFont("Verdana", 8, BaseColor.BLACK);
            Font font_attendance_bold = FontFactory.GetFont("Verdana", 8, Font.BOLD, BaseColor.BLACK);

            int titleBorders = 0;
            int valueBorders = Rectangle.BOX;
            BaseColor borderColor = new BaseColor(0.6f, 0.6f, 0.6f);

            PdfPTable attendanceTable = new PdfPTable(6);
            attendanceTable.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            attendanceTable.TotalWidth = 500;
            attendanceTable.LockedWidth = true;
            attendanceTable.SpacingAfter = standardElementSpacing;
            attendanceTable.KeepTogether = true;
            attendanceTable.SetWidths(new float[] {0.5f, 1f, 2f, 1f, 2f, 1f});
                        
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

            // Lates
            PdfPCell lateTitle = new PdfPCell(new Phrase("Lates: ", font_attendance_bold));
            lateTitle.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            lateTitle.VerticalAlignment = PdfPCell.ALIGN_BOTTOM;
            lateTitle.Border = titleBorders;
            lateTitle.BorderColor = borderColor;
            lateTitle.PaddingBottom = 5;
            attendanceTable.AddCell(lateTitle);

            StringBuilder lateDisplay2 = new StringBuilder();
            lateDisplay2.Append(totalLates);

            if (totalLateMinutes > 0)
            {
                lateDisplay2.Append(" (" + totalLateMinutes + " minutes)");
            }

            PdfPCell lateValue = new PdfPCell(new Phrase(lateDisplay2.ToString(), font_attendance));
            lateValue.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            lateValue.VerticalAlignment = PdfPCell.ALIGN_TOP;
            lateValue.Border = valueBorders;
            lateValue.BorderColor = borderColor;
            lateValue.PaddingBottom = 5;
            attendanceTable.AddCell(lateValue);


            // Excused absences
            PdfPCell excusedTitleCell = new PdfPCell(new Phrase("Excused Absences: ", font_attendance_bold));
            excusedTitleCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            excusedTitleCell.VerticalAlignment = PdfPCell.ALIGN_BOTTOM;
            excusedTitleCell.Border = titleBorders;
            excusedTitleCell.BorderColor = borderColor;
            excusedTitleCell.PaddingBottom = 5;
            attendanceTable.AddCell(excusedTitleCell);

            PdfPCell excusedValueCell = new PdfPCell(new Phrase(totalExcused.ToString(), font_attendance));
            excusedValueCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            excusedValueCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            excusedValueCell.Border = valueBorders;
            excusedValueCell.BorderColor = borderColor;
            excusedValueCell.PaddingBottom = 5;
            attendanceTable.AddCell(excusedValueCell);

            // Unexcused absences
            PdfPCell unexcusedTitlecell = new PdfPCell(new Phrase("Unexcused Absences: ", font_attendance_bold));
            unexcusedTitlecell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            unexcusedTitlecell.VerticalAlignment = PdfPCell.ALIGN_BOTTOM;
            unexcusedTitlecell.Border = titleBorders;
            unexcusedTitlecell.BorderColor = borderColor;
            unexcusedTitlecell.PaddingBottom = 5;
            attendanceTable.AddCell(unexcusedTitlecell);

            PdfPCell unexecusedValueCell = new PdfPCell(new Phrase(totalUnexcused.ToString(), font_attendance));
            unexecusedValueCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            unexecusedValueCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            unexecusedValueCell.Border = valueBorders;
            unexecusedValueCell.BorderColor = borderColor;
            unexecusedValueCell.PaddingBottom = 5;
            attendanceTable.AddCell(unexecusedValueCell);            

            return attendanceTable;
        }

        /// <summary>
        /// An explaination of what outcome values mean
        /// </summary>
        /// <param name="content"></param>
        /// <param name="barStyle"></param>
        /// <returns></returns>
        private static PdfPTable outcomeLegend(PdfContentByte content, OutcomeBarStyle barStyle)
        {
            Font font_legend = FontFactory.GetFont("Verdana", 8, BaseColor.BLACK);
            Font font_legend_bold = FontFactory.GetFont("Verdana", 8, Font.BOLD, BaseColor.BLACK);
            Font font_legend_title = FontFactory.GetFont("Verdana", 9, Font.BOLD, BaseColor.BLACK);

            PdfPTable outcomeLegendTable = new PdfPTable(2);
            outcomeLegendTable.HorizontalAlignment = 1;
            outcomeLegendTable.TotalWidth = 250;
            outcomeLegendTable.LockedWidth = true;

            float[] widths = new float[] { 1.35f, 0.65f };
            outcomeLegendTable.SetWidths(widths);

            PdfPCell Cell_Title = new PdfPCell(new Phrase("Achievement Legend", font_legend_title));
            Cell_Title.Border = 0;
            Cell_Title.Colspan = 2;
            Cell_Title.Padding = 2;
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
                Paragraph legendItemText = new Paragraph();
                legendItemText.Add(new Phrase(item.Value + ": " + item.Title, font_legend_bold));
                legendItemText.Add("\n");
                legendItemText.Add(new Phrase(item.Description, font_legend));
                
                PdfPCell Cell_LegendItemTitle = new PdfPCell(legendItemText);
                Cell_LegendItemTitle.Border = 0;
                Cell_LegendItemTitle.SetLeading(0, 1.25f);
                Cell_LegendItemTitle.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                Cell_LegendItemTitle.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                outcomeLegendTable.AddCell(Cell_LegendItemTitle);

                //PdfPCell Cell_Bar = new PdfPCell();
                PdfPCell Cell_Bar = new PdfPCell();
                Cell_Bar.AddElement(displayOutcomeBar(content, item.Value, barStyle));
                Cell_Bar.Border = 0;            
                Cell_Bar.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                Cell_Bar.VerticalAlignment = PdfPCell.ALIGN_TOP;
                outcomeLegendTable.AddCell(Cell_Bar);                
            }

            // Add some empty cells because the last cell gets stretched and screws up formatting
            PdfPCell blankCell = new PdfPCell();
            blankCell.Border = Rectangle.NO_BORDER;
            outcomeLegendTable.AddCell(blankCell);
            outcomeLegendTable.AddCell(blankCell);

            return outcomeLegendTable;
        }

        /// <summary>
        /// An explaination of what life skills values mean
        /// </summary>
        /// <param name="content"></param>
        /// <param name="grade"></param>
        /// <param name="barStyle"></param>
        /// <returns></returns>
        private static PdfPTable lifeSkillsLegend(PdfContentByte content, string grade, OutcomeBarStyle barStyle = OutcomeBarStyle.LifeSkills)
        {            
            Font font_legend_title = FontFactory.GetFont("Verdana", 9, Font.BOLD, BaseColor.BLACK);
            Font font_legend = FontFactory.GetFont("Verdana", 8, BaseColor.BLACK);
            Font font_legend_bold = FontFactory.GetFont("Verdana", 8, Font.BOLD, BaseColor.BLACK);

            Dictionary<int, string> potentialMarks = new Dictionary<int, string>();

            int gradeLevel = 0;
            if (!int.TryParse(grade, out gradeLevel)) 
            {
                gradeLevel = 0;
            }

            potentialMarks.Add(4, "Consistently demonstrates");
            potentialMarks.Add(3, "Usually demonstrates");
            potentialMarks.Add(2, "Occasionally demonstrates");
            potentialMarks.Add(1, "Beginning to demonstrate");

            SortedDictionary<string, string> lifeSkills = new SortedDictionary<string, string>();
            if (gradeLevel >= 10)
            {
                lifeSkills.Add("Engagement",    "Invested in learning, diligent in completing work.");
                lifeSkills.Add("Citizenship",   "Respectful, responsible, academically honest.");
                lifeSkills.Add("Collaborative", "Willing to work with all classmates, cooperative, willing to resolve conflict.");
                lifeSkills.Add("Leadership",    "Independent, takes initiative.");
                lifeSkills.Add("Self-Directed", "Arrives on time, prepared to advance learning, strong work habits.");
            }
            else if (gradeLevel >= 6)
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

            PdfPTable outcomeLegendTable = new PdfPTable(4);
            outcomeLegendTable.HorizontalAlignment = 1;
            outcomeLegendTable.TotalWidth = 250;
            outcomeLegendTable.LockedWidth = true;

            float[] widths = new float[] {0.5f, 1f, 1f, 0.5f };
            outcomeLegendTable.SetWidths(widths);
            
            PdfPCell titleCell = new PdfPCell(new Phrase("Characteristics of Successful Learning Behaviours", font_legend_title));
            titleCell.Border = 0;
            titleCell.Colspan = 4;
            titleCell.Padding = 1;
            titleCell.PaddingBottom = 2;
            titleCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            titleCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            outcomeLegendTable.AddCell(titleCell);
                        

            foreach (KeyValuePair<string, string> legendItem in lifeSkills)
            {
                Paragraph lifeSkillDescription = new Paragraph();
                lifeSkillDescription.Add(new Phrase(legendItem.Key + ": ", font_legend_bold));
                lifeSkillDescription.Add(new Phrase(legendItem.Value, font_legend));

                PdfPCell lifeskillCell = new PdfPCell(lifeSkillDescription);
                lifeskillCell.SetLeading(0, 1.25f);
                lifeskillCell.Border = 0;
                lifeskillCell.Colspan = 4;
                lifeskillCell.Padding = 5;
                lifeskillCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                lifeskillCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                outcomeLegendTable.AddCell(lifeskillCell);
            }
            
            // Spacing cell
            
            PdfPCell spacerCell = new PdfPCell(new Phrase(" "));
            spacerCell.Border = 0;
            spacerCell.Colspan = 3;
            //outcomeLegendTable.AddCell(spacerCell);

            // Add example bars
            Font font_key = FontFactory.GetFont("Verdana", 6, BaseColor.BLACK);
            Font font_key_bold = FontFactory.GetFont("Verdana", 6, Font.BOLD, BaseColor.BLACK);
            
            // Empty number bar
            if (barStyle == OutcomeBarStyle.LifeSkills)
            {
                PdfPCell blankCell = new PdfPCell();
                blankCell.Border = Rectangle.NO_BORDER;
                outcomeLegendTable.AddCell(blankCell);

                PdfPCell numberBarDescriptioncell = new PdfPCell();
                numberBarDescriptioncell.Border = 0;
                numberBarDescriptioncell.SetLeading(0, 1.25f);
                numberBarDescriptioncell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                outcomeLegendTable.AddCell(numberBarDescriptioncell);

                PdfPCell numberBarCell = new PdfPCell();
                numberBarCell.AddElement(outcomeBar_NumberBar(content));
                numberBarCell.Border = 0;
                numberBarCell.PaddingTop = 0;
                numberBarCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                outcomeLegendTable.AddCell(numberBarCell);

                outcomeLegendTable.AddCell(blankCell);

            }

            foreach (KeyValuePair<int, string> keyItem in potentialMarks)
            {
                PdfPCell blankCell = new PdfPCell();
                blankCell.Border = Rectangle.NO_BORDER;
                outcomeLegendTable.AddCell(blankCell);

                Paragraph descriptionParagraph = new Paragraph();
                //descriptionParagraph.Add(new Phrase(keyItem.Key.ToString() + ": ", font_key_bold));
                descriptionParagraph.Add(new Phrase(keyItem.Value, font_key));
                PdfPCell keyDescriptionCell = new PdfPCell(descriptionParagraph);
                keyDescriptionCell.Border = Rectangle.NO_BORDER;
                keyDescriptionCell.SetLeading(0, 1.25f);
                keyDescriptionCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                outcomeLegendTable.AddCell(keyDescriptionCell);

                PdfPCell keyValueCell = new PdfPCell();
                keyValueCell.AddElement(displayOutcomeBar(content, keyItem.Key.ToString(), barStyle));
                keyValueCell.Border = 0;
                keyValueCell.PaddingTop = 2;
                keyValueCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                outcomeLegendTable.AddCell(keyValueCell);

                outcomeLegendTable.AddCell(blankCell);

            }

            // Spacer cell for the bottom, because the bottom cell is always stretched to fit the parent cell            
            outcomeLegendTable.AddCell(spacerCell);

            return outcomeLegendTable;
        }

        /// <summary>
        /// This section combines the "outcomeLegend" nad "lifeskillsLegend" into two columns for display on the front page of the report
        /// </summary>
        /// <param name="content"></param>
        /// <param name="grade"></param>
        /// <param name="outcomeBarStyle"></param>
        /// <param name="lifeSkillsBarStyle"></param>
        /// <returns></returns>
        private static PdfPTable legend(PdfContentByte content, string grade, OutcomeBarStyle outcomeBarStyle, OutcomeBarStyle lifeSkillsBarStyle)
        {
            PdfPTable legendTable = new PdfPTable(2);
            legendTable.HorizontalAlignment = 1;
            legendTable.TotalWidth = 550f;
            legendTable.LockedWidth = true;
            legendTable.SpacingAfter = standardElementSpacing + 10;
            legendTable.SpacingBefore = 10;            
            float[] widths = new float[] { 250f, 250f };
            legendTable.SetWidths(widths);

            PdfPCell lifeskills_column = new PdfPCell(lifeSkillsLegend(content, grade, lifeSkillsBarStyle));
            lifeskills_column.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            lifeskills_column.Border = Rectangle.RIGHT_BORDER;
            lifeskills_column.PaddingLeft = 10;
            lifeskills_column.BorderColor = new BaseColor(190, 190, 190);
            legendTable.AddCell(lifeskills_column);

            PdfPCell outcomes_column = new PdfPCell(outcomeLegend(content, outcomeBarStyle));
            outcomes_column.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            outcomes_column.PaddingRight = 10;
            outcomes_column.Border = 0;

            legendTable.AddCell(outcomes_column);

            return legendTable;
        }
        
        /// <summary>
        /// The "Outcome" sub-section of a class section - this is called for each outcome that has marks
        /// </summary>
        /// <param name="outcome"></param>
        /// <param name="course"></param>
        /// <param name="content"></param>
        /// <param name="barStyle"></param>
        /// <returns></returns>
        private static PdfPCell outcomeChunk(Outcome outcome, SchoolClass course, PdfContentByte content, OutcomeBarStyle barStyle = OutcomeBarStyle.Slider)
        {   
            int ObjectivesTableDebuggingBorder = Rectangle.NO_BORDER;

            // The container for this objective
            PdfPTable objectiveChunkTable = new PdfPTable(2);
            objectiveChunkTable.KeepTogether = false;
            objectiveChunkTable.SetWidths(new float[] { 5f, 1f });

            // Add an extra column if this outcome has more than one mark, so we can display the report period the mark came from
            if (outcome.marks.Count > 1)
            {
                objectiveChunkTable = new PdfPTable(3);
                objectiveChunkTable.KeepTogether = false;
                objectiveChunkTable.SetWidths(new float[] { 4.25f, 0.75f, 1f });
            }
           
            if (outcome.marks.Count > 0)
            {   
                // Outcome Description
                PdfPCell objectiveDescriptionCell = new PdfPCell();
                objectiveDescriptionCell.Rowspan = outcome.marks.Count;
                String outcomeDescription = outcome.notes;
                objectiveDescriptionCell.AddElement(new Paragraph(outcomeDescription, font_small));                
                objectiveDescriptionCell.Border = ObjectivesTableDebuggingBorder;                
                objectiveDescriptionCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                objectiveDescriptionCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                objectiveDescriptionCell.PaddingLeft = 5;
                objectiveChunkTable.AddCell(objectiveDescriptionCell);
                               


                // Marks                
                PdfPCell objectiveMarksCell = new PdfPCell();

                foreach (OutcomeMark objectivemark in outcome.marks)
                {
                    // If there is more than 1 mark for this outcome, display a report period in a new cell (otherwise this cell doesn't exist)
                    if (outcome.marks.Count > 1)
                    {
                        PdfPCell reportPeriodCell = new PdfPCell(new Paragraph(objectivemark.reportPeriod.name + ":", font_very_small));
                        reportPeriodCell.Border = ObjectivesTableDebuggingBorder;
                        reportPeriodCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                        reportPeriodCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                        objectiveChunkTable.AddCell(reportPeriodCell);
                    }


                    // Display the mark
                    PdfPCell markCell = new PdfPCell();

                    // Figure out which mark to display here
                    #region Determine which mark to display and how to display it

                    bool displayNMark = false; // If this is false, display the cMark instead

                    // k-9 always uses the cMark
                    // 10-12 prioritizes the nMark, but falls back to cMark

                    if (course.isHighSchoolLevel())
                    {
                        if (objectivemark.nMark > 0)
                        {
                            displayNMark = true;
                        }
                    }

                    // Display the mark
                    //  - If the mark is 1-4 (or IE or NYM or another string), display as an outcome
                    //  - If the mark is greater than 4, display as a percent

                    if (displayNMark)
                    {
                        // If nMark is 1-4, display it as an outcome bar
                        if ((objectivemark.nMark >= 1) && (objectivemark.nMark <= 4)) {

                            // Display as an outcome bar

                            // Strings instead of number test
                            //PdfPCell Temp_MarkCell = new PdfPCell(new Phrase(LSKYCommon.getOutcomeString(objectivemark.cMark), font_body_bold));

                            PdfPCell Temp_MarkCell = new PdfPCell();
                            Temp_MarkCell.AddElement((displayOutcomeBar(content, objectivemark.nMark.ToString(), barStyle)));

                            Temp_MarkCell.Border = ObjectivesTableDebuggingBorder;
                            Temp_MarkCell.Padding = 2;
                            Temp_MarkCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                            Temp_MarkCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                            markCell = Temp_MarkCell;

                        } else {
                            // Display as a percent
                            PdfPCell Temp_ReportPeriodCell = new PdfPCell(new Phrase(Math.Round(objectivemark.nMark, 0) + "%", font_body_bold));
                            Temp_ReportPeriodCell.Border = ObjectivesTableDebuggingBorder;
                            Temp_ReportPeriodCell.Padding = 2;
                            Temp_ReportPeriodCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            Temp_ReportPeriodCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                            markCell = Temp_ReportPeriodCell;
                        }
                    }
                    else
                    {
                        // if the cMark is not null, Display an outcome bar for the cMark
                        if (!string.IsNullOrEmpty(objectivemark.cMark))
                        {
                            // Display an outcome bar
                            
                            // Strings instead of number test
                            //PdfPCell Temp_MarkCell = new PdfPCell(new Phrase(LSKYCommon.getOutcomeString(objectivemark.cMark), font_body_bold));

                            PdfPCell Temp_MarkCell = new PdfPCell();
                            Temp_MarkCell.AddElement((displayOutcomeBar(content, objectivemark.cMark, barStyle)));                            

                            Temp_MarkCell.Border = ObjectivesTableDebuggingBorder;
                            Temp_MarkCell.Padding = 0;
                            Temp_MarkCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                            Temp_MarkCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                            
                            markCell = Temp_MarkCell;
                        }
                        else
                        {
                            // Display an empty cell or an error
                            PdfPCell Temp_ReportPeriodCell = new PdfPCell(new Phrase("-", font_body_bold));
                            Temp_ReportPeriodCell.Border = ObjectivesTableDebuggingBorder;
                            Temp_ReportPeriodCell.Padding = 2;
                            Temp_ReportPeriodCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            Temp_ReportPeriodCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                            markCell = Temp_ReportPeriodCell;
                            
                        }
                    }
                    #endregion

                    objectiveChunkTable.AddCell(markCell);

                }

                objectiveMarksCell.Border = ObjectivesTableDebuggingBorder;
                objectiveMarksCell.Padding = 0;
                objectiveMarksCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                objectiveMarksCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                objectiveMarksCell.AddElement(objectiveChunkTable);
                objectiveChunkTable.AddCell(objectiveMarksCell);
                                
            }                       
            

            // Encapsulate the table in a cell object and return it
            PdfPCell objectiveChunkTableCell = new PdfPCell(objectiveChunkTable);
            objectiveChunkTableCell.Colspan = 2;
            objectiveChunkTableCell.Border = ObjectivesTableDebuggingBorder;

            return objectiveChunkTableCell;
        }

        private static PdfPCell lifeSkillsChunk(List<Outcome> lifeSkillsObjectives, PdfContentByte content, OutcomeBarStyle barStyle = OutcomeBarStyle.LifeSkills)
        {
            int lifeSkillsTableBorder = Rectangle.NO_BORDER;

            PdfPCell bufferCell = new PdfPCell(new Phrase(""));
            bufferCell.Border = lifeSkillsTableBorder;
            
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
                lifeSkillChunkTable.SpacingBefore = 5;


                // Display column names
                PdfPCell rpHeadingCell = new PdfPCell(new Phrase("Successful Learner Behaviors", font_small_bold));
                rpHeadingCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                rpHeadingCell.VerticalAlignment = PdfPCell.ALIGN_BOTTOM;
                rpHeadingCell.Border = lifeSkillsTableBorder;
                lifeSkillChunkTable.AddCell(rpHeadingCell);
                foreach (string lifeSkillName in lifeSkillsNames)
                {
                    PdfPCell columnHeadingCell = new PdfPCell(new Phrase(lifeSkillName, font_small_bold));
                    columnHeadingCell.Border = lifeSkillsTableBorder;
                    columnHeadingCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    columnHeadingCell.VerticalAlignment = PdfPCell.ALIGN_BOTTOM;
                    lifeSkillChunkTable.AddCell(columnHeadingCell);
                }

                // Display number bars
                if ((barStyle == OutcomeBarStyle.LifeSkills) || (barStyle == OutcomeBarStyle.Minimal))
                {
                    lifeSkillChunkTable.AddCell(bufferCell);
                    foreach (string lifeSkillName in lifeSkillsNames)
                    {
                        PdfPCell columnHeadingCell = new PdfPCell(outcomeBar_NumberBar(content), true);
                        columnHeadingCell.Border = lifeSkillsTableBorder;
                        columnHeadingCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                        lifeSkillChunkTable.AddCell(columnHeadingCell);
                    }
                }

                // Display outcome bars
                foreach (string reportPeriodName in reportPeriodNames)
                {
                    PdfPCell rpNamecell = new PdfPCell(new Phrase(reportPeriodName, font_small));
                    rpNamecell.Border = lifeSkillsTableBorder;
                    rpNamecell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    rpNamecell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    lifeSkillChunkTable.AddCell(rpNamecell);

                    foreach (string lifeSkillName in lifeSkillsNames)
                    {
                        PdfPCell markValueCell = new PdfPCell();
                        markValueCell.Border = lifeSkillsTableBorder;
                        markValueCell.PaddingTop = 3;

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

                                        markValueCell.AddElement(displayOutcomeBar(content, markToDisplay.ToString(), barStyle));

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

        private static PdfPTable classWithMarks(Student student, SchoolClass course, PdfContentByte content, bool anonymize = false, OutcomeBarStyle outcomeBarStyle = OutcomeBarStyle.Slider, OutcomeBarStyle lifeSkillsBarStyle = OutcomeBarStyle.LifeSkills)
        {
            // colors for this section
            BaseColor markBoxBorderColor = new BaseColor(0.7f, 0.7f, 0.7f);

            // Fonts for this section
            Font font_class_comments = FontFactory.GetFont("Verdana", 8, Font.NORMAL, BaseColor.BLACK);
            Font font_class_comments_bold = FontFactory.GetFont("Verdana", 8, Font.BOLD, BaseColor.BLACK);
            Font font_class_section_titles = FontFactory.GetFont("Verdana", 9, Font.BOLD, BaseColor.BLACK);
            Font font_class_title = FontFactory.GetFont("Verdana", 14, Font.BOLD, BaseColor.BLACK);
            Font font_class_teacher = FontFactory.GetFont("Verdana", 8, BaseColor.BLACK);
            Font font_numeric_mark = FontFactory.GetFont("Verdana", 12, Font.NORMAL, BaseColor.BLACK);
            
            // Table that the whole "class" will be displayed in
            PdfPTable classTable = new PdfPTable(2);            
            classTable.HorizontalAlignment = 1;
            classTable.TotalWidth = 500f;
            classTable.LockedWidth = true;
            classTable.SpacingAfter = standardElementSpacing;
            classTable.KeepTogether = true;
            float[] widths = new float[] { 2f, 1f };
            classTable.SetWidths(widths);

            // *********************************
            // * Course Title cell
            // *********************************

            #region Course Title

            PdfPTable classTitleTable = new PdfPTable(1);
            Paragraph courseTitleParagraph = new Paragraph();
            courseTitleParagraph.Add(new Phrase(course.name, font_class_title));

            // Teacher name
            if (anonymize)
            {
                courseTitleParagraph.Add(new Phrase(" (Mr. Teacher)", font_class_teacher));
            }
            else
            {
                courseTitleParagraph.Add(new Phrase(" (" + course.teacherName + ")", font_class_teacher));
            }

            // Whether the class is outcome based or not
            if (course.hasOutcomes() && course.isHighSchoolLevel())
            {
                //courseTitleParagraph.Add(new Phrase(" (Outcome based class)", font_small_italic));
            }
            
            PdfPCell classTitleCell = new PdfPCell(courseTitleParagraph);
            classTitleCell.Border = 0;
            classTitleCell.Padding = 0;
            classTitleCell.SetLeading(0, 1.25f);
            classTitleCell.PaddingLeft = 0;
            classTitleTable.AddCell(classTitleCell);    

            PdfPCell classTitleTableContainer = new PdfPCell(classTitleTable);
            classTitleTableContainer.Border = 0;
            classTable.AddCell(classTitleTableContainer);

            #endregion

            // *********************************
            // * Course Mark (Adjusted Grade)
            // *********************************

            #region Course marks
            
            // Create a blank cell that we can use later
            PdfPCell blankCell = new PdfPCell(new Paragraph(""));
            blankCell.Border = 0;
            blankCell.Padding = 5;
            blankCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;

            // Should we even display a mark at all? Don't display if the class is k-9
            if (course.isHighSchoolLevel())
            {
                // *********************************
                // * High School
                // *********************************

                // Is the class outcome based? If yes, only display the final report period                
                // If the class is not outcome based, display all marks
                if (course.hasOutcomes())
                {
                    // *********************************
                    // * High School - Outcome based class
                    // *********************************

                    // Only display the final report period of the term (in percents)
                    if (course.term.FinalReportPeriod != null)
                    {
                        // Figure out which mark to display (cMark or nMark)
                        //  - if nMark is not zero, display it
                        //  - if cMark is empty, ignore the mark entirely
                        //  - display cMark as number bar

                        // Get the mark from the final report period
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
                            string markToDisplay = string.Empty;
                            if (finalMark.nMark > 0)
                            {
                                // If the nMark is between 1 and 4, assume its an outcome
                                if ((finalMark.nMark >= 1) && (finalMark.nMark <= 4))
                                {
                                    // Final mark of a high school class should always be a percent, but display it as an outcome anyways, 
                                    //  in case someone is dumb

                                    // Display the mark as an outcome bar
                                    PdfPTable embeddedMarkTable = new PdfPTable(1);
                                    PdfPCell markCell = new PdfPCell(displayOutcomeBar(content, finalMark.cMark, outcomeBarStyle, true));
                                    markCell.Border = Rectangle.BOX;
                                    markCell.BorderColor = markBoxBorderColor;
                                    markCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    markCell.Padding = 5;

                                    PdfPCell titleCell = new PdfPCell(new Phrase("Final Mark", font_small));
                                    titleCell.Border = Rectangle.BOX;
                                    markCell.BorderColor = markBoxBorderColor;
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
                                    // Display the mark as a percent
                                    PdfPTable embeddedMarkTable = new PdfPTable(1);
                                    PdfPCell markCell = new PdfPCell(new Phrase(Math.Round(finalMark.nMark, 0) + "%", font_body_bold));
                                    markCell.Border = Rectangle.BOX;
                                    markCell.BorderColor = markBoxBorderColor;
                                    markCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    markCell.Padding = 5;

                                    PdfPCell titleCell = new PdfPCell(new Phrase("Final Mark", font_small));
                                    titleCell.Border = Rectangle.BOX;
                                    markCell.BorderColor = markBoxBorderColor;
                                    titleCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    titleCell.Padding = 2;
                                    titleCell.PaddingBottom = 3;
                                    embeddedMarkTable.AddCell(titleCell);
                                    embeddedMarkTable.AddCell(markCell);

                                    PdfPCell embeddedMarkTableContainer = new PdfPCell(embeddedMarkTable);
                                    embeddedMarkTableContainer.Border = 0;
                                    classTable.AddCell(embeddedMarkTableContainer);
                                }
                            }
                            else if (!string.IsNullOrEmpty(finalMark.cMark))
                            {
                                // Display the cMark
                                PdfPTable embeddedMarkTable = new PdfPTable(1);
                                PdfPCell markCell = new PdfPCell(displayOutcomeBar(content, finalMark.cMark, outcomeBarStyle, true));
                                markCell.Border = Rectangle.BOX;
                                markCell.BorderColor = markBoxBorderColor;
                                markCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                markCell.Padding = 5;
                                
                                PdfPCell titleCell = new PdfPCell(new Phrase("Final Mark", font_small));
                                titleCell.Border = Rectangle.BOX;
                                markCell.BorderColor = markBoxBorderColor;
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
                                // Class is outcome based and there is no mark to display
                                classTable.AddCell(blankCell);
                            }
                        }
                        else
                        {
                            // Class is outcome based, the final report period is present, but the mark is invalid (Probably won't ever happen, but just in case)
                            classTable.AddCell(blankCell);
                        }
                    }
                    else
                    {
                        // Class is outcome baed and there is no final report period present                        
                        /*
                        PdfPCell markCell = new PdfPCell(new Phrase("(Outcome based class)", font_small_italic));
                        markCell.Border = Rectangle.NO_BORDER;
                        markCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                        classTable.AddCell(markCell);
                        */
                        classTable.AddCell(blankCell);
                    }
                }
                else
                {
                    // *********************************
                    // * High School - Not outcome based class
                    // *********************************

                    // Course has no outcomes

                    // Display all marks from all given report periods

                    // For each available mark:
                    // Figure out which mark to display (cMark or nMark)
                    //  - if nMark is not zero, display it
                    //  - if cMark is empty, ignore the mark entirely
                    //  - display cMark as number bar

                    #region Class without outcomes (displaying all report periods)

                    // Get list of report periods to display (just the ones with marks in them)                
                    List<ReportPeriod> loadedReportPeriods = new List<ReportPeriod>();
                    foreach (Mark mark in course.Marks)
                    {
                        if ((mark.nMark > 0) || (!string.IsNullOrEmpty(mark.cMark)))
                        {
                            if (!loadedReportPeriods.Contains(mark.reportPeriod))
                            {
                                loadedReportPeriods.Add(mark.reportPeriod);
                            }
                        }
                    }

                    if (loadedReportPeriods.Count > 0)
                    {
                        PdfPTable embeddedMarkTable = new PdfPTable(loadedReportPeriods.Count);

                        // Display the report period names (if there are more than one)
                        if (loadedReportPeriods.Count > 1)
                        {
                            foreach (ReportPeriod rp in loadedReportPeriods)
                            {
                                PdfPCell reportPeriodNameCell = new PdfPCell(new Phrase(rp.name, font_small));
                                reportPeriodNameCell.Border = 0;
                                reportPeriodNameCell.Padding = 2;
                                reportPeriodNameCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                embeddedMarkTable.AddCell(reportPeriodNameCell);
                            }
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
                                // If the nMark exists, display it
                                //  - if the nMark is between 1-4, it is an outcome
                                // if the cMark 
                                
                                if (MarkForThisReportPeriod.nMark > 4)
                                {
                                    // Display the mark as a percent
                                    PdfPCell markCell = new PdfPCell(new Phrase(Math.Round(MarkForThisReportPeriod.nMark, 0) + "%", font_numeric_mark));
                                    markCell.Border = Rectangle.BOX;
                                    markCell.Padding = 5;
                                    markCell.PaddingBottom = 7;
                                    markCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    embeddedMarkTable.AddCell(markCell);
                                }
                                else
                                {
                                    // Display the mark as an outcome
                                    string markToDisplay = string.Empty;
                                    if (MarkForThisReportPeriod.nMark > 0)
                                    {
                                        // If the number is a whole number with decimal places, round off the decimal places
                                        // Otherwise show one decimal place

                                        decimal workingNMark = Math.Round(MarkForThisReportPeriod.nMark, 1);

                                        // If the decimal is ".0", we don't need to know that, so round it to the nearest whole number.
                                        // If there is a nonzero decimal, don't round.
                                        if ((workingNMark % 1) == 0)
                                        {
                                            // Number is a whole number
                                            workingNMark = Math.Round(MarkForThisReportPeriod.nMark, 0);
                                        }

                                        markToDisplay = workingNMark.ToString();
                                    }
                                    else
                                    {
                                        markToDisplay = MarkForThisReportPeriod.cMark;
                                    }
                                    
                                    //PdfPCell markCell = new PdfPCell(displayOutcomeBar(content, markToDisplay, outcomeBarStyle, true), false);
                                    //PdfPCell markCell = new PdfPCell(new Phrase(LSKYCommon.getOutcomeString(markToDisplay), font_numeric_mark));
                                    PdfPCell markCell = new PdfPCell(new Phrase(markToDisplay, font_numeric_mark));
                                    markCell.Border = Rectangle.BOX;
                                    markCell.Padding = 5;
                                    markCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                    embeddedMarkTable.AddCell(markCell);
                                }

                            }
                            else
                            {
                                // There is no mark for this report period
                                PdfPCell emptycell = new PdfPCell(new Paragraph(""));
                                emptycell.Border = 0;
                                emptycell.Padding = 5;
                                emptycell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                                embeddedMarkTable.AddCell(emptycell);
                            }
                        }


                        PdfPCell embeddedMarkTableContainer = new PdfPCell(embeddedMarkTable);
                        embeddedMarkTableContainer.Border = 0;
                        classTable.AddCell(embeddedMarkTableContainer);
                    }
                    else
                    {
                        // No valid marks to display
                        //classTable.AddCell(blankCell);
                        PdfPCell noMarksCell = new PdfPCell(new Phrase("", font_small_italic));
                        noMarksCell.Border = 0;
                        noMarksCell.Padding = 5;
                        noMarksCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                        classTable.AddCell(noMarksCell);
                    }
                    #endregion

                }

            }
            else
            {
                // *********************************
                // * k-9
                // *********************************

                // Class is not high school level, so there is no such thing as class marks for it
                classTable.AddCell(blankCell);
            }

            #endregion

            // *********************************
            // *  Outcomes
            // *********************************
            
            #region Outcomes

            // Find out how many outcomes actually have marks in them  
            if (course.hasOutcomes())
            {
                if (course.OutcomeMarks.Count > 0)
                {
                    // Outcome entries
                    // - Check for marks and category name before displaying the outcome
                    foreach (Outcome objective in course.Outcomes)
                    {
                        if (objective.marks.Count > 0)
                        {
                            if (objective.hasValidMarks())
                            {
                                classTable.AddCell(outcomeChunk(objective, course, content, outcomeBarStyle));
                            }
                        }
                    }
                }
                else
                {
                    // Could display a "No marks" message here, but I think it looked better without anything
                    
                    //Paragraph lifeskillsParagraph = new Paragraph();
                    //lifeskillsParagraph.Add(new Phrase("No outcome marks at this time", font_small_italic)); 
                    //PdfPCell lifeSkillsCell = new PdfPCell(lifeskillsParagraph);
                    //lifeSkillsCell.Border = 0;
                    //lifeSkillsCell.Padding = 5;
                    //lifeSkillsCell.Colspan = 2;
                    //classTable.AddCell(lifeSkillsCell);
                    
                }
            }
            #endregion

            // *********************************
            // * Life skills / Successful Learning Behaviors / SLBs
            // *********************************

            #region Life Skills

            if (course.LifeSkillMarks.Count > 0)
            {
                classTable.AddCell(lifeSkillsChunk(course.LifeSkills, content, lifeSkillsBarStyle));
            }

            #endregion

            // *********************************
            // * Comments (from the Marks table)
            // *********************************

            #region Comments

            // Check for comments
            bool hasAcademicComments = false;

            // This is to determine how many different report periods we have comments for,
            // so we know if we need to display the report period name or not.
            List<int> differentReportPeriods = new List<int>();

            foreach (Mark m in course.Marks)
            {
                if (!string.IsNullOrEmpty(m.comment))
                {
                    hasAcademicComments = true;

                    // Figure out how many different report periods we have comments for
                    if (!differentReportPeriods.Contains(m.reportPeriodID))
                    {
                        differentReportPeriods.Add(m.reportPeriodID);
                    }                    
                }
            }

            if (hasAcademicComments)
            {
                Paragraph commentTitleParagraph = new Paragraph();
                commentTitleParagraph.Add(new Phrase("Comments:", font_class_section_titles));

                PdfPCell commentTitleCell = new PdfPCell(commentTitleParagraph);
                commentTitleCell.Border = 0;
                commentTitleCell.Padding = 2;
                commentTitleCell.Colspan = 2;
                classTable.AddCell(commentTitleCell);

                Paragraph commentsParagraph = new Paragraph();
                foreach (Mark mark in course.Marks)
                {
                    if (!string.IsNullOrEmpty(mark.comment))
                    {
                        // Display the report period if there are multiple comments from multiple report periods
                        if (differentReportPeriods.Count > 1)
                        {
                            commentsParagraph.Add(new Phrase(mark.reportPeriod.name + ": ", font_class_comments_bold));
                        }

                        if (anonymize)
                        {
                            commentsParagraph.Add(new Phrase(LSKYCommon.getRandomLipsumString(), font_class_comments));
                        }
                        else
                        {
                            commentsParagraph.Add(new Phrase(parseCommentsWithVariables(student, mark.comment), font_class_comments));
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
            #endregion

            return classTable;
        }

        /// <summary>
        /// Creates a horizontal line, for separating parts
        /// </summary>
        /// <returns></returns>
        public static PdfPTable horizontalLine()
        {
            PdfPTable returnMe = new PdfPTable(1);
            returnMe.HorizontalAlignment = 1;
            returnMe.TotalWidth = 500f;
            returnMe.LockedWidth = true;
            returnMe.SpacingAfter = standardElementSpacing;

            // Draw a horizontal line to help separate sections of the page
            PdfPCell lineCell = new PdfPCell();
            lineCell.Colspan = 2;
            lineCell.Border = Rectangle.BOTTOM_BORDER;
            lineCell.BorderColor = new BaseColor(0.6f, 0.6f, 0.6f);
            returnMe.AddCell(lineCell);

            return returnMe;

        }

        private static PdfPTable administrativeCommentsSection(string comments)
        {
            Font font_comment = FontFactory.GetFont("Verdana", 8, BaseColor.BLACK);

            PdfPTable commentsTable = new PdfPTable(1);
            commentsTable.HorizontalAlignment = 1;
            commentsTable.TotalWidth = 500;
            commentsTable.LockedWidth = true;
            commentsTable.SpacingBefore = 10;
            commentsTable.SpacingAfter = standardElementSpacing;
            commentsTable.KeepTogether = true;

            PdfPCell titleCell = new PdfPCell(new Paragraph(comments, font_comment));
            titleCell.Border = Rectangle.TOP_BORDER;
            titleCell.BorderColor = new BaseColor(0.6f, 0.6f, 0.6f);
            titleCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            titleCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            titleCell.Colspan = 5;
            titleCell.PaddingBottom = 2;
            commentsTable.AddCell(titleCell);
            
            return commentsTable;
        }
        
        /// <summary>
        /// Piece together the report card parts into a document that can be sent out
        /// </summary>
        /// <param name="students"></param>
        /// <param name="reportPeriods"></param>
        /// <param name="anonymize"></param>
        /// <param name="showPhotos"></param>
        /// <param name="doubleSidedMode"></param>
        /// <param name="outcomeBarStyle"></param>
        /// <param name="lifeSkillsBarStyle"></param>
        /// <returns></returns>
        public static MemoryStream GeneratePDF(List<Student> students, List<ReportPeriod> reportPeriods, bool anonymize, bool showPhotos, bool doubleSidedMode, bool showClassAttendance, bool showLegends, bool showAttendanceSummary, string adminComments, OutcomeBarStyle outcomeBarStyle = OutcomeBarStyle.Slider, OutcomeBarStyle lifeSkillsBarStyle = OutcomeBarStyle.LifeSkills)
        {
            MemoryStream memstream = new MemoryStream();
            Document ReportCard = new Document(PageSize.LETTER);
            PdfWriter writer = PdfWriter.GetInstance(ReportCard, memstream);

            ReportCard.Open();
            PdfContentByte content = writer.DirectContent;

            PdfPageEventHandler PageEventHandler = new PdfPageEventHandler();
            writer.PageEvent = PageEventHandler;
            PageEventHandler.DoubleSidedMode = doubleSidedMode;
            PageEventHandler.ShowOnFirstPage = true;

            // Add a watermark to the first page (if applicable)
                        
            //if (showWatermark)
            /*
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
                OutcomeBarStyle determinedBarStyle_Outcomes = outcomeBarStyle;
                OutcomeBarStyle determinedBarStyle_LifeSkills = lifeSkillsBarStyle;

                int gradeParsed = 0;
                if (int.TryParse(student.getGradeFormatted(), out gradeParsed))
                {
                    if (gradeParsed >= 10)
                    {
                        determinedBarStyle_Outcomes = OutcomeBarStyle.JustNumber;
                        determinedBarStyle_LifeSkills = OutcomeBarStyle.JustNumber;
                    }
                    else
                    {
                        determinedBarStyle_Outcomes = OutcomeBarStyle.Slider;
                        determinedBarStyle_LifeSkills = OutcomeBarStyle.LifeSkills;
                    }
                }

                // Cover page
                ReportCard.Add(PDFReportCardParts.schoolNamePlate(student.school));
                ReportCard.Add(PDFReportCardParts.namePlateTable(student, anonymize, showPhotos));
                ReportCard.Add(PDFReportCardParts.reportNamePlate(reportPeriods));
                if (showLegends)
                {
                    ReportCard.Add(PDFReportCardParts.legend(content, student.getGrade(), determinedBarStyle_Outcomes, determinedBarStyle_LifeSkills));
                }

                // Start course list
                //ReportCard.Add(new Phrase(string.Empty));
                ReportCard.Add(horizontalLine());

                foreach (Term term in student.track.terms)
                {
                    foreach (SchoolClass course in term.Courses)
                    {
                        // Uncomment this to only display classes with data
                        //if ((course.Marks.Count > 0) || (course.OutcomeMarks.Count > 0) || (course.LifeSkillMarks.Count > 0))
                        {
                            ReportCard.Add(PDFReportCardParts.classWithMarks(student, course, content, anonymize, determinedBarStyle_Outcomes, determinedBarStyle_LifeSkills));

                            if (showClassAttendance)
                            {
                                if (!student.track.daily)
                                {
                                    ReportCard.Add(PDFReportCardParts.courseAttendanceSummary(student, course));
                                }
                            }

                            ReportCard.Add(horizontalLine());
                        }
                    }
                }

                // Report period comments (called "Report Card Comments" in SchoolLogic)
                if (student.ReportPeriodComments.Count > 0)
                {
                    ReportCard.Add(PDFReportCardParts.reportPeriodCommentsSection(student.ReportPeriodComments, anonymize));
                }

                // Attendance summary
                if (showAttendanceSummary)
                {
                    // Display the period attendance summary always for now - in the future figure out a way to allow the user to choose
                    ReportCard.Add(PDFReportCardParts.attendanceSummaryByPeriod(student));

                    //if (!student.track.daily)
                    //{
                    //ReportCard.Add(PDFReportCardParts.attendanceSummaryByClassName(student, true));
                    //}
                    //else
                    //{
                    //ReportCard.Add(PDFReportCardParts.attendanceSummaryByPeriod(student));
                    //}

                }

                // Administrative comment
                if (!string.IsNullOrEmpty(adminComments))
                {
                    ReportCard.Add(administrativeCommentsSection(adminComments));
                }

                // Reset the page numbers for the next student
                PageEventHandler.ResetPageNumbers(ReportCard);
            }

            ReportCard.Close();
            return memstream;
        }
        
    }
}