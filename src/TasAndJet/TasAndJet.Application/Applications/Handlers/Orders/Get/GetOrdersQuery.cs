using MediatR;
using SharedKernel.Paged;
using TasAndJet.Contracts.Response;

namespace TasAndJet.Application.Applications.Handlers.Orders.Get;

public record GetOrdersQuery(int Page, int PageSize) : IRequest<PagedList<OrderResponse>>;