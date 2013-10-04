<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="ReportPeriodsByDate.aspx.cs" Inherits="SLReports.ReportPeriods.ReportPeriodsByDate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h2>Open Report Periods</h2>
    <asp:Table ID="tblOpenReportPeriods" runat="server" CssClass="datatable" Width="1200">
        <asp:TableHeaderRow CssClass="datatable_header">
            <asp:TableHeaderCell>Term</asp:TableHeaderCell>
            <asp:TableHeaderCell>Report Period ID</asp:TableHeaderCell>            
            <asp:TableHeaderCell>Report Period Name</asp:TableHeaderCell>
            <asp:TableHeaderCell>School</asp:TableHeaderCell>            
            <asp:TableHeaderCell>Starts</asp:TableHeaderCell>
            <asp:TableHeaderCell>Ends</asp:TableHeaderCell>           
            <asp:TableHeaderCell>Opens</asp:TableHeaderCell>
            <asp:TableHeaderCell>Closes</asp:TableHeaderCell>
            <asp:TableHeaderCell>Days until closed</asp:TableHeaderCell>
        </asp:TableHeaderRow>
    </asp:Table>

    <h2>Upcoming Report Periods</h2>
    <asp:Table ID="tblReportPeriodsUpcoming" runat="server" CssClass="datatable" Width="1200">
        <asp:TableHeaderRow CssClass="datatable_header">
            <asp:TableHeaderCell>Term</asp:TableHeaderCell>
            <asp:TableHeaderCell>Report Period ID</asp:TableHeaderCell>            
            <asp:TableHeaderCell>Report Period Name</asp:TableHeaderCell>
            <asp:TableHeaderCell>School</asp:TableHeaderCell>            
            <asp:TableHeaderCell>Starts</asp:TableHeaderCell>
            <asp:TableHeaderCell>Ends</asp:TableHeaderCell>         
            <asp:TableHeaderCell>Opens</asp:TableHeaderCell>
            <asp:TableHeaderCell>Closes</asp:TableHeaderCell>
            <asp:TableHeaderCell>Days until Open</asp:TableHeaderCell>
            <asp:TableHeaderCell>Days until Closed</asp:TableHeaderCell>
        </asp:TableHeaderRow>
    </asp:Table>

    <h2>Completed Report Periods</h2>
    <asp:Table ID="tblReportPeriodsCompleted" runat="server" Width="1200">
        <asp:TableHeaderRow CssClass="datatable_header">
            <asp:TableHeaderCell>Term</asp:TableHeaderCell>
            <asp:TableHeaderCell>Report Period ID</asp:TableHeaderCell>            
            <asp:TableHeaderCell>Report Period Name</asp:TableHeaderCell>
            <asp:TableHeaderCell>School</asp:TableHeaderCell>            
            <asp:TableHeaderCell>Starts</asp:TableHeaderCell>         
            <asp:TableHeaderCell>Opens</asp:TableHeaderCell>
            <asp:TableHeaderCell>Closes</asp:TableHeaderCell>
            <asp:TableHeaderCell>Ends</asp:TableHeaderCell>
        </asp:TableHeaderRow>
    </asp:Table>
</asp:Content>
