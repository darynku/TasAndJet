using MediatR;
using SharedKernel.Paged;
using TasAndJet.Contracts.Response;

namespace TasAndJet.Application.Applications.Handlers.Orders.Get;

public record ClientQuery(Guid ClientId) : IRequest<PagedList<OrderResponse>>;
