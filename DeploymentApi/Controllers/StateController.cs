using ClassLibrary1;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DeploymentApi.Controllers
{
    [Area("State")]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class StateController : Controller
    {
        public PowerShellRunner powshell = new();

        [HttpGet]
        [Route("/GetState/")]
        public async Task<HttpStatusCode> GetPoolState(string name, PowerShellRunner.State Option)
        {
            try
            {
                string state = await powshell.GetState(name, Option);
                if (Option != PowerShellRunner.State.Pool || Option != PowerShellRunner.State.Web)
                {
                    return state.Contains("True") ? HttpStatusCode.OK : HttpStatusCode.Conflict;
                }
                else
                {
                    return state.Contains("Started") ? HttpStatusCode.OK : HttpStatusCode.Conflict;
                }
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
            }
        }       
    }
}
