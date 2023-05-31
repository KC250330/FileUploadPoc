using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FileUploadPoc.FileUpload
{
    public class FileUploadController : ApiController
    {
        // GET: api/FileUpload
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/FileUpload/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/FileUpload
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/FileUpload/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/FileUpload/5
        public void Delete(int id)
        {
        }
    }
}
