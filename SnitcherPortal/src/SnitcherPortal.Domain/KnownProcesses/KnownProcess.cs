using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace SnitcherPortal.KnownProcesses
{
    public class KnownProcess : Entity<Guid>
    {
        public virtual Guid SupervisedComputerId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        public virtual bool IsHidden { get; set; }

        public virtual bool IsImportant { get; set; }

        protected KnownProcess()
        {

        }

        public KnownProcess(Guid id, Guid supervisedComputerId, string name, bool isHidden, bool isImportant)
        {

            Id = id;
            Check.NotNull(name, nameof(name));
            Check.Length(name, nameof(name), KnownProcessConsts.NameMaxLength, KnownProcessConsts.NameMinLength);
            SupervisedComputerId = supervisedComputerId;
            Name = name;
            IsHidden = isHidden;
            IsImportant = isImportant;
        }

    }
}