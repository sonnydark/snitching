using SnitcherPortal.SupervisedComputers;
using System;
using System.Collections.Generic;

namespace SnitcherPortal;

public class DashboardDataDto
{
    public string ComputerName { get; set; } = "";
    public DateTime? LastUpdate { get; set; }
    public SupervisedComputerStatus Status { get; set; }
    public List<DashboardProcessDto> ProcessList { get; set; } = [];
    public List<DashboardActivityRecordDto> ActivityList { get; set; } = [];
}

public class DashboardProcessDto
{
    public string ProcessName { get; set; } = "";
    public bool IsImportatnt { get; set; }
}

public class DashboardActivityRecordDto
{
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Data { get; set; }
}
