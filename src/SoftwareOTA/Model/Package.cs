using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoftwareOTA.Model
{
    public class Package : Manifest
    {
        public string FileUri { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
