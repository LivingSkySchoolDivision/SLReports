<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.ReportCard.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <form id="Form1" runat="server">
    <div class="noPrint">        
        <h1>Report Card Demos</h1> 
        <b style="color: red; font-size: 16pt;">Report cards are a work in progress</b>
        <ul style="font-size: 14pt;">
            <li><a href="ReportCard_Demo.aspx?anon=true&students=11907;&reportperiods=282;">Anonymized Demo - NBCHS Grade 12</a></li>
            <li><a href="ReportCard_Demo.aspx?anon=true&students=12254;&reportperiods=282;">Anonymized Demo - NBCHS Grade 11</a></li>
            <li><a href="ReportCard_Demo.aspx?anon=true&students=600000866;&reportperiods=282;">Anonymized Demo - NBCHS Grade 9</a></li>
            <li><a href="ReportCard_Demo.aspx?anon=true&students=11907;12254;600000866;&reportperiods=282;">Several students in one</a></li>            
        </ul>
        <br /><br />
        <h1>Report Card Debug Data</h1> 
        <ul style="font-size: 14pt;">
            <li><a href="GetReportCard_DEBUG.aspx?anon=true&students=11907;&reportperiods=282;">Anonymized Demo - NBCHS Grade 12</a></li>
            <li><a href="GetReportCard_DEBUG.aspx?anon=true&students=12254;&reportperiods=282;">Anonymized Demo - NBCHS Grade 11</a></li>
            <li><a href="GetReportCard_DEBUG.aspx?anon=true&students=600000866;&reportperiods=282;">Anonymized Demo - NBCHS Grade 9</a></li>
            <li><a href="GetReportCard_DEBUG.aspx?anon=true&students=11907;12254;600000866;&reportperiods=282;">Several students in one</a></li>         
        </ul>
        <h1>Non Anonymized students for report card testing</h1>
        <ul style="font-size: 14pt;">
            <li>11907 (Grade 12) - <a href="GetReportCardPDF_ByStudent.aspx?students=11907;&reportperiods=282;">PDF</a> <a href="GetReportCard_DEBUG.aspx?students=11907;&reportperiods=282;">Data</a></li>
            <li>12254 (Grade 11) - <a href="GetReportCardPDF_ByStudent.aspx?students=12254;&reportperiods=282;">PDF</a> <a href="GetReportCard_DEBUG.aspx?students=12254;&reportperiods=282;">Data</a></li>
            <li>12518 (Grade 10) - <a href="GetReportCardPDF_ByStudent.aspx?students=12518;&reportperiods=282;">PDF</a> <a href="GetReportCard_DEBUG.aspx?students=12518;&reportperiods=282;">Data</a></li>
            <li>600000866 (Grade 9) - <a href="GetReportCardPDF_ByStudent.aspx?students=600000866;&reportperiods=282;">PDF</a> <a href="GetReportCard_DEBUG.aspx?students=600000866;&reportperiods=282;">Data</a></li>
            <li>600004912 (Grade 8) - <a href="GetReportCardPDF_ByStudent.aspx?students=600004912;&reportperiods=282;">PDF</a> <a href="GetReportCard_DEBUG.aspx?students=600004912;&reportperiods=282;">Data</a></li>
            <li>600000080 (Grade 9) Has invalid life skills - <a href="GetReportCardPDF_ByStudent.aspx?students=600000080;&reportperiods=282;">PDF</a> <a href="GetReportCard_DEBUG.aspx?students=600000080;&reportperiods=282;">Data</a></li>
            <li>11803 (Grade 12) Has life skills- <a href="GetReportCardPDF_ByStudent.aspx?students=11803;&reportperiods=282;283;">PDF</a> <a href="GetReportCard_DEBUG.aspx?students=11803;&reportperiods=282;283;">Data</a></li>
            <li>11907 (Grade 12) with 2 report periods - <a href="GetReportCardPDF_ByStudent.aspx?students=11907;&reportperiods=282;283">PDF</a> <a href="GetReportCard_DEBUG.aspx?students=11907;&reportperiods=282;283">Data</a></li>
        </ul>
    </div>              
    </form>
</asp:Content>
