using System;

namespace Anet.Entity
{
    public interface IEntity<TKey> : ITable
    {
        TKey Id { get; set; }
    }

    public interface IEntity : IEntity<long>
    {
    }
}
