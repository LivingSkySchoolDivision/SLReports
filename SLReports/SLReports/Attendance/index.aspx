<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/BasicTemplate.Master" CodeBehind="index.aspx.cs" Inherits="SLReports.Attendance.index" %>
<asp:Content ID="Content1"  ContentPlaceHolderID="Main" runat="server">

    <div style="float:right; border: 1px solid #C0C0C0;background-color: #F0F0F0; padding: 10px;">
<!-- <form method="get"> -->
    <input type="hidden" name="studentid" value="<% Response.Write(selectedStudentID); %>" />
    <table border="0">
        <tr>
            <td colspan="4" style="font-size: 10pt;"><b>Date Range</b></td>
        </tr>
        <tr>
            <td style="font-size: 10pt;">From</td>
            <td>    
    <select name="from_year">
<% 
        for (int year = (DateTime.Now.Year - 10); year < (DateTime.Now.Year + 10); year++)
    {
        Response.Write("<option value=\"" + year + "\" ");
        if (year == selectedStartDate.Year)
            Response.Write("SELECTED=SELECTED");
        Response.Write(">" + year + "</option>");
    }
%>
    </select></td>
            <td>    
    <select name="from_month">
        <option value="1" <% if (selectedStartDate.Month == 1) { Response.Write("SELECTED=SELECTED"); } %>>January</option>
        <option value="2" <% if(selectedStartDate.Month == 2) {Response.Write("SELECTED=SELECTED"); }%>>February</option>
        <option value="3" <% if(selectedStartDate.Month == 3) {Response.Write("SELECTED=SELECTED");} %>>March</option>
        <option value="4" <% if(selectedStartDate.Month == 4) {Response.Write("SELECTED=SELECTED"); }%>>April</option>
        <option value="5" <% if(selectedStartDate.Month == 5) {Response.Write("SELECTED=SELECTED"); }%>>May</option>
        <option value="6" <% if(selectedStartDate.Month == 6) {Response.Write("SELECTED=SELECTED"); }%>>June</option>
        <option value="7" <% if(selectedStartDate.Month == 7) {Response.Write("SELECTED=SELECTED");} %>>July</option>
        <option value="8" <% if(selectedStartDate.Month == 8) {Response.Write("SELECTED=SELECTED");} %>>August</option>
        <option value="9" <% if(selectedStartDate.Month == 9) {Response.Write("SELECTED=SELECTED"); }%>>September</option>
        <option value="10" <% if(selectedStartDate.Month == 10) {Response.Write("SELECTED=SELECTED"); }%>>October</option>
        <option value="11" <% if(selectedStartDate.Month == 11) {Response.Write("SELECTED=SELECTED"); }%>>November</option>
        <option value="12" <% if(selectedStartDate.Month == 12) {Response.Write("SELECTED=SELECTED"); }%>>December</option>

    </select></td>
            <td>    
    <select name="from_day">
<% 
    for (int day = 1; day <= 31; day++)
    {
        Response.Write("<option value=\"" + day + "\" ");
        if (day == selectedStartDate.Day)
            Response.Write("SELECTED=SELECTED");
        Response.Write(">" + day + "</option>");
    }
%>
    </select></td>
        </tr>
        <tr>
            <td style="font-size: 10pt;">To</td>
            <td>    
    <select name="to_year">
<% 
        for (int year = (DateTime.Now.Year - 10); year < (DateTime.Now.Year + 10); year++)
    {
        Response.Write("<option value=\"" + year + "\" ");
        if (year == selectedEndDate.Year)
            Response.Write("SELECTED=SELECTED"); 
        Response.Write(">" + year + "</option>");
    }
%>
    </select></td>
            <td>    
    <select name="to_month">
        <option value="1" <% if(selectedEndDate.Month == 1) {Response.Write("SELECTED=SELECTED"); }%>>January</option>
        <option value="2" <% if(selectedEndDate.Month == 2) {Response.Write("SELECTED=SELECTED"); }%>>February</option>
        <option value="3" <% if(selectedEndDate.Month == 3) {Response.Write("SELECTED=SELECTED"); }%>>March</option>
        <option value="4" <% if(selectedEndDate.Month == 4) {Response.Write("SELECTED=SELECTED"); }%>>April</option>
        <option value="5" <% if(selectedEndDate.Month == 5) {Response.Write("SELECTED=SELECTED"); }%>>May</option>
        <option value="6" <% if(selectedEndDate.Month == 6) {Response.Write("SELECTED=SELECTED"); }%>>June</option>
        <option value="7" <% if(selectedEndDate.Month == 7) {Response.Write("SELECTED=SELECTED"); }%>>July</option>
        <option value="8" <% if(selectedEndDate.Month == 8) {Response.Write("SELECTED=SELECTED");} %>>August</option>
        <option value="9" <% if(selectedEndDate.Month == 9) {Response.Write("SELECTED=SELECTED"); }%>>September</option>
        <option value="10" <% if(selectedEndDate.Month == 10) {Response.Write("SELECTED=SELECTED"); }%>>October</option>
        <option value="11" <% if(selectedEndDate.Month == 11) {Response.Write("SELECTED=SELECTED"); }%>>November</option>
        <option value="12" <% if(selectedEndDate.Month == 12) {Response.Write("SELECTED=SELECTED"); }%>>December</option>

    </select></td>
            <td>    
    <select name="to_day">
<% 
        for (int day = 1; day <= 31; day++)
        {
            Response.Write("<option value=\"" + day + "\" ");
            if (day == selectedEndDate.Day)
                Response.Write("SELECTED=SELECTED");
            Response.Write(">" + day + "</option>");
        }
%>
    </select></td>
        </tr>
        <tr>
            <td colspan="4" align="right"><input type="submit" value=">>"/></td>
        </tr>
</table>
<!-- </form> -->
</div>
<%
    if (selectedStudent != null)
    {

        Response.Write("<h3>Student Attendance Report</h3><br />");
        this.displayStudentNameplate(selectedStudent);
        
    }
    else
    {
        Response.Write("<h3>Student not found</h3>");
    }

    if (selectedStudentAbsences != null)
    {

        this.displayAbsenceStatistics(selectedStudentAbsences);
           
        if (false)
        {

            this.displayAbsenceListTable(selectedStudentAbsences);
        }
        else
        {
            this.displayAbsenceListText(selectedStudentAbsences);
        }

    }
    %>
    
</asp:Content>
