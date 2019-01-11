using Anet.Data.Entity;

namespace Sample.WebApi.Models.Entities
{
    public class User : EntityBase
    {
        [Varchar(20)]
        public string UserName { get; set; }
    }
}
