<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="ReportPeriodsByDate.aspx.cs" Inherits="SLReports.ReportPeriods.ReportPeriodsByDate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h2>Upcoming Report Periods</h2>
    <asp:Table ID="tblReportPeriodsUpcoming" runat="server" CssClass="datatable" Width="1200">
        <asp:TableRow CssClass="datatable_header">
            <asp:TableCell>Term</asp:TableCell>
            <asp:TableCell>Report Period ID</asp:TableCell>            
            <asp:TableCell>Report Period Name</asp:TableCell>
            <asp:TableCell>School</asp:TableCell>            
            <asp:TableCell>Starts</asp:TableCell>
            <asp:TableCell>Ends</asp:TableCell>
            <asp:TableCell>Days left</asp:TableCell>
        </asp:TableRow>
    </asp:Table>

    <h2>Completed Report Periods</h2>
    <asp:Table ID="tblReportPeriodsCompleted" runat="server" Width="1200">
        <asp:TableRow CssClass="datatable_header">
            <asp:TableCell>Term</asp:TableCell>
            <asp:TableCell>Report Period ID</asp:TableCell>            
            <asp:TableCell>Report Period Name</asp:TableCell>
            <asp:TableCell>School</asp:TableCell>            
            <asp:TableCell>Starts</asp:TableCell>
            <asp:TableCell>Ends</asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Content>
