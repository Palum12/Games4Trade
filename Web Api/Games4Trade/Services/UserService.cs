using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Games4Trade.Dtos;
using Games4Trade.Models;
using Games4Trade.Repositories;


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

        public async Task<UserDto> GetUserById(int id)
        {
            var user = await _unitOfWork.Users.GetASync(id);
            return _mapper.Map<User, UserDto>(user);
        }

        public async Task<int?> GetUserIdByLogin(string login)
        {
            var user = await _unitOfWork.Users.GetUserByLogin(login);
            return user.Id;
        }

        public async Task<OperationResult> CheckIfEmailExists(string email)
        {
            var result = new OperationResult();
            var user = await _unitOfWork.Users.FindASync(u => u.Email.Equals(email));
            result.IsSuccessful = user.Any();
            if (!result.IsSuccessful)
            {
                result.Message = "This email adress is already taken";
            }
            return result;
        }

        public async Task<OperationResult> CreateUser(UserRegisterDto newUser)
        {
            var result = new OperationResult();

            var mappedUser = _mapper.Map<UserRegisterDto, User>(newUser);
            mappedUser.Salt = _loginService.GetSalt();
            mappedUser.Password = _loginService.ComputeHash(
                mappedUser.Salt, mappedUser.Password);

            await _unitOfWork.Users.AddASync(mappedUser);

            var repoResult = await _unitOfWork.CompleteASync();

            if (repoResult > 0)
            {
                result.IsSuccessful = true;
            }
            else
            {
                result.IsSuccessful = false;
                result.Message = "Something went wrong with db connection!";
            }
            return result;
        }
    }
}