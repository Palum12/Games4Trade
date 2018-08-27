﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Games4Trade.Dtos;
using Games4Trade.Models;

namespace Games4Trade.Services
{
    public interface IUserService
    {
        Task<IList<UserDto>> Get();
        Task<OperationResult> CreateUser(UserRegisterDto newUser);
        Task<OperationResult> CheckIfEmailExists(string email);
    }
}