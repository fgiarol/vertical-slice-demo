using Demo.Application.Common.Interfaces.Persistence.Repositories;
using Demo.Domain.Entities;

namespace Demo.Infrastructure.Persistence.Repositories;

public class StepRepository(ApplicationDbContext context) : Repository<Step>(context), IStepRepository;