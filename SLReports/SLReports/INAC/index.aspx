﻿<%@ Page Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.INAC.index" %>
<asp:Content ID="Content1"  ContentPlaceHolderID="Main" runat="server">
    <h3>INAC Report</h3>    
    <form id="Form1" runat="server">
    

    <table border="0" cellpadding="5">
        <tr>
            <td>School<br /><asp:DropDownList ID="lstSchoolList" runat="server"></asp:DropDownList></td>
            <td>
                Date From <br />
                <asp:DropDownList ID="from_month" runat="server"></asp:DropDownList>
                <asp:DropDownList ID="from_day" runat="server"></asp:DropDownList>
                <asp:DropDownList ID="from_year" runat="server"></asp:DropDownList>
            </td>
            <td>
                Date To<br />                
                <asp:DropDownList ID="to_month" runat="server"></asp:DropDownList>
                <asp:DropDownList ID="to_day" runat="server"></asp:DropDownList>
                <asp:DropDownList ID="to_year" runat="server"></asp:DropDownList>

            </td>
            <td valign="bottom"><asp:Button ID="btnSelect" runat="server" Text="Display Students" onclick="btnSelect_Click"/></td>
        </tr>
    </table><br />

    <asp:HyperLink ID="lnkCSVDownload" Visible="false" runat="server"><img src="/SLReports/icon_xls.gif">Download CSV</asp:HyperLink><br /><br />
    <asp:Label ID="lblCount" runat="server" Text=""></asp:Label>
    <asp:Table ID="tblResults" runat="server" CssClass="datatable" CellPadding="5" Visible="false">
        <asp:TableHeaderRow CssClass="datatable_header">
            <asp:TableHeaderCell>Grade</asp:TableHeaderCell>
            <asp:TableHeaderCell>Student Name</asp:TableHeaderCell>
            <asp:TableHeaderCell>Date of Birth</asp:TableHeaderCell>
            <asp:TableHeaderCell>Band Affiliation</asp:TableHeaderCell>
            <asp:TableHeaderCell>Status #</asp:TableHeaderCell>
            <asp:TableHeaderCell>Reserve of Residence</asp:TableHeaderCell>
            <asp:TableHeaderCell>House #</asp:TableHeaderCell>
            <asp:TableHeaderCell>Parent / Guardian<sup></sup><br /><sup>Any priority 1 contact that lives with the student</sup></asp:TableHeaderCell>
            <asp:TableHeaderCell BackColor="#C11B17" ColumnSpan="2">Unexcused Absences<br /><sup>Hover mouse over to see how days were calculated</sup></asp:TableHeaderCell>
            <asp:TableHeaderCell ColumnSpan="2">Total Absences<br /><sup>Hover mouse over to see how days were calculated</sup></asp:TableHeaderCell>
            <asp:TableHeaderCell>Active InStatus Date</asp:TableHeaderCell>
            <asp:TableHeaderCell>List Absences</asp:TableHeaderCell>
        </asp:TableHeaderRow>
    </asp:Table>
    <br />
        <div><b>Note:</b> Parent / Guardian is calculated as any priority 1 contact that lives with the student.</div>
    </form>
</asp:Content>