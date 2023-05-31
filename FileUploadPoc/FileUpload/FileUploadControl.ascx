<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileUploadControl.ascx.cs" Inherits="FileUploadPoc.FileUpload.FileUploadControl" %>
  <link href="/fileupload/fileupload.css" rel="stylesheet" type="text/css" />
 <div class="fileupload-container">
     <form id="uploadForm" enctype="multipart/form-data">
        <!-- File Upload -->
        <div id="dropArea" class="fileUploadTextBox">
            <div>Drag and drop files</div>
            <div>or</div>
            <div>
                <input type="file" id="fileInput">
                <button id="addFileButton" onclick="return false;" >Browse</button>
            </div>
        </div>
        <div id="filePanel" class="fileUploadTextBox filePanel hide">
            <div class="filePanel-container">
            <div id="filePanelLayout" class="filePanelLayout">?</div>
            <div class="filePanelDelete">
                <button type="reset" onclick="this.form.reset();resetForm();return false;" class="btnResetForm">X</button>
            </div>
            </div>
        </div>
        <!-- Version -->
    <div class="versionContainer">
    <label for="version" class="versionLabel">Version:</label>
        <input id="version" type="text" required 
            oninvalid="this.setCustomValidity('Version cannot be empty.')" 
            onchange="this.setCustomValidity('')" 
        class="versionTextBox small-input" />
        
    </div>
        <!-- FileSize -->
        <label for="fileSize" class="fileSizeLabel">File Size:</label>
        <input id="fileSize" required 
            oninvalid="this.setCustomValidity('File Size cannot be empty.')" 
            onchange="this.setCustomValidity('')"  
            class="fileSizeTextBox small-input"  type="text" />
        <!-- Environment -->
        <label id="lblEnvironments" for="environment" class="environmentLabel">Environment:</label>
        <div bid="environmentsContainer" class="environmentDropDown">
             <select id="environments"><option value="">All Environment</option></select>
        </div>
        <!-- SHA256 -->
        <label for="hash" class="sha256Label">SHA256:</label>
        <input id="hash" type="text" required 
            oninvalid="this.setCustomValidity('SHA256 cannot be empty.')" 
            onchange="this.setCustomValidity('')" 
            class="sha256TextBox large-input" />
        <!-- Description -->
        <label for="description" class="descriptionLabel">Description:</label>
        <input class="descriptionTextBox large-input" id="description" type="text" />
        <!-- ForCompanyNumber -->
        <label id="lblForCompanyNumber" for="forCompanyNumber" class="companyNumberLabel">For Company Number:</label>
        <input class="companyNumberTextBox small-input" id="forCompanyNumber" type="text" />
        <!-- Authenticated -->
        <label id="lblAuthenticated" for="authenticated" class="authenticatedLabel">Authenticated:</label>
        <input class="authenticationDropDown small-input" id="authenticated" type="text" />
        <!-- Upload Button-->
        <div class="uploadButton">
            <div class="btnPanel-container">
                <div class="submitBtnPanel">
                     <button id="btnUploadFile" type="submit" onclick="postFile();return false;" disabled class="button btnUploadFile">UploadFile</button>
                </div>
                <div class="cancelBtnPanel">
                    <button id="btnCancel" type="reset" class="button btnCancel">Cancel</button>
                </div>
            </div>
           
        </div>
    </form>
</div>
<!-- progress bar -->
<div>
    <progress id="fileProgress" style="display: none"></progress>
    <div id="progressBar">
        <div id="progress"></div>
    </div>
</div>
<script type="text/javascript" src="/fileupload/fileupload.js"></script>