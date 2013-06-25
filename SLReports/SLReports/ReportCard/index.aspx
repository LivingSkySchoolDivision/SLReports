<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.ReportCard.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <div class="noPrint">
    <h3>Report Card</h3>
    <p style="font-size: 14pt; color: red; font-weight: bold;">This is a work in progress and should not be used to replace an actual report card until it is finished</p>
    <form runat="server">
        <asp:Table ID="Table1" runat="server">
            <asp:TableRow>
                <asp:TableCell><asp:DropDownList ID="drpSchoolList" runat="server"></asp:DropDownList></asp:TableCell>
                <asp:TableCell><asp:Button ID="Button1" runat="server" Text="Select School" width="150"  OnClick="Button1_Click"/></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>            
            <asp:TableRow ID="TableRow_Student" Visible="false">                
                <asp:TableCell><asp:DropDownList ID="drpStudentList" runat="server"></asp:DropDownList></asp:TableCell>
                <asp:TableCell><asp:Button ID="Button2" runat="server" Text="Select Student" width="150" OnClick="Button2_Click" /></asp:TableCell>
                <asp:TableCell></asp:TableCell>
            </asp:TableRow>          
            <asp:TableRow ID="TableRow_Term" Visible="false">                
                <asp:TableCell><asp:DropDownList ID="drpTermList" runat="server"></asp:DropDownList></asp:TableCell>                
                <asp:TableCell><asp:Button ID="btnTermGenHTML" runat="server" Text="Generate HTML" width="200" OnClick="btnTermGenHTML_Click" Enabled="false"/></asp:TableCell>
                <asp:TableCell><asp:Button ID="btnTermGenPDF" runat="server" Text="Generate PDF" width="200" OnClick="btnTermGenPDF_Click" Enabled="false"/></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow_ReportPeriod" Visible="false">                
                <asp:TableCell><asp:DropDownList ID="drpReportPeriodList" runat="server"></asp:DropDownList></asp:TableCell>
                <asp:TableCell><asp:Button ID="btnRPGenHTML" runat="server" Text="Generate HTML" width="200" OnClick="btnRPGenHTML_Click"/></asp:TableCell>
                <asp:TableCell><asp:Button ID="btnRPGenPDF" runat="server" Text="Generate PDF" width="200" OnClick="btnRPGenPDF_Click"/></asp:TableCell>                
            </asp:TableRow>
        </asp:Table>
        <br />
        <br />
        </div>      
        
    </form>

</asp:Content>
