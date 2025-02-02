using MediatR;
using TasAndJet.Api.Common;
using TasAndJet.Api.Common.Paged;
using TasAndJet.Api.Contracts.Response;

namespace TasAndJet.Api.Applications.Handlers.Orders.Get;

public record GetOrdersQuery(int Page, int PageSize) : IRequest<PagedList<OrderResponse>>;