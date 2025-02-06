using FluentValidation;

namespace TasAndJet.Application.Applications.Handlers.Reviews.Create
{
    public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
    {
        public CreateReviewCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Идентификатор отзыва обязателен.");

            RuleFor(x => x.ClientId)
                .NotEmpty().WithMessage("Идентификатор клиента обязателен.");

            RuleFor(x => x.DriverId)
                .NotEmpty().WithMessage("Идентификатор водителя обязателен.");

            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("Идентификатор заказа обязателен.");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Комментарий обязателен.")
                .MaximumLength(500).WithMessage("Комментарий не должен превышать 500 символов.");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Оценка должна быть от 1 до 5.");
        }
    }
}
