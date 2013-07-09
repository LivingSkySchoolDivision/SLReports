<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.Courses.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h2>All Courses <asp:Label ID="lblCourseCount" runat="server" Text=""></asp:Label></h2>
    <img src="../icon_xls.gif" /> <a href="GetCourseListCSV.aspx">Download this list as CSV</a>
    <asp:Table CssClass="datatable" CellPadding="5" ID="tblCourses" runat="server"></asp:Table>
</asp:Content>
