<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="ReportCard_ByGrade.aspx.cs" Inherits="SLReports.ReportCard.ReportCard_ByGrade" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h2>Report Cards: By Grade</h2>
    <form runat="server">
        <asp:Table ID="Table1" runat="server">
            <asp:TableRow>
                <asp:TableCell VerticalAlign="Top"><b>Choose a school</b></asp:TableCell>
                <asp:TableCell VerticalAlign="Top"><asp:DropDownList ID="drpSchools" runat="server"></asp:DropDownList></asp:TableCell>
                <asp:TableCell VerticalAlign="Top"><asp:Button ID="btnSchool" runat="server" Text="Select School" OnClick="btnSchool_Click" /></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="tblrow_Grade" Visible="false">
                <asp:TableCell VerticalAlign="Top"><b>Choose a grade</b></asp:TableCell>
                <asp:TableCell VerticalAlign="Top"><asp:DropDownList ID="drpGrades" runat="server"></asp:DropDownList></asp:TableCell>
                <asp:TableCell VerticalAlign="Top"><asp:Button ID="btnGrade" runat="server" Text="Select Grade" OnClick="btnGrade_Click" /></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="tblrow_ReportPeriod" Visible="false">
                <asp:TableCell VerticalAlign="Top"><b>Choose report periods</b></asp:TableCell>
                <asp:TableCell VerticalAlign="Top"><asp:CheckBoxList ID="chkReportPeriods" runat="server"></asp:CheckBoxList><br /><br /></asp:TableCell>
                <asp:TableCell VerticalAlign="Top"></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="tblrow_Options" Visible="false">
                <asp:TableCell VerticalAlign="Top"><b>Options</b></asp:TableCell>
                <asp:TableCell VerticalAlign="Top">
                    <asp:Table runat="server" CellPadding="5">
                        <asp:TableRow>
                            <asp:TableCell VerticalAlign="Top"><asp:CheckBox ID="chkDoubleSidedMode" runat="server" Checked="true"/></asp:TableCell>
                            <asp:TableCell Width="400" VerticalAlign="Top"><b>Format for printing on both sides of the page.</b><br />This will add extra pages to the end of report cards, when necesary, to ensure that one report card does not start on the back of the previous report card.</asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell VerticalAlign="Top"><asp:CheckBox ID="chkShowPhotos" runat="server" Checked="true"/></asp:TableCell>
                            <asp:TableCell VerticalAlign="Top"><b>Show student photos.</b></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell VerticalAlign="Top"><asp:CheckBox ID="chkAnonymize" runat="server" /></asp:TableCell>
                            <asp:TableCell Width="400" VerticalAlign="Top"><b>Anonymize data</b><br />Hide personal information from students, and show placeholder data instead.</asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </asp:TableCell>
                <asp:TableCell VerticalAlign="Bottom"><asp:Button ID="btnReportPeriod" runat="server" Text="Generate report cards" OnClick="btnReportPeriod_Click" /></asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </form>    
</asp:Content>
