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
        private TableRow addOutcomeRow(Outcome outcome)
        {
            TableRow returnMe = new TableRow();

            TableCell courseNameCell = new TableCell();
            courseNameCell.Text = outcome.courseName;

            TableCell courseCodeCell = new TableCell();
            courseCodeCell.Text = outcome.courseCode;

            TableCell subjectCell = new TableCell();
            subjectCell.Text = outcome.subject;

            TableCell notesCell = new TableCell();
            notesCell.Text = outcome.notes;

            TableCell categoryCell = new TableCell();
            categoryCell.Text = outcome.category;

            TableCell objectiveIDCell = new TableCell();
            objectiveIDCell.Text = outcome.id.ToString();

            TableCell markCountCell = new TableCell();
            if (outcome.marks.Count > 0)
            {
                markCountCell.Text = outcome.marks.Count.ToString();
            }

            returnMe.Cells.Add(objectiveIDCell);
            returnMe.Cells.Add(subjectCell);
            returnMe.Cells.Add(notesCell);
            returnMe.Cells.Add(categoryCell);
            returnMe.Cells.Add(courseNameCell);
            returnMe.Cells.Add(courseCodeCell);
            returnMe.Cells.Add(markCountCell);

            return returnMe;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            List<Outcome> outcomes = new List<Outcome>();
            List<OutcomeMark> allOutcomeMarks = new List<OutcomeMark>();

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                outcomes = Outcome.loadAllObjectives(connection);
                allOutcomeMarks = OutcomeMark.loadAllOutcomeMarks(connection);
            }

            // Add marks to their respective outcomes
            foreach (Outcome outcome in outcomes)
            {
                foreach (OutcomeMark om in allOutcomeMarks)
                {
                    if (outcome.id == om.objectiveID)
                    {
                        outcome.marks.Add(om);
                    }
                }
            }



            // add SLBs to the table
            // Sort by course name
            outcomes.Sort(
                delegate(Outcome first,
                Outcome next)
                {
                    return first.courseName.CompareTo(next.courseName);
                }
                );

            foreach (Outcome outcome in outcomes)
            {
                if (outcome.category == "Successful Learner Behaviours")
                {
                    tblSLBs.Rows.Add(addOutcomeRow(outcome));
                }
            }


            // add non-slbs to the other table
            // Sort by subject
            outcomes.Sort(
                delegate(Outcome first,
                Outcome next)
                {
                    return first.subject.CompareTo(next.subject);
                }
                );

            foreach (Outcome outcome in outcomes)
            {
                if (outcome.category != "Successful Learner Behaviours")
                {
                    tblOutcomes.Rows.Add(addOutcomeRow(outcome));
                }
            }
        }
    }
}