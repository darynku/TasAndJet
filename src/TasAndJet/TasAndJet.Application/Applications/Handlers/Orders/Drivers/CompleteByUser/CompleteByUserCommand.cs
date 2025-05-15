using MediatR;

namespace TasAndJet.Application.Applications.Handlers.Orders.Drivers.CompleteByUser;

public record CompleteByUserCommand(Guid OrderId) : IRequest;