using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SoftwareOTA.Model;

namespace SoftwareOTA.Contracts
{
    public interface IPackageRepository
    {
        void AddNewUpdate(string packagename, string filename, string version, string packagepath);
        void AddNewUpdate(Package package);
        Package GetLatest(string name);
        Task<Package> SavePackage(IFormFile file);
        Task<byte[]> GetBlobContent(Package package);
    }
}