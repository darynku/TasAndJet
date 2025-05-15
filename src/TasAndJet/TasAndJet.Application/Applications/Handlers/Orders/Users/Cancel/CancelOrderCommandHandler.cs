using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common.Exceptions;
using TasAndJet.Infrastructure;

namespace TasAndJet.Application.Applications.Handlers.Orders.Cancel;

public class CancelOrderCommandHandler(ApplicationDbContext context) : IRequestHandler<CancelOrderCommand>
{
    public async Task Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await context.Orders
                        .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
                    ?? throw new NotFoundException("Заказ не найден");

        order.Cancel();

        await context.SaveChangesAsync(cancellationToken);
    }
}
