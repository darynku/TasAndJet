using CSharpFunctionalExtensions;
using MediatR;
using SharedKernel.Common;
using SharedKernel.Common.Api;
using TasAndJet.Contracts.Response;
using TasAndJet.Domain.Entities.Orders;

namespace TasAndJet.Application.Applications.Handlers.Orders.GetById;

public record GetOrderQuery(Guid OrderId) : IRequest<Result<OrderResponse, Error>>;
