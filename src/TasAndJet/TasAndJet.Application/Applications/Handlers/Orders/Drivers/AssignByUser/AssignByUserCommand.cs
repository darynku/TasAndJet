using MediatR;

namespace TasAndJet.Application.Applications.Handlers.Orders.Drivers;

public record AssignByUseCommand(Guid OrderId, Guid ClientId) : IRequest;