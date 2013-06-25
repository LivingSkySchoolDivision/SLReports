<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.AttendanceByGrade.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <div class="noPrint">
    <h3>Attendance By Grade</h3>
    <form id="Form1" runat="server">
        <asp:Table ID="Table1" runat="server">
            <asp:TableRow>
                <asp:TableCell><asp:DropDownList ID="drpSchoolList" runat="server"></asp:DropDownList></asp:TableCell>
                <asp:TableCell><asp:Button ID="Button1" runat="server" Text="Select School" width="150"  OnClick="Button1_Click"/></asp:TableCell>
            </asp:TableRow>            
            <asp:TableRow ID="TableRow_Grade" Visible="false">                
                <asp:TableCell><asp:DropDownList ID="drpGradeList" runat="server"></asp:DropDownList></asp:TableCell>
                <asp:TableCell><asp:Button ID="Button2" runat="server" Text="Select Grade" width="150" OnClick="Button2_Click" /></asp:TableCell>
            </asp:TableRow>          
            <asp:TableRow ID="TableRow_Date" Visible="false">                
                <asp:TableCell>
                    <b>From</b><br />
                    <asp:DropDownList ID="from_Month" runat="server"></asp:DropDownList>
                    <asp:DropDownList ID="from_Day" runat="server"></asp:DropDownList>
                    <asp:DropDownList ID="from_Year" runat="server"></asp:DropDownList>
                    <br />
                    <b>To</b><br />
                    <asp:DropDownList ID="to_Month" runat="server"></asp:DropDownList>
                    <asp:DropDownList ID="to_Day" runat="server"></asp:DropDownList>
                    <asp:DropDownList ID="to_Year" runat="server"></asp:DropDownList>

                </asp:TableCell>
                <asp:TableCell><asp:Button ID="Button3" runat="server" Text="Select Date Range and generate PDF" width="150" OnClick="Button3_Click"/></asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <br />
        <br />
        </div>
        <asp:Literal ID="litNamePlate" runat="server"></asp:Literal>
        <asp:Literal ID="litMarks" runat="server"></asp:Literal>
        <asp:Literal ID="litAttendance" runat="server"></asp:Literal>
        
        
    </form>

</asp:Content>
