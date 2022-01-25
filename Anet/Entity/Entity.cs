namespace Anet.Entity;

public abstract class Entity<TKey> : IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    public virtual TKey Id { get; set; }
}

public abstract class Entity : Entity<long>, IEntity
{
}
