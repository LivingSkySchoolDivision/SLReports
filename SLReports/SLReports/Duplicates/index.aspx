<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.Duplicates.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h2>Duplicate Government IDs <asp:Label ID="lblIdsCount" runat="server" Text=""></asp:Label></h2>
    <asp:Table ID="tblGovIDs" runat="server" CssClass="datatable" CellPadding="5">
        <asp:TableHeaderRow CssClass="datatable_header">
            <asp:TableHeaderCell>Student Number</asp:TableHeaderCell>
            <asp:TableHeaderCell>Student Name</asp:TableHeaderCell>
            <asp:TableHeaderCell>Government ID</asp:TableHeaderCell>
            <asp:TableHeaderCell>Gender</asp:TableHeaderCell>
            <asp:TableHeaderCell>Date of Birth</asp:TableHeaderCell>
            <asp:TableHeaderCell>School</asp:TableHeaderCell>
        </asp:TableHeaderRow>
    </asp:Table>

    <h2>Duplicate Names <asp:Label ID="lblNamesCount" runat="server" Text=""></asp:Label></h2>
    <asp:Table ID="tblNames" runat="server" CssClass="datatable" CellPadding="5">
        <asp:TableHeaderRow CssClass="datatable_header">
            <asp:TableHeaderCell>Student Number</asp:TableHeaderCell>
            <asp:TableHeaderCell>Student Name</asp:TableHeaderCell>
            <asp:TableHeaderCell>Government ID</asp:TableHeaderCell>
            <asp:TableHeaderCell>Gender</asp:TableHeaderCell>
            <asp:TableHeaderCell>Date of Birth</asp:TableHeaderCell>
            <asp:TableHeaderCell>School</asp:TableHeaderCell>
        </asp:TableHeaderRow>
    </asp:Table>
</asp:Content>
