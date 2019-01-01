using System;

namespace Anet.Data
{
    public interface IEntity<TKey> : ITable
        where TKey : IEquatable<TKey>
    {
        TKey Id { get; }

        void SetId();
    }

    public interface IEntity : IEntity<long>
    {

    }
}
