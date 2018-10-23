using System;

namespace Anet.Data
{
    public class EntityAudit : Entity, IEntityAudit
    {
        public long CreatedBy { get; set; }
        public long UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
