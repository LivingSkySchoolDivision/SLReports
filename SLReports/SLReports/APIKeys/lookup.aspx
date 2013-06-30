<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="lookup.aspx.cs" Inherits="SLReports.APIKeys.lookup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h3>API Key lookup</h3>
    <form runat="server">
        <asp:Label ID="lblError" style="color: red; font-weight: bold;" runat="server" Text=""></asp:Label><br />
        <asp:TextBox Width="250" ID="txtKey" runat="server"></asp:TextBox><asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" />
        <br /><br />
        <asp:Literal ID="litKeyInfo" runat="server"></asp:Literal>
    </form>
</asp:Content>
