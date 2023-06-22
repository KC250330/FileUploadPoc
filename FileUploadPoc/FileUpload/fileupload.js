'use strict';
// $(function () {
/**
 * Setup constants and variables.
 */
const CODE_FILE = 'CodeFile';
const DYNAMIC_CONTENT_RESOURCE = 'DynamicContentResource';
const DEFAULT_VERSION = '0.0.0.0';
const CODE_FILE_TYPES  = ['.upd','.zip'];
const DYNAMIC_CONTENT_RESOURCE_TYPES  = ['.jpg','.png','.gif','.jpeg','.pdf','.txt','.msi','.rar','.zip'];
let validationCtrl;
let fileType = CODE_FILE;

/** 
 * Drag & Drop / Input type file, add file button and upload file button
 */ 

let dropArea = document.getElementById('dropArea');
let fileInput = document.getElementById('fileInput');
let addFileButton = document.getElementById('addFileButton');
let uploadFile = document.getElementById('btnUploadFile');
let selectedFile = new File([""], "filename");
/**
 * Add event listeners 
 */
//uploadFile.addEventListener('click', postFile, false);

/**
 * Handle selected files from file input
 */
fileInput.addEventListener('change', handleFile, false);

/**
 * Validates file name extension is valid based on codetype
 * @param {File} file
 */
function setFileInfo(file)
{
    // get version
    setVersion(file);
    // get file size
    setFileSize(file);
    // gen hash
    setShaHash(file);
}

/**
 * Set the environmentsDropDown options based on the supplied server array
 * @param {string[]} envs
 * @returns
 */
function setEnvironmentOptions(envs) {
    // get the environments dom object
    let selectElement = document.getElementById('environments');

    // early out
    if (!selectElement)
        return;

    // clear all items
    selectElement.innerHTML = "";

    // add all environments option
    selectElement.add(new Option('All Environments'));

    // loop and add all environments from array
    for (var i = 0; i < envs.length; i++) {
        selectElement.add(new Option(envs[i]));
    }
}

/**
 * Validates the files Version
 * @param {string} ctrlId
 * @returns
 */
function validateFile(ctrlId)
{  
    document.getElementById(ctrlId).click();
    return false;
}

/**
 * Set the file size
 */
function setFileSize(file) {
    //let file = getFile();
    setElementValue('fileSize', convertSize(file.size));
}

/**
 * Gets the version based on the file name
 */
async function setShaHash(file) {
    
    setElementValue('hash', await sha256(getFileContents(file)));
}

/**
 * Gets the version based on the file name
 */
function setVersion(file) {
    // need to parse out the version based on the old .net code
    let version = getVersion(file);
    setElementValue('version', version);
}

/**
 * Sets the value for an input type text
 * @param {string} id 
 * @param {string} value 
 * @returns 
 */
const setElementValue = (id, value) => {

    // get the element by id 
    var ctrl = document.getElementById(id);
    
    // early out
    if (!ctrl)
        return;

    //set the value
    ctrl.value = value;
};

/**
 * try and get the version from the uploaded file
 * @returns
 */
function getVersion(file) {
    //let file = getFile();
    // get the file extension from the file name
    let fExt = getFileExtension(file.name);

    // early out
    if (!fExt)
        return '';

    // check file name for upd
    if (fExt === '.upd') {

        // check for LibmtxEps type
        var version = checkLibmtxEps(file);

        // return the version 
        if (version)
            return version;
    }
    else if (fExt === '.zip') {
        // split on .
        let fileParts = file.name.split('.');

        // check 
        if (fileParts.length == 4) {
            // FileNameTextBox.Text = String.Format("{0}.{1}.zip", FileParts[0], FileParts[2]);
            changeUploadButtonState(true);
            return  fileParts[2];
            // FileVersionTextBox.Text = fileParts[4];
        }
        else {
            changeUploadButtonState(false);
            alert('Error: Invalid name pattern for this file type.\r\n\r\nExpected format: termcode.filename.version.zip\r\n\r\nPlease try another file or correct the name of this file and try again.')
        }
    }
    else {
        // disable upload button
        changeUploadButtonState(false);
        alert('Error: Invalid file selected. Please try another file.');
    }
}

/**
 * Enable/Disable upload button
 * @param {boolean} state
 */
function changeUploadButtonState(state) {
    document.getElementById('btnUploadFile').disable = state;
}

/**
 * Check if file is named libmtx_eps.upd if so it will get the version from the EPSDLLVERSION in the file.
 * @returns
 */
