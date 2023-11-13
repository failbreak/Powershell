using Microsoft.AspNetCore.Mvc;
using System.Net;
using ClassLibrary1;

namespace DeploymentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        // Find a better name than TestPower and powshell
        public TestPower powshell = new();

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
            int exitcode = Convert.ToInt32(await powshell.ExecuteCommand(powshell.UnzipCommand(filePath)));
            if (exitcode == 0)
            {
               await powshell.ExecuteCommand(powshell.DeleteCommand(filePath));
                return HttpStatusCode.OK;
            }
            else if (exitcode == 20)
            {
               await powshell.ExecuteCommand(powshell.DeleteCommand(filePath));
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
                string pool = await powshell.GetState(name, TestPower.State.Pool);
                string web = await powshell.GetState(name, TestPower.State.Web);
                if (pool.Contains("Started") || pool.Contains("Stopped") && !web.Contains("Started") && !web.Contains("Stopped")) // check if logic is correct
                {
                    await powshell.ExecuteCommand(powshell.CreateWebCommand(name, port, ipAddr));
                    return @$"Website: {name} Created.";
                }
                else if (pool.Contains("null") && !web.Contains("Started") && !web.Contains("Stopped")) // check if logic is correct
                {
                    await powshell.ExecuteCommand(powshell.CreatePoolCommand(name));
                    await powshell.ExecuteCommand(powshell.CreateWebCommand(name, port, ipAddr));
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
                string pool = await powshell.GetState(name, TestPower.State.Pool);
                if (!pool.Contains("Started") && !pool.Contains("Stopped"))
                {
                await powshell.ExecuteCommand(powshell.CreatePoolCommand(name));
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
                string pool = await powshell.GetState(name, TestPower.State.Pool);
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
            string web = await powshell.GetState(name, TestPower.State.Web);
            if (!web.Contains("Started"))
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
                string web = await powshell.GetState(name, TestPower.State.Web);
                if (web.Contains("Started"))
                {
                    await powshell.ExecuteCommand(powshell.StartWebCommand(name));
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
                string web = await powshell.GetState(name, TestPower.State.Web);
                if (web.Contains("Stopped"))
                {
                    await powshell.ExecuteCommand(powshell.StopWebCommand(name));
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
                string pool = await powshell.GetState(name, TestPower.State.Pool);
                if (pool.Contains("Stopped"))
                {
                    await powshell.ExecuteCommand(powshell.StartPoolCommand(name));
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
        [Route("/PostDeleteApplicationPool")]
        public async Task<string> DeletePool(string name)
        {
            try
            {
                string pool = await powshell.GetState(name, TestPower.State.Pool);
                if (pool.Contains("Stopped"))
                {
                    await powshell.ExecuteCommand(powshell.DeletePoolCommand(name));
                    return @$"ApplicationPool: {name} Has been Deleted.";
                }
                else
                    return @$"ApplicationPool: {name} couldnt be Deleted, Reason: Already deleted or cant be deleted";

            }
            catch (Exception)
            {
                return "error i dunno";

            }

        }
        [HttpPost]
        [Route("/PostDeleteWebSite")]
        public async Task<string> DeleteWeb(string name, bool deletePool = false)
        {
            try
            {
                string Web = await powshell.GetState(name, TestPower.State.Web);
                if (Web.Contains("Stopped") €€)
                {
                    await powshell.ExecuteCommand(powshell.DeletePoolCommand(name));
                    return @$"ApplicationPool: {name} Has been Deleted.";
                }
                else if()
                else
                    return @$"ApplicationPool: {name} couldnt be Deleted, Reason: Already deleted or cant be deleted";

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
                string pool = await powshell.GetState(name, TestPower.State.Pool);
                if (pool.Contains("Started"))
                {
                    await powshell.ExecuteCommand(powshell.StopPoolCommand(name));
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