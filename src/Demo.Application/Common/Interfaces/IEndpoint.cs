using Microsoft.AspNetCore.Routing;

namespace Demo.Application.Common.Interfaces;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}