<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.ReportCard.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <form id="Form1" runat="server">
    <div class="noPrint">
        <h1>Report Cards</h1>  
        <!--
        <table>
            <tr>
                <td width="50%">
                    <div style="text-align: center; padding-left: 15px; font-size: 16pt;"><a style="text-decoration: none; display: block; border: 1px solid black; background-color: #F0F0F0; padding: 20px;" href="ReportCard_ByStudent.aspx"><b>By Student</b><br />Generate a report card for specific students</a></div>
                </td>
                <td width="50%">
                    <div style="text-align: center; padding-left: 15px; font-size: 16pt;"><a style="text-decoration: none; display: block; border: 1px solid black; background-color: #F0F0F0; padding: 20px;" href="ReportCard_ByGrade.aspx"><b>By Grade</b><br />Generate report cards for an entire grade level at a time</a></div>
                </td>
            </tr>
        </table>         
          -->    
        <p style="font-size: 16pt;">Click <a href="ReportCard_Demo.aspx">here</a> to generate an anonymized demo PDF, or <a href="GetReportCardPDF_Students.aspx?debug=true">here</a> to see it in debug mode.</p>        
        
    </div>              
    </form>
</asp:Content>
