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

        public async Task<OperationResult> AddObservedUser(ObservedUsersRelationshipDto pair)
        {
            var observingUser = await GetUserById(pair.ObservingUserId);
            if (observingUser == null)
            {
                return new OperationResult()
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Message = "Observing user not found!"
                };
            }
            var observedUser = await GetUserById(pair.ObservedUserId);
            if (observedUser == null)
            {
                return new OperationResult()
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Message = "Observed user not found!"
                };
            }

            var currentList = await GetObservedUsersForUser(pair.ObservingUserId);
            var ids = currentList.Select(u => u.Id);
            if (ids.Contains(pair.ObservedUserId))
            {
                return new OperationResult()
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Payload = pair
                };
            }

            await _unitOfWork.Users.AddObsersvedUser(pair.ObservingUserId, pair.ObservedUserId);
            var result = await _unitOfWork.CompleteASync();
            if (result > 0)
            {
                return new OperationResult()
                {
                    IsSuccessful = true,
                    Payload = pair
                };
            }
            else
            {
                return OtherServices.GetIncorrectDatabaseConnectionResult();
            }
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

        public async Task<IList<UserDto>> GetObservedUsersForUser(int userId)
        {
            var users = await _unitOfWork.Users.GetObservedUsersForUser(userId);
            return _mapper.Map<IList<User>, IList<UserDto>>(users);
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