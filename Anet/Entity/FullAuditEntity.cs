using System;

namespace Anet.Entity
{
    public abstract class FullAuditEntity<TKey> : AuditEntity<TKey>, IFullAuditEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        public virtual TKey CreatedBy { get; set; }
        public virtual TKey UpdatedBy { get; set; }
    }

    public abstract class FullAuditEntity : AuditEntity, IFullAuditEntity
    {
        public virtual long CreatedBy { get; set; }
        public virtual long UpdatedBy { get; set; }
    }
}
