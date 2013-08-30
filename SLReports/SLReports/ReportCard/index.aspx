<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.ReportCard.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <form id="Form1" runat="server">
    <div class="noPrint">        
        <h1>Report Card Demos</h1> 
        <b style="color: red; font-size: 16pt;">Report cards are a work in progress</b>
        <ul style="font-size: 14pt;">
            <li><a href="ReportCard_Demo.aspx?anon=true&students=12511;&reportperiods=256;257;258;">Anonymized Demo - NBCHS Grade 9 - Multi report period</a></li>
            <li><a href="ReportCard_Demo.aspx?anon=true&students=12511;&reportperiods=258;">Anonymized Demo - NBCHS Grade 9 - Single report period (final)</a></li>
            <li><a href="ReportCard_Demo.aspx?anon=true&students=11804;&reportperiods=256;257;258;266;267;268;&debug=true">Anonymized Demo - NBCHS Grade 11</a></li>
            <li><a href="ReportCard_Demo.aspx?anon=true&students=80891;&reportperiods=266;267;268;">Anonymized Demo - McKitrick Grade 6</a></li>
            <li><a href="ReportCard_Demo.aspx?anon=true&students=80891;12511;11804;&reportperiods=266;267;268;256;257;258;266;267;268;">Several students in one</a></li>            
        </ul>
        <br /><br />
        <h1>Report Card Debug Data</h1> 
        <ul style="font-size: 14pt;">
            <li><a href="GetReportCard_DEBUG.aspx?anon=true&students=12511;&reportperiods=256;257;258;">Anonymized Demo - NBCHS Grade 9 - Multi report period</a></li>
            <li><a href="GetReportCard_DEBUG.aspx?anon=true&students=12511;&reportperiods=258;">Anonymized Demo - NBCHS Grade 9 - Single report period (final)</a></li>
            <li><a href="GetReportCard_DEBUG.aspx?anon=true&students=11804;&reportperiods=256;257;258;266;267;268;&debug=true">Anonymized Demo - NBCHS Grade 11</a></li>
            <li><a href="GetReportCard_DEBUG.aspx?anon=true&students=80891;&reportperiods=266;267;268;">Anonymized Demo - McKitrick Grade 6</a></li>
            <li><a href="GetReportCard_DEBUG.aspx?anon=true&students=80891;12511;11804;&reportperiods=266;267;268;256;257;258;266;267;268;">Several students in one</a></li>            
        </ul>
    </div>              
    </form>
</asp:Content>
