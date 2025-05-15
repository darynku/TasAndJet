using MediatR;
using SharedKernel.Paged;
using TasAndJet.Contracts.Response;
using TasAndJet.Domain.Entities.Enums;
using TasAndJet.Domain.Entities.Services;

namespace TasAndJet.Application.Applications.Handlers.Orders.Get;

public record GetDriversOrdersQuery(
    int Page, 
    int PageSize,
    string? Search, 
    VehicleType? VehicleTypeSearch,
    decimal? MinPrice, 
    decimal? MaxPrice, 
    string? Region,
    KazakhstanCity? City) : IRequest<PagedList<DriverOrderResponse>>;