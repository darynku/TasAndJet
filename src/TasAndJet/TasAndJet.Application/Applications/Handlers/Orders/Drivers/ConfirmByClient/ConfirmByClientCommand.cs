using MediatR;
using TasAndJet.Application.Applications.Handlers.Orders.Confirm;

namespace TasAndJet.Application.Applications.Handlers.Orders.Drivers.ConfirmByClient;

public record ConfirmByClientCommand(Guid OrderId) : IRequest;