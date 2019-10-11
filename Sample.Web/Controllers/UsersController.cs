using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sample.Web.Models.Dtos;
using Sample.Web.Repositories;
using Sample.Web.Services;

namespace Sample.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserRepository userRepository;
        private readonly UserService userService;
        public UsersController(
            UserRepository userRepository,
            UserService userService)
        {
            this.userRepository = userRepository;
            this.userService = userService;
        }

        [HttpGet]
        public Task<IEnumerable<UserResponseDto>> Get()
        {
            return userRepository.GetAllAsync();
        }

        [HttpGet("{id}")]
        public Task<UserResponseDto> Get(long id)
        {
            return userRepository.GetByIdAsync(id);
        }

        [HttpPost]
        public Task Post([FromBody] UserRequestDto dto)
        {
            return userService.CreateUserAsync(dto);
        }

        [HttpPut("{id}")]
        public Task Put(long id, [FromBody] UserRequestDto dto)
        {
            return userService.UpdateUserAsync(id, dto);
        }

        [HttpDelete("{id}")]
        public Task Delete(long id)
        {
            return userService.DeleteUserAsync(id);
        }
    }
}