function checkLibmtxEps(file) {
    //let file = getFile();
    // set initial position to 0
    let position = 0;

    // early out if file is undefined or null
    if (!file)
        return DEFAULT_VERSION;

    // early out if file name is not libmtx_eps.upd
    if (!file.name.toLowerCase() === 'libmtx_eps.upd')
        return DEFAULT_VERSION;

    // try and load the file content
    let fileContent = getFileContents();

    // early out if no file content
    if (!fileContent)
        return DEFAULT_VERSION;

    // look for the first instance of EPSDLLVERSION=
    position = fileContent.indexOf("EPSDLLVERSION=");

    //early out search was not satisfied.
    if (!position || position == 0)
        return DEFAULT_VERSION;

    // todo remove this debugger statement 
    debugger

    // return the substring based on the old silverlight code.
    return fileContent.subString(position, 14);
}

/**
 * Get the file extension based on the file name
 * @param {string} fileName
 * @returns
 */
function getFileExtension(fileName) {
    let extension = '';

    var lastDotIndex = fileName.lastIndexOf('.');
    if (lastDotIndex !== -1) {
        extension = fileName.substring(lastDotIndex);
    }
    return extension;
}

/**
 * Get the content of a file
 * @returns
 */
function getFileContents(file) {
    // let file = getFile();
    // early out
    if (!file)
        return null;
    // define a new file reader
    var reader = new FileReader();

    // read the contents
    reader.onload = function (e) {
        if (e.target && e.target.result)
            return new Uint8Array(e.target.result);
        return new byte[0];
    };

   reader.readAsText(file);
}

/**
 * Sets the file upload inputs accept file types attribute
 * @param {string} fType
 */
function setCodeType(fType) {

    // early out
    if (!fileInput)
        return;

    // check/set file type
    switch (fType) {
        case CODE_FILE:
            {
                fileType = CODE_FILE;
                hideCodeFileFields();
                fileInput.setAttribute('accept', '.upd,.zip');
            }
            break;
        case DYNAMIC_CONTENT_RESOURCE:
            {
                fileType = DYNAMIC_CONTENT_RESOURCE;
                hideDynamicContentResourceFields();
                fileInput.setAttribute('accept', '.jpg,.png,.gif,.jpeg,.pdf,.txt,.msi,.rar,.zip');
            }
            break;
        default:
            {
                hideCodeFileFields();
                fileInput.setAttribute('accept', '.upd,.zip');
            }
            break;
    }
}

/**
 * Hides environments select field.
 */
function hideDynamicContentResourceFields() {
    hideElement('lblEnvironments');
    hideElement('environments');
}

/**
 *  Hide authenticated and for company number fields
 */
function hideCodeFileFields() {
    hideElement('lblForCompanyNumber');
    hideElement('forCompanyNumber');
    hideElement('lblAuthenticated');
    hideElement('authenticated');
}

/**
 * Hide an element by name
 * @param {string} ctrlName
 */
function hideElement(ctrlName) {
    var element = document.getElementById(ctrlName);
    element.classList.remove('show');
    element.classList.add('hide');
}

/**
 * Show an element by name
 * @param {string} ctrlName
 */
function showElement(ctrlName) {
    var element = document.getElementById(ctrlName);
    element.classList.remove('hide');
    element.classList.add('show');
}

/**
 * Updates the file upload textbox with the client local file path
 * @param {string} oFileInput
 * @param {string} sTargetID
 */
function changeText(oFileInput, sTargetID) {
    document.getElementById(sTargetID).value = oFileInput.value;
}

/**
 * Convert file size to readable format
 * @param {number} size
 * @returns
 */
function convertSize(size) {
    var sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
    if (size == 0) return '0 Byte';
    var i = parseInt(Math.floor(Math.log(size) / Math.log(1024)));
    return Math.round(size / Math.pow(1024, i), 2) + ' ' + sizes[i];
}

/**
 * Generates a SHA256 encoded message
 * @param {any} message
 * @returns
 */
async function sha256(message) {
    // encode as UTF-8
    const msgBuffer = new TextEncoder().encode(message);

    // hash the message
    const hashBuffer = await crypto.subtle.digest('SHA-256', msgBuffer);

    // convert ArrayBuffer to Array
    const hashArray = Array.from(new Uint8Array(hashBuffer));

    // convert bytes to hex string                  
    const hashHex = hashArray.map(b => b.toString(16).padStart(2, '0')).join('');
    return hashHex;
}

/**
 * Post a file to the back end
 */
function postFileAjax()
{
    $.ajax({
        url: '/fileupload/FileUploadHandler.ashx',
        type: 'POST',
        data: new FormData($('form')[0]),
        cache: false,
        contentType: false,
        processData: false,
        success: function (file) {
            $("#fileProgress").hide();
            $("#lblMessage").html("<b>" + file.name + "</b> has been uploaded.");

        },
        xhr: function () {
            var fileXhr = $.ajaxSettings.xhr();
            if (fileXhr.upload) {
                $("progress").show();
                fileXhr.upload.addEventListener("progress", function (e) {
                    if (e.lengthComputable) {
                        $("#fileProgress").attr({
                            value: e.loaded,
                            max: e.total
                        });
                    }
                }, false);
            }
            
            return fileXhr;
        }
    });
}

