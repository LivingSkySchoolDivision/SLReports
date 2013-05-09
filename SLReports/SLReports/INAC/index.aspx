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
    </table>
    </form><br />
    <% this.buildStudentTable(AllStudents); %>
    <br />
</asp:Content>