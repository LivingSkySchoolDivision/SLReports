<%@ Page Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.INAC.index" %>
<asp:Content ID="Content1"  ContentPlaceHolderID="Main" runat="server">
    <h3>INAC Report</h3>
    <div class="large_infobox">Absences are listed in blocks rather than days, because the meaning of a block differs from school to school, and in some cases from student to student. A block may represent a single class, or an entire day, depending on the school.</div>
    <% this.buildStudentTable(AllStudents); %>
    Students: <% Response.Write(AllStudents.Count); %>
</asp:Content>