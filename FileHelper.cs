using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;

namespace Confie_Intranet.Helpers.RMS
{
    public static class FileHelper
    {
        
        public static void SetRequestFiles(Dictionary<string, HttpPostedFileBase> RequestFiles)
        {            
            foreach (string key in RequestFiles.Keys)
            {
                if (RequestFiles[key] != null)
                {
                    HttpPostedFileBase file = RequestFiles[key];
                    string AttachmentName = Path.GetFileName(file.FileName);
                    string contentType = file.ContentType;
                    MemoryStream stream = new MemoryStream();
                    file.InputStream.CopyTo(stream);
                    byte[] Attachment = stream.ToArray();                    
                }
            }
        }
    }
}