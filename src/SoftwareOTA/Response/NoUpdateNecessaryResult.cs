using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoftwareOTA.Response
{
    public class NoUpdateNecessaryResult : ContentResult
    {
        public NoUpdateNecessaryResult()
        {
            base.ContentType = "text/plain; charset=utf8";
            base.StatusCode = 304;
        }
    }
}
