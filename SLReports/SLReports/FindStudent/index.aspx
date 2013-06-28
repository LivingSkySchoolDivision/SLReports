<%@ Page Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.FindStudent.index" %>
<asp:Content ID="Content" ContentPlaceHolderID="Main" runat="server">    
<h3>Search for a student</h3> 
<div style="width: 400px; margin-left: auto; margin-right: auto;">    
    <form id="Form1" runat="server">
        <asp:TextBox ID="txtSearch" runat="server" Width="200"></asp:TextBox>
        <asp:Button ID="btnSearch" runat="server" Text="Search" onclick="btnSearch_Click"/>        
    </form>
    <div style="font-size: 8pt;">*Searches student name, ID number, and government ID number </div>
</div>
<br />
<asp:Table ID="tblResults" runat="server" CssClass="datatable" CellPadding="3" HorizontalAlign="Center"></asp:Table>
    
</asp:Content>