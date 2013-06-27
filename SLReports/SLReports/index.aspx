<%@ Page Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.index" %>
<asp:Content  ContentPlaceHolderID="Main" runat="server">

<div class="large_infobox">
    <b>About the Data Explorer</b><br />
    <p>
        This utility allows you to explore the <b style="color: red;">live</b> student data from the SchoolLogic, Teacherlogic, and Homelogic database.
    </p>
    <p>
        All data on this site is live, and updated in real time.
    </p>
    <p>
        Select a page/report from the dropdown list at the top of the page.
    </p>
    <p>
        There are currently <b><asp:Label ID="lblActiveStudentCount" runat="server" Text="" CssClass="small_infobox"></asp:Label></b> active students in the database, in <b><asp:Label ID="lblSchoolCount" runat="server" Text="" CssClass="small_infobox"></asp:Label></b> schools. 
        <b><asp:Label ID="lblMaleCount" runat="server" Text="" CssClass="small_infobox"></asp:Label></b> are male, <b><asp:Label ID="lblFemaleCount" runat="server" Text="" CssClass="small_infobox"></asp:Label></b> are female, and <b><asp:Label ID="lblBirthdayCount" runat="server" Text="" CssClass="small_infobox"></asp:Label></b> have a birthday today. Students travel from <b><asp:Label ID="lblCities" runat="server" Text="" CssClass="small_infobox"></asp:Label></b> communities to go to our schools.
    </p>    
    <p>
        If you experience a problem with this site, or would like to request a feature, please create a <a href="https://helpdesk.lskysd.ca">Help Desk</a> ticket.
    </p>
</div>

</asp:Content>