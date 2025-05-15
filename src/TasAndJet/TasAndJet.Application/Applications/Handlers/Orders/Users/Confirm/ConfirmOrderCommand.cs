using MediatR;

namespace TasAndJet.Application.Applications.Handlers.Orders.Confirm;

public record ConfirmOrderCommand(Guid OrderId) : IRequest;