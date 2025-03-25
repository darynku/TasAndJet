using MediatR;
using SharedKernel.Paged;
using TasAndJet.Contracts.Response;
using TasAndJet.Infrastructure;

namespace TasAndJet.Application.Applications.Handlers.Vehicles.Get;

public class GetVehiclesQueryHandler(ApplicationDbContext context) : IRequestHandler<GetVehiclesQuery, PagedList<VehicleResponse>>
{
    public async Task<PagedList<VehicleResponse>> Handle(GetVehiclesQuery request, CancellationToken cancellationToken)
    {
        var vehiclesQuery = context.Vehicles.Select(v => new VehicleResponse
        {
            Id = v.Id,
            UserId = v.UserId,
            Capacity = v.Capacity,
            Mark = v.Mark,
            PhotoUrl = v.PhotoUrl,
            VehicleType = v.VehicleType
        });
        
        return await vehiclesQuery.ToPagedListAsync(request.Page, request.PageSize, cancellationToken);
    }
}