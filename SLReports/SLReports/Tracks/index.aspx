<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.Tracks.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h2>All Tracks <asp:Label ID="lblTrackCount" runat="server" Text=""></asp:Label></h2>
    <asp:Table ID="tblTracks" CellPadding="5" CssClass="datatable" runat="server"></asp:Table>
</asp:Content>
