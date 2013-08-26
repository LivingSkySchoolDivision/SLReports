<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServerVariables.aspx.cs" Inherits="SLReports.ServerVariables" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="lblTest" runat="server" Text=""></asp:Label>

        <asp:Table ID="tblVars" runat="server">
            <asp:TableHeaderRow>
                <asp:TableCell>Property</asp:TableCell>
                <asp:TableCell>Value</asp:TableCell>
            </asp:TableHeaderRow> 
        </asp:Table>
    
    </div>
    </form>
</body>
</html>
