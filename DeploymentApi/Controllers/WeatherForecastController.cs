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
        
        public TestPower powshell = new TestPower();
        public List<string> commands =new() {
@"Test-Path IIS:\AppPools\;",
@"Get-Website -Name ;",
@"Expand-Archive -Path C:\inetpub\wwwroot\;.zip -DestinationPath C:\inetpub\wwwroot\;",
@"Start-Website -Name ;",
@"Stop-Website -Name ;",
@"Start-WebAppPool -Name ;",
@"Remove-Item 'C:\inetpub\wwwroot\;",
@"New-WebSite -Name ; -Port ; -IpAddress ; -PhysicalPath C:\inetpub\wwwroot\; -ApplicationPool ;"};
        
        
    //powshell.commands;

        
        //private readonly ILogger<WeatherForecastController> _logger;

        //public WeatherForecastController(ILogger<WeatherForecastController> logger)
        //{
        //    _logger = logger;
        //}

        //[HttpGet(Name = "GetWeatherForecast")]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        //        TemperatureC = Random.Shared.Next(-20, 55),
        //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}
        [HttpPost]
        [Route("/PostFileUploud")]
        [DisableRequestSizeLimit]
        public async Task<HttpStatusCode> FileUpload(IFormFile file)
        {
            
            string filePath =
                Path.Combine(Environment.CurrentDirectory,"file",file.FileName + Path.GetExtension(file.FileName));

            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory,"file"));

            using var stream = file.OpenReadStream();
            using (var fileStream = new FileStream(filePath, FileMode.Create)) { await stream.CopyToAsync(fileStream); }
            try
            {
                if ( powshell.ExecuteCommand(await powshell.UnzipCommand(filePath)).Contains("Finished"))
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
                //if (await powshell.poolCheck(name) != false) { 
                powshell.ExecuteCommand(await powshell.CreateWebCommand(name, port,ipAddr));
                //}
                //return  "error pool not created";
                return "Command executed";

            }
            catch (Exception)
            {
                return "error i dunno";

            }
            
        }


        [HttpPost]
        [Route("/PostCreatePool")]
        public async Task<string> CreatePool(string name)
        {
            try
            {
                //if (await powshell.poolCheck(name) != false)
                //{
                  powshell.ExecuteCommand(await powshell.CreatePoolCommand(name));
                //}
                //return "error pool not created";
                return "Command executed";

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
                //if (await powshell.webCheck(name) != false) { 
                    powshell.ExecuteCommand(await powshell.StopWebCommand(name));
                //}
                return "Command executed";

            }
            catch (Exception)
            {
                return "error i dunno";

            }
            
        }    
        [HttpPost]
        [Route("/PostStartWebsite")]
        public async Task<string> StartWebsite(string name)
        {
            try
            {
                //if (await powshell.webCheck(name) != false) { 
                    powshell.ExecuteCommand(await powshell.StartWebCommand(name));
                //}
                //return  "error web not started";
                return "Command executed";

            }
            catch (Exception)
            {
                return "error i dunno";

            }
            
        }
        [HttpPost]
        [Route("/PostStopPool")]
        public async Task<string> StartPool(string name)
        {
            try
            {
                //if (await powshell.poolCheck(name) != false)
                //{
                 powshell.ExecuteCommand(await powshell.StartWebCommand(name));
                //}
                //return "error pool not stopped";
                return "Command executed";

            }
            catch (Exception)
            {
                return "error i dunno";

            }

        }

    }
}