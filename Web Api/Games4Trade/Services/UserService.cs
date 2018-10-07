using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Games4Trade.Dtos;
using Games4Trade.Models;
using Games4Trade.Repositories;
using Microsoft.AspNetCore.Http;


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

        public async Task<OperationResult> DeleteObservedUser(ObservedUsersRelationshipDto pair)
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
                _unitOfWork.Users.DeleteObservedUser(pair.ObservingUserId, pair.ObservedUserId);
                var result = await _unitOfWork.CompleteASync();
                if (result > 0)
                {
                    return new OperationResult()
                    {
                        IsSuccessful = true
                    };
                }
                else
                {
                    return OtherServices.GetIncorrectDatabaseConnectionResult();
                }
            }
            return new OperationResult()
            {
                IsSuccessful = false,
                IsClientError = true,
                Message = "Pair of users has not been found",
                Payload = pair
            };
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

        public async Task<OperationResult> ChangeUserDescription(int userId, string description)
        {
            var user = await _unitOfWork.Users.GetASync(userId);
            if (user.Description != null && user.Description.Equals(description))
            {
                return new OperationResult
                {
                    IsSuccessful = true
                };
            }

            user.Description = description;
            var repoResult = await _unitOfWork.CompleteASync();
            if (repoResult > 0)
            {
                return new OperationResult
                {
                    IsSuccessful = true
                };
            }

            return OtherServices.GetIncorrectDatabaseConnectionResult();
        }

        public async Task<byte[]> GetUserPhoto(int userId)
        {
            var user = await _unitOfWork.Users.GetASync(userId);
            if (user?.PhotoId != null)
            {
                var photo = await _unitOfWork.Photos.GetASync(user.PhotoId.Value);
                var bytes = await File.ReadAllBytesAsync(photo.Path);
                return bytes;
            }

            return null;
        }

        public async Task<OperationResult> ChangeUserPhoto(int userId, IFormFile photo)
        {
            var fileId = Guid.NewGuid().ToString("N").ToUpper();
            // todo: create nested directory
            var directory = "user" + userId;
            Directory.CreateDirectory(directory);

            var path = Path.Combine(directory, fileId);
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await photo.CopyToAsync(fileStream);
            }

            var newPhoto = new Photo {DateCreated = DateTime.Now, Path = path};

            var user = await _unitOfWork.Users.GetASync(userId);
            if (user.PhotoId.HasValue)
            {
                var oldPhoto = await _unitOfWork.Photos.GetASync(user.PhotoId.Value);
                if (oldPhoto != null)
                {
                    File.Delete(oldPhoto.Path);
                }
            }

            await _unitOfWork.Photos.AddASync(newPhoto);
            user.PhotoId = newPhoto.Id;
            var repoResult = await _unitOfWork.CompleteASync();
            if (repoResult > 0)
            {
                return new OperationResult
                {
                    IsSuccessful = true
                };
            }

            return OtherServices.GetIncorrectDatabaseConnectionResult();
        }

        public async Task<OperationResult> ReplaceGenresForUser(int userId, IList<int> genresIds)
        {
            var genres = await _unitOfWork.Genres.GetAllASync();

            var areIdsInDatabase = genresIds.All(x => genres.Any(g => g.Id == x));
            if (!areIdsInDatabase)
            {
                return new OperationResult { IsSuccessful = false, IsClientError = true};
            }
            var newRelationships = genresIds
                .Select(x => new UserLikedGenre{UserId = userId, GenreId = x}).ToList();
            await _unitOfWork.Users.ReplaceGenresForUser(userId, newRelationships);
            var repoResult = await _unitOfWork.CompleteASync();
            if (repoResult > 0)
            {
                return new OperationResult
                {
                    IsSuccessful = true
                };
            }

            return OtherServices.GetIncorrectDatabaseConnectionResult();
        }

        public async Task<OperationResult> ReplaceSystemsForUser(int userId, IList<int> systemsIds)
        {
            var systems = await _unitOfWork.Systems.GetAllASync();

            var areIdsInDatabase = systemsIds.All(x => systems.Any(g => g.Id == x));
            if (!areIdsInDatabase)
            {
                return new OperationResult { IsSuccessful = false, IsClientError = true };
            }
            var newRelationships = systemsIds
                .Select(x => new UserOwnedSystem { UserId = userId, SystemId = x }).ToList();
            await _unitOfWork.Users.ReplaceSystemsForUser(userId, newRelationships);
            var repoResult = await _unitOfWork.CompleteASync();
            if (repoResult > 0)
            {
                return new OperationResult
                {
                    IsSuccessful = true
                };
            }

            return OtherServices.GetIncorrectDatabaseConnectionResult();
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