using Demo.Domain.Entities;

namespace Demo.Application.Common.Interfaces.Persistence.Repositories;

public interface ITodoRepository : IRepository<Todo>
{
    Task<Todo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Todo>> GetAllTodosAsync(CancellationToken cancellationToken = default);
}