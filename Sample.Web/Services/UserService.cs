using Anet;
using Anet.Data;
using Anet.Models;
using Sample.Web.Models.Dtos;
using Sample.Web.Models.Entities;

namespace Sample.Web.Services;

public class UserService : ServiceBase
{
    public UserService(Db db) : base(db)
    {
    }

    public async Task<PagedResult<UserDto>> GetAsync(int page, int size, string keyword = null)
    {
        var param = new SqlParams();
        var sql = Sql.Select().From("AnetUser").Where();

        if (!string.IsNullOrEmpty(keyword))
        {
            sql.AndLike("Name");
            param.Add("Name", $"'%{keyword}%'");
        }

        sql.OrderBy("Id").Page(page, size);

        var result = new PagedResult<UserDto>(page, size);
        result.Items = await Db.QueryAsync<UserDto>(sql, param);
        result.Total = await Db.QuerySingleAsync<int>(sql.Count(), param);

        return result;
    }

    public Task<UserDto> GetByIdAsync(long id)
    {
        var param = new { Id = id };
        var sql = Sql.Select("AnetUser", param);
        return Db.QueryFirstOrDefaultAsync<UserDto>(sql, param);
    }

    public async Task CreateAsync(UserEditDto dto)
    {
        var newUser = new AnetUser { UserName = dto.UserName };

        using var tran = Db.BeginTransaction();

        await Db.InsertAsync(newUser);

        // Other business logic code.

        tran.Commit();
    }

    public async Task UpdateAsync(long userId, UserEditDto dto)
    {
        var user = await Db.FindAsync<AnetUser>(new { Id = userId });
        if (user == null)
            throw new NotFoundException();

        using var tran = Db.BeginTransaction();

        user.UserName = dto.UserName;

        await Db.UpdateAsync(user);

        // Other business logic code.

        tran.Commit();
    }

    public async Task DeleteAsync(long id)
    {
        var rows = await Db.DeleteAsync("AnetUser", new { Id = id });
        if (rows == 0)
            throw new NotFoundException();
    }
}
