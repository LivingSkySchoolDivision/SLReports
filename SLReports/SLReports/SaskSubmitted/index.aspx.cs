using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.SaskSubmitted
{
    public partial class index : System.Web.UI.Page
    {
        TableRow addHeaderRow()
        {
            TableRow newRow = new TableRow();
            newRow.CssClass = "datatable_header";

            TableCell successCell = new TableCell();
            TableCell dateCell = new TableCell();
            TableCell firstNameCell = new TableCell();
            TableCell lastNameCell = new TableCell();
            TableCell studentNumberCell = new TableCell();

            successCell.Text = "Success";
            dateCell.Text = "Date Submitted";
            firstNameCell.Text = "First Name";
            lastNameCell.Text = "Last Name";
            studentNumberCell.Text = "Student Number";

            newRow.Cells.Add(successCell);
            newRow.Cells.Add(dateCell);
            newRow.Cells.Add(firstNameCell);
            newRow.Cells.Add(lastNameCell);
            newRow.Cells.Add(studentNumberCell);

            return newRow;
        }

        TableRow addEntryRow(SkSubmittedEntry entry)
        {
            TableRow newRow = new TableRow();
            newRow.CssClass = "datatable_row";

            TableCell successCell = new TableCell();
            TableCell dateCell = new TableCell();
            TableCell firstNameCell = new TableCell();
            TableCell lastNameCell = new TableCell();
            TableCell studentNumberCell = new TableCell();

            string successString = string.Empty;

            if (entry.failed)
            {
                successString = "<B style=\"color: red;\">FAILED</B>";
            }
            else
            {
                successString = "Success";
            }
            successCell.Text = successString;
            dateCell.Text = entry.SubmittedDate.ToShortDateString();
            firstNameCell.Text = entry.firstName;
            lastNameCell.Text = entry.lastName;
            studentNumberCell.Text = entry.StudentNumber;

            newRow.Cells.Add(successCell);
            newRow.Cells.Add(dateCell);
            newRow.Cells.Add(firstNameCell);
            newRow.Cells.Add(lastNameCell);
            newRow.Cells.Add(studentNumberCell);

            return newRow;
        }

        TableRow addCountRow(SaskSubmittedSchoolCount entry)
        {
            TableRow newRow = new TableRow();

            TableCell nameCell = new TableCell();
            TableCell countCell = new TableCell();
            TableCell successesCell = new TableCell();
            TableCell failuresCell = new TableCell();

            nameCell.Text = entry.schoolName;
            countCell.Text = entry.totalCount.ToString();
            successesCell.Text = entry.successes.ToString();
            failuresCell.Text = entry.failures.ToString();

            newRow.Cells.Add(nameCell);
            newRow.Cells.Add(successesCell);
            newRow.Cells.Add(failuresCell);
            newRow.Cells.Add(countCell);

            return newRow;
        }

        private List<SkSubmittedEntry> getEntriesForThisSchool(List<SkSubmittedEntry> entries, int schoolGovID)
        {
            List<SkSubmittedEntry> returnMe = new List<SkSubmittedEntry>();

            foreach (SkSubmittedEntry entry in entries)
            {
                if (entry.schoolID == schoolGovID)
                {
                    returnMe.Add(entry);
                }
            }

            return returnMe;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Populate school dropdown list
            // Load school counts
            List<SkSubmittedEntry> allEntries = new List<SkSubmittedEntry>();
            List<School> allSchools = new List<School>();

            using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogic))
            {
                    allSchools = School.loadAllSchools(connection);

                foreach (School school in allSchools)
                {
                    ListItem newItem = new ListItem();
                    newItem.Text = school.getName();
                    newItem.Value = school.getGovID().ToString();
                    drpSchoolList.Items.Add(newItem);
                }

                allEntries = SkSubmittedEntry.loadAllSaskSubmittedEntries(connection);                    
            }
                        
            //Dictionary<string, int> schoolsWithCounts = new Dictionary<string, int>();

            List<SaskSubmittedSchoolCount> schoolsWithCounts = new List<SaskSubmittedSchoolCount>();

            int totalCount = 0;
            int totalSuccesses = 0;
            int totalFailures = 0;

            foreach (School school in allSchools)
            {
                int thisSchoolCount = 0;
                int thisSchoolSuccess = 0;
                int thisSchoolFailures = 0;

                foreach (SkSubmittedEntry entry in allEntries)
                {
                    if (entry.schoolID == int.Parse(school.getSchoolLogicID()))
                    {
                        thisSchoolCount++;
                        totalCount++;

                        if (entry.failed)
                        {
                            thisSchoolFailures++;
                            totalFailures++;
                        }
                        else
                        {
                            thisSchoolSuccess++;
                            totalSuccesses++;
                        }
                    }

                    
                }

                if (thisSchoolCount > 0)
                {
                    schoolsWithCounts.Add(new SaskSubmittedSchoolCount(school.getName(), thisSchoolCount, thisSchoolFailures, thisSchoolSuccess));
                }                    
            }


            SaskSubmittedSchoolCount totals = new SaskSubmittedSchoolCount("<b>Totals</b>", totalCount, totalFailures, totalSuccesses);            
            tblCounts.Rows.Add(addCountRow(totals));

            foreach (SaskSubmittedSchoolCount entry in schoolsWithCounts)
            {
                tblCounts.Rows.Add(addCountRow(entry));
            }
        
        }

        protected void btnSchool_Click(object sender, EventArgs e)
        {
            // Parse the school ID
            int schoolID = -1;
            if (int.TryParse(drpSchoolList.SelectedValue, out schoolID)) 
            {
                // Try to load the school again to make sure it's valid
                School selectedSchool = null;
                List<SkSubmittedEntry> skSubmittedEntries = new List<SkSubmittedEntry>();
                using (SqlConnection connection = new SqlConnection(LSKYCommon.dbConnectionString_SchoolLogic))
                {
                    selectedSchool = School.loadThisSchool(connection, schoolID);
                    if (selectedSchool != null)
                    {
                        skSubmittedEntries = SkSubmittedEntry.loadEntriesFromThisSchool(connection, int.Parse(selectedSchool.getSchoolLogicID()));                        
                    }
                }

                if (skSubmittedEntries.Count > 0)
                {
                    lblEntryCount.Text = "Found " + skSubmittedEntries.Count + " entries for " + selectedSchool.getName();
                    lblEntryCount.Visible = true;

                    tblEntries.Rows.Clear();
                    tblEntries.Rows.Add(addHeaderRow());
                    foreach (SkSubmittedEntry entry in skSubmittedEntries)
                    {
                        tblEntries.Rows.Add(addEntryRow(entry));
                    }

                    tblEntries.Visible = true;
                }
                                

            }
        }
    }
}