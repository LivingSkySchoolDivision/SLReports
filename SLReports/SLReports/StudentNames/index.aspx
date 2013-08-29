<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.StudentNames.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <asp:Label ID="lblCount" runat="server" Text=""></asp:Label>
    <asp:Table ID="tblStudents" runat="server" CssClass="datatable" CellPadding="5">
        <asp:TableHeaderRow CssClass="datatable_header">
            <asp:TableCell>Student ID</asp:TableCell>
            <asp:TableCell>Legal First Name</asp:TableCell>
            <asp:TableCell>Display First Name</asp:TableCell>
            <asp:TableCell>Legal Middle Name</asp:TableCell>
            <asp:TableCell>Legal Last Name</asp:TableCell>
            <asp:TableCell>Display Last Name</asp:TableCell>
            <asp:TableCell>School</asp:TableCell>
        </asp:TableHeaderRow>
    </asp:Table>
</asp:Content>
