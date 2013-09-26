<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.SaskSubmitted.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h2>Sask Submitted Table</h2>
    <form id="Form1" runat="server">

    <asp:Table ID="tblCounts" runat="server" CssClass="datatable" Width="600" CellPadding="4">
        <asp:TableRow CssClass="datatable_header">
            <asp:TableCell>School</asp:TableCell>
            <asp:TableCell>Successes</asp:TableCell>
            <asp:TableCell>Failures</asp:TableCell>
            <asp:TableCell>Total</asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <br /><br />

    <asp:Table ID="Table1" runat="server">
        <asp:TableRow>
            <asp:TableCell>Select a school: </asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="drpSchoolList" runat="server"></asp:DropDownList>
            </asp:TableCell>
            <asp:TableCell>
                <asp:Button ID="btnSchool" runat="server" Text="Select School" OnClick="btnSchool_Click" />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>

    <br />
    <b><asp:Label ID="lblEntryCount" runat="server" Text="" Visible="false"></asp:Label></b>
    <br /><br />

    <asp:Table ID="tblEntries" runat="server" Visible="false" CellPadding="4" CssClass="datatable" Width="900">
    </asp:Table>

    </form>
</asp:Content>
