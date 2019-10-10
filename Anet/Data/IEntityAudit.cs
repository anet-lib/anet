using System;

namespace Anet.Data
{
    public interface IEntityAudit<TKey> : IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        DateTime UpdatedAt { get; set; }
    }

    public interface IEntityAudit : IEntityAudit<long>
    {
    }
}
