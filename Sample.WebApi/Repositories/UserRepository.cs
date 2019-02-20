using Anet.Data;
using Sample.WebApi.Models.Dtos;
using Sample.WebApi.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.WebApi.Repositories
{
    public class UserRepository : RepositoryBase<AnetUser>
    {
        public UserRepository(Database db) : base(db)
        {
        }

        public Task<IEnumerable<UserResponseDto>> GetAllAsync()
        {
            var sql = "SELECT * FROM AnetUser;";
            return Db.QueryAsync<UserResponseDto>(sql);
        }

        public Task<UserResponseDto> GetByIdAsync(long id)
        {
            var param = new { Id = id };
            var sql = Sql.Select("AnetUser", param);
            return Db.QueryFirstOrDefaultAsync<UserResponseDto>(sql, param);
        }
    }
}
