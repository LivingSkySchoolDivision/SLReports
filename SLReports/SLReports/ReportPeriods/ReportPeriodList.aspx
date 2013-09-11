<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="ReportPeriodList.aspx.cs" Inherits="SLReports.ReportPeriods.ReportPeriodList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">

    <h2>Report Periods</h2>
    <asp:Table ID="tblReportPeriodsUpcoming" runat="server" CssClass="datatable" Width="900">
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

</asp:Content>
