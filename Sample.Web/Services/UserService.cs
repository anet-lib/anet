using Anet;
using Sample.Web.Models.Dtos;
using Sample.Web.Models.Entities;
using Sample.Web.Repositories;
using System.Threading.Tasks;

namespace Sample.Web.Services
{
    public class UserService
    {
        private readonly UserRepository userRepository;
        public UserService(UserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task CreateUserAsync(UserRequestDto dto)
        {
            var newUser = new AnetUser { UserName = dto.UserName };

            using (var tran = userRepository.BeginTransaction())
            {
                await userRepository.InsertAsync(newUser);

                // Other business logic code.

                tran.Commit();
            }
        }

        public async Task UpdateUserAsync(long userId, UserRequestDto dto)
        {
            var user = await userRepository.FindAsync(userId);
            if (user == null)
                throw new NotFoundException();

            using(var tran = userRepository.BeginTransaction())
            {
                await userRepository.UpdateAsync(
                    update: new { dto.UserName }, 
                    clause: new { Id = userId });

                tran.Commit();
            }
        }

        public async Task DeleteUserAsync(long id)
        {
            var rows = await userRepository.DeleteAsync(id);
            if (rows == 0)
                throw new NotFoundException();
        }
    }
}
