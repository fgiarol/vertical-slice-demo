using Demo.Domain.Entities;

namespace Demo.Application.Common.Interfaces.Persistence.Repositories;

public interface ITagRepository : IRepository<Tag>
{
    Task<Tag?> GetTagByName(string name, CancellationToken cancellationToken);
    Task<IEnumerable<Tag>> GetAllTags(CancellationToken cancellationToken);
}