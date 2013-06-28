<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.APIKeys.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h3>Your API Keys</h3>
    <asp:Table ID="tblKeys" CssClass="datatable" runat="server"></asp:Table>
    <br /><br />
    <h3>All API Keys</h3>
    <asp:Table ID="tblAllKeys" CssClass="datatable" runat="server"></asp:Table>
    
</asp:Content>
