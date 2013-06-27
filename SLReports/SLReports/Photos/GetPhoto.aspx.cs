using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.Photos
{
    public partial class GetPhoto : System.Web.UI.Page
    {
        private byte[] createBlankPhoto(string text)
        {
            
            int imgHeight = 400;
            int imgWidth = 312;
            MemoryStream memstream = new MemoryStream();

            Bitmap bitmap = new Bitmap(imgWidth, imgHeight);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Font font = new Font("Arial", 12, FontStyle.Bold);
            StringFormat stringformat = new StringFormat();
            stringformat.LineAlignment = StringAlignment.Center;
            stringformat.Alignment = StringAlignment.Center;

            /* set a background color */
            graphics.Clear(Color.WhiteSmoke);

            Random random = new Random(DateTime.Now.Millisecond);
            int alpha = 50;
            int colorEnumerator = 0;

            /* Draw a blue ish background like the backdrop for most photos */
            SolidBrush[] backdropColors = { 
                                      new SolidBrush(Color.FromArgb(alpha, 0, 0, 255)), 
                                      new SolidBrush(Color.FromArgb(alpha, 0, 100, 255)), 
                                      new SolidBrush(Color.FromArgb(alpha, 0, 150, 255)), 
                                      new SolidBrush(Color.FromArgb(alpha, 0, 0, 200)),  
                                      new SolidBrush(Color.FromArgb(alpha, 200, 200, 250)), 
                                      new SolidBrush(Color.FromArgb(alpha, 150, 150, 230))};
            #region Draw backdrop
            for (int x = 0; x < 50; x++)
            {
                /* From top left */
                graphics.FillRectangle(backdropColors[colorEnumerator], 0, 0, random.Next(0, imgWidth), random.Next(0, imgHeight));

                colorEnumerator++;
                if (colorEnumerator >= backdropColors.Length)
                {
                    colorEnumerator = 0;
                }

                /* From top right */
                graphics.FillRectangle(backdropColors[colorEnumerator], random.Next(0, imgWidth), random.Next(0, imgHeight), imgWidth, 0);

                colorEnumerator++;
                if (colorEnumerator >= backdropColors.Length)
                {
                    colorEnumerator = 0;
                }

                /* From bottom left */
                graphics.FillRectangle(backdropColors[colorEnumerator], 0, imgHeight, random.Next(0, imgWidth), random.Next(0,imgHeight));

                colorEnumerator++;
                if (colorEnumerator >= backdropColors.Length)
                {
                    colorEnumerator = 0;
                }

                /* From bottom right */
                graphics.FillRectangle(backdropColors[colorEnumerator], random.Next(0, imgWidth), random.Next(0, imgHeight), imgWidth, imgHeight);

                colorEnumerator++;
                if (colorEnumerator >= backdropColors.Length)
                {
                    colorEnumerator = 0;
                }
            }
            #endregion

            #region Draw a head shaped blob
            int fleshAlpha = 75;
            SolidBrush[] fleshColors = { 
                                      new SolidBrush(Color.FromArgb(fleshAlpha, 255, 220, 117)),
                                      new SolidBrush(Color.FromArgb(fleshAlpha, 229, 194, 152)),
                                      new SolidBrush(Color.FromArgb(fleshAlpha, 228, 185, 142)),
                                      new SolidBrush(Color.FromArgb(fleshAlpha, 165, 57, 0)),
                                      new SolidBrush(Color.FromArgb(fleshAlpha, 225, 173, 164)),
                                      new SolidBrush(Color.FromArgb(fleshAlpha, 190, 114, 60)),
                                      new SolidBrush(Color.FromArgb(fleshAlpha, 187, 109, 74)),
                                      new SolidBrush(Color.FromArgb(fleshAlpha, 255, 224, 196))
                                       };

            colorEnumerator = 0;
            for (int x = 0; x < 20; x++)
            {
                graphics.FillRectangle(fleshColors[colorEnumerator],
                    (float)((imgWidth * 0.25) + random.Next(0, (imgWidth / 4))),
                    random.Next(0, (imgHeight / 2)),
                    random.Next(0, (imgWidth / 2)),
                    random.Next(0, (imgHeight / 2)));
                colorEnumerator++;
                if (colorEnumerator >= fleshColors.Length)
                {
                    colorEnumerator = 0;
                }
            }
            #endregion

            #region Draw a body / shirt shaped blob
            int bodyAlpha = 75;
            SolidBrush[] bodyColors = { 
                                      new SolidBrush(Color.FromArgb(bodyAlpha, 125, 0, 125)),
                                      new SolidBrush(Color.FromArgb(bodyAlpha, 125, 255, 0)),
                                      new SolidBrush(Color.FromArgb(bodyAlpha, 0, 125, 0)),
                                      new SolidBrush(Color.FromArgb(bodyAlpha, 0, 200, 125)),
                                      new SolidBrush(Color.FromArgb(bodyAlpha, 0, 0, 0)),
                                      new SolidBrush(Color.FromArgb(bodyAlpha, 255, 90, 0)),
                                      new SolidBrush(Color.FromArgb(bodyAlpha, 255, 0, 0))
                                       };

            colorEnumerator = 0;
            for (int x = 0; x < 20; x++)
            {
                graphics.FillRectangle(bodyColors[colorEnumerator],
                    (float)((imgWidth * 0.25) + random.Next(0, (imgWidth / 4))),
                    (float)(imgHeight * 0.60) + random.Next(0, (imgHeight / 2)),
                    random.Next(0, (imgWidth / 2)),
                    random.Next(0, (imgHeight / 2)));
                colorEnumerator++;
                if (colorEnumerator >= bodyColors.Length)
                {
                    colorEnumerator = 0;
                }
            }
            #endregion

            #region Draw text in a box
            graphics.FillRectangle(Brushes.Black, 0, (float)(imgHeight - (font.Height * 1.75)), imgWidth, imgHeight);
            graphics.DrawString(text, font, Brushes.White, new PointF((imgWidth / 2), (imgHeight) - font.Height),stringformat);
            #endregion

            /* Draw a black border around the picture */
            graphics.DrawRectangle(new Pen(Brushes.Black), 0, 0, imgWidth - 1, imgHeight - 1);

            bitmap.Save(memstream, ImageFormat.Png);

            return memstream.ToArray();
        }

        private void displayPhoto(byte[] photo, string graphicType)
        {
            Response.Clear();
            Response.ContentType = graphicType;
            Response.BinaryWrite(photo);
            Response.End();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            /* A good size for a photo is 312 x 400*/

            string StudNum = Request.QueryString["studentnumber"];

            if (!string.IsNullOrEmpty(StudNum))
            {
                String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(dbConnectionString))
                {
                    SqlCommand sqlCommand = new SqlCommand();
                    sqlCommand.Connection = connection;
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.CommandText = "SELECT Photo, PhotoType FROM LSKY_ActiveStudents WHERE StudentNumber=@StudNum";
                    sqlCommand.Parameters.AddWithValue("@StudNum", StudNum);
                    sqlCommand.Connection.Open();

                    SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                    if (dbDataReader.HasRows)
                    {
                        if (dbDataReader.Read())
                        {
                            if ((dbDataReader["Photo"] != null) && !string.IsNullOrEmpty(dbDataReader["PhotoType"].ToString()))
                            {
                                displayPhoto((byte[])dbDataReader["Photo"], dbDataReader["PhotoType"].ToString());
                            }
                            else
                            {
                                displayPhoto(createBlankPhoto("Student has no photo"), "image/png");                    
                            }
                        }
                    }
                    else 
                    {
                        displayPhoto(createBlankPhoto("Student not found"), "image/png");                    
                    }

                }
            }
            else
            {
                displayPhoto(createBlankPhoto("No student specified"), "image/png");
            }

        }
    }
}