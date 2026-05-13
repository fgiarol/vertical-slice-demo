using Demo.Application.Common.Interfaces.Persistence.Repositories;
using Demo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demo.Infrastructure.Persistence.Repositories;

public class TagRepository(ApplicationDbContext context) : Repository<Tag>(context), ITagRepository
{
    public async Task<Tag?> GetTagByName(string name, CancellationToken cancellationToken) =>
        await DbSet.SingleOrDefaultAsync(n => n.Name.Equals(name), cancellationToken);

    public async Task<IEnumerable<Tag>> GetAllTags(CancellationToken cancellationToken) =>
        await DbSet.ToListAsync(cancellationToken);
}