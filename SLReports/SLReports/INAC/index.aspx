<%@ Page Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.INAC.index" %>
<asp:Content ID="Content1"  ContentPlaceHolderID="Main" runat="server">
    <h3>INAC Report</h3>    
    <form id="Form1" runat="server">
    

    <table border="0" cellpadding="5">
        <tr>
            <td>School<br /><asp:DropDownList ID="lstSchoolList" runat="server"></asp:DropDownList></td>
            <td>
                Attendance From <br />
                <asp:DropDownList ID="from_month" runat="server"></asp:DropDownList>
                <asp:DropDownList ID="from_day" runat="server"></asp:DropDownList>
                <asp:DropDownList ID="from_year" runat="server"></asp:DropDownList>
            </td>
            <td>
                Attendance To<br />                
                <asp:DropDownList ID="to_month" runat="server"></asp:DropDownList>
                <asp:DropDownList ID="to_day" runat="server"></asp:DropDownList>
                <asp:DropDownList ID="to_year" runat="server"></asp:DropDownList>

            </td>
            <td valign="bottom"><asp:Button ID="Button1" runat="server" Text="Display Students" /></td>
        </tr>
    </table><br />

    <!-- <a href="#"><img src="/SLReports/icon_xls.gif">Download CSV</a> -->
    <asp:Label ID="lblCount" runat="server" Text=""></asp:Label>
    <asp:Table ID="tblResults" runat="server" CssClass="datatable" CellPadding="3" Visible="false">
        <asp:TableHeaderRow CssClass="datatable_header">
            <asp:TableHeaderCell>Grade</asp:TableHeaderCell>
            <asp:TableHeaderCell>Student Name</asp:TableHeaderCell>
            <asp:TableHeaderCell>Date of Birth</asp:TableHeaderCell>
            <asp:TableHeaderCell>Band Affiliation</asp:TableHeaderCell>
            <asp:TableHeaderCell>Status #</asp:TableHeaderCell>
            <asp:TableHeaderCell>Reserve of Residence</asp:TableHeaderCell>
            <asp:TableHeaderCell>House #</asp:TableHeaderCell>
            <asp:TableHeaderCell>Parent / Guardian<sup><a style="color: blue" href="#foot_1">1</a></sup></asp:TableHeaderCell>
            <asp:TableHeaderCell>Days Absent</asp:TableHeaderCell>
            <asp:TableHeaderCell>Date Register</asp:TableHeaderCell>
        </asp:TableHeaderRow>
    </asp:Table>
    <br />
        <div><a name="foot_1">1</a>: Parent / Guardian is calculated as any priority 1 contact that lives with the student.</div>
    </form>
</asp:Content>