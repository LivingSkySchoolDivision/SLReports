<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="ReportPeriodList.aspx.cs" Inherits="SLReports.ReportPeriods.ReportPeriodList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">

    <h2>Report Periods</h2>
    <asp:Table ID="tblReportPeriodsUpcoming" runat="server" CssClass="datatable" Width="1100">
        <asp:TableHeaderRow CssClass="datatable_header">
            <asp:TableHeaderCell>Term</asp:TableHeaderCell>
            <asp:TableHeaderCell>Report Period ID</asp:TableHeaderCell>            
            <asp:TableHeaderCell>Report Period Name</asp:TableHeaderCell>
            <asp:TableHeaderCell>School</asp:TableHeaderCell>            
            <asp:TableHeaderCell>Starts</asp:TableHeaderCell>
            <asp:TableHeaderCell>Ends</asp:TableHeaderCell>
            <asp:TableHeaderCell>Days left</asp:TableHeaderCell>        
            <asp:TableHeaderCell>Opens</asp:TableHeaderCell>
            <asp:TableHeaderCell>Closes</asp:TableHeaderCell>
        </asp:TableHeaderRow>
    </asp:Table>

</asp:Content>
