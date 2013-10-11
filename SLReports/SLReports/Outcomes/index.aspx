<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.Outcomes.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">

    <a href="#lifeskills">Go to life skills</a>

    <h2>Outcomes</h2>
    <img src="../icon_xls.gif" /> <a href="OutcomesCSV.aspx">Download this list as CSV</a><br /><br />
    <asp:Table ID="tblOutcomes" runat="server" CssClass="datatable">
        <asp:TableHeaderRow CssClass="datatable_header">
            <asp:TableHeaderCell CssClass="datatable_header">Objective ID</asp:TableHeaderCell>
            <asp:TableHeaderCell CssClass="datatable_header">Subject</asp:TableHeaderCell>
            <asp:TableHeaderCell CssClass="datatable_header">Notes</asp:TableHeaderCell>
            <asp:TableHeaderCell CssClass="datatable_header">Category</asp:TableHeaderCell>
            <asp:TableHeaderCell CssClass="datatable_header">Course Name</asp:TableHeaderCell>
            <asp:TableHeaderCell CssClass="datatable_header">Course Code</asp:TableHeaderCell>
            <asp:TableHeaderCell CssClass="datatable_header">Mark Count</asp:TableHeaderCell>
        </asp:TableHeaderRow>
    </asp:Table>

    <h2>Life Skills</h2><a name="lifeskills"></a>
    <asp:Table ID="tblSLBs" runat="server" CssClass="datatable">
        <asp:TableHeaderRow CssClass="datatable_header">
            <asp:TableHeaderCell CssClass="datatable_header">Objective ID</asp:TableHeaderCell>
            <asp:TableHeaderCell CssClass="datatable_header">Subject</asp:TableHeaderCell>
            <asp:TableHeaderCell CssClass="datatable_header">Notes</asp:TableHeaderCell>
            <asp:TableHeaderCell CssClass="datatable_header">Category</asp:TableHeaderCell>
            <asp:TableHeaderCell CssClass="datatable_header">Course Name</asp:TableHeaderCell>
            <asp:TableHeaderCell CssClass="datatable_header">Course Code</asp:TableHeaderCell>
            <asp:TableHeaderCell CssClass="datatable_header">Mark Count</asp:TableHeaderCell>
        </asp:TableHeaderRow>
    </asp:Table>
</asp:Content>
