using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Application.Common.Mappings;

public static class MappingConfiguration
{
    public static void RegisterMappingConfiguration(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;

        
    }
}