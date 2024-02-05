using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace SnitcherPortal.SupervisedComputers
{
    public class SupervisedComputerManager : DomainService
    {
        protected ISupervisedComputerRepository _supervisedComputerRepository;

        public SupervisedComputerManager(ISupervisedComputerRepository supervisedComputerRepository)
        {
            _supervisedComputerRepository = supervisedComputerRepository;
        }

        public virtual async Task<SupervisedComputer> CreateAsync(
        string name, string identifier, bool isCalendarActive, string? ipAddress = null, DateTime? banUntil = null)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.Length(name, nameof(name), SupervisedComputerConsts.NameMaxLength, SupervisedComputerConsts.NameMinLength);
            Check.NotNullOrWhiteSpace(identifier, nameof(identifier));
            Check.Length(identifier, nameof(identifier), SupervisedComputerConsts.IdentifierMaxLength, SupervisedComputerConsts.IdentifierMinLength);
            Check.Length(ipAddress, nameof(ipAddress), SupervisedComputerConsts.IpAddressMaxLength);

            var supervisedComputer = new SupervisedComputer(
             GuidGenerator.Create(),
             name, identifier, isCalendarActive, ipAddress, banUntil
             );

            return await _supervisedComputerRepository.InsertAsync(supervisedComputer);
        }

        public virtual async Task<SupervisedComputer> UpdateAsync(
            Guid id,
            string name, string identifier, bool isCalendarActive, string? ipAddress = null, DateTime? banUntil = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.Length(name, nameof(name), SupervisedComputerConsts.NameMaxLength, SupervisedComputerConsts.NameMinLength);
            Check.NotNullOrWhiteSpace(identifier, nameof(identifier));
            Check.Length(identifier, nameof(identifier), SupervisedComputerConsts.IdentifierMaxLength, SupervisedComputerConsts.IdentifierMinLength);
            Check.Length(ipAddress, nameof(ipAddress), SupervisedComputerConsts.IpAddressMaxLength);

            var supervisedComputer = await _supervisedComputerRepository.GetAsync(id);

            supervisedComputer.Name = name;
            supervisedComputer.Identifier = identifier;
            supervisedComputer.IsCalendarActive = isCalendarActive;
            supervisedComputer.IpAddress = ipAddress;
            supervisedComputer.BanUntil = banUntil;

            supervisedComputer.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _supervisedComputerRepository.UpdateAsync(supervisedComputer);
        }

    }
}