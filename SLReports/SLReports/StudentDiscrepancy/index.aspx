<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.StudentDiscrepancy.index" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h2>Student Count Discrepancy Calculator</h2>
    <p>This tool can be used to help determine why the number of current students is different here compared to within school logic. I am not sure how they are detemining their list of current students, so this tool can compare a copy and pasted SchoolLogic report with the database, and tell you which students are missing. Hopefully this helps fine tune the database query, and provide more accurate numbers.</p>
    <br />
    <h2>Copy and paste student ID numbers here</h2>
    <p>You should copy and paste the student ID column from an excel spreadsheet. This form expects <i>only</i> id numbers, each on a new line, no commas or semicolens.</p>
    <form runat="server">
        <asp:TextBox Height="300" TextMode="MultiLine" width="400" ID="txtInput" runat="server"></asp:TextBox><br />
        <asp:Button ID="btnGo" runat="server" Text="Analyze" OnClick="btnGo_Click" />
    </form>
    <p>This may take some time - it can take over 25 million calculations.</p>
    <br />
    <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>


    <asp:Table ID="tblResults" runat="server" Visible="false">
        <asp:TableRow>
            <asp:TableCell VerticalAlign="Top">
                <h2>Missing from database <asp:Label ID="lblMissingFromDatabaseCount" runat="server" Text=""></asp:Label></h2>
                <asp:Table ID="tblMissingFromDataSite" runat="server" CssClass="datatable" CellPadding="5">
                    <asp:TableHeaderRow CssClass="datatable_header">
                        <asp:TableCell Width="300">Given ID Number</asp:TableCell>
                    </asp:TableHeaderRow>               
                </asp:Table>                
            </asp:TableCell>

            <asp:TableCell VerticalAlign="Top">
                <h2>Missing from input <asp:Label ID="lblMissingFromInputCount" runat="server" Text=""></asp:Label></h2>
                <asp:Table ID="tblMissingFromInput" runat="server" CssClass="datatable" CellPadding="5">
                    <asp:TableHeaderRow CssClass="datatable_header">
                        <asp:TableCell Width="100">Student ID</asp:TableCell>
                        <asp:TableCell Width="200">Student Name</asp:TableCell>
                    </asp:TableHeaderRow>
                </asp:Table>
            </asp:TableCell>

            <asp:TableCell VerticalAlign="Top">
                <h2>Present in database <asp:Label ID="lblPresentCount" runat="server" Text=""></asp:Label></h2>
                <asp:Table ID="tblMatchingStudents" runat="server" CssClass="datatable" CellPadding="5">
                    <asp:TableHeaderRow CssClass="datatable_header">
                        <asp:TableCell Width="100">Student ID</asp:TableCell>
                        <asp:TableCell Width="200">Student Name</asp:TableCell>
                    </asp:TableHeaderRow>
                </asp:Table>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    
</asp:Content>
