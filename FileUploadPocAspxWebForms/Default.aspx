<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="FileUploadPoc._Default" %>
<%@ Register Src="~/FileUpload/FileUploadControl.ascx" TagPrefix="uc1" TagName="FileUploadControl" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main>
       <div>
           <uc1:FileUploadControl runat="server" ID="FileUploadControl" FileType="CodeFile" />
        </div>
    </main>
</asp:Content>
