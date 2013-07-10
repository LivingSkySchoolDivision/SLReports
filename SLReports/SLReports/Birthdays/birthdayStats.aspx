<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="birthdayStats.aspx.cs" Inherits="SLReports.Birthdays.birthdayStats" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h3>Student birthday statistics</h3>
    <h4>Birthday count by month</h4>
    <asp:Table ID="tblMonths" runat="server" CssClass="datatable">
        <asp:TableHeaderRow CssClass="datatable_header">
            <asp:TableCell Width="150">Month</asp:TableCell>
            <asp:TableCell width="50">Count</asp:TableCell>
            <asp:TableCell Width="50">Percent</asp:TableCell>
        </asp:TableHeaderRow>
    </asp:Table>
    
    <h4>Birthday count by day of the week</h4>
    <asp:Table ID="tblDays" runat="server" CssClass="datatable">
        <asp:TableHeaderRow CssClass="datatable_header">
            <asp:TableCell Width="150">Day</asp:TableCell>
            <asp:TableCell width="50">Count</asp:TableCell>
            <asp:TableCell Width="50">Percent</asp:TableCell>
        </asp:TableHeaderRow>
    </asp:Table>
</asp:Content>
