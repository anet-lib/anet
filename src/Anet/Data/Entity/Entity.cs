using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Anet.Data
{
    public abstract class Entity<TKey> : IEntity<TKey>
        where TKey: IEquatable<TKey>
    {
        public virtual TKey Id { get; set; }

        public abstract void SetId();
    }

    public abstract class Entity : Entity<long>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override long Id { get; set; }

        public override void SetId()
        {
            Id = IdGenerator.NewId();
        }
    }
}
