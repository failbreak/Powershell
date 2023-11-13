using Microsoft.AspNetCore.Mvc;
using System.Net;
using ClassLibrary1;

namespace DeploymentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {

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
            int exitcode = Convert.ToInt32(await powshell.ExecuteCommand(await powshell.UnzipCommand(filePath)));
            if (exitcode == 0)
            {
                powshell.ExecuteCommand(await powshell.DeleteCommand(filePath));
                return HttpStatusCode.OK;
            }
            else if (exitcode == 20)
            {
                powshell.ExecuteCommand(await powshell.DeleteCommand(filePath));
                return HttpStatusCode.Conflict;
            }
            else
                return HttpStatusCode.Conflict;


        }

        [HttpPost]
        [Route("/PostCreateWebsite")]
        public async Task<string> CreateWebsite(string name, int port, string ipAddr = "*")
        {
            
            try
            {
                string pool = await powshell.poolGetState(name);
                string web = await powshell.poolGetState(name);
                if (pool.Contains("Started") || pool.Contains("Stopped") && !web.Contains('3') && !web.Contains('1')) // check if logic is correct
                {
                    powshell.ExecuteCommand(await powshell.CreateWebCommand(name, port, ipAddr));
                    return @$"Website: {name} Created.";
                }
                else if (pool.Contains("does not exist") && !web.Contains('3') && !web.Contains('1')) // check if logic is correct
                {
                    powshell.ExecuteCommand(await powshell.CreatePoolCommand(name));
                    powshell.ExecuteCommand(await powshell.CreateWebCommand(name, port, ipAddr));
                    return @$"Website: {name} Created & Applicationpool: {name} Created.";
                }
                else
                    return @$"Website: {name}. could not be Created.";
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
                string pool = await powshell.poolGetState(name);
                if (!pool.Contains("Started") && !pool.Contains("Stopped"))
                {
                powshell.ExecuteCommand(await powshell.CreatePoolCommand(name));
                    return @$"ApplicationPool: {name} Created.";
                }
                else
                    return @$"Couldnt Create ApplicationPool:{name}.";


            }
            catch (Exception)
            {
                return "error";

            }

        }



        [HttpPost]
        [Route("/PostGetPoolState")]
        public async Task<HttpStatusCode> GetPoolState(string name)
        {
            try
            {
                string pool = await powshell.poolGetState(name);
                if (!pool.Contains("Started"))
                {
                    return HttpStatusCode.Conflict;
                }
                else
                {
                    return HttpStatusCode.OK;
                }
            }
            catch (Exception)
            {

                return HttpStatusCode.Conflict;
            }
           
        }


        [HttpPost]
        [Route("/PostGetWebState")]
        public async Task<HttpStatusCode> GetWebSate(string name)
        {
            try
            {
            string web = await powshell.WebGetState(name);
            if (!web.Contains("3"))
            {
                return HttpStatusCode.Conflict;
            }
            else
            {
                return HttpStatusCode.OK;
            }

            }
            catch (Exception)
            {

                return HttpStatusCode.Conflict;
            }
        }

            [HttpPost]
        [Route("/PostStartWebsite")]
        public async Task<string> StartWebsite(string name)
        {
            try
            {
                string web = await powshell.WebGetState(name);
                if (web.Contains("3"))
                {
                    powshell.ExecuteCommand(await powshell.StartWebCommand(name));
                    return @$"Website: {name}.  Has been started.";
                }
                else
                    return @$"{name} could not be started, Reason: already on or doesnt exist";

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
                string web = await powshell.WebGetState(name);
                if (web.Contains("1"))
                {
                    powshell.ExecuteCommand(await powshell.StopWebCommand(name));
                    return @$"Stopped Website: {name}.";
                }
                else
                    return @$"Couldnt Stop Website: {name}, Reason: already off or doesnt exist";

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
                string pool = await powshell.poolGetState(name);
                if (pool.Contains("Stopped"))
                {
                    powshell.ExecuteCommand(await powshell.StartPoolCommand(name));
                    return @$"ApplicationPool: {name} Has been started.";
                }
                else
                    return @$"ApplicationPool: {name} couldnt be started, Reason: already on or doesnt exist";

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
                string pool = await powshell.poolGetState(name);
                if (pool.Contains("Started"))
                {
                    powshell.ExecuteCommand(await powshell.StopPoolCommand(name));
                return @$"ApplicationPool: {name} Stopped";
                }
                else
                    return @$"ApplicationPool: {name} couldnt be started, Reason: already on or doesnt exist";


            }
            catch (Exception)
            {
                return "error i dunno";

            }
        }
        //[HttpPost]
        //[Route("/PostTestCommand")]
        //public async Task<string> Test(string name)
        //{
        //    return powshell.ExecuteCommand(await powshell.CommandCheck(name.ToString()));
        //}
    }
}