using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Games4Trade.Dtos;

namespace Games4Trade.Validators
{
    public class UserRegisterDtoValidator : AbstractValidator<UserRegisterDto>
    {
        public UserRegisterDtoValidator()
        {
            RuleFor(u => u.Email).EmailAddress().MinimumLength(5).MaximumLength(128).NotEmpty();
            RuleFor(u => u.Login).MinimumLength(4).MaximumLength(32).NotEmpty();
            RuleFor(u => u.Password).MinimumLength(4).MaximumLength(64).NotEmpty();
        }
        
    }
    public class  UserRecoverDtoValidator: AbstractValidator<UserRecoverDto>
    {
        public UserRecoverDtoValidator()
        {
            RuleFor(u => u.NewPassword).MinimumLength(4).MaximumLength(64).NotEmpty();
        }
    }
}
