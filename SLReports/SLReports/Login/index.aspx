﻿<%@ Page Language="C#"  MasterPageFile="~/BasicTemplate.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SLReports.Login.index" %>
<asp:Content ID="Content1"  ContentPlaceHolderID="Main" runat="server">
    <br />
    <div class="LoginFormContainer">
    <form method="post" runat="server" id="LoginForm"> 
        <table class="CenteredTable">
            <tr>
                <td><div style="font-size: 10pt;">Username</div></td>
                <td><asp:TextBox ID=txtUsername Width="200" Runat=server ></asp:TextBox></td>
            </tr>
            <tr>
                <td><div style="font-size: 10pt;">Password</div></td>
                <td><asp:TextBox ID="txtPassword" Width="200" Runat=server TextMode=Password></asp:TextBox></td>
            </tr>
            <tr>
                <td colspan="2"><div style="color: red; font-weight: bold;font-size: 10pt;"><asp:Label ID="lblStatus" runat="server" Text="" /></div></td>                
            </tr>
            <tr>
                <td colspan="2">
                    <div style="text-align: right;">
                        <img src="../lock.png" /> <asp:Button ID="btnLogin" Runat=server Text="Login" OnClick="btnLogin_Click"></asp:Button>
                    </div>
                </td>
            </tr>
        </table>                       
    </form>
    </div>
</asp:Content>
