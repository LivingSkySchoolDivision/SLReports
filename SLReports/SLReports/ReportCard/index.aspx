<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.ReportCard.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h3>Report Card (Work In Progress)</h3>
    <form runat="server">
        <asp:Table ID="Table1" runat="server">
            <asp:TableRow>
                <asp:TableCell><asp:DropDownList ID="drpSchoolList" runat="server"></asp:DropDownList></asp:TableCell>
                <asp:TableCell><asp:Button ID="Button1" runat="server" Text="Select School" width="150"  OnClick="Button1_Click"/></asp:TableCell>
            </asp:TableRow>            
            <asp:TableRow ID="TableRow_Student" Visible="false">                
                <asp:TableCell><asp:DropDownList ID="drpStudentList" runat="server"></asp:DropDownList></asp:TableCell>
                <asp:TableCell><asp:Button ID="Button2" runat="server" Text="Select Student" width="150" OnClick="Button2_Click" /></asp:TableCell>
            </asp:TableRow>          
            <asp:TableRow ID="TableRow_Term" Visible="false">                
                <asp:TableCell><asp:DropDownList ID="drpTermList" runat="server"></asp:DropDownList></asp:TableCell>
                <asp:TableCell><asp:Button ID="Button3" runat="server" Text="Select Term" width="150" OnClick="Button3_Click"/></asp:TableCell>
            </asp:TableRow>
        </asp:Table>

        <asp:Literal ID="litNamePlate" runat="server"></asp:Literal>
        <asp:Literal ID="litMarks" runat="server"></asp:Literal>
        <asp:Literal ID="litAttendance" runat="server"></asp:Literal>
        
        
    </form>

</asp:Content>
