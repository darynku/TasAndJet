using MediatR;
using SharedKernel.Paged;
using TasAndJet.Contracts.Response;

namespace TasAndJet.Application.Applications.Handlers.Vehicles.Get;

public record GetVehiclesQuery(int Page, int PageSize) : IRequest<PagedList<VehicleResponse>>;