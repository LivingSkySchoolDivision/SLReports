<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.MiniDashboard.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <div class="large_infobox">
    <p>        
        <h3>Moderately interesting statistics</h3>
        <ul>
            <li>There are currently <asp:Label ID="lblActiveStudentCount" runat="server" Text="" CssClass="small_infobox"></asp:Label> active students in the database, in <asp:Label ID="lblSchoolCount" runat="server" Text="" CssClass="small_infobox"></asp:Label> schools.</li>            
            <ul>                
                <li><small><i>Note:</i> This number <i>is</i> accurate - SchoolLogic is more selective in the students it lists as "currently enrolled".</small></li>
            </ul>
            <li><asp:Label ID="lblMalePercent" runat="server" Text="" CssClass="small_infobox"></asp:Label> <asp:Label ID="lblMaleCount" runat="server" Text="" CssClass="small_infobox"></asp:Label> students are male </li>
            <li><asp:Label ID="lblFemalePercent" runat="server" Text="" CssClass="small_infobox"></asp:Label> <asp:Label ID="lblFemaleCount" runat="server" Text="" CssClass="small_infobox"></asp:Label> students are female </li>
            <li><asp:Label ID="lblBirthdayCount" runat="server" Text="" CssClass="small_infobox"></asp:Label> students have a birthday today</li>
            <li>Students reside in <asp:Label ID="lblCities" runat="server" Text="" CssClass="small_infobox"></asp:Label> communities, in <asp:Label ID="lblRegions" runat="server" Text="" CssClass="small_infobox"></asp:Label> provinces</li>
            <li>There are <asp:Label ID="lblStaffCount" runat="server" Text="" CssClass="small_infobox"></asp:Label> active staff accounts in the SchoolLogic/TeacherLogic system</li>
            <li>There are <asp:Label ID="lblActiveSessions" runat="server" Text="" CssClass="small_infobox"></asp:Label> active sessions on this website right now, including <asp:Label ID="lblAdminSessions" runat="server" Text="" CssClass="small_infobox"></asp:Label> administrators.</li>
        </ul>
    </p>    
</div>
</asp:Content>
