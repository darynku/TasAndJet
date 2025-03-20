using MediatR;

namespace TasAndJet.Application.Applications.Handlers.Orders.Cancel;

public record CancelOrderCommand(Guid OrderId) : IRequest;