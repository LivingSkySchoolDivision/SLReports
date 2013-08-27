<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="ByGrade.aspx.cs" Inherits="SLReports.StudentList.ByGrade" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h2>Students by Grade</h2>
    <form runat="server">
        <asp:DropDownList ID="drpSchools" runat="server"></asp:DropDownList>
        <asp:Button ID="btnChooseSchool" runat="server" Text="Select School" OnClick="btnChooseSchool_Click" />
    </form>    
    <br />
    <asp:Label ID="lblAnchors" runat="server" Text=""></asp:Label>
    <br />
    <asp:Table ID="tblStudents" runat="server" Visible="false" CssClass="datatable" CellPadding="4">

    </asp:Table>
</asp:Content>
