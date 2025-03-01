using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common;
using SharedKernel.Common.Api;
using TasAndJet.Infrastructure;

namespace TasAndJet.Application.Applications.Handlers.Orders.ChangeStatus;

public class ChangeStatusHandler(ApplicationDbContext context) : IRequestHandler<ChangeStatusCommand, UnitResult<ErrorList>>
{
    public async Task<UnitResult<ErrorList>> Handle(ChangeStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await context.Orders.FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);
        if (order is null)
            return Errors.General.NotFound(null, "order").ToErrorList();

        var result = order.ChangeStatus(request.NewStatus);
        
        if (result.IsFailure)
            return result.Error.ToErrorList();

        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success<ErrorList>();
    }
}