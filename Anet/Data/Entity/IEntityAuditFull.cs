using System;

namespace Anet.Data.Entity
{
    public interface IEntityAuditFull<TKey> : IEntityAudit<TKey>
        where TKey : IEquatable<TKey>
    {
        TKey CreatedBy { get; set; }
        TKey UpdatedBy { get; set; }
    }

    public interface IEntityAuditFull : IEntityAuditFull<string>
    {
    }
}
