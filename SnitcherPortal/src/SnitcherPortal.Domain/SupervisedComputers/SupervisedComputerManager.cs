using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;
using System.Text.Json;
using SnitcherPortal.Calendars;

namespace SnitcherPortal.SupervisedComputers
{
    public class SupervisedComputerManager : DomainService
    {
        protected ISupervisedComputerRepository _supervisedComputerRepository;
        protected CalendarManager _calendarManager;

        public SupervisedComputerManager(ISupervisedComputerRepository supervisedComputerRepository,
            CalendarManager calendarManager)
        {
            _supervisedComputerRepository = supervisedComputerRepository;
            _calendarManager = calendarManager;
        }

        public virtual async Task<SupervisedComputer> CreateAsync(
        string name, string identifier, bool isCalendarActive, string? connectionId = null, DateTime? banUntil = null)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.Length(name, nameof(name), SupervisedComputerConsts.NameMaxLength, SupervisedComputerConsts.NameMinLength);
            Check.NotNullOrWhiteSpace(identifier, nameof(identifier));
            Check.Length(identifier, nameof(identifier), SupervisedComputerConsts.IdentifierMaxLength, SupervisedComputerConsts.IdentifierMinLength);
            Check.Length(connectionId, nameof(connectionId), SupervisedComputerConsts.ConnectionIdMaxLength);

            var supervisedComputer = new SupervisedComputer(GuidGenerator.Create(), name, identifier, isCalendarActive, connectionId, banUntil)
            {
                Status = SupervisedComputerStatus.OFFLINE
            };

            JsonSerializerOptions options = new() { WriteIndented = false };
            var calendarDefaultsWorkingDays = JsonSerializer.Serialize(new CalendarSettingsJson()
            {
                Quota = 5,
                Hours =
                [
                    new()
                    {
                        Start = new TimeSpan(13, 0, 0),
                        End = new TimeSpan(20, 0, 0)
                    }
                ]
            }, options);

            var calendarDefaultsWeekend = JsonSerializer.Serialize(new CalendarSettingsJson()
            {
                Quota = 8,
                Hours =
                [
                    new()
                    {
                        Start = new TimeSpan(10, 0, 0),
                        End = new TimeSpan(20, 0, 0)
                    }
                ]
            }, options);

            await _calendarManager.CreateAsync(supervisedComputer.Id, (int)DayOfWeek.Monday, calendarDefaultsWorkingDays);
            await _calendarManager.CreateAsync(supervisedComputer.Id, (int)DayOfWeek.Tuesday, calendarDefaultsWorkingDays);
            await _calendarManager.CreateAsync(supervisedComputer.Id, (int)DayOfWeek.Wednesday, calendarDefaultsWorkingDays);
            await _calendarManager.CreateAsync(supervisedComputer.Id, (int)DayOfWeek.Thursday, calendarDefaultsWorkingDays);
            await _calendarManager.CreateAsync(supervisedComputer.Id, (int)DayOfWeek.Friday, calendarDefaultsWorkingDays);
            await _calendarManager.CreateAsync(supervisedComputer.Id, (int)DayOfWeek.Saturday, calendarDefaultsWeekend);
            await _calendarManager.CreateAsync(supervisedComputer.Id, (int)DayOfWeek.Sunday, calendarDefaultsWeekend);

            return await _supervisedComputerRepository.InsertAsync(supervisedComputer);
        }

        public virtual async Task<SupervisedComputer> UpdateAsync(
            Guid id,
            string name, string identifier, bool isCalendarActive, SupervisedComputerStatus status, string? connectionId = null, DateTime? banUntil = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.Length(name, nameof(name), SupervisedComputerConsts.NameMaxLength, SupervisedComputerConsts.NameMinLength);
            Check.NotNullOrWhiteSpace(identifier, nameof(identifier));
            Check.Length(identifier, nameof(identifier), SupervisedComputerConsts.IdentifierMaxLength, SupervisedComputerConsts.IdentifierMinLength);
            Check.Length(connectionId, nameof(connectionId), SupervisedComputerConsts.ConnectionIdMaxLength);

            var supervisedComputer = await _supervisedComputerRepository.GetAsync(id);

            supervisedComputer.Name = name;
            supervisedComputer.Identifier = identifier;
            supervisedComputer.IsCalendarActive = isCalendarActive;
            supervisedComputer.ConnectionId = connectionId;
            supervisedComputer.Status = status;
            supervisedComputer.BanUntil = banUntil;

            supervisedComputer.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _supervisedComputerRepository.UpdateAsync(supervisedComputer);
        }

    }
}