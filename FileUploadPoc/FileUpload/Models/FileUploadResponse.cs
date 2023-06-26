using System;

namespace FileUploadPoc.FileUpload.Models
{
    public class FileUploadResponse
    {
        public FileUploadInfo FileInformation { get; set; } = new FileUploadInfo();
        public Guid FileId { get; set; }
        public bool IsValid { get; set; }
        public string Message { get; set; }
    }
}