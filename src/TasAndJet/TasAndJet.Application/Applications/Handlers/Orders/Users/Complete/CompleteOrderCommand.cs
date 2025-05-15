using MediatR;

namespace TasAndJet.Application.Applications.Handlers.Orders.Complete;

public record CompleteOrderCommand(Guid OrderId) : IRequest;