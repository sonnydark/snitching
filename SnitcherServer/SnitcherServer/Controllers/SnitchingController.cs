using DeviceId;
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
        [Route("status")]
        public SnitchingDataDto GetStatus()
        {
            string? machineIdentifier = null;
            List<string>? processes = null;
            List<string>? logs = null;

            try
            {
                machineIdentifier = new DeviceIdBuilder().AddMachineName().ToString();
                logs = Services.AppDomain.Instance.Logs;
                processes = NastyStuffService.GetProcesses();
                Services.AppDomain.Instance.Logs = new List<string>();
            }
            catch (Exception ex)
            {
                logs = logs == null ? new List<string>() : null;
                logs!.Add(ex.ToString());
            }

            return new SnitchingDataDto()
            {
                MachineIdentifier = machineIdentifier,
                Logs = logs,
                Processes = processes,
            };
        }

        [HttpPut]
        [Route("kill")]
        public void KillProcesses(List<string> processNames)
        {
            try
            {
                NastyStuffService.KillProcess(processNames);
            }
            catch (Exception ex)
            {
                Services.AppDomain.Instance.Logs.Add(ex.ToString());
            }
        }
    }
}