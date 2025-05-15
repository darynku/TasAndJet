using CSharpFunctionalExtensions;
using MediatR;
using SharedKernel.Common.Api;

namespace TasAndJet.Application.Applications.Handlers.Orders.GetById;

public record GetOrderQuery(Guid OrderId) : IRequest<Result<OrderDetailsResponse, Error>>;
