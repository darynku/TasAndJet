using MediatR;
using SharedKernel.Paged;
using TasAndJet.Contracts.Response;
using TasAndJet.Domain.Entities.Services;

namespace TasAndJet.Application.Applications.Handlers.Orders.Get;

public record GetOrdersQuery(
    int Page, 
    int PageSize,
    string? Search, 
    VehicleType? VehicleTypeSearch,
    decimal? MinPrice, 
    decimal? MaxPrice, 
    string? Region) : IRequest<PagedList<OrderResponse>>;