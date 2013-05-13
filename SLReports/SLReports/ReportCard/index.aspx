<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.ReportCard.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h3>Report Card (Work In Progress)</h3>
    <br />
    Report generated: <asp:Label ID="lblDate" runat="server" Text=""></asp:Label>
    <br /><br />
    <asp:Table ID="Table1" runat="server">
        <asp:TableRow>
            <asp:TableCell ColumnSpan="2"><asp:Label ID="lblStudentName" runat="server" Text=""></asp:Label></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="200">Home room</asp:TableCell>
            <asp:TableCell>
                <asp:Label ID="lblHomeRoom" runat="server" Text=""></asp:Label></asp:TableCell>
        </asp:TableRow>
    </asp:Table>

</asp:Content>
