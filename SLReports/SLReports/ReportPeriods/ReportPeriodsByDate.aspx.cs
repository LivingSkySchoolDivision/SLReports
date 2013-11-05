using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.ReportPeriods
{
    public partial class ReportPeriodsByDate : System.Web.UI.Page
    {

        TableRow addOpenReportPeriodRow(ReportPeriod rp, string termString, string schoolName)
        {
            TableRow newRow = new TableRow();

            TableCell cellTerm = new TableCell();
            TableCell cellID = new TableCell();
            TableCell cellName = new TableCell();
            TableCell cellSchool = new TableCell();
            TableCell cellStartDate = new TableCell();
            TableCell cellEndDate = new TableCell();
            TableCell cellDaysLeft = new TableCell();
            TableCell cellOpenDate = new TableCell();
            TableCell cellCloseDate = new TableCell();

            cellTerm.Text = termString;
            cellID.Text = rp.ID.ToString();
            cellName.Text = rp.name;
            cellSchool.Text = schoolName;
            cellStartDate.Text = rp.startDate.ToShortDateString();
            cellEndDate.Text = rp.endDate.ToShortDateString();
            cellOpenDate.Text = rp.DateOpens.ToShortDateString();
            cellCloseDate.Text = rp.DateCloses.ToShortDateString();

            TimeSpan daysLeft = rp.DateCloses.Subtract(DateTime.Today);

            cellDaysLeft.Text = daysLeft.Days.ToString();
            cellDaysLeft.HorizontalAlign = HorizontalAlign.Center;

            newRow.Cells.Add(cellTerm);
            newRow.Cells.Add(cellID);
            newRow.Cells.Add(cellName);
            newRow.Cells.Add(cellSchool);
            newRow.Cells.Add(cellStartDate);
            newRow.Cells.Add(cellEndDate);
            newRow.Cells.Add(cellOpenDate);
            newRow.Cells.Add(cellCloseDate);
            newRow.Cells.Add(cellDaysLeft);

            return newRow;
        }

        TableRow addUpcomingReportPeriodRow(ReportPeriod rp, string termString, string schoolName)
        {
            TableRow newRow = new TableRow();

            TableCell cellTerm = new TableCell();
            TableCell cellID = new TableCell();
            TableCell cellName = new TableCell();
            TableCell cellSchool = new TableCell();
            TableCell cellStartDate = new TableCell();
            TableCell cellEndDate = new TableCell();
            TableCell cellOpenDate = new TableCell();
            TableCell cellCloseDate = new TableCell();
            TableCell cellDaysUntilOpen = new TableCell();
            TableCell cellDaysUntilClose = new TableCell();

            cellTerm.Text = termString;
            cellID.Text = rp.ID.ToString();
            cellName.Text = rp.name;
            cellSchool.Text = schoolName;
            cellStartDate.Text = rp.startDate.ToShortDateString();
            cellEndDate.Text = rp.endDate.ToShortDateString();
            cellOpenDate.Text = rp.DateOpens.ToShortDateString();
            cellCloseDate.Text = rp.DateCloses.ToShortDateString();

            TimeSpan daysUntilClose = rp.DateCloses.Subtract(DateTime.Today);
            TimeSpan daysUntilOpen = rp.DateOpens.Subtract(DateTime.Today);

            cellDaysUntilOpen.Text = daysUntilOpen.Days.ToString();
            cellDaysUntilClose.Text = daysUntilClose.Days.ToString();
            cellDaysUntilClose.HorizontalAlign = HorizontalAlign.Center;
            cellDaysUntilOpen.HorizontalAlign = HorizontalAlign.Center;

            newRow.Cells.Add(cellTerm);
            newRow.Cells.Add(cellID);
            newRow.Cells.Add(cellName);
            newRow.Cells.Add(cellSchool);
            newRow.Cells.Add(cellStartDate);
            newRow.Cells.Add(cellEndDate);
            newRow.Cells.Add(cellOpenDate);
            newRow.Cells.Add(cellCloseDate);
            newRow.Cells.Add(cellDaysUntilOpen);
            newRow.Cells.Add(cellDaysUntilClose);

            return newRow;
        }

        TableRow addCompletedReportPeriodRow(ReportPeriod rp, string termString, string schoolName)
        {
            TableRow newRow = new TableRow();

            TableCell cellTerm = new TableCell();
            TableCell cellID = new TableCell();
            TableCell cellName = new TableCell();
            TableCell cellSchool = new TableCell();
            TableCell cellStartDate = new TableCell();
            TableCell cellEndDate = new TableCell();
            TableCell cellOpenDate = new TableCell();
            TableCell cellCloseDate = new TableCell();

            cellTerm.Text = termString;
            cellID.Text = rp.ID.ToString();
            cellName.Text = rp.name;
            cellSchool.Text = schoolName;
            cellStartDate.Text = rp.startDate.ToShortDateString();
            cellEndDate.Text = rp.endDate.ToShortDateString();
            cellOpenDate.Text = rp.DateOpens.ToShortDateString();
            cellCloseDate.Text = rp.DateCloses.ToShortDateString();

            newRow.Cells.Add(cellTerm);
            newRow.Cells.Add(cellID);
            newRow.Cells.Add(cellName);
            newRow.Cells.Add(cellSchool);
            newRow.Cells.Add(cellStartDate);
            newRow.Cells.Add(cellOpenDate);
            newRow.Cells.Add(cellCloseDate);
            newRow.Cells.Add(cellEndDate);

            return newRow;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            List<ReportPeriod> allReportPeriods = new List<ReportPeriod>();
            List<Term> allTerms = new List<Term>();
            List<Track> allTracks = new List<Track>();
            List<School> allSchools = new List<School>();
            using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogic))
            {
                allReportPeriods = ReportPeriod.loadAllReportPeriods(connection);
                allTracks = Track.loadAllTracks(connection);
                allTerms = Term.loadAllTerms(connection);
                allSchools = School.loadAllSchools(connection);
            }

            allReportPeriods.Sort(
                delegate(ReportPeriod first,
                ReportPeriod next)
                {
                    return first.endDate.CompareTo(next.endDate);
                }
                );


            foreach (ReportPeriod rp in allReportPeriods)
            {
                // Find this report period's term
                string termString = "Unknown";
                string schoolString = "";

                foreach (Term term in allTerms)
                {
                    if (term.ID == rp.termID)
                    {
                        foreach (Track track in allTracks)
                        {
                            if (term.trackID == track.ID)
                            {
                                termString = track.name + " - " + term.name;
                            }
                        }
                    }
                }

                // Find the school
                foreach (School school in allSchools)
                {
                    if (rp.schoolID == int.Parse(school.getSchoolLogicID()))
                    {
                        schoolString = school.getName();
                    }
                }

                if ((DateTime.Today >= rp.DateOpens) && (DateTime.Today <= rp.DateCloses.AddDays(1)))
                {
                    tblOpenReportPeriods.Rows.Add(addOpenReportPeriodRow(rp, termString, schoolString));
                }
                else if (rp.DateOpens > DateTime.Today)
                {
                    tblReportPeriodsUpcoming.Rows.Add(addUpcomingReportPeriodRow(rp, termString, schoolString));
                }
                else
                {
                    tblReportPeriodsCompleted.Rows.Add(addCompletedReportPeriodRow(rp, termString, schoolString));
                }
            }


        }
    }
}