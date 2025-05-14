using CSharpFunctionalExtensions;
using MediatR;
using SharedKernel.Common;
using TasAndJet.Infrastructure;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common.Api;
using SharedKernel.Validators;
using TasAndJet.Domain.Entities.Orders;
using TasAndJet.Domain.Entities.Reviews;

namespace TasAndJet.Application.Applications.Handlers.Reviews.Create;

public class CreateReviewCommandHandler(
    ApplicationDbContext context,
    IValidator<CreateReviewCommand> validator) : IRequestHandler<CreateReviewCommand, UnitResult<ErrorList>>
{
    public async Task<UnitResult<ErrorList>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var validate = await validator.ValidateAsync(request, cancellationToken);
        if (!validate.IsValid)
            return validate.ToList();
        
        var order = await context.Orders.FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);
        if (order == null)
            return Errors.General.NotFound(null, "order").ToErrorList();
        
        if (order.Status != OrderStatus.Completed)
            return Errors.General.ValueIsInvalid("Нельзя оставить отзыв на незавершённый заказ.").ToErrorList(); 

        // Проверка существующего отзыва
        var existingReview = await context.Reviews
            .AnyAsync(r => r.OrderId == request.OrderId, cancellationToken);

        if (existingReview)
            return Errors.Reviews.InvalidOperation().ToErrorList();

        // Создание отзыва
        var review = Review.Create(Guid.NewGuid(), order.ClientId, order.DriverId, order.Id, request.Comment, request.Rating);

        await context.Reviews.AddAsync(review, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success<ErrorList>();
    }
}
