
'use strict';

/**
 * Setup constants and variables.
 */
const CODE_FILE = 'CodeFile';
const DYNAMIC_CONTENT_RESOURCE = 'DynamicContentResource';
const DEFAULT_VERSION = '0.0.0.0';
const CODE_FILE_TYPES = ['.upd', '.zip'];
const DYNAMIC_CONTENT_RESOURCE_TYPES = ['.jpg', '.png', '.gif', '.jpeg', '.pdf', '.txt', '.msi', '.rar', '.zip'];
const CODE_FILE_ALLOWED_EXTENSIONS = CODE_FILE_TYPES.join(', ');
const DYNAMIC_CONTENT_RESOURCE_ALLOWED_EXTENSIONS = DYNAMIC_CONTENT_RESOURCE_TYPES.join(', ');
let fileType = CODE_FILE;

/** 
 * Drag & Drop / Input type file, add file button and upload file button
 */
let form = document.getElementById('uploadForm');
let dropArea = document.getElementById('dropArea');
let fileInput = document.getElementById('fileInput');
let addFileButton = document.getElementById('addFileButton');
let uploadFile = document.getElementById('btnUploadFile');
let selectedFile = new File([""], "filename");
let progress = document.getElementById('progress');
let version = document.getElementById('version');
let sha256Hash = document.getElementById('hash');
let progressContainer = document.getElementById('uploadProgressBar');
let fileSize = document.getElementById('fileSize');
let fileInfoPanel = document.getElementById('fileInformation');
let allowedFileTypesText = document.getElementById('allowedFileTypesText');
let filePanelLayout = document.getElementById('filePanelLayout');
let fileId = '?';

/**
 * prevent post backs
 * @param {any} e
 */
function preventDefaults(e) {
    e.preventDefault();
    e.stopPropagation();
}

// ------------------- Drag Drop & Browse Button Begin ---------------------------

// -- Browse button
/**
 * Handle selected files from file input
 */
fileInput.addEventListener('change', handleFile, false);

/**
 * Fires when the add button is clicked and a file is selected
 * @param {File} file
 * @returns
 */
function handleFile(file) {

    var file = getFile();

    //setFileInfo(file);

    console.log('File:', file);

    setFileName(file.name);
    hideElement('dropArea');
    showElement('filePanel');
    toggleUploadFileButton(true);
    validateData();
    return;

}

// -- save file button

/**
 * Enable and Disable the file upload button
 * @param {boolean} flag 
 */
const toggleUploadFileButton = (flag) => {
    uploadFile.disabled = !flag;
};

// -- Drag and Drop

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
 * Returns a file object based on the file input textbox
 * @returns {File}
 */
const getFile = (() => {
    var file = fileInput.files[0];
    if (!file)
        return new File([""], "filename");

    return file;
});

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
        alert(`Invalid file type. The supported type are ${fileType == CODE_FILE ? CODE_FILE_ALLOWED_EXTENSIONS : DYNAMIC_CONTENT_RESOURCE_ALLOWED_EXTENSIONS} files.`)
        return;
    }

    //setFileInfo(file);

    console.log('File:', file);

    setFileName(file.name);

    hideElement('dropArea');
    showElement('filePanel');

    toggleUploadFileButton(true);
    validateData(file);
    return;
}

// ------------------- Drag Drop & Browse Button End ---------------------------

// ------------------- Form Events Begin ---------------------------
/**
 * clear the files form the form and reset the form
 */
const clearFile = () => {

    uploadForm.reset();
};

/**
 * Reset the form
 */
const resetForm = () => {
    setFileName('?');
    hideElement('filePanel');
    showElement('dropArea');
    toggleUploadFileButton(false);
};

// ------------------- Form Events End ---------------------------

// ------------------- General Events Begin -------------------
/**
 *  Hide authenticated and for company number fields
 */
const hideCodeFileFields = () => {
    hideElement('lblForCompanyNumber');
    hideElement('forCompanyNumber');
    hideElement('lblAuthenticated');
    hideElement('authenticated');
};

/**
 * Hide an element by id
 * @param {string} ctrlId
 */
const hideElement = (ctrlId) => {
    var element = document.getElementById(ctrlId);
    element.classList.remove('show');
    element.classList.add('hide');
};

/**
 * Show an element by id
 * @param {string} ctrlId
 */
const showElement = (ctrlId) => {
    var element = document.getElementById(ctrlId);
    element.classList.remove('hide');
    element.classList.add('show');
};

/**
 * Updates the file upload textbox with the client local file path
 * @param {string} oFileInput
 * @param {string} sTargetID
 */
const changeText = (oFileInput, sTargetID) => {
    document.getElementById(sTargetID).value = oFileInput.value;
};

/**
 * Convert file size to readable format
 * @param {number} size
 * @returns
 */
