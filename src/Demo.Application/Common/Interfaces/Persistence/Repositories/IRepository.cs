using System.Linq.Expressions;
using Demo.Domain.Entities;

namespace Demo.Application.Common.Interfaces.Persistence.Repositories;

public interface IRepository<TEntity> where TEntity : Entity
{
    Task<TEntity?> FindById(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> Search(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    Task Update(TEntity entity, CancellationToken cancellationToken = default);
    Task Remove(TEntity entity, CancellationToken cancellationToken = default);
}