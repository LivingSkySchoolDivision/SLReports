<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.FirstClassOfTheDay.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">    
    <h3>Student first class of the day</h3>
    <form id="Form1" runat="server">

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
        <asp:TableRow ID="tblrow_Track" Visible="false">
            <asp:TableCell>Select a track: </asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="drpTrack" runat="server"></asp:DropDownList>
            </asp:TableCell>
            <asp:TableCell>
                <asp:Button ID="btnTrack" runat="server" Text="Select Track" OnClick="btnTrack_Click" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="tblrow_Term" Visible="false">
            <asp:TableCell>Select a term: </asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="drpTerm" runat="server"></asp:DropDownList>
            </asp:TableCell>
            <asp:TableCell>
                <asp:Button ID="btnTerm" runat="server" Text="Select Term" OnClick="btnTerm_Click" />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <br /><br />
        
    <asp:Table ID="tblCSVLink" runat="server" Visible="false">
        <asp:TableRow>
            <asp:TableCell><img src="../icon_xls.gif" /> <asp:HyperLink ID="lnkCSVLink" runat="server">Export list to CSV</asp:HyperLink></asp:TableCell>
        </asp:TableRow>
    </asp:Table>    
    <asp:Table ID="tblStudents" runat="server" Visible="false" CssClass="datatable" CellPadding="3">      

    </asp:Table>


    </form>
</asp:Content>
