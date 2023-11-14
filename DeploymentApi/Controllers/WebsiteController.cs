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
        public async Task<string> CreateWebsite(string name, int port, string ipAddr = "*")
        {

            try
            {
                string pool = await powshell.GetState(name, TestPower.State.Pool);
                string web = await powshell.GetState(name, TestPower.State.Web);
                if (pool.Contains("Started") || pool.Contains("Stopped") && !web.Contains("Started") && !web.Contains("Stopped")) // check if logic is correct
                {
                    await powshell.ExecuteCommand(TestPower.CreateWebCommand(name, port, ipAddr));
                    return @$"Website: {name} Created.";
                }
                else if (pool.Contains("null") && !web.Contains("Started") && !web.Contains("Stopped")) // check if logic is correct
                {
                    await powshell.ExecuteCommand(TestPower.CreatePoolCommand(name));
                    await powshell.ExecuteCommand(TestPower.CreateWebCommand(name, port, ipAddr));
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
        [Route("/Website/Start")]
        public async Task<string> StartWebsite(string name)
        {
            try
            {
                string web = await powshell.GetState(name, TestPower.State.Web);
                if (web.Contains("Started"))
                {
                    await powshell.ExecuteCommand(TestPower.StartWebCommand(name));
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
        [Route("/Website/Stop/")]
        public async Task<string> StopWebsite(string name)
        {
            try
            {
                string web = await powshell.GetState(name, TestPower.State.Web);
                if (web.Contains("Stopped"))
                {
                    await powshell.ExecuteCommand(TestPower.StopWebCommand(name));
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
        [HttpDelete]
        [Route("/Website/Delete")]
        public async Task<string> DeleteWeb(string name, bool deletePool = false)
        {
            try
            {
                string Web = await powshell.GetState(name, TestPower.State.Web);
                string pool = await powshell.GetState(name, TestPower.State.Pool);
                if (Web.Contains("Stopped") && pool.Contains("Stopped") && deletePool)
                {
                    await powshell.ExecuteCommand(TestPower.DeletePoolCommand(name));
                    return @$"ApplicationPool: {name} Has been Deleted.";

                }
                else if (Web.Contains("Stopped") && !deletePool)
                {
                    await powshell.ExecuteCommand(TestPower.DeleteWebCommand(name));
                    return @$"Website: {name} Has been Deleted. & ApplicationPool: {name} Preserved";
                }
                else
                    return @$"Website: {name} couldnt be Deleted, Reason: Already deleted or cant be deleted. CheckLogs";

            }
            catch (Exception)
            {
                return "error i dunno";

            }


        }
    }
}
