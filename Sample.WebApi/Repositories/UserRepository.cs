using Anet.Data;
using Sample.WebApi.Models.Dtos;
using Sample.WebApi.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.WebApi.Repositories
{
    public class UserRepository : RepositoryBase<User>
    {
        public UserRepository(Database db) : base(db)
        {
        }

        public Task<IEnumerable<UserResponseDto>> GetAllAsync()
        {
            var sql = "SELECT * FROM User;";
            return Db.QueryAsync<UserResponseDto>(sql);
        }

        public Task<UserResponseDto> GetByIdAsync(long id)
        {
            var sql = Sql.Select("User", new { Id = id });
            return Db.QueryFirstOrDefaultAsync<UserResponseDto>(sql);
        }
    }
}
