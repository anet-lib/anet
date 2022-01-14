namespace Anet.Entity;

public interface IEntity<TKey>
{
    TKey Id { get; set; }
}

public interface IEntity : IEntity<long>
{
}
