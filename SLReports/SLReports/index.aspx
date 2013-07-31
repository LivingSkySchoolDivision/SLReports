<%@ Page Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.index" %>
<asp:Content  ContentPlaceHolderID="Main" runat="server">
<div class="large_infobox">
    <h3>About the Data Explorer</h3>
    <p>
        This utility allows you to explore the <b style="color: red;">live</b> student data from the SchoolLogic, Teacherlogic, and Homelogic database.
    </p>
    <p>
        All data on this site is live, and updated in real time.
    </p>    
    <p>
        All data here is <b style="color: red;">read only</b> - it is not possible to edit student data anywhere on this site.
    </p>    
</div>
    <br />
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
            <li><asp:Label ID="lblBirthdayCount" runat="server" Text="" CssClass="small_infobox"></asp:Label> <a href="Birthdays/">students have a birthday today</a></li>
            <li>Students reside in <asp:Label ID="lblCities" runat="server" Text="" CssClass="small_infobox"></asp:Label> communities, in <asp:Label ID="lblRegions" runat="server" Text="" CssClass="small_infobox"></asp:Label> provinces</li>
            <li>There are <asp:Label ID="lblStaffCount" runat="server" Text="" CssClass="small_infobox"></asp:Label> active staff accounts in the SchoolLogic/TeacherLogic system</li>
        </ul>
    </p>    
</div>
<br />
<div class="large_infobox">
    <p>        
        <h3>Recent changes / additions</h3>
        <ul>
            <li><b>July 25th, 2013</b> - <a href="Duplicates/index.aspx">Duplicate student finder</a> - A list of potentially duplicate students or data entry errors in the database</li>
            <li><b>July 9th, 2013</b> - <a href="Courses/CoursesWithclasses.aspx">Classes by Course</a> - The list of courses, with each of it's child classes</li>
            <li><b>July 9th, 2013</b> - <a href="Courses/">Course list</a> - A list of all courses in the system</li>            
            <li><b>June 30th, 2013</b> - <a href="Staff/">Staff account list</a></li>
            <li><b>June 29th, 2013</b> - Added a <a href="Birthdays/index.aspx">Birthday List</a>, which displays student birthday information for the current day, the next day, and the current month.</li>
            
        </ul>
    </p>    
</div>
</asp:Content>