﻿<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="ReportCard_ByStudent.aspx.cs" Inherits="SLReports.ReportCard.ReportCard_ByStudent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h2>Report Cards: Individual Student</h2>
    <p style="font-size: 8pt;">(<a href="ReportCard_ByGrade.aspx">Click here to create report cards a grade level at a time</a>)</p>
    <form id="Form1" runat="server">
        <asp:Table ID="Table1" runat="server">
            <asp:TableRow>
                <asp:TableCell VerticalAlign="Top" Width="200"><b>Choose a school</b></asp:TableCell>
                <asp:TableCell VerticalAlign="Top" Width="400"><asp:DropDownList ID="drpSchools" runat="server"></asp:DropDownList></asp:TableCell>
                <asp:TableCell VerticalAlign="Top" Width="100"><asp:Button ID="btnSchool" runat="server" Text="Select School" OnClick="btnSchool_Click" /></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="tblrow_Grade" Visible="false">
                <asp:TableCell VerticalAlign="Top"><b>Choose a student</b></asp:TableCell>
                <asp:TableCell VerticalAlign="Top"><asp:DropDownList ID="drpStudents" runat="server"></asp:DropDownList></asp:TableCell>
                <asp:TableCell VerticalAlign="Top"><asp:Button ID="btnStudent" runat="server" Text="Select Student" OnClick="btnStudent_Click" /></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="tblrow_ReportPeriod" Visible="false">
                <asp:TableCell VerticalAlign="Top"><b>Choose report periods</b><br />
                    <p style="font-size: 8pt;">Place a check beside each report period that should show up on the report card.</p>
                </asp:TableCell>
                <asp:TableCell VerticalAlign="Top"><asp:CheckBoxList ID="chkReportPeriods" runat="server"></asp:CheckBoxList><br /><br /></asp:TableCell>
                <asp:TableCell VerticalAlign="Top"></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="tblrow_Options" Visible="false">
                <asp:TableCell VerticalAlign="Top"><b>Options</b></asp:TableCell>
                <asp:TableCell VerticalAlign="Top" ColumnSpan="2">
                    <asp:Table ID="Table2" runat="server" CellPadding="5">
                        <asp:TableRow>
                            <asp:TableCell VerticalAlign="Top"><asp:CheckBox ID="chkDoubleSidedMode" runat="server" Checked="true"/></asp:TableCell>
                            <asp:TableCell Width="400" VerticalAlign="Top"><b>I will be printing on both sides of the page</b><br />This will add extra pages to the end of report cards, when necesary, to ensure that one report card does not start on the back of the previous report card.</asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell VerticalAlign="Top"><asp:CheckBox ID="chkShowAttendanceSummary" runat="server" Checked="true"/></asp:TableCell>
                            <asp:TableCell Width="400" VerticalAlign="Top"><b>Show attendance summary</b><br />Shows a summary of all absences at the end of the report card.</asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell VerticalAlign="Top"><asp:CheckBox ID="chkClassAttendance" runat="server" Checked="true"/></asp:TableCell>
                            <asp:TableCell Width="400" VerticalAlign="Top"><b>Show attendance summaries for each class</b><br />This will add short attendance summaries with each class, showing lates and absences. This will only apply to tracks that are set up for period attendance - this option is ignored if the track is set for daily attendance.</asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell VerticalAlign="Top"><asp:CheckBox ID="chkShowPhotos" runat="server" Checked="false"/></asp:TableCell>
                            <asp:TableCell VerticalAlign="Top"><b>Show student photos</b></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell VerticalAlign="Top"><asp:CheckBox ID="chkAnonymize" runat="server"/></asp:TableCell>
                            <asp:TableCell VerticalAlign="Top"><b>Anonymize data</b><br /><div class="checkbox_description">Check this to strip report cards of personal information. Use this if you are showing the report card to others.</div></asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </asp:TableCell>                
            </asp:TableRow>
            <asp:TableRow ID="tblrow_Options2" Visible="false">
                <asp:TableCell VerticalAlign="Top">
                    <b>Administrative Comment</b><br /><p style="font-size: 8pt;">The contents of this text box are added to the bottom of the last page of every report card.</p>
                </asp:TableCell>                
                <asp:TableCell ColumnSpan="2">                    
                    <asp:TextBox id="txtAdminComment" runat="server" TextMode="MultiLine" Height="150" Width="100%" Wrap="false" CssClass="large_text_area"></asp:TextBox>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="tblrow_Options3" HorizontalAlign="Right" Visible="false">
                <asp:TableCell ColumnSpan="3" VerticalAlign="Bottom"><asp:Button ID="btnReportPeriod" runat="server" Text="Generate report cards" OnClick="btnGenerate_Click" /></asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </form> 
</asp:Content>
