<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileUploadControl.ascx.cs" Inherits="FileUploadPoc.FileUpload.FileUploadControl" %>
  <link href="/FileUpload/test.css" rel="stylesheet" type="text/css" />
<div>
    <div>
      File Name:
      <asp:FileUpload ID="FileUpload" runat="server"/>
    </div>
    <div>
      Version:
      <asp:TextBox ID="FileVersionTextBox" runat="server" onblur="OnBlur();" />
    </div>
     <div>
      File Size:
      <asp:TextBox ID="FileSizeTextBox" runat="server" onblur="OnBlur();" />
    </div>
    <div>
      SHA256:
      <asp:TextBox ID="HashTextBox" runat="server" />
    </div>
    <div>
      <asp:Button ID="btnSubmit" runat="server" Text="Upload File" />
    </div>
    <asp:LinkButton id="lnkHidden" runat="server" OnClick="OnBlur" />
    </div>

<asp:Label Text="Hello World"  CssClass="test-1" runat="server"></asp:Label>

<%--<script type="text/javascript">

    function OnBlur() {
        console.log("before click")
        document.getElementById("<%=lnkHidden.ClientID%>").click();
        console.log("after click")
    };

</script>--%>