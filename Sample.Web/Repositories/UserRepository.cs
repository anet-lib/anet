﻿using Anet.Data;
using Dapper;
using Sample.Web.Models.Dtos;
using Sample.Web.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Web.Repositories
{
    public class UserRepository : Repository<AnetUser>
    {
        public UserRepository(Db db) : base(db)
        {
        }

        public Task<IEnumerable<UserResponseDto>> GetAllAsync()
        {
            var sql = "SELECT * FROM AnetUser;";
            return Db.Connection.QueryAsync<UserResponseDto>(sql);
        }

        public Task<UserResponseDto> GetByIdAsync(long id)
        {
            var param = new { Id = id };
            var sql = Sql.Select("AnetUser", param);
            return Db.Connection.QueryFirstOrDefaultAsync<UserResponseDto>(sql, param);
        }
    }
}
