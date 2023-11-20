using Microsoft.AspNetCore.Mvc;
using System.Net;
using ClassLibrary1;

namespace DeploymentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ApplicationPoolController : ControllerBase
    {
        // Find a better name than TestPower and powshell
        public PowerShellRunner powshell = new();

        [HttpPost]
        [Route("/ApplicationPool/Create")]
        public async Task<HttpStatusCode> CreatePool(string name)
        {
            try
            {
                Website website = new(name);
                string pool = await powshell.GetState(name, PowerShellRunner.State.Pool);
                if (!pool.Contains("Started") && !pool.Contains("Stopped"))
                {
                    await powshell.ExecuteCommand(PowerShellRunner.CreatePoolCommand(website));
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
        [Route("/ApplicationPool/Start")]
        public async Task<HttpStatusCode> StartPool(string name)
        {
            try
            {
                Website website = new(name);
                string pool = await powshell.GetState(name, PowerShellRunner.State.Pool);
                if (pool.Contains("Stopped"))
                {
                    await powshell.ExecuteCommand(PowerShellRunner.StartPoolCommand(website));
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
        [Route("/ApplicationPool/Stop")]
        public async Task<HttpStatusCode> StopPool(string name)
        {
            try
            {
                Website website = new(name);
                string pool = await powshell.GetState(name, PowerShellRunner.State.Pool);
                if (pool.Contains("Started"))
                {
                    await powshell.ExecuteCommand(PowerShellRunner.StopPoolCommand(website));
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
        [Route("/ApplicationPool/Delete")]
        public async Task<HttpStatusCode> DeletePool(string name)
        {
            try
            {
                Website website = new(name);
                string pool = await powshell.GetState(name, PowerShellRunner.State.Pool);
                if (pool.Contains("Stopped"))
                {
                    await powshell.ExecuteCommand(PowerShellRunner.DeletePoolCommand(website));
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
