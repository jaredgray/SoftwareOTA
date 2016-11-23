using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SoftwareOTA.Contracts;
using SoftwareOTA.Response;

namespace SoftwareOTA.Controllers
{
    [Route("api/[controller]")]
    public class UpdateController : Controller
    {
        public UpdateController(IPackageRepository repo)
        {
            Repo = repo;
        }
        private IPackageRepository Repo;

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// asks the api for a new update for a particular software package
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{packageName}/{currentVersion}")]
        public async Task<IActionResult> Get(string packageName, string currentVersion)
        {
            var current = Repo.GetLatest(packageName);
            if (null == current || current.Version == currentVersion)
            {
                return new NoUpdateNecessaryResult();
            }
            else
            {
                var data = await Repo.GetBlobContent(current);
                return new DownloadBlobFileResult(data, "application/octet-stream") { FileDownloadName = current.FileName };
            }
        }

        // POST api/Update
        //[HttpPost]
        //public async Task<IActionResult> Post()
        //{
        //    //ICollection<IFormFile> files
        //    //foreach (var file in files)
        //    //{
        //    //    var package = await Repo.SavePackage(file);
        //    //    Repo.AddNewUpdate(package);
        //    //}

        //    foreach(var file in Request.Form.Files)
        //    {
        //        var package = await Repo.SavePackage(file);
        //        Repo.AddNewUpdate(package);
        //    }

        //    return Ok();
        //}
        [HttpPost]
        public async Task<IActionResult> Post(ICollection<IFormFile> files)
        {
            try
            {
                foreach (var file in files)
                {
                    var package = await Repo.SavePackage(file);
                    Repo.AddNewUpdate(package);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return Ok();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
