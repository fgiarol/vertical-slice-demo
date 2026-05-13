using System.Reflection;
using Demo.Application.Common.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Demo.Application.Common.Extensions;

public static class EndpointExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddEndpoints()
        {
            services.AddEndpoints(Assembly.GetExecutingAssembly());

            return services;
        }

        public IServiceCollection AddEndpoints(Assembly assembly)
        {
            ServiceDescriptor[] endpointDescriptors = assembly
                .DefinedTypes
                .Where(type => type is { IsAbstract: false, IsInterface: false } && 
                               type.IsAssignableTo(typeof(IEndpoint)))
                .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
                .ToArray();

            services.TryAddEnumerable(endpointDescriptors);

            var handlerTypes = assembly.GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false, ContainsGenericParameters: false })
                .ToList();

            foreach (var implementation in handlerTypes)
            {
                var handlerInterfaces = implementation
                    .GetInterfaces()
                    .Where(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IHandler<,>));

                foreach (var handlerInterface in handlerInterfaces)
                    services.AddScoped(handlerInterface, implementation);
            }

            return services;
        }
    }

    public static IApplicationBuilder MapEndpoints(this WebApplication app, RouteGroupBuilder? routeGroupBuilder = null)
    {
        var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;

        foreach (var endpoint in endpoints)
            endpoint.MapEndpoint(builder);

        return app;
    }
}