<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileUploadControl.ascx.cs" Inherits="FileUploadPoc.FileUpload.FileUploadControl" %>
<link href="/fileupload/styles/fileupload.css" rel="stylesheet" type="text/css" />
<div class="fileupload-container">
    <form id="uploadForm" enctype="multipart/form-data">
        <!-- File Upload -->
        <div id="dropArea" class="fileUploadTextBox">
        <div>
        <svg xmlns="http://www.w3.org/2000/svg" height="1.5em" viewBox="0 0 640 512"><path d="M144 480C64.5 480 0 415.5 0 336c0-62.8 40.2-116.2 96.2-135.9c-.1-2.7-.2-5.4-.2-8.1c0-88.4 71.6-160 160-160c59.3 0 111 32.2 138.7 80.2C409.9 102 428.3 96 448 96c53 0 96 43 96 96c0 12.2-2.3 23.8-6.4 34.6C596 238.4 640 290.1 640 352c0 70.7-57.3 128-128 128H144zm79-217c-9.4 9.4-9.4 24.6 0 33.9s24.6 9.4 33.9 0l39-39V392c0 13.3 10.7 24 24 24s24-10.7 24-24V257.9l39 39c9.4 9.4 24.6 9.4 33.9 0s9.4-24.6 0-33.9l-80-80c-9.4-9.4-24.6-9.4-33.9 0l-80 80z"/></svg>
        </div>
            <div>Drag and drop files</div>
            <span id="allowedFileTypesText"></span>
            <div>or</div>
            <div>
                <input type="file" id="fileInput">
                <button id="addFileButton" onclick="return false;">Browse</button>
            </div>
        </div>
        <div id="filePanel" class="fileUploadTextBox filePanel hide">
            <div class="filePanel-container">
               <label class="fileContentHeader">File:</label>
                <div id="filePanelLayout" class="filePanelLayout fileName">?</div>
                <div class="filePanelDelete">
                    <button type="reset" onclick="this.form.reset();resetForm();return false;" class="btnResetForm">X</button>
                </div>
                <!-- Progress Bar-->
                <div id="uploadProgressBar" class="progressBar">
                    <div id="progress"></div>
                </div>
                <div id="fileInformation" class="fileInfoPanel" style="display: none;">
                  <div id="fileInfo-container"> 
                    <!-- file size-->
                    <label for="fileSize" class="fileSizeLabel labelHeader">File Size:</label>
                    <span id="fileSize" class="fileSizeValue"></span>
                    <!-- version -->
                    <label for="version" class="versionLabel labelHeader">Version:</label>
                    <span id="version" class="versionValue"></span>
                    <!-- sha256 -->
                    <label for="hash" class="sha256Label labelHeader">SHA256:</label>
                    <span id="hash" class="sha256Value"></span>
                    <!-- Description -->
                    <label for="description" class="descriptionLabel labelHeader">Description:</label>
                    <input class="descriptionTextBox large-input" id="description" type="text" />
                     <!-- ForCompanyNumber -->
                    <label id="lblForCompanyNumber" for="forCompanyNumber" class="companyNumberLabel">For Company Number:</label>
                    <input class="companyNumberTextBox small-input" id="forCompanyNumber" type="text" />
                    <!-- Authenticated -->
                    <label id="lblAuthenticated" for="authenticated" class="authenticatedLabel">Authenticated:</label>
                    <input class="authenticationDropDown small-input" id="authenticated" type="text" />
                </div>
                 <!-- Upload Button-->
                    <div class="uploadButton">
                        <div class="btnPanel-container">
                            <div class="submitBtnPanel">
                                <button id="btnUploadFile" type="submit" onclick="saveFile();return false;" disabled class="button btnUploadFile">Save File</button>
                            </div>
                            <div class="cancelBtnPanel">
                                <button id="btnCancel" type="reset" onclick="this.form.reset();resetForm();return false;" class="button btnCancel">Cancel</button>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
           
        </div>
    </form>
</div>
<script type="text/javascript" src="/fileupload/javascript/fileUpload.js"></script>