<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.APIKeys.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h3>What are API Keys?</h3>
    <div style="padding-left: 10px;">
        <p>
            API Keys allow you to use data without the need to log in with a username and password.
        </p>
        <p>
            An example of when this would be useful, is if you wanted to import data from a dynamic XML or CSV file generated on this site into Microsoft Excel. This site's login system does not allow you remain logged in indefinitely, so eventually you would have to re-enter your username and password. This can make embedding data into documents or scripts difficult or impossible. Using an API key, your script or document can access data without having to enter a username and password, in a way that is still secure and controllable.
        </p>
    </div>
    <h3>Your API Keys</h3>
    <div style="padding-left: 10px;">
    <asp:Table ID="tblKeys" CssClass="datatable" runat="server"></asp:Table>
    <br />
    <div style="text-align: left; padding: 5px; padding-left: 10px;">
        <form id="Form1" runat="server">
            <asp:TextBox ID="txtDescription" Width="300" runat="server"></asp:TextBox>
            <asp:Button ID="btnKey" runat="server" OnClick="btnKey_Click" Text="Create new API Key" />
        </form>
    </div>
    </div>
    <br /><br />
    <a href="lookup.aspx">Key lookup</a>
    <br /><br />
    <h3>All API Keys</h3>
    <div style="padding-left: 10px;">
        <asp:Table ID="tblAllKeys" CssClass="datatable" runat="server"></asp:Table>
    </div>
    
</asp:Content>
