using System.IO;
using System.Web.Services;

namespace FileUploadPoc.FileUpload
{
    /// <summary>
    /// Summary description for FileUpload
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class FileUpload : WebService, IValidateFiles
    {

        [WebMethod]
        public bool ValidateFile(byte[] contents, string fileName)
        {
            return true;
        }

        [WebMethod]
        public void Upload(byte[] contents, string filename)
        {
            var appData = Server.MapPath("~/App_Data");
            var file = Path.Combine(appData, Path.GetFileName(filename));
            File.WriteAllBytes(file, contents);
        }
    }

    internal interface IValidateFiles
    {
        bool ValidateFile(byte[] contents, string FileName);
    }
}
