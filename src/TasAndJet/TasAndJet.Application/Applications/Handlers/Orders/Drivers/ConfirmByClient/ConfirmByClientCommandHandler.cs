using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common.Exceptions;
using TasAndJet.Infrastructure;

namespace TasAndJet.Application.Applications.Handlers.Orders.Drivers.ConfirmByClient;

public class ConfirmByClientCommandHandler(ApplicationDbContext context) : IRequestHandler<ConfirmByClientCommand>
{
    public async Task Handle(ConfirmByClientCommand request, CancellationToken cancellationToken)
    {
        var order = await context.Orders
                        .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
                    ?? throw new NotFoundException("Заказ не найден");

        order.Confirm();

        await context.SaveChangesAsync(cancellationToken);
    }
}