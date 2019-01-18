using System;

namespace Anet.Data.Entity
{
    public interface IEntity<TKey> : IDbTable
        where TKey : IEquatable<TKey>
    {
        TKey Id { get; set; }
        void SetId();
    }

    public interface IEntity : IEntity<long>
    {
    }
}
