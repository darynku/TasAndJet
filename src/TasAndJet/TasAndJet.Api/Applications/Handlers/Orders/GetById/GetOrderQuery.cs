using MediatR;
using TasAndJet.Api.Entities.Orders;

namespace TasAndJet.Api.Applications.Handlers.Orders.GetById;

public record GetOrderQuery(Guid OrderId) : IRequest<Order>;
