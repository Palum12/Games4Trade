using FluentValidation;
using Games4TradeAPI.Dtos;

namespace Games4TradeAPI.Validators
{
    public class AnnoucementSaveValidator : AbstractValidator<AnnouncementSaveDto>
    {
        public AnnoucementSaveValidator()
        {
            RuleFor(a => a.Title).MinimumLength(2).NotEmpty();
            RuleFor(a => a.Content).MinimumLength(2).NotEmpty();
        }
    }
}
