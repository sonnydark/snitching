namespace SnitcherServer.Interface;

public class SnitchingDataDto
{
    public List<RunningProcessDto> Processes { get; set; }
    public RuntimeConfigDto Config { get; set; }
}

public class RunningProcessDto
{
    public string ProcessName { get; set; }
    public DateTime? StartTime { get; set; }
}

public class RuntimeConfigDto
{
    public string AutokillInterval { get; set; }
    public string AutokillProcesses { get; set; }
}