using Demo.Application.Common.Interfaces.Persistence.Repositories;
using Demo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demo.Infrastructure.Persistence.Repositories;

public class TodoRepository(ApplicationDbContext context) : Repository<Todo>(context), ITodoRepository
{
    public async Task<Todo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(t => t.Steps)
            .Include(t => t.Tags)
            .SingleOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Todo>> GetAllTodosAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(t => t.Steps)
            .Include(t => t.Tags)
            .ToListAsync(cancellationToken);
    }
}