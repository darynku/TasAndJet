using MediatR;
using Microsoft.EntityFrameworkCore;
using TasAndJet.Api.Entities.Orders;
using TasAndJet.Api.Infrastructure;

namespace TasAndJet.Api.Applications.Handlers.Orders.GetById;

public class GetOrderQueryHandler(ApplicationDbContext context) : IRequestHandler<GetOrderQuery, Order>
{
    public async Task<Order> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        return await context.Orders.FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken)
            ?? throw new Exception("Заказ не найден");
    }
}