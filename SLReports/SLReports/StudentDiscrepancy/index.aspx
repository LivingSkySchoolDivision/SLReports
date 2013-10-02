<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.StudentDiscrepancy.index" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <div style="width: 700px; margin-left: auto; margin-right: auto;">
        <h2>Student Count Discrepancy Calculator</h2>
        <p>This tool can be used to help determine why the number of current students is different here compared to within school logic. </p>
        <p>I am not sure how they are detemining their list of current students, so this tool can compare a copy and pasted SchoolLogic report with the database, and tell you which students are missing. Hopefully this helps fine tune the database query, and provide more accurate numbers.</p>
        <h2>How to use</h2>
        <ol>
            <li>Run a report in School Logic containing Student ID numbers. An existing "User Defined Report" that would do this is called "Strendin - Student ID Numbers".</li>
            <li>Open the output of that report in Excel</li>
            <li>Select just the student IDs of the students you want, and copy (CTRL+C). If you used the "Strendin - Student ID Numbers" report, it will give you all currently enrolled students at all schools. If you are working with one school, you may want to select only students from that school.</li>
            <li>Paste the numbers into the box below</li>
            <li>If applicable, select which school you want to compare, or leave the default of "all schools"</li>
            <li>Click the "Analyze" button below. It may take up to a minute to analyze the list. <b>Do not click the button more than once</b> or you will be waiting a long time.</li>            
        </ol>
        <br />
        <h2>Copy and paste student ID numbers here</h2>
        <p>You should copy and paste the student ID column from an excel spreadsheet. This form expects <i>only</i> id numbers, each on a new line, no commas or semicolens.</p>
        <form id="Form1" runat="server">
            <asp:TextBox Height="300" TextMode="MultiLine" width="400" ID="txtInput" runat="server"></asp:TextBox><br />
            <asp:DropDownList ID="drpSchool" runat="server">
                <asp:ListItem Value="0">All Schools</asp:ListItem>
            </asp:DropDownList>
            <asp:Button ID="btnGo" runat="server" Text="Analyze" OnClick="btnGo_Click" />
        </form>
        <br />
    </div>
    
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
