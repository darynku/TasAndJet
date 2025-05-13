using FluentValidation;

namespace TasAndJet.Application.Applications.Handlers.Accounts.Google;

public class GoogleAuthCommandValidator : AbstractValidator<GoogleAuthCommand>
{
    public GoogleAuthCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Номер телефона обязателен.");
    }
}