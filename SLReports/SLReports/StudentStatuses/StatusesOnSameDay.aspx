<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="StatusesOnSameDay.aspx.cs" Inherits="SLReports.StudentStatuses.StatusesOnSameDay" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <div style="width: 900px;">
        <h2>Students with an OutStatus and InStatus on the same day</h2>
    <br />
        <asp:Literal ID="litStudents" runat="server"></asp:Literal>
    </div>
</asp:Content>
