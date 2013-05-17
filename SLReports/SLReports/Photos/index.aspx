<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.Photos.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        var popbackground = "white" 
        var windowtitle = "Photo Preview"

        function detectexist(obj) {
            return (typeof obj != "undefined")
        }

        function jkpopimage(imgpath, popwidth, popheight, textdescription) {

            function getpos() {
                leftpos = (detectexist(window.screenLeft)) ? screenLeft + document.body.clientWidth / 2 - popwidth / 2 : detectexist(window.screenX) ? screenX + innerWidth / 2 - popwidth / 2 : 0
                toppos = (detectexist(window.screenTop)) ? screenTop + document.body.clientHeight / 2 - popheight / 2 : detectexist(window.screenY) ? screenY + innerHeight / 2 - popheight / 2 : 0
                if (window.opera) {
                    leftpos -= screenLeft
                    toppos -= screenTop
                }
            }

            getpos()
            var winattributes = 'width=' + popwidth + ',height=' + popheight + ',resizable=yes,left=' + leftpos + ',top=' + toppos
            var bodyattribute = (popbackground.indexOf(".") != -1) ? 'background="' + popbackground + '"' : 'bgcolor="' + popbackground + '"'
            if (typeof jkpopwin == "undefined" || jkpopwin.closed)
                jkpopwin = window.open("", "", winattributes)
            else {
                jkpopwin.resizeTo(popwidth, popheight + 30)
            }
            jkpopwin.document.open()
            jkpopwin.document.write('<html><title>' + windowtitle + ' - ' + textdescription + '</title><body ' + bodyattribute + '><img src="' + imgpath + '" style="margin-bottom: 0.5em"><br />' + textdescription + '</body></html>')
            jkpopwin.document.close()
            jkpopwin.focus()
        }

</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h3>Students missing photos</h3>
    <form id="Form1" runat="server">
    <asp:Table ID="Table1" runat="server">
        <asp:TableRow>
            <asp:TableCell>
                <asp:DropDownList ID="drpSchoolList" runat="server"></asp:DropDownList>
            </asp:TableCell>
            <asp:TableCell>
                <asp:Button ID="btnFilter" runat="server" Text="Filter by School" OnClick="btnFilter_Click" />
            </asp:TableCell>
        </asp:TableRow>

    </asp:Table>
    <br />
    <asp:Table ID="tblContainer" runat="server" Visible="false">
        <asp:TableRow>
            <asp:TableCell Width="45%" VerticalAlign="Top" HorizontalAlign="Center">
                <asp:Table ID="tblWithoutPhoto" runat="server" CssClass="datatable">
                    <asp:TableHeaderRow CssClass="datatable_sub_header">
                        <asp:TableHeaderCell ColumnSpan="8">
                            <asp:Label ID="lblTotalWithout" runat="server" Text=""></asp:Label>
                        </asp:TableHeaderCell>      
                    </asp:TableHeaderRow>
                    <asp:TableHeaderRow CssClass="datatable_header">
                        <asp:TableHeaderCell>Student Number</asp:TableHeaderCell>
                        <asp:TableHeaderCell>Given Name</asp:TableHeaderCell>
                        <asp:TableHeaderCell>Surname</asp:TableHeaderCell>
                        <asp:TableHeaderCell>Grade</asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                </asp:Table>
            </asp:TableCell>
            <asp:TableCell Width="45%" VerticalAlign="Top" HorizontalAlign="Center">
                <asp:Table ID="tblWithPhoto" runat="server" CssClass="datatable">
                    <asp:TableHeaderRow CssClass="datatable_sub_header">
                        <asp:TableHeaderCell ColumnSpan="8">
                            <asp:Label ID="lblTotalWith" runat="server" Text=""></asp:Label>
                        </asp:TableHeaderCell>      
                    </asp:TableHeaderRow>
                    <asp:TableHeaderRow CssClass="datatable_header">
                        <asp:TableHeaderCell>Student Number</asp:TableHeaderCell>
                        <asp:TableHeaderCell>Given Name</asp:TableHeaderCell>
                        <asp:TableHeaderCell>Surname</asp:TableHeaderCell>
                        <asp:TableHeaderCell>Grade</asp:TableHeaderCell>
                        <asp:TableHeaderCell CssClass="noPrint">Preview</asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                </asp:Table>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>

    
    </form>
</asp:Content>
