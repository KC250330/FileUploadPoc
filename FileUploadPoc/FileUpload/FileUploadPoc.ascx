﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileUploadPoc.ascx.cs" Inherits="FileUploadPoc.FileUpload.FileUplaodPoc" %>
<form name="form1" method="post" enctype="multipart/form-data" action="api/fileupload">
    <div>
        <label for="caption">Image Caption</label>
        <input name="caption" type="text" />
    </div>
    <div>
        <label for="image1">Image File</label>
        <input name="image1" type="file" />
    </div>
    <div>
        <input type="submit" value="Submit" />
    </div>
</form>