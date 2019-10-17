using System;

namespace Anet.Data
{
    public abstract class EntityAuditFull<TKey> : EntityAudit<TKey>, IEntityAuditFull<TKey>
        where TKey : IEquatable<TKey>
    {
        public virtual TKey CreatedBy { get; set; }
        public virtual TKey UpdatedBy { get; set; }
    }

    public abstract class EntityAuditFull : EntityAudit, IEntityAuditFull
    {
        public virtual long CreatedBy { get; set; }
        public virtual long UpdatedBy { get; set; }
    }
}