/**
 * Check that the file extension is allow by type
 * @param {string} fileExtension
 * @param {string} fileType
 * @returns
 */
function validateFileExtension(fileExtension, fileType) {
    let filter = '';
    switch (fileType) {
        case CODE_FILE:
            filter = CODE_FILE_TYPES.find(element => element === fileExtension);
            break;
        case DYNAMIC_CONTENT_RESOURCE:
            filter = DYNAMIC_CONTENT_RESOURCE_TYPES.find(element => element > fileExtension);
            break;
        default:
            break;
    }
    if (!filter)
        return false;

    return true;
}

/**
 * Prevent default drag behaviors
 */
['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
    dropArea.addEventListener(eventName, preventDefaults, false);
    document.body.addEventListener(eventName, preventDefaults, false);
});

/**
 * Highlight drop area when item is dragged over it
 */
['dragenter', 'dragover'].forEach(eventName => {
    dropArea.addEventListener(eventName, highlight, false);
});

/**
 * Unhighlight drop area when item is dragged out of it
 */
['dragleave', 'drop'].forEach(eventName => {
    dropArea.addEventListener(eventName, unhighlight, false);
});

/**
 * Handle dropped files
 */
dropArea.addEventListener('drop', handleDrop, false);

/**
 * Add event listener to "Add File" button
 */
addFileButton.addEventListener('click', function () {
    fileInput.click();
});


function preventDefaults(e) {
    e.preventDefault();
    e.stopPropagation();
}

/**
 * Change the highlight color
 */
function highlight() {
    dropArea.style.background = '#e1e7f0';
}

/**
 * Change to default 
 */
function unhighlight() {
    dropArea.style.background = 'none';
}

/**
 * Handles the drop file event
 * @param {Event} e 
 */
function handleDrop(e) {
    var files = e.dataTransfer.files;
    handleFiles(files);
}

/**
 * Sets the filename in the file Panel
 * @param {string} fileName 
 */
function setFileName(fileName){
    document.getElementById('filePanelLayout').innerHTML = fileName;
}

/**
 * clear the files form the form and reset the form
 */
function clearFile(){

    uploadForm.reset();
}

function resetForm(){
    setFileName('?');
    hideElement('filePanel');
    showElement('dropArea');
    toggleUploadFileButton(false);
}

/**
 * Fires when a file is dropped on the drag drop box
 * @param {any} files
 * @returns
 */
function handleFiles(files) {

    selectedFile = files[0];

    var file = files[0];
    var ext = getFileExtension(file.name);

    var isValidFileExtension = validateFileExtension(ext, fileType);
  
    if (!isValidFileExtension) { 
        alert(`Invalid file type. The supported type are ${fileType == CODE_FILE ? CODE_FILE_TYPES.join(', ') : DYNAMIC_CONTENT_RESOURCE_TYPES.join(', ')} files.`)
        return;
    }

    setFileInfo(file);
    
    console.log('File:', file);

    setFileName(file.name);

    hideElement('dropArea');
    showElement('filePanel');
    toggleUploadFileButton(true);
    return;
}

/**
 * Returns a file object based on the file input textbox
 * @returns {File}
 */
const getFile = (()=>
{
    var file = fileInput.files[0];
    if(!file)
        return new File([""], "filename");

    return file;
});

/**
 * Fires when the add button is clicked and a file is selected
 * @param {File} file
 * @returns
 */
function handleFile(file) {

    var file = getFile();

    setFileInfo(file);

    console.log('File:', file);

    setFileName(file.name);
    hideElement('dropArea');
    showElement('filePanel');
    toggleUploadFileButton(true);
    return;

}

/**
 * Enable and Disable the file upload button
 * @param {boolean} flag 
 */
function toggleUploadFileButton(flag) {
    uploadFile.disabled = !flag;
}

function postFile() {

    var file = getFile();

    if (file) {
        var formData = new FormData();
        formData.append('file', file);

        fetch('/fileupload/FileUploadHandler.ashx', {
            method: 'POST',
            body: formData,
            onUploadProgress: function (progressEvent) {
                var percent = (progressEvent.loaded / progressEvent.total) * 100;
                var progress = document.getElementById('progress');
                progress.style.width = percent + '%';
            }
        })
        .then(function (response) {
            if (response.ok) {
                alert('File uploaded successfully.');
                fileInput.value = ''; // Clear the file input
                var progress = document.getElementById('progress');
                progress.style.width = '0%';
            } else {
                alert('Error uploading file.');
            }
        })
        .catch(function (error) {
            alert('Error uploading file: ' + error.message);
        });
    } else {
        alert('Please select a file to upload.');
    }
}
//});