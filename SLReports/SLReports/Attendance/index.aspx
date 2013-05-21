<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/BasicTemplate.Master" CodeBehind="index.aspx.cs" Inherits="SLReports.Attendance.index" %>
<asp:Content ID="Content1"  ContentPlaceHolderID="Main" runat="server">

    <form id="Form2" runat="server">    
        <asp:Table ID="Table2" runat="server">
            <asp:TableRow>
                <asp:TableCell><asp:DropDownList ID="drpSchoolList" runat="server"></asp:DropDownList></asp:TableCell>
                <asp:TableCell><asp:Button ID="Button1" runat="server" Text="Select School" width="150"/></asp:TableCell>
            </asp:TableRow>            
            <asp:TableRow ID="TableRow_Student" Visible="false">        
                <asp:TableCell><asp:DropDownList ID="drpStudentList" runat="server"></asp:DropDownList></asp:TableCell>
                <asp:TableCell><asp:Button ID="Button2" runat="server" Text="Select Student" width="150" /></asp:TableCell>
            </asp:TableRow>          
            <asp:TableRow ID="TableRow_Term" Visible="false">                
                <asp:TableCell><asp:DropDownList ID="drpTermList" runat="server"></asp:DropDownList></asp:TableCell>
                <asp:TableCell><asp:Button ID="Button3" runat="server" Text="Select Term" width="150"/></asp:TableCell>
            </asp:TableRow>
        </asp:Table>

    
    <div style="float:right; border: 1px solid #C0C0C0;background-color: #F0F0F0; padding: 10px;">
        <asp:Table ID="Table1" runat="server">
            <asp:TableRow>
                <asp:TableCell><asp:DropDownList ID="drpFrom_year" runat="server"></asp:DropDownList></asp:TableCell>
                <asp:TableCell><asp:DropDownList ID="drpFrom_month" runat="server"></asp:DropDownList></asp:TableCell>
                <asp:TableCell><asp:DropDownList ID="drpFrom_day" runat="server"></asp:DropDownList></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell><asp:DropDownList ID="drpTo_year" runat="server"></asp:DropDownList></asp:TableCell>
                <asp:TableCell><asp:DropDownList ID="drpTo_month" runat="server"></asp:DropDownList></asp:TableCell>
                <asp:TableCell><asp:DropDownList ID="drpTo_day" runat="server"></asp:DropDownList></asp:TableCell>

            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell ColumnSpan="3"><asp:Button ID="btnFilterDate" runat="server" Text="Adjust date range" onclick="btnFilterDate_Click"/></asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>
    </form>

    <asp:Literal ID="litContent" runat="server"></asp:Literal>
<%
    if (selectedStudent != null)
    {

        Response.Write("<h3>Student Attendance Report</h3><br />");
        this.displayStudentNameplate(selectedStudent);
        
    }
    else
    {
        Response.Write("<h3>Student not found</h3>");
    }

    if (selectedStudentAbsences != null)
    {

        this.displayAbsenceStatistics(selectedStudentAbsences);
           
        if (false)
        {

            this.displayAbsenceListTable(selectedStudentAbsences);
        }
        else
        {
            this.displayAbsenceListText(selectedStudentAbsences);
        }

    }
    %>
    
</asp:Content>
