using Anet;
using Anet.Data;
using Sample.Web.Models.Dtos;
using Sample.Web.Models.Entities;

namespace Sample.Web.Services;

public class UserService : ServiceBase
{
    public UserService(Db db) : base(db)
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

    public async Task CreateUserAsync(UserRequestDto dto)
    {
        var newUser = new AnetUser { UserName = dto.UserName };

        using var tran = Db.BeginTransaction();

        await Db.InsertAsync(newUser);

        // Other business logic code.

        tran.Commit();
    }

    public async Task UpdateUserAsync(long userId, UserRequestDto dto)
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

    public async Task DeleteUserAsync(long id)
    {
        var rows = await Db.DeleteAsync("AnetUser", new { Id = id });
        if (rows == 0)
            throw new NotFoundException();
    }
}
