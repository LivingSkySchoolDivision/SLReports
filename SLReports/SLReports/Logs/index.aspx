﻿<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.Logs.index" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h3>Current Sessions:</h3>
    <asp:Table ID="tblSessions" runat="server">
        <asp:TableHeaderRow CssClass="datatable_header">
            <asp:TableHeaderCell>Username</asp:TableHeaderCell>
            <asp:TableHeaderCell>Session Start</asp:TableHeaderCell>
            <asp:TableHeaderCell>Session expires</asp:TableHeaderCell>
            <asp:TableHeaderCell>IP</asp:TableHeaderCell>
        </asp:TableHeaderRow>
    </asp:Table>

    <h3>Recent activity (Most recent 100 events, Newest at top)</h3>
    <asp:Table ID="tblLogins_All" runat="server" CssClass="datatable" Width="1050" CellPadding="5">
        <asp:TableHeaderRow CssClass="datatable_header">            
            <asp:TableHeaderCell>Type</asp:TableHeaderCell>
            <asp:TableHeaderCell>Time</asp:TableHeaderCell>
            <asp:TableHeaderCell>Username</asp:TableHeaderCell>
            <asp:TableHeaderCell>IP Address</asp:TableHeaderCell>
            <asp:TableHeaderCell>Info</asp:TableHeaderCell>
            <asp:TableHeaderCell>User Agent</asp:TableHeaderCell>
        </asp:TableHeaderRow>
    </asp:Table>
</asp:Content>
