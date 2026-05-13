using Demo.Application.Common.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Demo.Application.Common.Extensions;

public static class ValidationExtensions
{
    public static RouteHandlerBuilder AddValidationFilter<TRequest>(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<ValidationFilter<TRequest>>();
    }
}
