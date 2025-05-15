using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common.Exceptions;
using TasAndJet.Infrastructure;

namespace TasAndJet.Application.Applications.Handlers.Orders.Drivers.CancelByUser;

public record CancelByUserCommand(Guid OrderId) : IRequest;

public class CancelByUserCommandHandler(ApplicationDbContext context) : IRequestHandler<CancelByUserCommand>
{
    public async Task Handle(CancelByUserCommand request, CancellationToken cancellationToken)
    {
        var order = await context.Orders
                        .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
                    ?? throw new NotFoundException("Заказ не найден");

        order.Cancel();

        await context.SaveChangesAsync(cancellationToken);
    }
}
