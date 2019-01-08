using System;

namespace Anet.Data.Entity
{
    public abstract class EntityAudit<TKey> : EntityBase<TKey>, IEntityAudit<TKey>
        where TKey : IEquatable<TKey>
    {
        public virtual DateTime CreatedAt { get; set; } = DateTime.Now;
        public virtual DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public abstract class EntityAudit : EntityAudit<long>, IEntityAudit
    {
        public void SetId()
        {
            Id = IdGenerator.NewId();
        }
    }
}
