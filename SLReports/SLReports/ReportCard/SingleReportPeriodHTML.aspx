﻿<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="SingleReportPeriodHTML.aspx.cs" Inherits="SLReports.ReportCard.SingleReportPeriodHTML" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../outcomeBar.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <div style="width: 800px;">
        <asp:Literal ID="litNamePlate" runat="server"></asp:Literal>
        <asp:Literal ID="litObjectiveKey" runat="server"></asp:Literal>
        <asp:Literal ID="litMarks" runat="server"></asp:Literal>
        <asp:Literal ID="litAttendance" runat="server"></asp:Literal>
    </div>
</asp:Content>