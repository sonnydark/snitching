using Microsoft.AspNetCore.Mvc;
using SnitcherServer.Interface;
using SnitcherServer.Services;

namespace SnitcherServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SnitchingController : ControllerBase
    {
        public SnitchingController()
        {
        }

        [HttpGet]
        public SnitchingDataDto Get()
        {
            return new SnitchingDataDto()
            {
                Processes = NastyStuffService.GetProcesses(),
                Config = RuntimeConfigService.GetConfigJson()
            };
        }

        [HttpPut]
        public void Put(string processName)
        {
            NastyStuffService.KillProcess(processName);
        }

        [HttpPut]
        [Route("configure")]
        public void Configure(RuntimeConfigDto config)
        {
            RuntimeConfigService.SetConfig(config);
        }
    }
}