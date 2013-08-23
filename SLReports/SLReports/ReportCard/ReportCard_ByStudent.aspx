<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="ReportCard_ByStudent.aspx.cs" Inherits="SLReports.ReportCard.ReportCard_ByStudent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h2>Report Cards: By Student</h2>
    
    <form runat="server">
        <asp:Table ID="tbl_Step1" runat="server" HorizontalAlign="Center">
            <asp:TableRow>
                <asp:TableCell>
                    <div class="large_infobox" style="width: 600px; margin-left: auto; margin-right: auto;">
                        <h3>Step 1: Choose a School</h3>
                        <div style="text-align: center;">
                            <asp:DropDownList ID="drpSchools" runat="server"></asp:DropDownList>
                        </div>            
                        <p>Choose a school, and then click the "Next Step" button below.</p>
                        <div style="text-align: right;">
                            <asp:Button ID="btn_Step1" runat="server" Text="Next Step" OnClick="btnStep1_Click" />            
                        </div>            
                    </div>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
            
        <asp:Table ID="tbl_Step2" runat="server" HorizontalAlign="Center" Visible="false">
            <asp:TableRow>
                <asp:TableCell>
                    <div class="large_infobox" style="width: 600px; margin-left: auto; margin-right: auto;">
                        <h3 style="color: #C0C0C0;">Step 1: <asp:Label ID="lblSelectedSchool" runat="server" Text=""></asp:Label></h3>
                        <h3>Step 2: Choose one or more students</h3>

                        <asp:Table ID="Table1" runat="server">
                            <asp:TableRow id="tblRow_StudentsByFirstName">
                                <asp:TableCell>Organized by first name</asp:TableCell>
                                <asp:TableCell><asp:DropDownList ID="drpStudentsByFirstName" Width="100%" runat="server"></asp:DropDownList></asp:TableCell>
                                <asp:TableCell><asp:Button ID="btnByFirstName" runat="server" Text="Add Student" OnClick="btnByFirstName_Click" /></asp:TableCell>            
                            </asp:TableRow>   
                            <asp:TableRow id="tblRow_StudentsByLastName">
                                <asp:TableCell>Organized by last name</asp:TableCell>
                                <asp:TableCell><asp:DropDownList ID="drpStudentsByLastName" Width="100%" runat="server"></asp:DropDownList></asp:TableCell>
                                <asp:TableCell><asp:Button ID="btnByLastName" runat="server" Text="Add Student" OnClick="btnByLastName_Click" /></asp:TableCell>
                            </asp:TableRow>                
                            <asp:TableRow ID="tblRow_StudentsByID">
                                <asp:TableCell>Organized by ID</asp:TableCell>
                                <asp:TableCell><asp:DropDownList ID="drpStudentsByID" Width="100%" runat="server"></asp:DropDownList></asp:TableCell>
                                <asp:TableCell><asp:Button ID="btnByID" runat="server" Text="Add Student" OnClick="btnByID_Click" /></asp:TableCell>
                            </asp:TableRow> 
                            <asp:TableRow>
                                <asp:TableCell ColumnSpan="2" VerticalAlign="Top">
                                <br /><br />
                                    <b>Generate report cards for these students:</b>
                                <asp:CheckBoxList ID="lstSelectedStudents" runat="server"></asp:CheckBoxList>
                                <br />
                                <asp:Button ID="btnUnSelectStudents" runat="server" Text="Remove Checked Students" onclick="btnUnSelectStudents_Click"/>
                            </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                        <p>When you are finished selecting students, click the "Next Step" button below.</p>
                           
                        <br /><br />
                        <asp:Table ID="Table2" runat="server" Width="100%">
                            <asp:TableRow>
                                <asp:TableCell HorizontalAlign="Left"><asp:Button ID="btn_BackToStep1" runat="server" Text="Previous Step" OnClick="btn_BackToStep1_Click" /></asp:TableCell>
                                <asp:TableCell HorizontalAlign="Right"><asp:Button ID="btn_Step2" runat="server" Text="Next Step" OnClick="btn_Step2_Click" /></asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>          
                    </div>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>

        <asp:Table ID="tbl_Step3" runat="server" Visible="false" HorizontalAlign="Center" >
            <asp:TableRow>
                <asp:TableCell>
                    <div class="large_infobox" style="width: 600px; margin-left: auto; margin-right: auto;">
                        <h3 style="color: #C0C0C0;">Step 1: <asp:Label ID="lblSelectedSchool2" runat="server" Text=""></asp:Label></h3>
                        <h3 style="color: #C0C0C0;">Step 2: <asp:Label ID="lblSelectedStudents" runat="server" Text=""></asp:Label></h3>
                        <h3>Step 3: Choose one or more report periods</h3>

                        <asp:Table ID="Table4" runat="server">
                            <asp:TableRow>
                                <asp:TableCell>
                                    <asp:DropDownList ID="drpReportPeriods" runat="server"></asp:DropDownList></asp:TableCell>
                                <asp:TableCell>
                                    <asp:Button ID="btnAddReportPeriod" runat="server" Text="Add Report Period" OnClick="btnAddReportPeriod_Click" /></asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow VerticalAlign="Top">
                                <asp:TableCell>
                                    <br /><br />
                                    <b>Generate report cards for these Report Periods</b>
                                    <br />
                                    <asp:CheckBoxList ID="lstSelectedReportPeriods" runat="server">

                                    </asp:CheckBoxList>
                                    <br />
                                    <asp:Button ID="btnUnSelectReportPeriod" runat="server" Text="Remove Checked Report Periods" OnClick="btnUnSelectReportPeriod_Click" />
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                        

                        <br /><br />
                        <asp:Table ID="Table3" runat="server" Width="100%">
                            <asp:TableRow>
                                <asp:TableCell HorizontalAlign="Left"><asp:Button ID="btn_BackToStep2" runat="server" Text="Previous Step" OnClick="btn_BackToStep2_Click" /></asp:TableCell>
                                <asp:TableCell HorizontalAlign="Right"><asp:Button ID="btn_Step3" runat="server" Text="Next Step" OnClick="btn_Step3_Click" /></asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    
                    </div>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>        
    </form>
</asp:Content>
