namespace SnitcherServer.Interface;

public class SnitchingDataDto
{
    public string? MachineIdentifier { get; set; }
    public List<string>? Logs { get; set; }

    public List<string>? Processes { get; set; }
}


public class CommandDto
{
    public string Test { get; set; }
}