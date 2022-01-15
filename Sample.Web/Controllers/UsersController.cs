using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sample.Web.Models.Dtos;
using Sample.Web.Services;

namespace Sample.Web.Controllers
{
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
        public Task<IEnumerable<UserResponseDto>> Get()
        {
            return _userService.GetAllAsync();
        }

        [HttpGet("{id}")]
        public Task<UserResponseDto> Get(long id)
        {
            return _userService.GetByIdAsync(id);
        }

        [HttpPost]
        public Task Post([FromBody] UserRequestDto dto)
        {
            return _userService.CreateUserAsync(dto);
        }

        [HttpPut("{id}")]
        public Task Put(long id, [FromBody] UserRequestDto dto)
        {
            return _userService.UpdateUserAsync(id, dto);
        }

        [HttpDelete("{id}")]
        public Task Delete(long id)
        {
            return _userService.DeleteUserAsync(id);
        }
    }
}
