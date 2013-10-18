<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="StudentIETasks.aspx.cs" Inherits="SLReports.GradeBook.StudentIETasks" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h2>Tasks with IE marks</h2>
    <form runat="server">
    <asp:Table ID="Table1" runat="server">
        <asp:TableRow>
            <asp:TableCell VerticalAlign="Top"><b>Choose a school</b></asp:TableCell>
            <asp:TableCell VerticalAlign="Top"><asp:DropDownList ID="drpSchools" runat="server"></asp:DropDownList></asp:TableCell>
            <asp:TableCell VerticalAlign="Top"><asp:Button ID="btnSchool" runat="server" Text="Select School" OnClick="btnSchool_Click" /></asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    
    <asp:Table ID="tblStudents" runat="server" CssClass="datatable" Width="900">

    </asp:Table>
    </form>
</asp:Content>
