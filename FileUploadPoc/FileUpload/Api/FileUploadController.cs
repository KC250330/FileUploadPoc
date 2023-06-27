using FileUploadPoc.FileUpload.Models;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Security.Cryptography;
using System.Linq;
using WebGrease.Activities;

namespace FileUploadPoc.FileUpload
{
    [Route("api/fileupload")]
    public class FileUploadController : ApiController
    {

        #region Private fields
       
        private const string _basePath = @$"C:\\temp\\fileupload\\";

        #endregion

        #region Api

        [HttpPost]
        [Route("api/fileupload/validate")]
        public Task<HttpResponseMessage> ValidateUpload()
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;

            // create a new guid for folder level operations. Note: guid will be returned in the response
            var guid = Guid.NewGuid();

            // create a guid dir
            string filePath = @$"{_basePath}{guid}";
            Directory.CreateDirectory(filePath);

            if (httpRequest.Files.Count > 0)
            {
                FileUploadResponse validateResult = new FileUploadResponse();
 
                // foreach (string file in httpRequest.Files)
                //{
                    var postedFile = httpRequest.Files[0];
                    filePath += @$"\\{postedFile.FileName}";

                    var fileExtension = Path.GetExtension(filePath).ToLower();
                    // save to disk
                    postedFile.SaveAs(filePath);
                    // read it back to a file info
                    FileInfo fileInfo = new FileInfo(filePath);
                    // validate
                    validateResult = ValidateCodeFile(fileInfo, guid);
                //}

                if (!validateResult.IsValid)
                   result = Request.CreateResponse(HttpStatusCode.BadRequest, validateResult);
                else
                    result = Request.CreateResponse(HttpStatusCode.OK, validateResult);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return Task.FromResult(result);
        }

        [HttpPost]
        [Route("api/fileupload/upload")]
        public Task<HttpResponseMessage> Post()
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count > 0)
            {
                FileUploadResponse validateResult = new FileUploadResponse();

                // foreach (string file in httpRequest.Files)
                //{

                //var postedFile = httpRequest.Files[0];
                byte[] fileData = null;
                using (var binaryReader = new BinaryReader(httpRequest.Files[0].InputStream))
                {
                    fileData = binaryReader.ReadBytes(httpRequest.Files[0].ContentLength);
                }

                // validate
                validateResult = ValidateCodeFile(fileData, httpRequest.Files[0].FileName);
                //}

                if (!validateResult.IsValid)
                    result = Request.CreateResponse(HttpStatusCode.BadRequest, validateResult);
                else
                    result = Request.CreateResponse(HttpStatusCode.OK, validateResult);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return Task.FromResult(result);
        }

        [HttpGet]
        [Route("api/fileupload/cleanup/{guid}")]
        public Task<HttpResponseMessage> CleanUp(Guid guid)
        {
            HttpResponseMessage result = null;
            if(RemovedDirectory(guid))
            {
                //todo get a response form the db and return more info
                result = Request.CreateResponse(HttpStatusCode.OK, "File Removed");
            }
            else
            {
                //todo add logic to return actual error
                result = Request.CreateResponse(HttpStatusCode.InternalServerError, "Error Removing file");
            }

            return Task.FromResult(result);
        }

