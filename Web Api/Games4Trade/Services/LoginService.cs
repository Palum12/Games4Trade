﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Games4Trade.Dtos;
using Games4Trade.Models;
using Games4Trade.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace Games4Trade.Services
{
    public class LoginService : ILoginService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LoginService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Method loging in user and generating token.
        /// </summary>
        /// <param name="user"> User dto with data to login</param>
        /// <returns>Token or message if login has failed.</returns>
        public async Task<OperationResult> LoginUser(UserLoginDto user)
        {
            var userInDb = await _unitOfWork.Users.GetUserByLogin(user.Login);
            var result = new OperationResult();

            if (userInDb == null)
            {
                result.IsSuccessful = false;
                result.Message = "User does not exist in database";
                return result;
            }
            if (!string.IsNullOrEmpty(userInDb.RecoveryAddress))
            {
                result.IsSuccessful = false;
                result.Message = "Password recovery procedure was started on this account";
                return result;
            }

            if (IsPasswordValid(user.Password, userInDb))
            {
                var token = GenerateToken(userInDb);
                result.IsSuccessful = true;
                result.Payload = token;
                return result;
            }

            result.IsSuccessful = false;
            result.Message = "Passwords do not match!";
            return result;

        }

        public async Task<OperationResult> CheckIfLoginIsTaken(string login)
        {
            var user = await _unitOfWork.Users.GetUserByLogin(login);
            var result = new OperationResult();
            result.IsSuccessful = user != null;
            return result;
        }

        /// <summary>
        /// Compute hash for given password
        /// </summary>
        /// <param name="salt">Salt which will be added to password</param>
        /// <param name="password">Password to hash</param>
        /// <returns>Hashed password with added salt</returns>
        public string ComputeHash(string salt, string password)
        {
            var saltBytes = Convert.FromBase64String(salt);
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 1000))
                return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));
        }

        /// <summary>
        /// Generates Salt
        /// </summary>
        /// <returns>32 character salt</returns>
        public string GetSalt()
        {
            byte[] bytes = new byte[16];
            using (var keyGenerator = RandomNumberGenerator.Create())
            {
                keyGenerator.GetBytes(bytes);
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        private string GenerateToken(User user)
        {
            var claims = new []
            {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString()),
            };

            var token = new JwtSecurityToken(
                new JwtHeader(new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes("7uUzzYky7Lxb4pkGLRzU77dxpazhWEr4")),
                    SecurityAlgorithms.HmacSha256)),
                new JwtPayload(claims));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool IsPasswordValid(string password, User user)
        {
            return !string.IsNullOrEmpty(password) &&
                   user.Password.Equals(ComputeHash(user.Salt, password));
        }
    }
}
