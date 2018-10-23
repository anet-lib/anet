using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Anet.Data
{
    public class Entity : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual void SetNewId()
        {
            Id = IdGenerator.NewId();
        }
    }
}
