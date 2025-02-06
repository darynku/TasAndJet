using CSharpFunctionalExtensions;
using MediatR;
using SharedKernel.Common;
using TasAndJet.Infrastructure;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TasAndJet.Domain.Entities.Orders;
using TasAndJet.Domain.Entities.Reviews;

namespace TasAndJet.Application.Applications.Handlers.Reviews.Create;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, Result<Guid, Error>>
{
    private readonly ApplicationDbContext _dbContext;

    public CreateReviewCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Guid, Error>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        // Проверка существования заказа и водителя
        var order = await _dbContext.Orders.FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);
        if (order == null)
            return Result.Failure<Guid, Error>(Errors.General.NotFound(request.OrderId, "Заказ не найден."));

        if (order.Status != OrderStatus.Completed)
            return Result.Failure<Guid, Error>(Errors.General.ValueIsInvalid("Нельзя оставить отзыв на незавершённый заказ.")); 

        // Проверка существующего отзыва
        var existingReview = await _dbContext.Reviews
            .AnyAsync(r => r.OrderId == request.OrderId, cancellationToken);

        if (existingReview)
            return Result.Failure<Guid, Error>(Errors.Reviews.InvalidOperation());

        // Создание отзыва
        var review = Review.Create(request.Id, request.ClientId, request.DriverId, request.OrderId, request.Comment, request.Rating);

        await _dbContext.Reviews.AddAsync(review, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success<Guid, Error>(review.Id);
    }
}
