using FluentValidation;

namespace TasAndJet.Application.Applications.Handlers.Accounts.Google;

public class GoogleAuthCommandValidator : AbstractValidator<GoogleAuthCommand>
{
    public GoogleAuthCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Номер телефона обязателен.")
            .Matches(@"^\+7(701|702|705|707|708|747|750|751|760|761|762|763|764|771|775|776|777|778)\d{6}$")
            .WithMessage("Некорректный номер телефона Казахстана.");

    }
}