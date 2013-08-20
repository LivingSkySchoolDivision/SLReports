using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.Outcomes
{
    public partial class index : System.Web.UI.Page
    {
        private TableRow addOutcomeRow(Objective outcome)
        {
            TableRow returnMe = new TableRow();

            TableCell courseNameCell = new TableCell();
            courseNameCell.Text = outcome.courseName;

            TableCell courseCodeCell = new TableCell();
            courseCodeCell.Text = outcome.courseCode;

            TableCell subjectCell = new TableCell();
            subjectCell.Text = outcome.subject;

            TableCell notesCell = new TableCell();
            notesCell.Text = outcome.description;

            TableCell categoryCell = new TableCell();
            categoryCell.Text = outcome.category;

            TableCell objectiveIDCell = new TableCell();
            objectiveIDCell.Text = outcome.id.ToString();

            returnMe.Cells.Add(objectiveIDCell);
            returnMe.Cells.Add(subjectCell);
            returnMe.Cells.Add(notesCell);
            returnMe.Cells.Add(categoryCell);
            returnMe.Cells.Add(courseNameCell);
            returnMe.Cells.Add(courseCodeCell);

            return returnMe;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            List<Objective> outcomes = new List<Objective>();

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                outcomes = Objective.loadAllObjectives(connection);
            }

            foreach (Objective outcome in outcomes)
            {
                tblOutcomes.Rows.Add(addOutcomeRow(outcome));
            }
        }
    }
}