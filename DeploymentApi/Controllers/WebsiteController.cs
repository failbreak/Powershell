using ClassLibrary1;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DeploymentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class WebsiteController : Controller
    {
        public PowerShellRunner powshell = new();

        [HttpPost]
        [Route("/Website/Create")]
        public async Task<HttpStatusCode> CreateWebsite(string name, int port, string ipAddr)
        {
            try
            {
                Website website = new(name,ipAddr, port);

                string pool = await powshell.GetState(website.Name, PowerShellRunner.State.Pool);
                string web = await powshell.GetState(website.Name, PowerShellRunner.State.Web);
                if (pool.Contains("Started") || pool.Contains("Stopped") && !web.Contains("Started") && !web.Contains("Stopped")) // check if logic is correct
                {
                    await powshell.ExecuteCommand(PowerShellRunner.CreateWebCommand(website));
                    return HttpStatusCode.OK;
                }
                else if (pool.Contains("null") && !web.Contains("Started") && !web.Contains("Stopped")) // check if logic is correct
                {
                    await powshell.ExecuteCommand(PowerShellRunner.CreatePoolCommand(website));
                    await powshell.ExecuteCommand(PowerShellRunner.CreateWebCommand(website));
                    return HttpStatusCode.OK;
                }
                else
                    return HttpStatusCode.Conflict;
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;

            }

        }
        [HttpPost]
        [Route("/Website/Start")]
        public async Task<HttpStatusCode> StartWebsite(string name)
        {
            try
            {
            Website website = new(name);
                string web = await powshell.GetState(name, PowerShellRunner.State.Web);
                if (web.Contains("Started"))
                {
                    await powshell.ExecuteCommand(PowerShellRunner.StartWebCommand(website));
                    return HttpStatusCode.OK;
                }
                else
                    return HttpStatusCode.Conflict;

            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;

            }

        }


        [HttpPut]
        [Route("/WebBinding/Update")]
        public async Task<HttpStatusCode> UpdateWebBinding(string name, int port, string ipAddr)
        {
            try
            {
                Website website = new(name, ipAddr, port);
                string State = await powshell.GetState(name, PowerShellRunner.State.Web);
                if (!State.Contains("null"))
                {
                    await powshell.ExecuteCommand(PowerShellRunner.SetWebbindingCommand(website));
                    return HttpStatusCode.OK;
                }
                else
                    return HttpStatusCode.Conflict;

            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;

            }

        }


        [HttpPost]
        [Route("/Website/Stop/")]
        public async Task<HttpStatusCode> StopWebsite(string name)
        {
            try
            {
                Website website = new(name);
                string web = await powshell.GetState(name, PowerShellRunner.State.Web);
                if (web.Contains("Stopped"))
                {
                    await powshell.ExecuteCommand(PowerShellRunner.StopWebCommand(website));
                    return HttpStatusCode.OK;
                }
                else
                    return HttpStatusCode.Conflict;

            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;

            }
        }
        [HttpDelete]
        [Route("/Website/Delete")]
        public async Task<HttpStatusCode> DeleteWeb(string name, bool deletePool = false)
        {
            try
            {
                Website website = new(name);
                string Web = await powshell.GetState(name, PowerShellRunner.State.Web);
                string pool = await powshell.GetState(name, PowerShellRunner.State.Pool);
                if (Web.Contains("Stopped") && pool.Contains("Stopped") && deletePool)
                {
                    await powshell.ExecuteCommand(PowerShellRunner.DeletePoolCommand(website));
                    return HttpStatusCode.OK;

                }
                else if (Web.Contains("Stopped") && !deletePool)
                {
                    await powshell.ExecuteCommand(PowerShellRunner.DeleteWebCommand(website));
                    return HttpStatusCode.OK;
                }
                else
                    return HttpStatusCode.Conflict;

            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;

            }


        }
    }
}
