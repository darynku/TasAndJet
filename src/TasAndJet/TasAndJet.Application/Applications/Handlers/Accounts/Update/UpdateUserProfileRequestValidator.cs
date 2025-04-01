using FluentValidation;

namespace TasAndJet.Application.Applications.Handlers.Accounts.Update;

public class UpdateUserProfileRequestValidator : AbstractValidator<UpdateUserProfileRequest>
{
    public UpdateUserProfileRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Имя обязательно для заполнения")
            .MaximumLength(100).WithMessage("Имя не должно превышать 100 символов");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Фамилия обязательна для заполнения")
            .MaximumLength(100).WithMessage("Фамилия не должна превышать 100 символов");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен для заполнения")
            .EmailAddress().WithMessage("Некорректный формат email");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Номер телефона обязателен для заполнения")
            .Matches(@"^(\+7|8)(7\d{9})$").WithMessage("Некорректный номер телефона Казахстана");

        RuleFor(x => x.Region)
            .NotEmpty().WithMessage("Регион обязателен для заполнения")
            .MaximumLength(100).WithMessage("Регион не должен превышать 100 символов");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Адрес обязателен для заполнения")
            .MaximumLength(200).WithMessage("Адрес не должен превышать 200 символов");
    }
}