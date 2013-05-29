<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="SingleReportPeriod.aspx.cs" Inherits="SLReports.ReportCard.SingleReportPeriod" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../outcomeBar.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
        <asp:Literal ID="litNamePlate" runat="server"></asp:Literal>
        <asp:Literal ID="litObjectiveKey" runat="server"></asp:Literal>
        <asp:Literal ID="litMarks" runat="server"></asp:Literal>
        <asp:Literal ID="litAttendance" runat="server"></asp:Literal>
</asp:Content>
