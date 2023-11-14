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
        public TestPower powshell = new();

        [HttpPost]
        [Route("/ApplicationPool/Create")]
        public async Task<string> CreatePool(string name)
        {
            try
            {
                string pool = await powshell.GetState(name, TestPower.State.Pool);
                if (!pool.Contains("Started") && !pool.Contains("Stopped"))
                {
                    await powshell.ExecuteCommand(TestPower.CreatePoolCommand(name));
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
        [Route("/ApplicationPool/Start")]
        public async Task<string> StartPool(string name)
        {
            try
            {
                string pool = await powshell.GetState(name, TestPower.State.Pool);
                if (pool.Contains("Stopped"))
                {
                    await powshell.ExecuteCommand(TestPower.StartPoolCommand(name));
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
        [Route("/ApplicationPool/Stop")]
        public async Task<string> StopPool(string name)
        {
            try
            {
                string pool = await powshell.GetState(name, TestPower.State.Pool);
                if (pool.Contains("Started"))
                {
                    await powshell.ExecuteCommand(TestPower.StopPoolCommand(name));
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

        [HttpPut]
        [Route("/WebBinding/Update")]
        public async Task<HttpStatusCode> UpdateWebBinding(string name, int port, string IpAdd = "*")
        {
            try
            {
                string State = await powshell.GetState(name, TestPower.State.Web);
                if (!State.Contains("null"))
                {
                    await powshell.ExecuteCommand(TestPower.SetWebbindingCommand(name, port, IpAdd));
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
                string pool = await powshell.GetState(name, TestPower.State.Pool);
                if (pool.Contains("Stopped"))
                {
                    await powshell.ExecuteCommand(TestPower.DeletePoolCommand(name));
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