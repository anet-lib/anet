using Anet.Data.Entity;

namespace Sample.WebApi.Models.Entities
{
    public class AnetUser : EntityBase
    {
        [Varchar(50)]
        public string UserName { get; set; }
    }
}
