<%@ Page Title="Pure Js FileUpload" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="FileUploadPoc._Default" %>
<%@ Register Src="~/FileUpload/FileUploadControl.ascx" TagPrefix="uc1" TagName="FileUploadControl" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main>
       <div>
           <uc1:FileUploadControl runat="server" ID="FileUploadControl" FileType="CodeFile"/>
         <%--  <div>&nbsp;</div>
           <uc1:FileUploadAspWebFormsControl runat="server" ID="FileUploadAspWebFormsControl" FileType="CodeFile" Environments="<%# this.Environments %>" />--%>
        </div>
    </main>
</asp:Content>
