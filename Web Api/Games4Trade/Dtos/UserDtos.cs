﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Games4Trade.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
    }

    public class UserLoginDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class UserRegisterDto
    {
        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserRecoverDto
    {
        public string RecoveryString { get; set; }
        public string NewPassword { get; set; }
    }
}
