<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="CoursesWithClasses.aspx.cs" Inherits="SLReports.Courses.CoursesWithClasses" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h2>All Courses with Classes</h2>
    <div class="large_infobox">
        Total courses: <asp:Label ID="lblCourseCount" runat="server" Text=""></asp:Label><br />
        Total classes: <asp:Label ID="lblClassCount" runat="server" Text=""></asp:Label><br />
    </div>
    <br />
    <asp:Table CssClass="datatable" CellPadding="5" ID="tblCourses" runat="server"></asp:Table>
</asp:Content>
