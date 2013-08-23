<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="ReportCard_ByGrade.aspx.cs" Inherits="SLReports.ReportCard.ReportCard_ByGrade" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <asp:Table ID="Table1" runat="server">
        <asp:TableRow>
            <asp:TableCell>Choose a school</asp:TableCell>
            <asp:TableCell><asp:DropDownList ID="drpSchools" runat="server"></asp:DropDownList></asp:TableCell>
            <asp:TableCell><asp:Button ID="Button1" runat="server" Text="Submit" /></asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Content>
