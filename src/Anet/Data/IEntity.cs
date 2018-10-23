using System;

namespace Anet.Data
{
    public interface IEntity<TKey> where TKey : IEquatable<TKey>
    {
        TKey Id { get; set; }
        DateTime CreatedAt { get; set; }

        void SetNewId();
    }

    public interface IEntity : IEntity<long>
    {
    }
}
