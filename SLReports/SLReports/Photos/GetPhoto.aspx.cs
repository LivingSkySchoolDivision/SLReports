using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.Photos
{
    public partial class GetPhoto : System.Web.UI.Page
    {
        String dbUser = @"sql_readonly";
        String dbPassword = @"XTXVDUNHlrdbefjTBgY4";
        String dbHost = "dcsql.lskysd.ca";
        String dbDatabase = "SchoolLogicDB";

        protected void Page_Load(object sender, EventArgs e)
        {
            string StudNum = Request.QueryString["studentnumber"];

            if (!string.IsNullOrEmpty(StudNum))
            {

                String dbConnectionString = "data source=" + dbHost + ";initial catalog=" + dbDatabase + ";user id=" + dbUser + ";password=" + dbPassword + ";Trusted_Connection=false";
                SqlConnection dbConnection = new SqlConnection(dbConnectionString);
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = dbConnection;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = "SELECT Photo, PhotoType FROM LSKY_ActiveStudents WHERE StudentNumber=@StudNum";
                sqlCommand.Parameters.AddWithValue("@StudNum", StudNum);

                sqlCommand.Connection.Open();
                SqlDataReader dbDataReader = sqlCommand.ExecuteReader();

                if (dbDataReader.HasRows)
                {
                    if (dbDataReader.Read())
                    {
                        Response.Clear();
                        Response.ContentType = dbDataReader["PhotoType"].ToString();
                        Response.BinaryWrite((byte[])dbDataReader["Photo"]);
                        Response.End();
                    }
                }

                sqlCommand.Connection.Close();
            }

        }
    }
}