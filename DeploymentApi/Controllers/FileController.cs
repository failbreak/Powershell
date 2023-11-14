using ClassLibrary1;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DeploymentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class FileController : Controller
    {
        public TestPower powshell = new();
        [HttpPost]
        [Route("/FileUploud")]
        [DisableRequestSizeLimit]
        public async Task<HttpStatusCode> FileUpload(IFormFile file)
        {

            string filePath =
                Path.Combine(Environment.CurrentDirectory, "file", file.FileName + Path.GetExtension(file.FileName));

            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "file"));

            using var stream = file.OpenReadStream();
            using (var fileStream = new FileStream(filePath, FileMode.Create)) { await stream.CopyToAsync(fileStream); }
            int exitcode = Convert.ToInt32(await powshell.ExecuteCommand(TestPower.UnzipCommand(filePath)));
            if (exitcode == 0)
            {
                await powshell.ExecuteCommand(TestPower.DeleteCommand(filePath));
                return HttpStatusCode.OK;
            }
            else if (exitcode == 20)
            {
                await powshell.ExecuteCommand(TestPower.DeleteCommand(filePath));
                return HttpStatusCode.Conflict;
            }
            else
                return HttpStatusCode.InternalServerError;
        }

    }
}
