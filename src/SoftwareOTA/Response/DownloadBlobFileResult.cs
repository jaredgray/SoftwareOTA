using Microsoft.AspNetCore.Mvc;
using SoftwareOTA.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SoftwareOTA.Response
{
    public class DownloadBlobFileResult : FileContentResult
    {
        public DownloadBlobFileResult(byte[] fileContents, string contentType) : base(fileContents, contentType)
        {
         
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.ContentLength = base.FileContents.Length;
            return base.ExecuteResultAsync(context);
        }

        public override void ExecuteResult(ActionContext context)
        {
            context.HttpContext.Response.ContentLength = base.FileContents.Length;
            base.ExecuteResult(context);
        }
    }
}
