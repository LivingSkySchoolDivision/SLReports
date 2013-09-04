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

    <h3>Recent activity (Most recent 100 events, Newest at top)</h3>
    <asp:Table ID="tblLogins_All" runat="server" CssClass="datatable" Width="850" CellPadding="5">
        <asp:TableHeaderRow CssClass="datatable_header">            
            <asp:TableHeaderCell>Type</asp:TableHeaderCell>
            <asp:TableHeaderCell>Time</asp:TableHeaderCell>
            <asp:TableHeaderCell>Username</asp:TableHeaderCell>
            <asp:TableHeaderCell>IP Address</asp:TableHeaderCell>
            <asp:TableHeaderCell>Info</asp:TableHeaderCell>
        </asp:TableHeaderRow>
    </asp:Table>

    <table>
        <tr>
            <td style="vertical-align: top;">
                <h3>Recent successful logins (Newest at top)</h3>
                <asp:Table ID="tblLogins_Success" runat="server" CssClass="datatable">
                    <asp:TableHeaderRow CssClass="datatable_header">
                        <asp:TableHeaderCell>Time</asp:TableHeaderCell>
                        <asp:TableHeaderCell>Username</asp:TableHeaderCell>
                        <asp:TableHeaderCell>IP Address</asp:TableHeaderCell>
                        <asp:TableHeaderCell>Info</asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                </asp:Table>
            </td>
            <td style="vertical-align: top;">
                <h3>Recent unsuccessful logins (Newest at top)</h3>
                <asp:Table ID="tblLogins_Failure" runat="server" CssClass="datatable">
                    <asp:TableHeaderRow CssClass="datatable_header">
                        <asp:TableHeaderCell>Time</asp:TableHeaderCell>
                        <asp:TableHeaderCell>Entered Username</asp:TableHeaderCell>
                        <asp:TableHeaderCell>IP Address</asp:TableHeaderCell>
                        <asp:TableHeaderCell>Info</asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                </asp:Table>

            </td>
        </tr>
    </table>
    <h3></h3>
    

</asp:Content>
