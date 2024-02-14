using SnitcherPortal.ActivityRecords;
using SnitcherPortal.SupervisedComputers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SnitcherPortal.Engine;

public static class DashboardMapper
{
    public static DashboardDataDto Map(SupervisedComputer entity, List<ActivityRecord> activityRecords, List<string>? detectedProcessList)
    {
        var result = new DashboardDataDto
        {
            LastUpdate = detectedProcessList == null ? entity.LastModificationTime : DateTime.Now,
            ComputerName = entity.Name,
            Status = entity.Status,
            ActivityList = activityRecords.Select(ar => new DashboardActivityRecordDto()
            {
                StartTime = ar.StartTime,
                EndTime = ar.EndTime,
                Data = ar.Data
            }).OrderByDescending(ar => ar.StartTime).ToList()
        };

        if (detectedProcessList != null)
        {
            // Create list for all processes
            result.ProcessList = detectedProcessList.Select(kpn => new DashboardProcessDto()
            {
                ProcessName = kpn,
                IsImportatnt = false,
            }).ToList();

            // Mark important ones by using KnownProcesses, also with contains operator
            var importantProcesses = entity.KnownProcesses.Where(e => e.IsImportant).ToList();
            result.ProcessList.ForEach(e => { e.IsImportatnt = importantProcesses.Any(ip => e.ProcessName.Contains(ip.Name, StringComparison.CurrentCultureIgnoreCase)); });

            // Remove hidden processes
            var hiddenProcesses = entity.KnownProcesses.Where(kp => kp.IsHidden).Select(kp => kp.Name);
            result.ProcessList = result.ProcessList.Where(e => e.IsImportatnt || !hiddenProcesses.Contains(e.ProcessName)).ToList();

            // Order
            result.ProcessList = result.ProcessList.OrderBy(e => e.ProcessName).ToList();

            result.LastUpdate = DateTime.Now;
        }

        return result;
    }
}
