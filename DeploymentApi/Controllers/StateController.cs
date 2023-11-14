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
        public TestPower powshell = new();


        [HttpGet]
        [Route("/GetState/")]
        public async Task<HttpStatusCode> GetPoolState(string name, TestPower.State Option)
        {
            try
            {
                string state = await powshell.GetState(name, Option);
                if (Option != TestPower.State.Pool || Option != TestPower.State.Web)
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
                return HttpStatusCode.Unauthorized;
               
            }

        }       
       
    }
}
