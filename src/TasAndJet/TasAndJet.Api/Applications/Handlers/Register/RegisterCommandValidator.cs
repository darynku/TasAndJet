using FluentValidation;

namespace TasAndJet.Api.Applications.Handlers.Register;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Имя обязательно для заполнения.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Фамилия обязательна для заполнения.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Электронная почта обязательна для заполнения.")
            .EmailAddress().WithMessage("Укажите корректный адрес электронной почты.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен для заполнения.")
            .MinimumLength(6).WithMessage("Пароль должен содержать минимум 6 символов.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Номер телефона обязателен для заполнения.");

        RuleFor(x => x.Region)
            .NotEmpty().WithMessage("Регион обязателен для заполнения.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Адрес обязателен для заполнения.");

        RuleFor(x => x.RoleId)
            .GreaterThan(0)
            .NotNull().WithMessage("Роль обязательна для выбора.");
    }
}
