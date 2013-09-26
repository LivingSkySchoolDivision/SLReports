<%@ Page Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.StudentList.index" %>
<asp:Content ID="Content1"  ContentPlaceHolderID="Main" runat="server">
    <h3>Currently Enrolled Student List</h3>
    <form runat="server">
        <asp:Literal ID="litCSVLink" runat="server">
            <a href="https://sldata.lskysd.ca/SLReports/StudentList/allStudentsCSV.aspx"><img src="/SLReports/icon_xls.gif" />Download CSV (All Schools)</a><br /><br />
        </asp:Literal>
    </form>    

    <% this.buildSchoolDropdown(Schools); %>
    <br />
    <% this.buildStatisticsTable(DisplayedStudents); %>
    <br />
    <% this.buildStudentTable(DisplayedStudents); %>
</asp:Content>