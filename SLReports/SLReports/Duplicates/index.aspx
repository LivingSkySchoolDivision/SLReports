<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.Duplicates.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h2>All duplicates <asp:Label ID="lblAllCount" runat="server" Text=""></asp:Label></h2>    
    <img src="../icon_xls.gif" /> <a href="GetDuplicatesCSV.aspx">Download this list as CSV</a>
    <asp:Table ID="tblAllDuplicates" runat="server" CssClass="datatable" CellPadding="5">
        <asp:TableHeaderRow CssClass="datatable_header">
            <asp:TableHeaderCell>Student Number</asp:TableHeaderCell>
            <asp:TableHeaderCell ColumnSpan="3">Student Name</asp:TableHeaderCell>
            <asp:TableHeaderCell>Government ID</asp:TableHeaderCell>
            <asp:TableHeaderCell>Gender</asp:TableHeaderCell>
            <asp:TableHeaderCell>Date of Birth</asp:TableHeaderCell>
            <asp:TableHeaderCell>School</asp:TableHeaderCell>
            <asp:TableHeaderCell>Reason</asp:TableHeaderCell>
        </asp:TableHeaderRow>
    </asp:Table>

    <h2>Just Duplicate Government IDs <asp:Label ID="lblIdsCount" runat="server" Text=""></asp:Label></h2>
    <asp:Table ID="tblGovIDs" runat="server" CssClass="datatable" CellPadding="5">
        <asp:TableHeaderRow CssClass="datatable_header">
            <asp:TableHeaderCell>Student Number</asp:TableHeaderCell>
            <asp:TableHeaderCell ColumnSpan="3">Student Name</asp:TableHeaderCell>
            <asp:TableHeaderCell>Government ID</asp:TableHeaderCell>
            <asp:TableHeaderCell>Gender</asp:TableHeaderCell>
            <asp:TableHeaderCell>Date of Birth</asp:TableHeaderCell>
            <asp:TableHeaderCell>School</asp:TableHeaderCell>
        </asp:TableHeaderRow>
    </asp:Table>

    <h2>Just Duplicate Names <asp:Label ID="lblNamesCount" runat="server" Text=""></asp:Label></h2>
    <asp:Table ID="tblNames" runat="server" CssClass="datatable" CellPadding="5">
        <asp:TableHeaderRow CssClass="datatable_header">
            <asp:TableHeaderCell>Student Number</asp:TableHeaderCell>
            <asp:TableHeaderCell ColumnSpan="3">Student Name</asp:TableHeaderCell>
            <asp:TableHeaderCell>Government ID</asp:TableHeaderCell>
            <asp:TableHeaderCell>Gender</asp:TableHeaderCell>
            <asp:TableHeaderCell>Date of Birth</asp:TableHeaderCell>
            <asp:TableHeaderCell>School</asp:TableHeaderCell>
        </asp:TableHeaderRow>
    </asp:Table>

    <h2>Just Duplicate Treaty Numbers <asp:Label ID="lblTreaty" runat="server" Text=""></asp:Label></h2>
    <asp:Table ID="tblTreaty" runat="server" CssClass="datatable" CellPadding="5">
        <asp:TableHeaderRow CssClass="datatable_header">
            <asp:TableHeaderCell>Student Number</asp:TableHeaderCell>
            <asp:TableHeaderCell ColumnSpan="3">Student Name</asp:TableHeaderCell>
            <asp:TableHeaderCell>Government ID</asp:TableHeaderCell>
            <asp:TableHeaderCell>Treaty Status No</asp:TableHeaderCell>
            <asp:TableHeaderCell>Gender</asp:TableHeaderCell>
            <asp:TableHeaderCell>Date of Birth</asp:TableHeaderCell>
            <asp:TableHeaderCell>School</asp:TableHeaderCell>
        </asp:TableHeaderRow>
    </asp:Table>
</asp:Content>
