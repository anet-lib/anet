using System;

namespace Anet.Data
{
    public interface IEntityAudit<TKey, TUid> : IEntity<TKey>
        where TKey : IEquatable<TKey>
        where TUid : IEquatable<TUid>
    {
        TUid CreatedBy { get; set; }
        TUid UpdatedBy { get; set; }
        DateTime UpdatedAt { get; set; }
    }

    public interface IEntityAudit<TKey> : IEntityAudit<TKey, long>
        where TKey : IEquatable<TKey>
    {
    }

    public interface IEntityAudit : IEntityAudit<long, long>
    {
    }
}
