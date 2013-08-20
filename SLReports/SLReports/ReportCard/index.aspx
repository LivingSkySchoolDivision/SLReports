<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.ReportCard.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <div class="noPrint">
    <h2>Report Card</h2>


        <p style="font-size: 14pt; color: red; font-weight: bold;">Report cards are still in development, and the user interface has not been completed yet.</p>
        <p style="font-size: 14pt; color: green; font-weight: bold;">Click <a href="Grade_Term_PDF.aspx">here</a> to generate the test PDF, or <a href="Grade_Term_PDF.aspx?debug=true">here</a> to see it in debug mode.</p>
        <br /><br /><br />
    <form runat="server" visible="false">
        <asp:Table ID="Table1" runat="server">
            <asp:TableRow>
                <asp:TableCell><asp:DropDownList ID="drpSchoolList" runat="server"></asp:DropDownList></asp:TableCell>
                <asp:TableCell><asp:Button ID="btnSchool" runat="server" Text="Select School" width="150"  OnClick="btnSchool_Click"/></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>            
            <asp:TableRow ID="TableRow_Student" Visible="false">                
                <asp:TableCell><asp:DropDownList ID="drpStudentList" runat="server"></asp:DropDownList></asp:TableCell>
                <asp:TableCell><asp:Button ID="btnStudent" runat="server" Text="Select Student" width="150" OnClick="btnStudent_Click" /></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>          
            <asp:TableRow ID="TableRow_Term" Visible="false">                
                <asp:TableCell><asp:DropDownList ID="drpTermList" runat="server"></asp:DropDownList></asp:TableCell>                                
                <asp:TableCell><asp:Button ID="btnTermGenPDF" runat="server" Text="Term Report Card (PDF)" width="200" OnClick="btnTermGenPDF_Click" /></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow_ReportPeriod" Visible="false">                
                <asp:TableCell><asp:DropDownList ID="drpReportPeriodList" runat="server"></asp:DropDownList></asp:TableCell>                
                <asp:TableCell><asp:Button ID="btnRPGenPDF" runat="server" Text="Single Report Period (PDF)" width="200" OnClick="btnRPGenPDF_Click"/></asp:TableCell>                
            </asp:TableRow>
        </asp:Table>
        <br />
        <br />
        </div>      
        
    </form>

</asp:Content>
