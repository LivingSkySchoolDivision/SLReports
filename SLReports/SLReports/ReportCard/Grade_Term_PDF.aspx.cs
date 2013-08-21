﻿using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
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
            //PageEventHandler.bottomCenter = "Printed " + DateTime.Now.ToLongDateString();

            foreach (Student student in students)
            {
                PageEventHandler.bottomLeft = student.getDisplayName();
                ReportCard.Add(PDFReportCardParts.schoolNamePlate(student.school));
                ReportCard.Add(PDFReportCardParts.namePlateTable(student));
                ReportCard.Add(PDFReportCardParts.lifeSkillsLegend(content, student.getGrade()));
                ReportCard.Add(PDFReportCardParts.outcomeLegend(content));
                ReportCard.NewPage();
                ReportCard.Add(new Phrase(string.Empty));
                

                foreach (Term term in student.track.terms)
                {
                    foreach (SchoolClass course in term.Courses)
                    {
                        ReportCard.Add(PDFReportCardParts.classWithMarks(course, content));
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

        private void displayError(string error)
        {
            Response.Write(error);
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            List<Student> students = new List<Student>();
            List<Student> displayedStudents = new List<Student>();

            List<ReportPeriod> selectedReportPeriods = new List<ReportPeriod>();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogic2013"].ConnectionString;
            //String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {                
                /* Debugging info */
                //selectedTerm = Term.loadThisTerm(connection, 20);

                /* McKitrick report periods for testing */
                //selectedReportPeriods.Add(ReportPeriod.loadThisReportPeriod(connection, 266));
                //selectedReportPeriods.Add(ReportPeriod.loadThisReportPeriod(connection, 267));
                //selectedReportPeriods.Add(ReportPeriod.loadThisReportPeriod(connection, 268));

                /* McKitrick students for testing */
                //students.Add(Student.loadThisStudent(connection, "80451"));
                //students.Add(Student.loadThisStudent(connection, "80891"));

                /* NBCHS report periods for testing */
                
                selectedReportPeriods.Add(ReportPeriod.loadThisReportPeriod(connection, 258));
                selectedReportPeriods.Add(ReportPeriod.loadThisReportPeriod(connection, 257));
                //selectedReportPeriods.Add(ReportPeriod.loadThisReportPeriod(connection, 256));
                //selectedReportPeriods.Add(ReportPeriod.loadThisReportPeriod(connection, 255));
                //selectedReportPeriods.Add(ReportPeriod.loadThisReportPeriod(connection, 254));
                //selectedReportPeriods.Add(ReportPeriod.loadThisReportPeriod(connection, 253));
                

                /* NBCHS students for testing */
                students.Add(Student.loadThisStudent(connection, "12511"));
                students.Add(Student.loadThisStudent(connection, "11871"));
                students.Add(Student.loadThisStudent(connection, "11804"));
            }

            Response.Write("<br><B>TIMER: </b> Loaded basic student data in: " + stopwatch.Elapsed);
            stopwatch.Reset();

            Response.Write("<bR><b>Loaded report periods</b>");
            foreach (ReportPeriod rp in selectedReportPeriods)
            {
                Response.Write("<BR>" + rp);
                if (rp == null)
                {
                    Response.Write("<i>Null</i>");
                }
            }

            Response.Write("<br><B>Loaded students</b>");

            foreach (Student student in students)
            {
                Response.Write("<BR>" + student);
            }
            
            stopwatch.Start();
            
            selectedReportPeriods.Sort();
            
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                foreach (Student student in students)
                {
                    if (student != null)
                    {
                        Stopwatch studentStopWatch = new Stopwatch();
                        studentStopWatch.Start();
                        displayedStudents.Add(LSKYCommon.loadStudentMarkData(connection, student, selectedReportPeriods));
                        studentStopWatch.Stop();
                        Response.Write("<br>&nbsp;&nbsp;<B>TIMER: </b> Loaded data for student \"" + student.getDisplayName() + "\" in: " + studentStopWatch.Elapsed);
                    }
                }
                students.Clear();
            }

            Response.Write("<br><B>TIMER: </b> Loaded all mark data in: " + stopwatch.Elapsed);
            stopwatch.Stop();

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
                            Response.Write("<BR>&nbsp;&nbsp;&nbsp;<b>Objective Marks:</b> " + c.OutcomeMarks.Count);
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

                                foreach (OutcomeMark om in c.OutcomeMarks)
                                {
                                    if (om.reportPeriodID == rp.ID)
                                    {
                                        Response.Write("<BR>&nbsp;&nbsp;&nbsp;&nbsp;<b>ObjectiveMark:</b> " + om);
                                    }
                                }
                            }
                            Response.Write("<BR>&nbsp;&nbsp;&nbsp;<b>Objectives:</b> " + c.Outcomes.Count);
                            foreach (Outcome o in c.Outcomes)
                            {
                                Response.Write("<BR>&nbsp;&nbsp;&nbsp;&nbsp;<b>Objective:</b> " + o);
                                foreach (OutcomeMark om in o.marks) 
                                {
                                    Response.Write("<BR>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>ObjectiveMark:</b> " + om);
                                }
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