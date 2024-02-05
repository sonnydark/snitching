using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace SnitcherPortal.KnownProcesses
{
    public class KnownProcessManager : DomainService
    {
        protected IKnownProcessRepository _knownProcessRepository;

        public KnownProcessManager(IKnownProcessRepository knownProcessRepository)
        {
            _knownProcessRepository = knownProcessRepository;
        }

        public virtual async Task<KnownProcess> CreateAsync(
        Guid supervisedComputerId, string name, bool isHidden, bool isImportant)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.Length(name, nameof(name), KnownProcessConsts.NameMaxLength, KnownProcessConsts.NameMinLength);

            var knownProcess = new KnownProcess(
             GuidGenerator.Create(),
             supervisedComputerId, name, isHidden, isImportant
             );

            return await _knownProcessRepository.InsertAsync(knownProcess);
        }

        public virtual async Task<KnownProcess> UpdateAsync(
            Guid id,
            Guid supervisedComputerId, string name, bool isHidden, bool isImportant
        )
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.Length(name, nameof(name), KnownProcessConsts.NameMaxLength, KnownProcessConsts.NameMinLength);

            var knownProcess = await _knownProcessRepository.GetAsync(id);

            knownProcess.SupervisedComputerId = supervisedComputerId;
            knownProcess.Name = name;
            knownProcess.IsHidden = isHidden;
            knownProcess.IsImportant = isImportant;

            return await _knownProcessRepository.UpdateAsync(knownProcess);
        }

    }
}