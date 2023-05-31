using System.IO;
using System.Web;

namespace FileUploadPoc.FileUpload
{
    /// <summary>
    /// Summary description for FileUploadHandler1
    /// </summary>
    public class FileUploadHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if (context != null && context.Request.Files.Count > 0)
            {
                HttpPostedFile hpf = context.Request.Files[0] as HttpPostedFile;
                if (hpf.ContentLength > 0)
                {
                    var fname = hpf.FileName;
                    BinaryReader b = new BinaryReader(hpf.InputStream);
                    byte[] binData = b.ReadBytes(hpf.ContentLength);
                    using MemoryStream memoryStream = new MemoryStream(binData);
                    using FileStream file = new FileStream(@$"C:\temp\fileupload\{fname}", FileMode.Create, FileAccess.Write);
                    memoryStream.WriteTo(file);
                }
            }         
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}