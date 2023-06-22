using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web.UI;
using FileUploadPoc.FileUpload.Models;

namespace FileUploadPoc.FileUpload
{
    public partial class FileUploadControl : UserControl
    {
        #region Private fields

        private byte[] CurrentFileContents { get; set; }
        private byte[] CurrentFileHash { get; set; }

        #endregion

        #region Public Properties

        
        public string FileType { get; set; }

        public string[] Environments { get; set; }

        public string UserName { get; set; }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #region Events

        protected void OnBlur(object sender, EventArgs e)
        {
            GenerateHash();

            GetFileSize();

            ValidateFile();
        }
        #endregion

        #region Helpers


        private void GetFileSize()
        {
            if (FileUpload.HasFiles)
            {
                // set the file size text box
                FileSizeTextBox.Text = FileUpload.FileBytes.Length.BytesToString();
            }
        }

        private void GenerateHash()
        {
           if(FileUpload.HasFiles)
            {
                // get the file content
                CurrentFileContents = FileUpload.FileBytes;
                // get the sha256 hash and set the text box
                using (SHA256Managed Sha = new SHA256Managed())
                {
                    CurrentFileHash = Sha.ComputeHash(CurrentFileContents);
                    HashTextBox.Text = string.Concat(CurrentFileHash.Select(b => string.Format("{0:X2}", b)));
                }
            }
        }

        private bool ValidateFile()
        {
            if (FileUpload.HasFiles)
            {
                switch (FileType)
                {
                    case "CodeFile":
                        return ValidateCodeFile(FileUpload.FileBytes, FileUpload.FileName);
                    case "DynamicContentResource":
                        return ValidateDynamicContentResource(FileUpload.FileName);
                    default:
                        throw new InvalidOperationException(string.Format("'{0}' is not a known FileType", FileType));
                }
            }

            return false;
        }

        private bool ValidateDynamicContentResource(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            var fileExtension = Path.GetExtension(fileName);
            switch (fileExtension.ToLower())
            {
                case ".gif":
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".pdf":
                case ".msi":
                case ".rar":
                case ".zip":
                case ".txt":
                    FileVersionTextBox.Text = "n/a";
                    //FileNameTextBox.Text = file.Name;
                    return true;
            }
            throw new NotImplementedException();
        }

        private bool ValidateCodeFile(byte[] file, string fileName)
        {
            string fName = fileName.ToLower();
            string fileExtension = Path.GetExtension(fName);

            if (string.Compare(fName, "libmtx_eps.upd") == 0)
            {
                string version = null;
                //todo we need to read the byte to a string and check the EPSDLLVERSION

                if (version == null)
                {
                    // MessageBox.Show("Error: Invalid file selected, version could not be read. Please try another file.", "Invalid File", MessageBoxButton.OK);
                    return false;
                }

                FileVersionTextBox.Text = version;
            }
            else if (fileExtension.EndsWith(".upd"))
            {
                try
                {
                    PEFile PEFile = PEFile.Parse(CurrentFileContents);
                    if ((PEFile.Header.Characteristics & 0x2000) == 0)
                        throw new InvalidOperationException("File is not a DLL.");
                    FileVersionInfo FileInfo = FileVersionInfo.GetVersionInfo(PEFile);
                    FileVersionTextBox.Text = FileInfo.FileVersion.ToString();
  
                }
                catch (Exception Exception)
                {
                    //MessageBox.Show(string.Format("Error: Invalid file selected, could not parse PE format. Please try another file.\r\n\r\nError: {0}", Exception.Message), "Invalid File Type", MessageBoxButton.OK);
                    return false;
                }
            }
            else if (fileExtension.EndsWith(".zip"))
            {
                string[] FileParts = System.Text.RegularExpressions.Regex.Split(fileExtension, @"(\.)");
                if (FileParts.Length == 7)
                {
                    //FileNameTextBox.Text = String.Format("{0}.{1}.zip", FileParts[0], FileParts[2]);
                    FileVersionTextBox.Text = FileParts[4];
                }
                else
                {
                    //MessageBox.Show("Error: Invalid name pattern for this file type.\r\n\r\nExpected format: termcode.filename.version.zip\r\n\r\nPlease try another file or correct the name of this file and try again.", "Invalid File Name", MessageBoxButton.OK);
                    return false;
                }
            }
            else
            {
                //MessageBox.Show("Error: Invalid file selected. Please try another file.", "Invalid File Type", MessageBoxButton.OK);
                return false;
            }

            return true;
        }


        #endregion
    }

    public static class Helpers
    {
        public static string BytesToString(this int byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }
    }
}