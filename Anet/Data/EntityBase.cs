using System;

namespace Anet.Data
{
    public abstract class EntityBase<TKey> : IEntity<TKey>
        where TKey: IEquatable<TKey>
    {
        public virtual TKey Id { get; set; }
        public virtual DateTime CreatedAt { get; set; }

        public abstract void SetNewId();
    }

    public abstract class EntityBase : EntityBase<long>, IEntity
    {
        // [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override long Id { get; set; }

        public override void SetNewId()
        {
            Id = IdGen.NewId();
        }
    }
}
