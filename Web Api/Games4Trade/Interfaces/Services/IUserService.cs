﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Models;
using Microsoft.AspNetCore.Http;

namespace Games4TradeAPI.Interfaces.Services
{
    public interface IUserService
    {
        Task<OperationResult> AddObservedUser(ObservedUsersRelationshipDto pair);
        Task<OperationResult> DeleteObservedUser(ObservedUsersRelationshipDto pair);
        Task<IList<UserDto>> Get();
        Task<UserDto> GetUserById(int id);
        Task<UserProfileDto> GetUserProfile(int id, int? currentUser = null);
        Task<int?> GetUserIdByLogin(string login);
        Task<IList<ObservedUserDto>> GetObservedUsersForUser(int userId, int? page = null);
        Task<OperationResult> CreateUser(UserRegisterDto newUser);
        Task<OperationResult> CheckIfEmailExists(string email);
        Task<OperationResult> ChangeUserDescription(int userId, string description);
        Task<OperationResult> ChangeUserEmail(int userId, string email);
        Task<OperationResult> ChangeUserPhone(int userId, string phone);
        Task<Byte[]> GetUserPhoto(int userId);
        Task<OperationResult> ChangeUserPhoto(int userId, IFormFile photo);
        Task<OperationResult> ReplaceGenresForUser(int userId, IList<int> genresIds);
        Task<OperationResult> ReplaceSystemsForUser(int userId, IList<int> systemsIds);
    }
}