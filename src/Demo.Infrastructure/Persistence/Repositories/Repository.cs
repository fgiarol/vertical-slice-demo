using System.Linq.Expressions;
using Demo.Application.Common.Interfaces.Persistence.Repositories;
using Demo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demo.Infrastructure.Persistence.Repositories;

public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    protected readonly DbSet<TEntity> DbSet;

    protected Repository(ApplicationDbContext context)
    {
        DbSet = context.Set<TEntity>();
    }

    public async Task<TEntity?> FindById(Guid id, CancellationToken cancellationToken = default) => 
        await DbSet.FindAsync([id], cancellationToken);

    public async Task<IEnumerable<TEntity>> Search(Expression<Func<TEntity, bool>> predicate, 
        CancellationToken cancellationToken = default) => 
        await DbSet.Where(predicate).ToListAsync(cancellationToken);

    public void Add(TEntity entity) => DbSet.Add(entity);

    public void AddRange(IEnumerable<TEntity> entities) => DbSet.AddRange(entities);

    public async Task Update(TEntity entity, CancellationToken cancellationToken = default) => 
        await Task.Run(() => DbSet.Update(entity), cancellationToken);

    public async Task Remove(TEntity entity, CancellationToken cancellationToken = default) => 
        await Task.Run(() => DbSet.Remove(entity), cancellationToken);
}