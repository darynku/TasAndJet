using FluentValidation;

namespace TasAndJet.Application.Applications.Handlers.Orders.CreateFreight;

public class CreateFreightOrderCommandValidator : AbstractValidator<CreateFreightOrderCommand>
{
    public CreateFreightOrderCommandValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("ID клиента обязателен");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Описание обязательно");

        RuleFor(x => x.PickupAddress)
            .NotEmpty().WithMessage("Адрес отправления обязателен");

        RuleFor(x => x.DestinationAddress)
            .NotEmpty().WithMessage("Адрес назначения обязателен");

        RuleFor(x => x.CargoWeight)
            .GreaterThan(0).WithMessage("Вес груза должен быть больше 0");

        RuleFor(x => x.CargoType)
            .NotEmpty().WithMessage("Тип груза обязателен");

        RuleFor(x => x.TotalPrice)
            .GreaterThan(0).WithMessage("Стоимость должна быть больше 0");

        RuleFor(x => x.VehicleType)
            .IsInEnum().WithMessage("Неверный тип транспорта");

        RuleFor(x => x.City)
            .IsInEnum().WithMessage("Неверный город");
    }
}
