using Anet.Models;
using Microsoft.AspNetCore.Mvc;
using Sample.Web.Models.Dtos;
using Sample.Web.Services;

namespace Sample.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public Task<PagedResult<UserDto>> Get(int page = 1, int size = 10, string keyword = null)
    {
        return _userService.GetAsync(page, size, keyword);
    }

    [HttpGet("{id}")]
    public Task<UserDto> Get(long id)
    {
        return _userService.GetByIdAsync(id);
    }

    [HttpPost]
    public Task Post([FromBody] UserEditDto dto)
    {
        return _userService.CreateAsync(dto);
    }

    [HttpPut("{id}")]
    public Task Put(long id, [FromBody] UserEditDto dto)
    {
        return _userService.UpdateAsync(id, dto);
    }

    [HttpDelete("{id}")]
    public Task Delete(long id)
    {
        return _userService.DeleteAsync(id);
    }
}

