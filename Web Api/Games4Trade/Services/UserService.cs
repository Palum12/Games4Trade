using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Games4Trade.Dtos;
using Games4Trade.Models;
using Games4Trade.Repositories;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Games4Trade.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoginService _loginService;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, ILoginService loginService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _loginService = loginService;
        }

        public async Task<IList<UserDto>> Get()
        {
            var users = await _unitOfWork.Users.GetAllASync();
            var mappedUsers = _mapper.Map<IEnumerable<User>, IEnumerable<UserDto>>(users);
            return mappedUsers.ToList();
        }

        public async Task<bool> CheckIfEmailExists(string email)
        {
            var user = await  _unitOfWork.Users.FindASync(u => u.Email.Equals(email));
            return user.Any();
        }

        public async Task<bool> CreateUser(UserRegisterDto newUser)
        {
            var mappedUser = _mapper.Map<UserRegisterDto, User>(newUser);
            mappedUser.Salt = _loginService.GetSalt();
            mappedUser.Password = _loginService.ComputeHash(
                mappedUser.Salt, mappedUser.Password);

            await _unitOfWork.Users.AddASync(mappedUser);
            var result = await _unitOfWork.CompleteASync();
            return result > 0;
        }
    }
}