        [HttpGet]
        [Route("api/fileupload/heartbeat")]
        public Task<HttpResponseMessage> Heartbeat()
        {
            HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK, "ping");
            return Task.FromResult(result);

        }

        #endregion

        #region Helpers

        /// <summary>
        /// remove a directory by guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        private bool RemovedDirectory(Guid guid)
        {
            // build directory path
            string dirPath = $"{_basePath}{guid}";

            try
            {
                // delete the directory
                Directory.Delete(dirPath, true);
                return true;
            }
            catch (Exception ex)
            {
                //todo log error 
                return false;
            } 
        }

        private FileUploadResponse ValidateCodeFile(byte[] file, string fileName)
        {
            FileUploadResponse response = new FileUploadResponse();
            
            // get sha256 hash
            response.FileInformation.Hash = GetHash(file);
            // get the content
            string content = System.Text.Encoding.UTF8.GetString(file, 0, file.Length);

            if (string.Compare(fileName.ToLower(), "libmtx_eps.upd") == 0)
            {
                string version = null;

                if (content.IndexOf("EPSDLLVERSION=") > 0)
                {
                    int StartPos = content.IndexOf("EPSDLLVERSION=") + 14;

                    int EndPos = StartPos;
                    for (EndPos = StartPos; EndPos < content.Length; EndPos++)
                    {
                        char theChar = content[EndPos];
                        if (!(char.IsNumber(theChar) || theChar == '.')) break;
                    }
                    version = content.Substring(StartPos, EndPos - StartPos);
                }

                if (version == null)
                {
                    response.Message = "Error: Invalid file selected, version could not be read. Please try another file.";
                    return response;
                }

                response.IsValid = true;
                response.FileInformation.Version = version;
                response.FileInformation.FileName = fileName;

            }
            else if (fileName.EndsWith(".upd"))
            {
                try
                {

                    PEFile PEFile = PEFile.Parse(file);
                    if ((PEFile.Header.Characteristics & 0x2000) == 0)
                        throw new InvalidOperationException("File is not a DLL.");

                    FileVersionInfo FileInfo = FileVersionInfo.GetVersionInfo(PEFile);
                    response.IsValid = true;
                    response.FileInformation.Version = FileInfo.FileVersion.ToString();
                    response.FileInformation.FileName = fileName;
                }
                catch (Exception Exception)
                {
                    response.Message = $"Error: Invalid file selected, could not parse PE format. Please try another file.\r\n\r\nError: {Exception.Message}";
                    return response;
                }
            }
            else if (fileName.EndsWith(".zip"))
            {
                string[] FileParts = System.Text.RegularExpressions.Regex.Split(fileName, @"(\.)");
                if (FileParts.Length == 7)
                {
                    response.IsValid = true;
                    response.FileInformation.Version = FileParts[4];
                    response.FileInformation.FileName = $"{FileParts[0]}.{FileParts[2]}.zip";
                }
                else
                {
                    response.Message = "Error: Invalid name pattern for this file type.\r\n\r\nExpected format: termcode.filename.version.zip\r\n\r\nPlease try another file or correct the name of this file and try again.";
                    return response;
                }
            }
            else
            {
                response.Message = "Error: Invalid file selected. Please try another file.";
                return response;
            }
            return response;
        }

        /// <summary>
        /// Validate the provide file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
           
        private FileUploadResponse ValidateCodeFile(FileInfo file, Guid guid)
        {
            string fileName = file.Name.ToLower();
            FileUploadResponse  response  = new FileUploadResponse { FileId = guid };
            byte[] fileBytes;
            //Open file for Read\Write
            using (FileStream fs = new FileStream(file.FullName, FileMode.Open,
                                      FileAccess.Read, FileShare.ReadWrite))
            {
                //create byte array of same size as FileStream length
                fileBytes = new byte[fs.Length];
            }

            response.FileInformation.Hash = GetHash(fileBytes);

            if (string.Compare(fileName, "libmtx_eps.upd") == 0)
            {
                string version = null;
                using (StreamReader Stream = file.OpenText())
                {
                    string Line = null;
                    while ((Line = Stream.ReadLine()) != null)
                    {
                        if (Line.IndexOf("EPSDLLVERSION=") > 0)
                        {
                            int StartPos = Line.IndexOf("EPSDLLVERSION=") + 14;

                            int EndPos = StartPos;
                            for (EndPos = StartPos; EndPos < Line.Length; EndPos++)
                            {
                                char theChar = Line[EndPos];
                                if (!(char.IsNumber(theChar) || theChar == '.')) break;
                            }
                            version = Line.Substring(StartPos, EndPos - StartPos);
                            break;
                        }
                    }

                    if (version == null)
                    {
                        response.Message = "Error: Invalid file selected, version could not be read. Please try another file.";
                        return response;
                    }

                    response.IsValid = true;
                    response.FileInformation.Version = version;
                    response.FileInformation.FileName = fileName;
                }
            }
            else if (fileName.EndsWith(".upd"))
            {
                try
                {

                    PEFile PEFile = PEFile.Parse(fileBytes);
                    if ((PEFile.Header.Characteristics & 0x2000) == 0)
                        throw new InvalidOperationException("File is not a DLL.");

                    FileVersionInfo FileInfo = FileVersionInfo.GetVersionInfo(PEFile);
                    response.IsValid = true;
                    response.FileInformation.Version = FileInfo.FileVersion.ToString();
                    response.FileInformation.FileName = fileName;
                }
                catch (Exception Exception)
                {
                    response.Message = $"Error: Invalid file selected, could not parse PE format. Please try another file.\r\n\r\nError: {Exception.Message}";
                    return response;
                }
            }
            else if (fileName.EndsWith(".zip"))
            {
                string[] FileParts = System.Text.RegularExpressions.Regex.Split(fileName, @"(\.)");
                if (FileParts.Length == 7)
                {
                    response.IsValid = true;
                    response.FileInformation.Version = FileParts[4];
                    response.FileInformation.FileName = $"{FileParts[0]}.{FileParts[2]}.zip";
                }
                else
                {
                    response.Message = "Error: Invalid name pattern for this file type.\r\n\r\nExpected format: termcode.filename.version.zip\r\n\r\nPlease try another file or correct the name of this file and try again.";
                    return response;
                }
            }
            else
            {
                response.Message = "Error: Invalid file selected. Please try another file.";
                return response;
            }

            return response;
        }

        /// <summary>
        /// Generate a sha256 hash
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string GetHash(byte[] data) {

            using (SHA256Managed Sha = new SHA256Managed())
            {
               var currentFileHash = Sha.ComputeHash(data);
               return string.Concat(currentFileHash.Select(b => string.Format("{0:X2}", b)));
            }
        }

        #endregion
    }
}
