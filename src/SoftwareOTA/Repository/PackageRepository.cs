using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SoftwareOTA.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Hosting;
using SoftwareOTA.Contracts;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Options;

namespace SoftwareOTA.Repository
{
    public class PackageRepository : IPackageRepository
    {
        public PackageRepository(IHostingEnvironment env, IOptions<ConfigurationOptions> options)
        {
            RepositoryPath = Path.Combine(env.ContentRootPath, @"Data\updates.json");
            this.AllPackages = JsonConvert.DeserializeObject<PackageCollection>(File.ReadAllText(RepositoryPath));
            this.HostingEnvironment = env;
            Options = options.Value;
        }
        string RepositoryPath = null;

        private IHostingEnvironment HostingEnvironment { get; set; }
        private ConfigurationOptions Options { get; set; }
        private PackageCollection AllPackages { get; set; }
        public Package GetLatest(string name)
        {
            return AllPackages.Packages.Where(x => x.PackageName == name).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
        }

        public void AddNewUpdate(string packagename, string filename, string version, string packagepath)
        {
            AllPackages.Packages.Add(new Package()
            {
                PackageName = packagename,
                FileName = filename,
                Version = version,
                FileUri = packagepath,
                CreatedDate = DateTime.Now
            });
            File.WriteAllText(RepositoryPath, JsonConvert.SerializeObject(AllPackages));
        }

        public void AddNewUpdate(Package package)
        {
            AllPackages.Packages.Add(package);
            File.WriteAllText(RepositoryPath, JsonConvert.SerializeObject(AllPackages));
        }

        public async Task<Package> SavePackage(IFormFile file)
        {
            var pkg = new Package();

            using (var zip = new ZipFile(file.OpenReadStream()))
            {
                var manifestentry = zip.GetEntry("manifest.json");
                using (var stream = zip.GetInputStream(manifestentry))
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        var manifestcontents = await sr.ReadToEndAsync();
                        var manifest = JsonConvert.DeserializeObject<Manifest>(manifestcontents);
                        var binentry = zip.GetEntry(manifest.FileName);
                        using (var binstream = zip.GetInputStream(binentry))
                        {
                            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Options.AzureConnection);

                            // Create the blob client.
                            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                            // Retrieve a reference to a container.
                            CloudBlobContainer container = blobClient.GetContainerReference("softwareimages");

                            CloudBlockBlob blockBlob = container.GetBlockBlobReference(binentry.Name);

                            await blockBlob.UploadFromStreamAsync(binstream);

                            pkg.FileUri = blockBlob.StorageUri.PrimaryUri.ToString();
                            pkg.FileName = manifest.FileName;
                            pkg.PackageName = manifest.PackageName;
                            pkg.Version = manifest.Version;
                            pkg.CreatedDate = DateTime.Now;
                            
                        }
                    }
                }
            }
            return pkg;
        }

        public async Task<byte[]> GetBlobContent(Package package)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Options.AzureConnection);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference("softwareimages");

            var blobref = container.GetBlobReference(package.FileName);
            await blobref.FetchAttributesAsync();
            var data = new byte[blobref.Properties.Length];

            await blobref.DownloadToByteArrayAsync(data, 0);

            return data;
        }
    }
}
