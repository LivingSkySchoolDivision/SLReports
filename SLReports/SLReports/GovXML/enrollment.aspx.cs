using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SLReports.GovXML
{
    public partial class enrollment : System.Web.UI.Page
    {

        public void create_student_node(Student thisStudent)
        {
            
            if (thisStudent != null)
            {
                Response.Write("<StudentSchoolenrollment RefID=\"" + thisStudent.getStudentID() + "\">");

                Response.Write("<StudentIdentification>");
                Response.Write("<DeptAssignedPersonId>"+thisStudent.getGovernmentID()+"</DeptAssignedPersonId>");
                Response.Write("<BirthDate>" + thisStudent.getDateOfBirth().ToString("yyyy-MM-dd") + "</BirthDate>");
                Response.Write("</StudentIdentification>");
                
                Response.Write("<StudentInfo>");
                Response.Write("<SchoolAssignedPersonId>"+thisStudent.getStudentID()+"</SchoolAssignedPersonId>");
                Response.Write("<HSN/>"); /* Health Services number */
                Response.Write("<Name Type=\"Legal\">");
                Response.Write("<LastName>"+thisStudent.getSN()+"</LastName>");
                Response.Write("<FirstName>"+thisStudent.getGivenName()+"</FirstName>");
                Response.Write("<MiddleName>"+thisStudent.getMiddleName()+"</MiddleName>");
                Response.Write("<PreferredName>"+thisStudent.getGivenName()+"</PreferredName>");
                Response.Write("</Name>");
                Response.Write("<Name Type=\"Alias\">");
                Response.Write("<LastName>" + thisStudent.getSN() + "</LastName>");
                Response.Write("<FirstName>" + thisStudent.getGivenName() + "</FirstName>");
                Response.Write("<MiddleName>" + thisStudent.getMiddleName() + "</MiddleName>");
                Response.Write("<PreferredName>" + thisStudent.getGivenName() + "</PreferredName>");
                Response.Write("</Name>");
                Response.Write("<Demographics>");
                Response.Write("<Gender>"+thisStudent.getGender().Substring(0,1).ToUpper()+"</Gender>");
                Response.Write("<CountryOfBirth Code=\"\"/>");
                Response.Write("<CountryOfCitizenShip Code=\"\"/>");
                Response.Write("<CountryOfOrigin Code=\"\"/>");
                Response.Write("<Language Code=\"\"/>");
                Response.Write("</Demographics>");
                Response.Write("<StudentAddress>");
                Response.Write("<Address Type=\"Mailing\">");
                Response.Write("<Street>");
                Response.Write("<StreetNumber>"+ thisStudent.getHouseNo()+"</StreetNumber>");
                Response.Write("<StreetName>" + thisStudent.getStreet() + "</StreetName>");
                Response.Write("<StreetSuffix></StreetSuffix>");
                Response.Write("<AptNumber>"+thisStudent.getApartmentNo()+"</AptNumber>");
                Response.Write("</Street>");
                Response.Write("<City>"+thisStudent.getCity()+"</City>");
                Response.Write("<StatePr Code=\""+thisStudent.getRegion()+"\"></StatePr>");
                Response.Write("<Country Code=\"\"/>");
                Response.Write("<PostalCode>"+thisStudent.getPostalCode()+"</PostalCode>");
                Response.Write("</Address>");
                Response.Write("</StudentAddress>");
                Response.Write("<PhoneNumber Type=\"Home\" Format=\"NA\">"+thisStudent.getTelephoneFormatted()+"</PhoneNumber>");
                Response.Write("<SaskResident>");
                if (thisStudent.getRegion().ToLower().Equals("sk"))
                {
                    Response.Write("Yes");
                }
                else
                {
                    Response.Write("No");
                }
                Response.Write("</SaskResident>");
                Response.Write("</StudentInfo>");

                Response.Write("<SchoolId>"+thisStudent.getSchoolID()+"</SchoolId>");

                Response.Write("<SchoolEnrollmentInfo>");
                Response.Write("<EntryDate>" + thisStudent.getEnrollDate().ToString("yyyy-MM-dd") + "</EntryDate>");
                Response.Write("<EntryType Code=\"\"/>");
                Response.Write("<Grade Code=\""+thisStudent.getGradeFormatted()+"\"/>");
                Response.Write("<PreviousProvState Code=\"\"/>");
                Response.Write("<PreviousCountry Code=\"\"/>");
                Response.Write("<ImmersionType Code=\"\"/>");
                Response.Write("<HomeBound Code=\"\"/>");
                Response.Write("<CumulativeFolder Code=\"\"/>");
                Response.Write("</SchoolEnrollmentInfo>");

                Response.Write("<DeptAssignedProgramId/>");

                Response.Write("<ProgramEnrollmentInfo>");
                Response.Write("<EntryDate>"+thisStudent.getEnrollDate().ToString("yyyy-MM-dd")+"</EntryDate>");
                Response.Write("</ProgramEnrollmentInfo>");

                Response.Write("</StudentSchoolenrollment>");
                Response.Write("\n");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "text/xml; charset=utf-8";

            String dbConnectionString = ConfigurationManager.ConnectionStrings["SchoolLogicDatabase"].ConnectionString;
            Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            Response.Write("<SL_Message xmlns=\"http://www.k12.gov.sk.ca/xsd/sl/1.x/SLMessage.xsd\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.k12.gov.sk.ca/xsd/sl/1.x/SLMessage.xsd http://www.k12.gov.sk.ca/xsd/sl/1.x/SLMessage.xsd\"><SL_Event>");

            Response.Write("<SL_Header>");
            Response.Write("</SL_Header>");

            Response.Write("<SL_ObjectData>");

            /* How can I determine if this is an add or a change? */
            /* Will this basically mean programming an identity manager like FIM? */

            Response.Write("<SL_EventObject Action=\"Change\" ObjectName=\"StudentSchoolEnrollment\">");

            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                Student SelectedStudent = Student.loadThisStudent(connection, "600000367");
                if (SelectedStudent != null)
                {
                    create_student_node(SelectedStudent);
                }
            }
            Response.Write("</SL_EventObject>");
            Response.Write("</SL_ObjectData>");
            Response.Write("</SL_Event></SL_Message>");
            Response.End();
        }
    }
}