using System.Collections.Generic;

namespace SnitcherPortal.Engine;

public class SnitchingDataDto
{
    public string? MachineIdentifier { get; set; }
    public List<string>? Logs { get; set; }

    public List<string>? Processes { get; set; }
}
