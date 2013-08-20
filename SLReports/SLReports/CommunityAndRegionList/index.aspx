<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.CommunityAndRegionList.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <script type="text/javascript">
        function toggleVisible(id)
        {
            var e = document.getElementById(id);
            if (e.style.display == 'block')
                e.style.display = 'none';
            else
                e.style.display = 'block';
        }
    </script>

    <h2>Provinces, Territories, States and Regions</h2>
    <asp:Table ID="tblRegions" runat="server" CssClass="datatable" CellPadding="5">
        <asp:TableHeaderRow CssClass="datatable_header">
            <asp:TableCell>Region</asp:TableCell>
            <asp:TableCell>Students</asp:TableCell>
        </asp:TableHeaderRow>
    </asp:Table>

    <h2>Communities</h2>
    <asp:Table ID="tblCommunities" runat="server" CssClass="datatable" CellPadding="5">
        <asp:TableHeaderRow CssClass="datatable_header">
            <asp:TableCell>Community</asp:TableCell>
            <asp:TableCell>Students</asp:TableCell>
        </asp:TableHeaderRow>
    </asp:Table>
</asp:Content>

