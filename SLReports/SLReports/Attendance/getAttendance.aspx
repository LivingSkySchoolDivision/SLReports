<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="getAttendance.aspx.cs" Inherits="SLReports.Attendance.getAttendance" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h2>Attendance: <asp:Label ID="lblStudentName" runat="server" Text=""></asp:Label></h2>
    <asp:Label ID="lblDates" runat="server" Text=""></asp:Label>
    <br />
    <asp:Label ID="lblError" runat="server" Text="" Visible="false"></asp:Label>
    <br />
    <asp:Table ID="tblAbsences" runat="server" CssClass="datatable" CellPadding="5" Width="800" Visible="false">
        <asp:TableHeaderRow CssClass="datatable_header">
            <asp:TableCell>Date & Time</asp:TableCell>
            <asp:TableCell>Block</asp:TableCell>
            <asp:TableCell>Status</asp:TableCell>
            <asp:TableCell>Reason</asp:TableCell>
            <asp:TableCell>Excused</asp:TableCell>
            <asp:TableCell>Comment</asp:TableCell>
        </asp:TableHeaderRow>
    </asp:Table>
</asp:Content>
