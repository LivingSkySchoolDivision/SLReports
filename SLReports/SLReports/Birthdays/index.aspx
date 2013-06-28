<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.Birthdays.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h3>Students with a birthday today <asp:Label ID="lblTodayCount" runat="server" Text=""></asp:Label></h3>
    <asp:Table ID="tblToday" runat="server" CssClass="datatable" CellPadding="3" HorizontalAlign="Center"></asp:Table>
    <br /><br />
    <h3>Students with a birthday tomorrow <asp:Label ID="lblTomorrowCount" runat="server" Text=""></asp:Label></h3>
    <asp:Table ID="tblTomorrow" runat="server" CssClass="datatable" CellPadding="3" HorizontalAlign="Center"></asp:Table>
    <br /><br />
    <h3>Students with a birthday this month <asp:Label ID="lblMonthCount" runat="server" Text=""></asp:Label></h3>
    <asp:Table ID="tblthisMonth" runat="server" CssClass="datatable" CellPadding="3" HorizontalAlign="Center"></asp:Table>

</asp:Content>
