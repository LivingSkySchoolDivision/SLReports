<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="attendanceByStudent.aspx.cs" Inherits="SLReports.Attendance.attendanceByStudent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <div class="noPrint">
    <h3>Attendance Report By Student</h3>
    <form id="Form1" runat="server">
        <asp:Table ID="Table1" runat="server">
            <asp:TableRow>
                <asp:TableCell><asp:DropDownList ID="drpSchoolList" runat="server"></asp:DropDownList></asp:TableCell>
                <asp:TableCell><asp:Button ID="btnSchool" runat="server" Text="Select School" width="150"  OnClick="btnSchool_Click"/></asp:TableCell>
            </asp:TableRow>            
            <asp:TableRow ID="TableRow_Students" Visible="false">                
                <asp:TableCell><asp:DropDownList ID="drpStudentList" runat="server"></asp:DropDownList></asp:TableCell>
                <asp:TableCell><asp:Button ID="btnStudent" runat="server" Text="Select Student" width="150" OnClick="btnStudent_Click" /></asp:TableCell>
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
                <asp:TableCell>
                    <asp:Button ID="btnDate" runat="server" Text="Create PDF" width="250" OnClick="btnDate_Click"/><br />
                    <asp:Button ID="btnHTML" runat="server" Text="View in Browser" width="250" OnClick="btnHTML_Click"/>

                </asp:TableCell>
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
