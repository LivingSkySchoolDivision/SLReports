<%@ Page Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.DivisionStats.index" %>

<asp:Content ID="Content1"  ContentPlaceHolderID="Main" runat="server">
    <% this.buildEnrollmentTable(Schools,AllStudents); %>  
</asp:Content>
