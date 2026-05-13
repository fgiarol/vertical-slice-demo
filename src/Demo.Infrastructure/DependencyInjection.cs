using Demo.Application.Common.Interfaces.Persistence;
using Demo.Application.Common.Interfaces.Persistence.Repositories;
using Demo.Infrastructure.Persistence;
using Demo.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), providerOptions =>
            {
                providerOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            });
        });
        
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IStepRepository, StepRepository>();
        services.AddScoped<ITodoRepository, TodoRepository>();

        services.AddScoped<ApplicationDbContext>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
    }
}