using Anet.Entity;

namespace Sample.Web.Models.Entities
{
    public class AnetUser : IEntity
    {
        public long Id { get; set; }
        public string UserName { get; set; }
    }
}
