using Microsoft.EntityFrameworkCore.Storage;

namespace TasAndJet.Infrastructure.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Создание транзакции
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Созданная транзакция</returns>
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Добаволяет сущность в базу данных
    /// </summary>
    /// <param name="entity">Сущность</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken);
}