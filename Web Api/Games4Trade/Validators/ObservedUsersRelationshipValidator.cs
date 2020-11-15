using FluentValidation;
using Games4Trade.Dtos;

namespace Games4TradeAPI.Validators
{
    public class ObservedUsersRelationshipValidator : AbstractValidator<ObservedUsersRelationshipDto>
    {
        public ObservedUsersRelationshipValidator()
        {
            RuleFor(u => u.ObservedUserId).NotEqual(u => u.ObservingUserId)
                .WithMessage("Observed user id cannot be the same as observing user id.");
        }
    }
}
