<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.Photos.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
        <div style=" float: right; position: absolute;"><img src="#" name="photo_preview" /></div>
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
                        <asp:TableHeaderCell>View</asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                </asp:Table>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>

    
    </form>
</asp:Content>
