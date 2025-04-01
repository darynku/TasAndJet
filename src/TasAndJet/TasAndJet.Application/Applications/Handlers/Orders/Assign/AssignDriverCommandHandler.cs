using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common.Exceptions;
using TasAndJet.Infrastructure;

namespace TasAndJet.Application.Applications.Handlers.Orders.Assign;

public class AssignDriverCommandHandler(ApplicationDbContext context) : IRequestHandler<AssignDriverCommand>
{
    public async Task Handle(AssignDriverCommand request, CancellationToken cancellationToken)
    {
        var order = await context.Orders
                        .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
                    ?? throw new NotFoundException("Заказ не найден");

        var driver = await context.Users
                         .FirstOrDefaultAsync(o => o.Id == request.DriverId, cancellationToken)
                     ?? throw new NotFoundException("Водитель не найден");
        
        driver.AddDriverOrder(order);
        order.AssignDriver(request.DriverId);
        await context.SaveChangesAsync(cancellationToken);
    }
}