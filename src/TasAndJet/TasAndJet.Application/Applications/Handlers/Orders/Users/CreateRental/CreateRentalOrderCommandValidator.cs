using FluentValidation;

namespace TasAndJet.Application.Applications.Handlers.Orders.CreateRental;

public class CreateRentalOrderCommandValidator : AbstractValidator<CreateRentalOrderCommand>
{
    public CreateRentalOrderCommandValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("ID клиента обязателен");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Описание обязательно");

        RuleFor(x => x.RentalStartDate)
            .NotNull().WithMessage("Дата начала аренды обязательна");

        RuleFor(x => x.RentalEndDate)
            .NotNull().WithMessage("Дата окончания аренды обязательна")
            .GreaterThan(x => x.RentalStartDate)
            .WithMessage("Дата окончания должна быть позже даты начала");

        RuleFor(x => x.TotalPrice)
            .GreaterThan(0).WithMessage("Стоимость должна быть больше 0");

        RuleFor(x => x.VehicleType)
            .IsInEnum().WithMessage("Неверный тип транспорта");

        RuleFor(x => x.City)
            .IsInEnum().WithMessage("Неверный город");

        RuleFor(x => x.Images)
            .Must(x => x is { Count: > 0 }).WithMessage("Необходимо загрузить хотя бы одно изображение");
    }
}
