using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Games4Trade.Dtos;
using Games4Trade.Repositories;

namespace Games4Trade.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IList<UserDto>> Get()
        {

            var users = await _unitOfWork.Users.GetAllASync();
            return new List<UserDto>(){new UserDto(){Email = "a@a.pl", Id = 1, Login = "Benia"}};
        }
    }
}