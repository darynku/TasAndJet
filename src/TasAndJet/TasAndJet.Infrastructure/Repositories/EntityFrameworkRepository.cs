using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace TasAndJet.Infrastructure.Repositories;

public class EntityFrameworkRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly DbContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;

    public EntityFrameworkRepository(DbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = _dbContext.Set<TEntity>();
    }

    /// <summary>
    /// Создание транзакции
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Созданная транзакция</returns>
    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Сохранение изменений в базе данных
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    private async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Добавление сущности в базу данных
    /// </summary>
    /// <param name="entity">Сущность</param>
    /// <param name="cancellationToken">Токен отмены</param>
    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        
        await _dbSet.AddAsync(entity, cancellationToken);
        await SaveChangesAsync(cancellationToken);
    }
}