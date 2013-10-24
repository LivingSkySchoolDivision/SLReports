<%@ Page Title="" Language="C#" MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.ReportCard.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <form id="Form1" runat="server">
    <div class="noPrint">
        <h1>Report Card Examples</h1>
        <p>These examples use live, current data from SchoolLogic.</p>
        <p><b style="color: #FF0000;">If you intend to share these examples with others, please use "Anonymized" versions to protect the privacy of our students.</b></p>
        <ul style="font-size: 14pt;">
            <li>NBCHS Grade 12 student - (<a href="GetReportCardPDF_ByStudent.aspx?anon=true&students=12438;&reportperiods=282;&showphoto=true">Anonymized PDF</a>) (<a href="GetReportCardPDF_ByStudent.aspx?students=12438;&reportperiods=282;&showphoto=true">Actual PDF</a>) (<a href="GetReportCard_DEBUG.aspx?anon=true&students=12438;&reportperiods=282;">Debug Data</a>)</li>
            <li>NBCHS Grade 11 student - (<a href="GetReportCardPDF_ByStudent.aspx?anon=true&students=12609;&reportperiods=282;&showphoto=true">Anonymized PDF</a>) (<a href="GetReportCardPDF_ByStudent.aspx?students=12609;&reportperiods=282;&showphoto=true">Actual PDF</a>) (<a href="GetReportCard_DEBUG.aspx?anon=true&students=12609;&reportperiods=282;">Debug Data</a>)</li>
            <li>NBCHS Grade 10 student - (<a href="GetReportCardPDF_ByStudent.aspx?anon=true&students=600001775;&reportperiods=282;&showphoto=true">Anonymized PDF</a>) (<a href="GetReportCardPDF_ByStudent.aspx?students=600001775;&reportperiods=282;&showphoto=true">Actual PDF</a>) (<a href="GetReportCard_DEBUG.aspx?anon=true&students=600001775;&reportperiods=282;">Debug Data</a>)</li>
            <li>NBCHS Grade 9 student - (<a href="GetReportCardPDF_ByStudent.aspx?anon=true&students=600003958;&reportperiods=282;&showphoto=true">Anonymized PDF</a>) (<a href="GetReportCardPDF_ByStudent.aspx?students=600003958;&reportperiods=282;&showphoto=true">Actual PDF</a>) (<a href="GetReportCard_DEBUG.aspx?anon=true&students=600003958;&reportperiods=282;">Debug Data</a>)</li>
            <li>NBCHS Grade 8 student - (<a href="GetReportCardPDF_ByStudent.aspx?anon=true&students=600004397;&reportperiods=282;&showphoto=true">Anonymized PDF</a>) (<a href="GetReportCardPDF_ByStudent.aspx?&students=600004397;&reportperiods=282;&showphoto=true">Actual PDF</a>) (<a href="GetReportCard_DEBUG.aspx?anon=true&students=600004397;&reportperiods=282;">Debug Data</a>)</li>
            <li>NBCHS - All above students in one file (<a href="GetReportCardPDF_ByStudent.aspx?anon=true&students=12438;12609;600001775;600003958;600004397;&reportperiods=282;&showphoto=true">Anonymized PDF</a>) (<a href="GetReportCardPDF_ByStudent.aspx?students=12438;12609;600001775;600003958;600004397;&reportperiods=282;&showphoto=true">Actual PDF</a>) (<a href="GetReportCard_DEBUG.aspx?students=12438;12609;600001775;600003958;600004397;&reportperiods=282;">Debug data</a>)</li>            
        </ul>
        <br /><br />        
        <h1>Examples of students with specific data (For development purposes)</h1>
        <ul style="font-size: 14pt;">             
            <li>Student with invalid life skills values - (<a href="GetReportCardPDF_ByStudent.aspx?students=600000080;&reportperiods=282;&showphoto=true&anon=true&showphoto=true">Anonymized PDF</a>) (<a href="GetReportCardPDF_ByStudent.aspx?students=600000080;&reportperiods=282;&showphoto=true&showphoto=true">Actual PDF</a>) (<a href="GetReportCard_DEBUG.aspx?students=600000080;&reportperiods=282;">Data</a>)</li>
            <li>Student with a report card comment - (<a href="GetReportCardPDF_ByStudent.aspx?students=600002057;&reportperiods=282;283;&showphoto=true&anon=true&showphoto=true">Anonymized PDF</a>) (<a href="GetReportCardPDF_ByStudent.aspx?students=600002057;&reportperiods=282;283;&showphoto=true&showphoto=true">Actual PDF</a>) (<a href="GetReportCard_DEBUG.aspx?students=600002057;&reportperiods=282;283;">Data</a>)</li>
        </ul>
    </div>              
    </form>
</asp:Content>
