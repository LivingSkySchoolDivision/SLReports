<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="ReportCard_ByGrade.aspx.cs" Inherits="SLReports.ReportCard.ReportCard_ByGrade" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h2>Report Cards: By Grade</h2>
    <form runat="server">
        <asp:Table ID="Table1" runat="server">
            <asp:TableRow>
                <asp:TableCell VerticalAlign="Top">Choose a school</asp:TableCell>
                <asp:TableCell VerticalAlign="Top"><asp:DropDownList ID="drpSchools" runat="server"></asp:DropDownList></asp:TableCell>
                <asp:TableCell VerticalAlign="Top"><asp:Button ID="btnSchool" runat="server" Text="Select School" OnClick="btnSchool_Click" /></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="tblrow_Grade" Visible="false">
                <asp:TableCell VerticalAlign="Top">Choose a grade</asp:TableCell>
                <asp:TableCell VerticalAlign="Top"><asp:DropDownList ID="drpGrades" runat="server"></asp:DropDownList></asp:TableCell>
                <asp:TableCell VerticalAlign="Top"><asp:Button ID="btnGrade" runat="server" Text="Select Grade" OnClick="btnGrade_Click" /></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="tblrow_ReportPeriod" Visible="false">
                <asp:TableCell VerticalAlign="Top">Choose report periods</asp:TableCell>
                <asp:TableCell VerticalAlign="Top"><asp:CheckBoxList ID="chkReportPeriods" runat="server"></asp:CheckBoxList></asp:TableCell>
                <asp:TableCell VerticalAlign="Top"><asp:Button ID="btnReportPeriod" runat="server" Text="Select Report Periods" OnClick="btnReportPeriod_Click" /></asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </form>    
</asp:Content>
