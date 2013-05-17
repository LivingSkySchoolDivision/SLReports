<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.PotentialProblems.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h3>Students with InStatus E15 : Continuation of Enrollment</h3>
    
    <form id="Form1" runat="server">
    <asp:Table ID="tblE15Students" runat="server" CssClass="datatable">
        <asp:TableHeaderRow CssClass="datatable_sub_header">
            <asp:TableHeaderCell ColumnSpan="8">
                <asp:Label ID="lblTotal" runat="server" Text=""></asp:Label>
            </asp:TableHeaderCell>      
        </asp:TableHeaderRow>
        <asp:TableHeaderRow CssClass="datatable_header">
            <asp:TableHeaderCell>School</asp:TableHeaderCell>
            <asp:TableHeaderCell>Grade</asp:TableHeaderCell>
            <asp:TableHeaderCell>Student Number</asp:TableHeaderCell>
            <asp:TableHeaderCell>Given Name</asp:TableHeaderCell>
            <asp:TableHeaderCell>Surname</asp:TableHeaderCell> 
            <asp:TableHeaderCell>InDate</asp:TableHeaderCell> 
            <asp:TableHeaderCell>InStatus Code</asp:TableHeaderCell>            
            <asp:TableHeaderCell>InStatus</asp:TableHeaderCell> 
        </asp:TableHeaderRow>
    </asp:Table>
    </form>
</asp:Content>
