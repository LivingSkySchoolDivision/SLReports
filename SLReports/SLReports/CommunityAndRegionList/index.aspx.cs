using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.CommunityAndRegionList
{
    public partial class index : System.Web.UI.Page
    {
        Random random = new Random();

        private TableRow addRow(KeyValuePair<string, List<Student>> community)
        {
            TableRow newRow = new TableRow();

            TableCell nameCell = new TableCell();
            TableCell valueCell = new TableCell();

            StringBuilder toolTip = new StringBuilder();

                       
            foreach (Student student in community.Value)
            {
                toolTip.Append(student.getStudentID() + " " + student.getDisplayName() + " (" + student.getSchoolName() + ")<br>&#10;");
            }

            nameCell.Text = community.Key;
            nameCell.VerticalAlign = VerticalAlign.Top;


            string divName = LSKYCommon.removeSpaces(community.Key) + "_" + random.Next(1000, 9999);

            valueCell.Text = "<a style=\"color: blue; text-decoration: underline; cursor:pointer;\" onclick=\"toggleVisible('" + divName + "');\">" + community.Value.Count.ToString() + "</a><div id=\"" + divName + "\" style=\"display: none;\">" + toolTip.ToString() + "</div>";
            valueCell.VerticalAlign = VerticalAlign.Top;
            
            newRow.Cells.Add(nameCell);
            newRow.Cells.Add(valueCell);

            return newRow;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            List<Student> allStudents = new List<Student>();

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                allStudents = Student.loadAllStudents(connection);
            }

            SortedDictionary<string, List<Student>> regions = new SortedDictionary<string, List<Student>>();
            SortedDictionary<string, List<Student>> communities = new SortedDictionary<string, List<Student>>();

            foreach (Student student in allStudents)
            {
                if (!regions.ContainsKey(student.getRegion()))
                {
                    regions.Add(student.getRegion(), new List<Student>());
                }

                if (!communities.ContainsKey(student.getCity()))
                {
                    communities.Add(student.getCity(), new List<Student>());
                }

                regions[student.getRegion()].Add(student);
                communities[student.getCity()].Add(student);
            }

            foreach (KeyValuePair<string, List<Student>> community in communities)
            {
                tblCommunities.Rows.Add(addRow(community));
            }

            foreach (KeyValuePair<string, List<Student>> community in regions)
            {
                tblRegions.Rows.Add(addRow(community));
            }
            

        }
    }
}