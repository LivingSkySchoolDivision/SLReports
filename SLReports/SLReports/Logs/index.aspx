<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.Logs.index" %>
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

</asp:Content>
