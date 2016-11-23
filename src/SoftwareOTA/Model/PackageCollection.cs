using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoftwareOTA.Model
{
    public class PackageCollection
    {
        public PackageCollection() { Packages = new List<Package>(); }
        public List<Package> Packages { get; set; }
    }
}
