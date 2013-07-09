using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.Tracks
{
    public partial class index : System.Web.UI.Page
    {
        private TableRow addTrackTableHeaders()
        {
            TableRow newRow = new TableRow();
            newRow.CssClass = "datatable_header";

            TableCell newCell = null;

            newCell = new TableCell();
            newCell.Text = "School";
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = "Track Name";
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = "Code";
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = "Database ID";
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = "Start Date";
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = "End Date";
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = "Daily";
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = "Days in Cycle";
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = "Blocks per day";
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = "Daily blocks per day";
            newRow.Cells.Add(newCell);
            
            return newRow;
        }

        private string boolToYesOrNo(bool thisBool)
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

        private TableRow addTrackTableRow(Track track)
        {
            TableRow newRow = new TableRow();
            newRow.CssClass = "datatable_row";

            TableCell newCell = null;

            newCell = new TableCell();
            if (track.school != null)
            {
                newCell.Text = track.school.getName();
            }
            else
            {
                newCell.Text = track.schoolID.ToString();
            }
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = track.name;
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = track.code;
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = track.ID.ToString();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = track.startDate.ToShortDateString();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = track.endDate.ToShortDateString();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = boolToYesOrNo(track.daily);
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = track.daysInCycle.ToString();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = track.blocksPerDay.ToString();
            newRow.Cells.Add(newCell);

            newCell = new TableCell();
            newCell.Text = track.dailyBlocksPerDay.ToString();
            newRow.Cells.Add(newCell);

            return newRow;

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            List<Track> allTracks = new List<Track>();

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                allTracks = Track.loadAllTracks(connection);
            }

            tblTracks.Rows.Clear();
            tblTracks.Rows.Add(addTrackTableHeaders());

            foreach (Track track in allTracks)
            {
                tblTracks.Rows.Add(addTrackTableRow(track));
            }

            lblTrackCount.Text = " ("+allTracks.Count+")";



        }
    }
}