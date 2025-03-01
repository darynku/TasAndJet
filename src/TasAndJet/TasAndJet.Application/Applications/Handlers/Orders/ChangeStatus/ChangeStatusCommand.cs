using SharedKernel.Common;
using SharedKernel.Common.Api;
using TasAndJet.Contracts.Data.Orders;

namespace TasAndJet.Application.Applications.Handlers.Orders.ChangeStatus;

using CSharpFunctionalExtensions;
using MediatR;
using TasAndJet.Domain.Entities.Orders;

public class ChangeStatusCommand(Guid orderId, ChangeStatusData data) : IRequest<UnitResult<ErrorList>>
{
    public Guid OrderId { get; } = orderId;
    public OrderStatus NewStatus { get; } = data.OrderStatus;
} 
