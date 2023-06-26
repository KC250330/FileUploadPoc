using Newtonsoft.Json;
using System;
using System.Web.UI;

namespace FileUploadPoc.FileUpload
{
    public partial class FileUploadControl : UserControl
    {
        #region Private fields

        #endregion

        #region Public Properties

        
        public string FileType { get; set; }

        public string Environment { get; set; }
        public string[] Environments { get; set; }

        public string UserName { get; set; }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //SetEnvironmentsList();
                SetCodeType();
            }
        }

        //private void SetEnvironmentsList()
        //{
        //    string jsonArray = JsonConvert.SerializeObject(Environments);
        //    Page.ClientScript.RegisterStartupScript(this.GetType(), "setEnvironmentOptions", $"setEnvironmentOptions({jsonArray});", true);
        //}

        private void SetCodeType()
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "setCodeType", $"setCodeType('{FileType}');", true);
        }

        #region Events


        //private void SetEnvironmentsList()
        //{
        //    string[] environments = new string[Environments.Length + 1];
        //    environments[0] = "All Environments"; // set the prepended value for all envs
        //    Array.Copy(Environments, 0, environments, 1, Environments.Length);

        //    EnvironmentsDropDown.DataSource = environments;
        //    EnvironmentsDropDown.DataBind();
        //}

        #endregion

        #region Helpers


        //private void GetFileSize()
        //{

        //    if (FileUpload.PostedFile != null)
        //    {
        //        // set the file size text box
        //        FileSizeTextBox.Value = FileUpload.PostedFile.ContentLength.BytesToString();
        //    }
        //}

        //private void GenerateHash()
        //{
        //    if (FileUpload.PostedFile != null)
        //    {
        //        // get the file content
        //        BinaryReader b = new(FileUpload.PostedFile.InputStream);
        //        CurrentFileContents = b.ReadBytes(FileUpload.PostedFile.ContentLength);

        //        // get the sha256 hash and set the text box
        //        using SHA256Managed Sha = new();
        //        CurrentFileHash = Sha.ComputeHash(CurrentFileContents);
        //        HashTextBox.Value = string.Concat(CurrentFileHash.Select(b => string.Format("{0:X2}", b)));
        //    }

        //}

        #endregion

    }
}