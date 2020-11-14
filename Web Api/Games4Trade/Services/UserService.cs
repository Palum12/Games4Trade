using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Games4Trade.Dtos;
using Games4Trade.Models;
using Games4Trade.Interfaces.Repositories;
using Games4Trade.Interfaces.Services;
using Microsoft.AspNetCore.Http;


namespace Games4Trade.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoginService _loginService;
        private const int PageSize = 5;

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

        public async Task<UserProfileDto> GetUserProfile(int id, int? currentUser = null)
        {
            var user = await _unitOfWork.Users.GetASync(id);
            if (user == null)
            {
                return null;
            }
            var result = new UserProfileDto
            {
                Id = user.Id,
                Description = user.Description,
                Login = user.Login
            };
            result.LikedGenres =
                (await _unitOfWork.Genres.GetGenresForUser(user.Id))
                .Select(g => g.Value).ToList();

            var tempSystems = await _unitOfWork.Systems.GetSystemsForUser(user.Id);
            result.InterestingSystems =
                tempSystems.Select(s => s.Manufacturer + " " + s.Model).ToList();
            if (currentUser.HasValue)
            {
                var observedUsers = await _unitOfWork.Users.GetObservedUsersForUser(currentUser.Value);
                result.IsUserObserved = observedUsers.Any(u => u.Id == id);
            }
            return result;
        }

        public async Task<int?> GetUserIdByLogin(string login)
        {
            var user = await _unitOfWork.Users.GetUserByLogin(login);
            return user.Id;
        }

        public async Task<IList<ObservedUserDto>> GetObservedUsersForUser(int userId, int? page = null)
        {
            IList<User> users;
            if (page.HasValue)
            {
                users = await _unitOfWork.Users.GetObservedUsersForUser(userId, page, PageSize);
            }
            else
            {
                users = await _unitOfWork.Users.GetObservedUsersForUser(userId);
            }
            var resultList = new List<ObservedUserDto>();
            foreach (var user in users)
            {
                var tempUser = new ObservedUserDto();
                tempUser.Id = user.Id;
                tempUser.Login = user.Login;
                tempUser.Description = user.Description;

                tempUser.LikedGenres = 
                    (await _unitOfWork.Genres.GetGenresForUser(user.Id))
                    .Select(g => g.Value).ToList();

                var tempSystems = await _unitOfWork.Systems.GetSystemsForUser(user.Id);
                tempUser.InterestingSystems = 
                    tempSystems.Select(s => s.Manufacturer + " " + s.Model).ToList();

                resultList.Add(tempUser);

            }
            return resultList;
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

        public async Task<OperationResult> ChangeUserEmail(int userId, string email)
        {
            var user = await _unitOfWork.Users.GetASync(userId);
            if (user.Email.Equals(email))
            {
                return new OperationResult
                {
                    IsSuccessful = true
                };
            }

            user.Email = email;
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

        public async Task<OperationResult> ChangeUserPhone(int userId, string phone)
        {
            var user = await _unitOfWork.Users.GetASync(userId);
            if (user.PhoneNumber != null && user.PhoneNumber.Equals(phone))
            {
                return new OperationResult
                {
                    IsSuccessful = true
                };
            }

            user.PhoneNumber = phone;
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

            return await File.ReadAllBytesAsync(@"photos/defaultUserPhoto.png");
        }

        public async Task<OperationResult> ChangeUserPhoto(int userId, IFormFile photo)
        {
            var user = await _unitOfWork.Users.GetASync(userId);
            if (photo == null && user.PhotoId.HasValue)
            {
                var oldPhoto = await _unitOfWork.Photos.GetASync(user.PhotoId.Value);
                if (oldPhoto != null)
                {
                    File.Delete(oldPhoto.Path);
                }
                _unitOfWork.Photos.Remove(oldPhoto);
                user.PhotoId = null;
                var repoRes = await _unitOfWork.CompleteASync();
                if (repoRes > 0)
                {
                    return new OperationResult{IsSuccessful = true};
                }
                throw new DataException();
            }

            if (photo == null)
            {
                return new OperationResult { IsSuccessful = true };
            }

            var fileId = Guid.NewGuid().ToString("N").ToUpper();
            var directory = @"photos/user" + userId;
            Directory.CreateDirectory(directory);

            var path = Path.Combine(directory, fileId);
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await photo.CopyToAsync(fileStream);
            }
            var newPhoto = new Photo {DateCreated = DateTime.Now, Path = path};

            if (user.PhotoId.HasValue)
            {
                var oldPhoto = await _unitOfWork.Photos.GetASync(user.PhotoId.Value);
                if (oldPhoto != null)
                {
                    File.Delete(oldPhoto.Path);
                }
                _unitOfWork.Photos.Remove(oldPhoto);
                user.PhotoId = null;
                user.Photo = null;
                var repoRes = await _unitOfWork.CompleteASync();
                if (repoRes == 0)
                {
                    File.Delete(path);
                    throw new DataException();
                }
            }

            await _unitOfWork.Photos.AddASync(newPhoto);
            user.Photo = newPhoto;
            var repoResult = await _unitOfWork.CompleteASync();
            if (repoResult > 0)
            {
                return new OperationResult
                {
                    IsSuccessful = true
                };
            }
            File.Delete(path);
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
            return new OperationResult { IsSuccessful = true };
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
            return new OperationResult {IsSuccessful = true};
        }

        public async Task<OperationResult> CreateUser(UserRegisterDto newUser)
        {
            var result = new OperationResult();

            var mappedUser = _mapper.Map<UserRegisterDto, User>(newUser);
            mappedUser.Salt = _loginService.GetSalt();
            mappedUser.Password = _loginService.ComputeHash(
                mappedUser.Salt, "TempPass");
            var tempUsers = await _unitOfWork.Users.GetAllASync();
            if (!tempUsers.Any())
            {
                mappedUser.Role = "Admin";
            }

            await _unitOfWork.Users.AddASync(mappedUser);

            var repoResult = await _unitOfWork.CompleteASync();
            // todo: FixThis
            //if (repoResult > 0)
            //{
            //    result.IsSuccessful = true;
            //    var mailPassword = await _loginService.RecoverPassword(mappedUser.Email);
            //    if (mailPassword.IsSuccessful)
            //    {
            //        result.IsSuccessful = true;
            //    }
            //    else
            //    {
            //        result.IsSuccessful = false;
            //        result.Message = "Ups! Coś poszło nie tak podczas wysyłania wiadomości z hasłem!";
            //    }
            //}
            //else
            //{
            //    return OtherServices.GetIncorrectDatabaseConnectionResult();
            //}
            return result;
        }
    }
}