using FluentValidation;
using SharedKernel.Common.Api;

namespace TasAndJet.Application.Applications.Handlers.Accounts.VerifyCode;

public class VerifyCodeCommandValidator : AbstractValidator<VerifyCodeCommand>
{
    public VerifyCodeCommandValidator()
    {
        RuleFor(v => v.Code)
            .NotEmpty().WithMessage("Код не может быть пустым")
            .Matches(ValidationConstants.ValidCodePattern).WithMessage("Некоректный вид кода");
        
        RuleFor(v => v.PhoneNumber)
            .NotEmpty().WithMessage("Телефон не может быть пустым")
            .Matches(ValidationConstants.ValidPhoneNumberPattern).WithMessage("Некоректный вид номера телефона");
    }
}