const convertSize = (size) => {
    var sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
    if (size == 0) return '0 Byte';
    var i = parseInt(Math.floor(Math.log(size) / Math.log(1024)));
    return Math.round(size / Math.pow(1024, i), 2) + ' ' + sizes[i];
};

/**
 * Check that the file extension is allow by type
 * @param {string} fileExtension
 * @param {string} fileType
 * @returns
 */
const validateFileExtension = (fileExtension, fileType) => {
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
};

/**
 * Sets the file upload inputs accept file types attribute
 * @param {string} fType
 */
const setCodeType = (fType) => {

    // early out
    if (!fileInput)
        return;

    // check/set file type
    switch (fType) {
        case CODE_FILE:
            {
                fileType = CODE_FILE;
                hideCodeFileFields();
                fileInput.setAttribute('accept', CODE_FILE_ALLOWED_EXTENSIONS);
                allowedFileTypesText.textContent = `Allowed types: [${CODE_FILE_ALLOWED_EXTENSIONS}]`;
            }
            break;
        case DYNAMIC_CONTENT_RESOURCE:
            {
                fileType = DYNAMIC_CONTENT_RESOURCE;
                fileInput.setAttribute('accept', DYNAMIC_CONTENT_RESOURCE_ALLOWED_EXTENSIONS);
                allowedFileTypesText.textContent = `Allowed types: [${DYNAMIC_CONTENT_RESOURCE_ALLOWED_EXTENSIONS}]`;
            }
            break;
        default:
            {
                hideCodeFileFields();
                fileInput.setAttribute('accept', CODE_FILE_ALLOWED_EXTENSIONS);
                allowedFileTypesText.textContent = `Allowed types: [${CODE_FILE_ALLOWED_EXTENSIONS}]`;
            }
            break;
    }
};

/**
 * Set the file size
 */
const setFileSize = (file) => {
    //let file = getFile();
    fileSize.textContent = convertSize(file.size);
};

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
 * Sets the filename in the file Panel
 * @param {string} fileName 
 */
const setFileName = (fileName) => {
    filePanelLayout.innerHTML = fileName;
};

/**
 * Get the file extension based on the file name
 * @param {string} fileName
 * @returns
 */
const getFileExtension = (fileName) => {
    return `.${fileName.split('.').pop()}`;
};


// ------------------- General Events End -------------------

// ------------------- API Begin ---------------------------

/**
 * Post the file and details to the api
 */
// Using Promise syntax:
function postFile() {
    var file = getFile();
    if (file) {
        var formData = new FormData();
        formData.append('file', file);
        fetch('/api/fileupload/validate', {
            method: 'POST',
            body: formData,
            onUploadProgress: function (progressEvent) {
                var percent = (progressEvent.loaded / progressEvent.total) * 100;
                progress.style.width = percent + '%';
            }
        })
            .then(response => response.json())
            .then(data => {
                if (data) {
                    //todo add check if is valid if not we need to show the error to the ui
                    // set version and sha256 in ui
                    version.textContent = data.fileInformation.version;
                    sha256Hash.textContent = data.fileInformation.hash;
                    fileId = data.fileId;
                    console.log(data);
                }
            })
            .catch(error => console.error(error));
    }
}

/**
 * Send the file to the server for validation
 */
const validateData = ((file) => {

    //todo invert this if
    if (!file) {
        file = getFile();
    }

    if (file) {
        // const file = fileInput.files[0];
        const formData = new FormData();
        formData.append('file', file);

        const xhr = new XMLHttpRequest();
        xhr.responseType = 'json';
        xhr.open('POST', '/api/fileupload/validate', true);
        xhr.addEventListener('load', function(e) {
            hideProgress();
            console.log('status: ' + xhr.status);
            if(xhr.status === 200 ) {
                // get the response 
                const data = xhr.response;
                // check data and update the UI
                if(data) {
                    version.textContent = data.fileInformation.version;
                    sha256Hash.textContent = data.fileInformation.hash;
                    fileId = data.fileId;
                    setFileSize(file);
                    showFileInfoPanel();
                    console.log(data);
                }
            }
        });
        xhr.upload.addEventListener('progress', function(e) {
            // update progress bar
            let percent_completed = (e.loaded / e.total) * 100;
            progress.style.width = percent_completed + '%';
        });

        // show progress bar
        showProgress();
        hideFileInfoPanel();
        // send the request
        xhr.send(formData);
        
    }
});

const showProgress = (() => {   
    progressContainer.style.display = 'block';   
});

const hideProgress = (() =>{   
    progressContainer.style.display = 'none';   
});

const showFileInfoPanel = (() => {   
    fileInfoPanel.style.display = 'block';  
});

const hideFileInfoPanel = (() => {   
    fileInfoPanel.style.display = 'none';   
});


//todo implement this call.
function saveFile() {
    alert('saving fileId:' + fileId);
};

//todo implement this call.
function cleanUpFile() {

};

// ------------------- API end ---------------------------