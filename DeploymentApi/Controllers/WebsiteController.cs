using ClassLibrary1;
using Microsoft.AspNetCore.Mvc;

namespace DeploymentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class WebsiteController : Controller
    {
        public TestPower powshell = new();

        [HttpPost]
        [Route("/Website/Create")]
        public async Task<HttpStatusCode> CreateWebsite(string name, int port, string ipAddr = "*")
        {

            try
            {
                string pool = await powshell.GetState(name, TestPower.State.Pool);
                string web = await powshell.GetState(name, TestPower.State.Web);
                if (pool.Contains("Started") || pool.Contains("Stopped") && !web.Contains("Started") && !web.Contains("Stopped")) // check if logic is correct
                {
                    await powshell.ExecuteCommand(TestPower.CreateWebCommand(name, port, ipAddr));
                    return HttpStatusCode.OK;
                }
                else if (pool.Contains("null") && !web.Contains("Started") && !web.Contains("Stopped")) // check if logic is correct
                {
                    await powshell.ExecuteCommand(TestPower.CreatePoolCommand(name));
                    await powshell.ExecuteCommand(TestPower.CreateWebCommand(name, port, ipAddr));
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
                string web = await powshell.GetState(name, TestPower.State.Web);
                if (web.Contains("Started"))
                {
                    await powshell.ExecuteCommand(TestPower.StartWebCommand(name));
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
                string web = await powshell.GetState(name, TestPower.State.Web);
                if (web.Contains("Stopped"))
                {
                    await powshell.ExecuteCommand(TestPower.StopWebCommand(name));
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
                string Web = await powshell.GetState(name, TestPower.State.Web);
                string pool = await powshell.GetState(name, TestPower.State.Pool);
                if (Web.Contains("Stopped") && pool.Contains("Stopped") && deletePool)
                {
                    await powshell.ExecuteCommand(TestPower.DeletePoolCommand(name));
                    return HttpStatusCode.OK;

                }
                else if (Web.Contains("Stopped") && !deletePool)
                {
                    await powshell.ExecuteCommand(TestPower.DeleteWebCommand(name));
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
