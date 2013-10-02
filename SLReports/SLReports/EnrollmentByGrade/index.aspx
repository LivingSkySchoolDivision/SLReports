<%@ Page Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.DivisionStats.index" %>
<asp:Content ID="Content1"  ContentPlaceHolderID="Main" runat="server">    
    <form runat="server">
        <img src="../icon_xls.gif" /> <a href="getCSV.aspx">Download as CSV</a>
        <asp:Table ID="tblSchoolStats" runat="server" CssClass="datatable" CellPadding="5"></asp:Table>
    </form>
</asp:Content>