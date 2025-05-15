using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common.Exceptions;
using TasAndJet.Infrastructure;

namespace TasAndJet.Application.Applications.Handlers.Orders.Drivers.CompleteByUser;

public class CompleteByUserCommandHandler(ApplicationDbContext context) : IRequestHandler<CompleteByUserCommand>
{
    public async Task Handle(CompleteByUserCommand request, CancellationToken cancellationToken)
    {
        var order = await context.Orders
                        .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
                    ?? throw new NotFoundException("Заказ не найден");
        
        order.Complete();
        
        await context.SaveChangesAsync(cancellationToken);
    }
}