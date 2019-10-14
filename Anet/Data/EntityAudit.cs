using System;

namespace Anet.Data
{
    public abstract class EntityAudit<TKey> : Entity<TKey>, IEntityAudit<TKey>
        where TKey : IEquatable<TKey>
    {
        public virtual DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public abstract class EntityAudit : EntityBase, IEntityAudit
    {
        public virtual DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
