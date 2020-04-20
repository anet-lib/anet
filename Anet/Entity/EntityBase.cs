using System;

namespace Anet.Entity
{
    public abstract class EntityBase<TKey> : IEntity<TKey>
        where TKey: IEquatable<TKey>
    {
        public virtual TKey Id { get; set; }
    }

    public abstract class EntityBase : EntityBase<long>, IEntity
    {
    }
}
