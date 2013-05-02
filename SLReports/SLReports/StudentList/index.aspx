<%@ Page Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.StudentList.index" %>
<asp:Content ID="Content1"  ContentPlaceHolderID="Main" runat="server">
    <h3>Currently Enrolled Student List</h3>  
    <% this.buildSchoolDropdown(Schools); %>
    <br />
    <% this.buildGradeStatisticsTable(DisplayedStudents); %>
    <br />
    <% this.buildStudentTable(DisplayedStudents); %>
</asp:Content>