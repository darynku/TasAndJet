using MediatR;
using SharedKernel.Paged;
using TasAndJet.Contracts.Response;
using TasAndJet.Domain.Entities.Enums;
using TasAndJet.Domain.Entities.Services;

namespace TasAndJet.Application.Applications.Handlers.Orders.Get;

public record ClientQuery(Guid ClientId) : IRequest<PagedList<OrderResponse>>;
