using FluentValidation;
using Games4Trade.Dtos;

namespace Games4TradeAPI.Validators
{
    public class UserRegisterDtoValidator : AbstractValidator<UserRegisterDto>
    {
        public UserRegisterDtoValidator()
        {
            RuleFor(u => u.Email).EmailAddress().MinimumLength(5).MaximumLength(128).NotEmpty();
            RuleFor(u => u.Login).MinimumLength(4).MaximumLength(32).NotEmpty();
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
