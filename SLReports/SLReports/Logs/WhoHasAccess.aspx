<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="WhoHasAccess.aspx.cs" Inherits="SLReports.Logs.WhoHasAccess" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <div style="width: 600px; margin-left: auto; margin-right: auto;">
    <h2>Who has access to this website</h2>

    <p>
        The users listed below are able to access this website.
    </p>

    <p>
        To add a user to the access list, please contact the <a href="https://helpdesk.lskysd.ca">Technology Help Desk</a>, or a member of the tech department.
    </p>


    <asp:Table ID="Table1" runat="server" Width="500" HorizontalAlign="Center">
        <asp:TableRow>
            <asp:TableCell VerticalAlign="Top">
                <h3>Site administrators</h3>
                <asp:Table ID="tblAdministrators" runat="server">
                </asp:Table>

            </asp:TableCell>
            <asp:TableCell VerticalAlign="Top">
                <h3>Site users</h3>
                <asp:Table ID="tblUsers" runat="server">
                </asp:Table>

            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    </div>
    

    

</asp:Content>
