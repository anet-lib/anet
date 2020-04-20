using System;

namespace Anet.Entity
{
    public interface IFullAuditEntity<TKey> : IAuditEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        TKey CreatedBy { get; set; }
        TKey UpdatedBy { get; set; }
    }

    public interface IFullAuditEntity : IFullAuditEntity<long>
    {
    }
}
