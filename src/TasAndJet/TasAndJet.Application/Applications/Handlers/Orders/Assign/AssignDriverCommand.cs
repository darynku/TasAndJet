using MediatR;

namespace TasAndJet.Application.Applications.Handlers.Orders.Assign;

public record AssignDriverCommand(Guid OrderId, Guid DriverId) : IRequest;