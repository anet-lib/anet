using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Anet.Data.Entity
{
    public abstract class EntityBase<TKey> : IEntity<TKey>
        where TKey: IEquatable<TKey>
    {
        public virtual TKey Id { get; set; }
    }

    public abstract class EntityBase : EntityBase<long>, IEntity
    {
        public EntityBase()
        {
            SetId();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override long Id { get; set; }

        public void SetId()
        {
            Id = IdGenerator.NewId();
        }
    }
}
