using Microsoft.AspNetCore.Mvc;
using System.Net;
using ClassLibrary1;

namespace DeploymentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        //private static readonly string[] Summaries = new[]
        //{"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"};

        public TestPower powshell = new();
        //        public List<string> commands =new() {
        //@"Test-Path IIS:\AppPools\;",
        //@"Get-Website -Name ;",
        //@"Expand-Archive -Path C:\inetpub\wwwroot\;.zip -DestinationPath C:\inetpub\wwwroot\;",
        //@"Start-Website -Name ;",
        //@"Stop-Website -Name ;",
        //@"Start-WebAppPool -Name ;",
        //@"Remove-Item 'C:\inetpub\wwwroot\;",
        //@"New-WebSite -Name ; -Port ; -IpAddress ; -PhysicalPath C:\inetpub\wwwroot\; -ApplicationPool ;"};


        [HttpPost]
        [Route("/PostFileUploud")]
        [DisableRequestSizeLimit]
        public async Task<HttpStatusCode> FileUpload(IFormFile file)
        {

            string filePath =
                Path.Combine(Environment.CurrentDirectory, "file", file.FileName + Path.GetExtension(file.FileName));

            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "file"));

            using var stream = file.OpenReadStream();
            using (var fileStream = new FileStream(filePath, FileMode.Create)) { await stream.CopyToAsync(fileStream); }
            try
            {
                if (powshell.ExecuteCommand(await powshell.UnzipCommand(filePath)).Contains("Finished"))
                {
                    powshell.ExecuteCommand(await powshell.DeleteCommand(filePath));
                }

            }
            catch (Exception)
            {


            }
            return HttpStatusCode.OK;
        }

        [HttpPost]
        [Route("/PostCreateWebsite")]
        public async Task<string> CreateWebsite(string name, int port, string ipAddr = "*")
        {
            try
            {
                if (await powshell.poolCheck(name))
                {
                    powshell.ExecuteCommand(await powshell.CreateWebCommand(name, port, ipAddr));
                    return "Website Created";
                }
                else
                    return "There was no ApplicationPool";



            }
            catch (Exception)
            {
                return "error";

            }

        }


        [HttpPost]
        [Route("/PostCreateApplicationPool")]
        public async Task<string> CreatePool(string name)
        {
            try
            {
                if (await powshell.poolCheck(name))
                {
                    powshell.ExecuteCommand(await powshell.CreatePoolCommand(name));
                    return "ApplicationPool Created";
                }
                else
                    return "Couldnt Create ApplicationPool";


            }
            catch (Exception)
            {
                return "error";

            }

        }

        [HttpPost]
        [Route("/PostStartWebsite")]
        public async Task<string> StartWebsite(string name)
        {
            try
            {
                if (await powshell.webCheck(name))
                {
                    powshell.ExecuteCommand(await powshell.StartWebCommand(name));
                    return "Website Started";
                }
                else
                    return "error web not started";

            }
            catch (Exception)
            {
                return "error i dunno";

            }

        }

        [HttpPost]
        [Route("/PostStopWebsite")]
        public async Task<string> StopWebsite(string name)
        {
            try
            {
                if (await powshell.webCheck(name))
                {
                    powshell.ExecuteCommand(await powshell.StopWebCommand(name));
                    return "Stopped Website";
                }
                else
                    return "Couldnt Stop Website";

            }
            catch (Exception)
            {
                return "error i dunno";

            }

        }
        
        [HttpPost]
        [Route("/PostStartApplicationPool")]
        public async Task<string> StartPool(string name)
        {
            try
            {
                if (await powshell.poolCheck(name))
                {
                    powshell.ExecuteCommand(await powshell.StartPoolCommand(name));
                    return "Command executed";
                }
                else
                    return "error pool not stopped";

            }
            catch (Exception)
            {
                return "error i dunno";

            }

        }

        [HttpPost]
        [Route("/PostStopApplicationPool")]
        public async Task<string> StopPool(string name)
        {
            try
            {
                if (await powshell.poolCheck(name))
                {
                    powshell.ExecuteCommand(await powshell.StopPoolCommand(name));
                    return "Command executed";
                }
                else
                    return "error pool not stopped";

            }
            catch (Exception)
            {
                return "error i dunno";

            }

        }
    }
}