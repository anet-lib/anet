using System;

namespace Anet.Entity
{
    public interface IAuditEntity<TKey> : IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
    }

    public interface IAuditEntity : IAuditEntity<long>
    {
    }
}
