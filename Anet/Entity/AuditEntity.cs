namespace Anet.Entity;

public abstract class AuditEntity<TKey> : Entity<TKey>, IAuditEntity<TKey>
    where TKey : IEquatable<TKey>
{
    [Datetime]
    public virtual DateTime CreatedAt { get; set; } = DateTime.Now;
    [Datetime]
    public virtual DateTime UpdatedAt { get; set; } = DateTime.Now;
}

public abstract class AuditEntity : Entity, IAuditEntity
{
    [Datetime]
    public virtual DateTime CreatedAt { get; set; } = DateTime.Now;
    [Datetime]
    public virtual DateTime UpdatedAt { get; set; } = DateTime.Now;
}
