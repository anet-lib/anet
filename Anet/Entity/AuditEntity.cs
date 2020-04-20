using System;

namespace Anet.Entity
{
    public abstract class AuditEntity<TKey> : EntityBase<TKey>, IAuditEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        public virtual DateTime CreatedAt { get; set; } = DateTime.Now;
        public virtual DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public abstract class AuditEntity : EntityBase, IAuditEntity
    {
        public virtual DateTime CreatedAt { get; set; } = DateTime.Now;
        public virtual DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
